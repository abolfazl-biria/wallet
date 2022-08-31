using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.SeedData
{
    public static class SeedData
    {
        public static async Task SeedDefault(UserManager<MyUser> userManager, RoleManager<MyRole> roleManager)
        {
            await SeedRoles(roleManager);
            await SeedUsers(userManager);
        }
        public static Task SeedUsers(UserManager<MyUser> userManager)
        {
            if (userManager.FindByNameAsync("09017815959").Result == null)
            {
                var user = new MyUser()
                {
                    UserName = "09017815959",
                    PhoneNumber = "09017815959",
                    Email = "abolfazlbiria@gmail.com",
                    PhoneNumberConfirmed = true,
                    EmailConfirmed = true,
                };

                var result = userManager.CreateAsync(user, "a123456789").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }

            return Task.CompletedTask;
        }

        public static Task SeedRoles(RoleManager<MyRole> _roleManager)
        {
            if (!_roleManager.RoleExistsAsync("Admin").Result)
            {
                _roleManager.CreateAsync(new MyRole { Name = "Admin" });
            }

            return Task.CompletedTask;
        }
    }
}