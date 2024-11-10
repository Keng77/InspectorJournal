using InspectorJournal.Models;
using Microsoft.AspNetCore.Identity;

public static class DbUserInitializer
{
    public static async Task Initialize(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Проверяем наличие ролей
        string adminRole = "Admin";
        string userRole = "User";

        var roleExist = await roleManager.RoleExistsAsync(adminRole);
        if (!roleExist)
        {
            // Создаем роли, если они не существуют
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        roleExist = await roleManager.RoleExistsAsync(userRole);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(userRole));
        }

        // Проверяем, есть ли администратор
        ApplicationUser user = await userManager.FindByEmailAsync("admin@example.com");
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                RegistrationDate = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, "Password123!");
            if (result.Succeeded)
            {
                // Добавляем админа в роль Admin
                await userManager.AddToRoleAsync(user, adminRole);
            }
        }
    }
}
