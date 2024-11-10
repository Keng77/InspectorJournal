using InspectorJournal.Models;
using Microsoft.AspNetCore.Identity;

public class DbInitializerMiddleware
{
    private readonly RequestDelegate _next;

    public DbInitializerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Проверка, что инициализация выполняется только один раз
        if (!context.Session.Keys.Contains("starting"))
        {
            // Получаем UserManager и RoleManager из контейнера зависимостей
            var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = context.RequestServices.GetRequiredService<RoleManager<IdentityRole>>();

            // Инициализация данных
            await DbUserInitializer.Initialize(context.RequestServices, userManager, roleManager);

            // Устанавливаем флаг, чтобы инициализация не выполнялась снова
            context.Session.SetString("starting", "Yes");
        }

        // Вызов следующего компонента в конвейере
        await _next.Invoke(context);
    }
}
