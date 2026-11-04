using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project_tracker.Models;

namespace Project_tracker.Data
{
    public static class SeedData
    {
        public static async Task Initialize(
            AppDbContext context,
            IPasswordHasher<User> passwordHasher)
        {
            // Если администратор уже есть, ничего не делаем
            if (await context.Users.AnyAsync(u => u.Role == "Admin"))
                return;

            var admin = new User
            {
                Username = "super_admin",
                Email = "admin@test.com",
                Role = "Admin"
            };

            admin.PasswordHash = passwordHasher.HashPassword(
                admin,
                "123"
            );

            context.Users.Add(admin);

            await context.SaveChangesAsync();
        }
    }
}