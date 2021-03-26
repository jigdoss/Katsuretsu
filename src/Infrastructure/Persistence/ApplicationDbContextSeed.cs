using Katsuretsu.Infrastructure.Configuration;
using Katsuretsu.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Katsuretsu.Infrastructure.Persistence
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedApplicationDataAsync(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();
            var context = services.GetService<ApplicationDbContext>();
            context.Database.Migrate();


            var identityDataConfiguration = services.GetRequiredService<IdentityServerDataConfiguration>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            await SeedDefaultUserAsync(userManager, roleManager, identityDataConfiguration);
            await SeedSampleDataAsync(context);

        }

        private static async Task SeedDefaultUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IdentityServerDataConfiguration configuration)
        {

            // adding roles from seed
            foreach (var r in configuration.Roles)
            {
                if (!await roleManager.RoleExistsAsync(r.Name))
                {
                    var role = new IdentityRole(r.Name);
                    var result = await roleManager.CreateAsync(role);

                    if (result.Succeeded)
                    {
                        foreach (var claim in r.Claims)
                        {
                            await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim(claim.Type, claim.Value));
                        }
                    }
                }
            }

            // adding users from seed
            foreach (var user in configuration.Users)
            {
                var identityUser = new ApplicationUser
                {
                    UserName = user.Username,
                    Email = user.Email,
                    EmailConfirmed = true
                };

                var userByUserName = await userManager.FindByNameAsync(user.Username);
                var userByEmail = await userManager.FindByEmailAsync(user.Email);

                // User is already exists in database
                if (userByUserName != default || userByEmail != default)
                {
                    continue;
                }

                // if there is no password we create user without password
                // user can reset password later, because accounts have EmailConfirmed set to true
                var result = !string.IsNullOrEmpty(user.Password)
                ? await userManager.CreateAsync(identityUser, user.Password)
                : await userManager.CreateAsync(identityUser);

                if (result.Succeeded)
                {
                    foreach (var claim in user.Claims)
                    {
                        await userManager.AddClaimAsync(identityUser, new System.Security.Claims.Claim(claim.Type, claim.Value));
                    }

                    foreach (var role in user.Roles)
                    {
                        await userManager.AddToRoleAsync(identityUser, role);
                    }
                }
            }
        }

        private static async Task SeedSampleDataAsync(ApplicationDbContext context)
        {


            await context.SaveChangesAsync();
        }

    }
}
