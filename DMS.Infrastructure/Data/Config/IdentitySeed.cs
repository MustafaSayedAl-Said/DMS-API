using DMS.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace DMS.Infrastructure.Data.Config
{
    public class IdentitySeed
    {
        public static async Task SeedUserAsync(UserManager<User> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new User
                {
                    DisplayName = "Geralt",
                    Email = "Geralt@Rivia.com",
                    UserName = "Geralt@Rivia.com",
                    Workspace = new Workspace
                    {
                        Name = "GeraltWorkspace",
                    }
                };
                var result = await userManager.CreateAsync(user, password: "`£!t78j-YT5K");
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        // Log or handle the error as needed
                        Console.WriteLine(error.Description);
                    }
                }
            }
            string email = "admin@admin.com";
            string password = "P@$$w0rd1";

            if (await userManager.FindByEmailAsync(email) == null)
            {
                var adminUser = new User
                {
                    DisplayName = "Admin",
                    Email = email,
                    UserName = email,
                    Workspace = new Workspace
                    {
                        Name = "AdminWorkspace",
                    }
                };
                await userManager.CreateAsync(adminUser, password);
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        public static async Task SeedUserRolesAsync(RoleManager<IdentityRole<int>> roleManager)
        {
            var roles = new[] { "Admin", "Member" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole<int> { Name = role });
            }
        }
    }
}
