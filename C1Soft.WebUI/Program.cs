using C1Soft.Business.Interfaces;
using C1Soft.Business.Services;
using C1Soft.Business.Validators;
using C1Soft.DataAccess.Context;
using C1Soft.DataAccess.Security;
using C1Soft.DataAccess.Seed;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabanı Bağlantısı (Microsoft SQL Server LocalDB)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("'DefaultConnection' bağlantı dizesi bulunamadı.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.MigrationsAssembly("C1Soft.DataAccess");
    }));


// 2. Güvenlik ve Şifreleme Servisi (PBKDF2/SHA256)
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

// 3. Kimlik Doğrulama (Cookie Authentication)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "C1Soft.B2B.Auth";
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Events.OnRedirectToLogin = context =>
        {
            // AJAX / Fetch veya JSON isteklerinde 302 HTML Login yerine 401 Unauthorized dön
            if (context.Request.Headers.XRequestedWith == "XMLHttpRequest" ||
                context.Request.Headers.Accept.ToString().Contains("application/json") ||
                context.Request.Path.StartsWithSegments("/Cart"))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }

            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            if (context.Request.Headers.XRequestedWith == "XMLHttpRequest" ||
                context.Request.Headers.Accept.ToString().Contains("application/json"))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            }

            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };
    });

// 4. MVC Controller ve View Servisleri + FluentValidation entegrasyonu
builder.Services.AddControllersWithViews();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<LoginValidator>();

// 5. Business Layer Servisleri (DI)
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBannerService, BannerService>();


var app = builder.Build();

// 5. Veritabanı Otomatik Migration ve Seed Data Çalıştırıcısı
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var passwordHasher = services.GetRequiredService<IPasswordHasher>();
        await DbInitializer.SeedAsync(context, passwordHasher);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Veritabanı migration veya tohumlama (seed) sırasında bir hata oluştu.");
    }
}

// 6. HTTP Pipeline Yapılandırması
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// 7. Area ve Standart Rotalar
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
