using ApiVersioning.Examples;
using Application;
using Asp.Versioning;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApi.Filters;
using WebBlog.Filters;
using WebBlog.Seeder;

var builder = WebApplication.CreateBuilder(args);
var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Development;

//build configurations

#region Serilog

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .WriteTo.Console(
        LogEventLevel.Debug, builder.Configuration["Logging:OutputTemplate"], theme: SystemConsoleTheme.Colored)
    .WriteTo.File(Path.Combine(AppContext.BaseDirectory, builder.Configuration["Logging:Dir"]),
        rollingInterval: RollingInterval.Day, fileSizeLimitBytes: null)
    .CreateLogger();

builder.Host.UseSerilog();
#endregion

#region Controllers
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
    options.Filters.Add<ValidateModelStateFilter>();
    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails),
       StatusCodes.Status500InternalServerError));
});
#endregion

#region Infrastructure

builder.Services.AddInfrastructure(builder.Configuration);

#endregion

#region Application

builder.Services.AddApplication();

#endregion

#region Auth
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Unconfirmed", policy =>
    {
        policy.RequireClaim("Poster", "true");
    });
    options.AddPolicy("PosterPolicy", policy =>
    {
        policy.RequireClaim("Active", "True")
        .RequireClaim("Poster", "true");
    });
    options.AddPolicy("AdminPolicy", policy =>
    {
        policy.RequireClaim("AdminUser", "true")
        .RequireClaim("Master", "true");
    });
});
#endregion

#region Swagger builder
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(
    options =>
    {
        var securitySchema = new OpenApiSecurityScheme
        {
            Description =
            "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        };

        options.AddSecurityDefinition("Bearer", securitySchema);

        var securityRequirement = new OpenApiSecurityRequirement
        {
             { securitySchema, new[] { "Bearer" } }
        };
        options.AddSecurityRequirement(securityRequirement);


        options.OperationFilter<SwaggerDefaultValues>();

        var fileName = typeof(Program).Assembly.GetName().Name + ".xml";
        var filePath = Path.Combine(AppContext.BaseDirectory, fileName);

        // integrate xml comments
        //options.IncludeXmlComments(filePath);
    });
#endregion

#region ApiVersioning
builder.Services.AddApiVersioning(
                    options =>
                    {
                        // reporting api versions will return the headers
                        // "api-supported-versions" and "api-deprecated-versions"
                        options.ReportApiVersions = true;
                        options.AssumeDefaultVersionWhenUnspecified = true;
                        options.Policies.Sunset(0.9)
                                        .Effective(DateTimeOffset.Now.AddDays(60))
                                        .Link("policy.html")
                                            .Title("Versioning Policy")
                                            .Type("text/html");
                    })
                .AddMvc()
                .AddApiExplorer(
                    options =>
                    {
                        // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                        // note: the specified format code will format the version as "'v'major[.minor][-status]"
                        options.GroupNameFormat = "'v'VVV";
                        // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                        // can also be used to control the format of the API version in route templates
                        options.SubstituteApiVersionInUrl = true;
                    });
#endregion 

#region ProblemDetails
builder.Services.AddProblemDetails();
#endregion

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

//app configurations

if (app.Environment.IsStaging() || app.Environment.IsDevelopment())
{
    #region Swagger App
    app.UseSwagger();
    app.UseSwaggerUI(
    options =>
    {
        var descriptions = app.DescribeApiVersions();

        // build a swagger endpoint for each discovered API version
        foreach (var description in descriptions)
        {
            var url = $"/swagger/{description.GroupName}/swagger.json";
            var name = description.GroupName.ToUpperInvariant();
            options.SwaggerEndpoint(url, name);
        }
    });
    #endregion
}

await IdentitySeeder.SeedAsync(app.Services, builder.Configuration);

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
