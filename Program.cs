using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TransportCompanyWithAuthorize.Data;
using TransportCompanyWithAuthorize.Middleware;
using TransportCompanyWithAuthorize.Models;
using TransportCompanyWithAuthorize.Service;
using TransportCompanyWithAuthorize.Data.Initializer;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DBConnection")
    ?? throw new InvalidOperationException("Connection string 'DBConnection' not found.");


// Регистрация HairdressingContext
builder.Services.AddDbContext<HairdressingContext>(options =>
    options.UseSqlServer(connectionString));

// Регистрация ApplicationDbContext для Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddMemoryCache();
builder.Services.AddScoped<CachedDataService>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login"; // Путь для перенаправления на страницу входа
    options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // Путь для страницы с ограничением доступа
});


builder.Services.AddControllersWithViews(options =>
{
    // Добавляем глобальную политику авторизации
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter(policy));
});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AllowAnonymousToPage("/Identity/Account/Login");
    options.Conventions.AllowAnonymousToPage("/Identity/Account/Register");
    options.Conventions.AllowAnonymousToPage("/Identity/Account/AccessDenied");
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.Lax;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

// Настройка middleware
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession(); // Сессии пользователя

// Инициализация базы данных
app.UseDbInitializer();

// Настройка маршрутов
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
