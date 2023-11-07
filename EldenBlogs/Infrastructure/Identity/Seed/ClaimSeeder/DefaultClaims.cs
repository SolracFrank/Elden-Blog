using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Infrastructure.Identity.Seed.ClaimSeeder
{
    public static class DefaultClaims
    {
        public static List<Claim> AdminClaimsSeeder = new()
        {
            new("AdminUser","true"),
            new("Master","true"),
            new("Active","true"),
        };

    }
}
