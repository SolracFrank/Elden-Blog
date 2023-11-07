using Domain.Exceptions;
using Infrastructure.CustomEntities;
using Infrastructure.Identity.Seed.RoleSeeder;
using Infrastructure.Identity.Seed.UserSeeder;
using Microsoft.AspNetCore.Identity;

namespace WebBlog.Seeder
{
    public class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {


            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var userManager = services.GetRequiredService<UserManager<User>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    await DefaultRoles.SeedAsync(roleManager);
                    await DefaultAdmin.AdminSeeder(userManager, configuration.GetSection("AdminSecrets").Get<AdminSecrets>());
                }
                catch 
                {
                    throw new InfrastructureException("Error on loading Seeder");
                }
            }
        }
    }
}
