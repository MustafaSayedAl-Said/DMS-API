using DMS.Core.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
    }
}
