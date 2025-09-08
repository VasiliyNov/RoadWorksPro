using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RoadWorksPro.Data;
using RoadWorksPro.Models.Entities;
using RoadWorksPro.Services;

namespace RoadWorksPro
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add HttpContextAccessor
            builder.Services.AddHttpContextAccessor();

            // Add Cart Service
            builder.Services.AddScoped<ICartService, CartService>();

            // Add Email Service
            builder.Services.AddScoped<IEmailService, EmailService>();

            // Add Telegram Service
            builder.Services.AddScoped<ITelegramService, TelegramService>();

            // Add Newtonsoft.Json for session serialization
            builder.Services.AddControllers().AddNewtonsoftJson();

            // Add Entity Framework and SQLite
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add Identity
            builder.Services.AddIdentity<AdminUser, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;

                // User settings
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Configure cookie
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Admin/Login";
                options.LogoutPath = "/Admin/Logout";
                options.AccessDeniedPath = "/Admin/AccessDenied";
            });

            // Add session for cart
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
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

            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Initialize database with sample data
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    await DbInitializer.InitializeAsync(services);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while initializing the database.");
                }
            }

            app.Run();
        }
    }
}
