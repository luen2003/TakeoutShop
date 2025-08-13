using CakeShop.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CakeShop.Persistence
{
    public static class DbInitializer
{
    public static void SeedDatabase(
        CakeShopDbContext context,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        Console.WriteLine("Seeding... - Start");

        // Seed categories và cakes nếu chưa có
        if (!context.Categories.Any() && !context.Cakes.Any())
        {
            var categories = new List<Category>
            {
                new Category { Name = "Pizza"},
                new Category { Name = " Hamburger" },
                new Category { Name = "Fruit Cakes"}
            };
            context.Categories.AddRange(categories);

            var cakes = new List<Cake>
            {
                new Cake
                {
                    Name ="Pizza",
                    Price = 48.00M,
                    ShortDescription ="Pizza",
                    LongDescription ="Icing carrot cake jelly-o Pizza...",
                    Category = categories[0],
                    ImageUrl ="/img/vanilla-cake2.jpg",
                    IsCakeOfTheWeek = true,
                },
                new Cake
                {
                    Name ="Hamburger 1",
                    Price =45.50M,
                    ShortDescription ="Yammy! Hamburger",
                    LongDescription ="Hamburger...",
                    Category = categories[1],
                    ImageUrl ="/img/chocolate-cake4.jpg",
                    IsCakeOfTheWeek = true,
                },
                new Cake
                {
                    Name ="Hamburger",
                    Price = 40.50M,
                    ShortDescription ="Taste Our Special Hamburger",
                    LongDescription ="Icing carrot cake jelly-o Hamburger...",
                    Category = categories[1],
                    ImageUrl ="/img/chocolate-cake3.jpg",
                    IsCakeOfTheWeek = false,
                },
                new Cake
                {
                    Name ="Pizza 1",
                    Price=35.00M,
                    ShortDescription ="Our Special Pizza",
                    LongDescription ="Icing carrot cake jelly-o Pizza...",
                    Category = categories[0],
                    ImageUrl ="/img/vanilla-cake4.jpg",
                    IsCakeOfTheWeek = true,
                },
                new Cake
                {
                    Name ="Chicken",
                    Price = 30.50M,
                    ShortDescription ="Chicken",
                    LongDescription ="Icing carrot cake jelly-o Chicken...",
                    Category = categories[2],
                    ImageUrl ="/img/fruit-cake.jpg",
                    IsCakeOfTheWeek =true,
                }
            };
            context.Cakes.AddRange(cakes);
            context.SaveChanges();
        }

        // Seed roles
        if (!roleManager.RoleExistsAsync("Admin").Result)
        {
            roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
        }

        // Danh sách các admin cần tạo
        var adminUsers = new List<(string Username, string Email)>
        {
            (configuration["Admin:Username"] ?? "admin", configuration["Admin:Email"] ?? "admin@admin.com"),
            ("admin1", "admin1@gmail.com"),
            ("admin2", "admin2@gmail.com")
        };

        string defaultPassword = configuration["Admin:Password"] ?? "abc123456789@";

        foreach (var (username, email) in adminUsers)
        {
            var existingUser = userManager.FindByEmailAsync(email).Result;
            if (existingUser == null)
            {
                var user = new IdentityUser
                {
                    Email = email,
                    UserName = username,
                    EmailConfirmed = true
                };

                var result = userManager.CreateAsync(user, defaultPassword).Result;
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create user {username}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }

                userManager.AddToRoleAsync(user, "Admin").Wait();
            }
            else
            {
                if (!userManager.IsInRoleAsync(existingUser, "Admin").Result)
                {
                    userManager.AddToRoleAsync(existingUser, "Admin").Wait();
                }
            }
        }

        context.SaveChanges();
        Console.WriteLine("Seeding... - End");
    }
}
}
