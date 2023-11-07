using Application.enums;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.Seed.RoleSeeder
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Mod.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Poster.ToString()));
        }
    }
}
