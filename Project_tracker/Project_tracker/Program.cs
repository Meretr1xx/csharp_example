
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project_tracker.Data;
using Project_tracker.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Подключение MVC
builder.Services.AddControllersWithViews();

// 2. Подключение PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// 3. Хеширование паролей
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();


// 4. Настройка Session
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    var sessionSettings = builder.Configuration.GetSection("Session");

    options.IdleTimeout = TimeSpan.FromMinutes(
        sessionSettings.GetValue<int>("IdleTimeoutMinutes")
    );

    options.Cookie.Name = sessionSettings["CookieName"];
    options.Cookie.HttpOnly = sessionSettings.GetValue<bool>("HttpOnly");
    options.Cookie.IsEssential = sessionSettings.GetValue<bool>("IsEssential");
    options.Cookie.SameSite = SameSiteMode.Lax;
});


// 5. Настройка Cookie Authentication
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        var authSettings = builder.Configuration.GetSection("Authentication");

        options.Cookie.Name = authSettings["CookieName"];
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;

        options.LoginPath = authSettings["LoginPath"];
        options.AccessDeniedPath = authSettings["AccessDeniedPath"];

        options.ExpireTimeSpan = TimeSpan.FromHours(
            authSettings.GetValue<int>("ExpireHours")
        );

        options.SlidingExpiration =
            authSettings.GetValue<bool>("SlidingExpiration");
    });


// 6. Создание приложения
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AppDbContext>();

    var passwordHasher =
        services.GetRequiredService<IPasswordHasher<User>>();

    await SeedData.Initialize(context, passwordHasher);
}


// 7. Настройка HTTP-конвейера
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Порядок middleware важен!
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
