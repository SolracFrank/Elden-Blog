using Application.enums;
using Domain.Exceptions;
using Infrastructure.CustomEntities;
using Infrastructure.Identity.Seed.ClaimSeeder;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.Seed.UserSeeder
{
    public class DefaultAdmin
    {
        public static async Task AdminSeeder(UserManager<User> userManager, AdminSecrets adminSecrets)
        {
            var defaultAdmin = new User
            {
                UserName = "admin",
                AccessFailedCount = 0,
                Email = adminSecrets.Email,
                BirthDate = new DateTime(1998,12,9),
                EmailConfirmed = true,
                LockoutEnabled = true,
                NormalizedEmail = adminSecrets.Email.ToUpper(),
                NormalizedUserName = "ADMIN",
            };
            if (userManager.Users.All(u => u.Id != defaultAdmin.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultAdmin.Email);
                if (user == null)
                {
                    var result = await userManager.CreateAsync(defaultAdmin, adminSecrets.Password);
                    if (!result.Succeeded) throw new BadRequestException("Error on seeding admin");

                    result = await userManager.AddToRoleAsync(defaultAdmin, Roles.Admin.ToString());

                    if (!result.Succeeded) throw new BadRequestException("Error on adding Admin user to Admin role");

                    result = await userManager.AddClaimsAsync(defaultAdmin, DefaultClaims.AdminClaimsSeeder);

                    if (!result.Succeeded) throw new BadRequestException("Error on adding Admin user to Admin claims");
                }
            }

        }
    }
}
