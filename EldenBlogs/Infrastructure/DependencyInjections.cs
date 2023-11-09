using Infrastructure.Data;
using Infrastructure.CustomEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Services.AuthServices;
using Application.Interfaces.AuthServices;
using Application.Interfaces.TokenServices;
using Infrastructure.Services.TokenServices;
using Domain.Dtos.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Azure;
using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Application.Interfaces.AppServices.ConnectionServices;

namespace Infrastructure
{
    public static class DependencyInjections
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                builder =>
                {
                    builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    builder.EnableRetryOnFailure(4, TimeSpan.FromSeconds(5), null);
                }));

            #region Add Identity
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 5;
                options.Password.RequireLowercase = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(30);
                options.Lockout.MaxFailedAccessAttempts = 3;
            });
            #endregion

            #region Services
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork,UnitOfWork>();
            services.AddScoped(typeof(IGenerateJWTService<>), typeof(GenerateJWTService<>));
            services.AddTransient<IIpManagerService,IpManagerService>();
            services.AddTransient<IValidateRefreshTokenService,ValidateRefreshTokenService>();
            services.AddScoped<IAuthService, AuthService>();
            #endregion


            services.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));

            #region Auth Token Configuration
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer("EldenJWT", jwtOption => {
                jwtOption.RequireHttpsMetadata = false;
                jwtOption.SaveToken = true;
                jwtOption.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1),
                    ValidIssuer = configuration["JWTSettings:Issuer"],
                    ValidAudience = configuration["JWTSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]))
                };

                jwtOption.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = context =>
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "text/plain";
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("token-Expired", "true");
                        }

                        return context.Response.WriteAsync(context.Exception.Message);
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(new UnAuthorizedException("Usted no está autorizado"));
                        return context.Response.WriteAsync(result);
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = 403;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(new UnAuthorizedException("Usted no tiene permisos sobre este recurso"));
                        return context.Response.WriteAsync(result);
                    }
                };
            });
             
            #endregion

            #region Authorization

            #endregion
        }
    }
}
