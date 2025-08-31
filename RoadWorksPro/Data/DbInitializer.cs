using Microsoft.AspNetCore.Identity;
using RoadWorksPro.Models.Entities;
using RoadWorksPro.Models.Enums;

namespace RoadWorksPro.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AdminUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Create admin role if it doesn't exist
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Create default admin if no admins exist
            if (!userManager.Users.Any())
            {
                var defaultAdmin = new AdminUser
                {
                    UserName = "admin@roadpro.ua",
                    Email = "admin@roadpro.ua",
                    FullName = "Адміністратор",
                    EmailConfirmed = true,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(defaultAdmin, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(defaultAdmin, "Admin");
                }
            }

            // Add sample products if none exist
            if (!context.Products.Any())
            {
                var products = new[]
                {
                    new RoadProduct
                    {
                        Name = "Знак 'Пішохідний перехід'",
                        Description = "Дорожній знак для позначення пішохідного переходу. Виготовлений з оцинкованої сталі з світловідбиваючою плівкою.",
                        Price = 1250.00m,
                        Dimensions = "700x700 мм",
                        Material = "Оцинкована сталь",
                        Standard = "ДСТУ 4100-2002",
                        ImagePath = "/images/products/pedestrian-sign.jpg",
                        IsActive = true
                    },
                    new RoadProduct
                    {
                        Name = "Сигнальний конус",
                        Description = "Конус дорожній сигнальний для тимчасового обмеження руху. Яскравий помаранчевий колір з світловідбиваючими смугами.",
                        Price = 450.00m,
                        Dimensions = "Висота 750 мм",
                        Material = "ПВХ пластик",
                        Standard = "ДСТУ 8752:2017",
                        ImagePath = "/images/products/cone.jpg",
                        IsActive = true
                    },
                    new RoadProduct
                    {
                        Name = "Дорожній бар'єр",
                        Description = "Пластиковий дорожній бар'єр для організації руху та безпеки дорожніх робіт.",
                        Price = 2800.00m,
                        Dimensions = "1200x800 мм",
                        Material = "Поліетилен",
                        Standard = "EN 13422",
                        ImagePath = "/images/products/barrier.jpg",
                        IsActive = true
                    },
                    new RoadProduct
                    {
                        Name = "Світловідбивач дорожній КД-3",
                        Description = "Катафот дорожній для розмітки узбіч та розділових смуг.",
                        Price = 180.00m,
                        Dimensions = "100x100x20 мм",
                        Material = "Алюмінієвий корпус",
                        Standard = "ДСТУ 8751:2017",
                        ImagePath = "/images/products/reflector.jpg",
                        IsActive = true
                    },
                    new RoadProduct
                    {
                        Name = "Знак 'Стоп'",
                        Description = "Знак зупинки обов'язкової. Восьмикутна форма з червоним фоном.",
                        Price = 1450.00m,
                        Dimensions = "700x700 мм",
                        Material = "Оцинкована сталь",
                        Standard = "ДСТУ 4100-2002",
                        ImagePath = "/images/products/stop-sign.jpg",
                        IsActive = true
                    }
                };

                await context.Products.AddRangeAsync(products);
            }

            // Add sample services if none exist
            if (!context.Services.Any())
            {
                var services = new[]
                {
                    new RoadService
                    {
                        Name = "Дорожня розмітка",
                        Description = "Нанесення всіх видів дорожньої розмітки термопластиком та фарбою. Розмітка паркінгів, пішохідних переходів, розділових смуг. Використовуємо сертифіковані матеріали з гарантією до 3 років.",
                        PriceInfo = "від 250 грн/м²",
                        ImagePath = "/images/services/road-marking.jpg",
                        IsActive = true
                    },
                    new RoadService
                    {
                        Name = "Ремонт асфальту",
                        Description = "Ямковий ремонт, укладання нового покриття, фрезерування старого асфальту. Працюємо з гарячим та холодним асфальтом. Гарантія на роботи 2 роки.",
                        PriceInfo = "від 800 грн/м²",
                        ImagePath = "/images/services/asphalt-repair.jpg",
                        IsActive = true
                    },
                    new RoadService
                    {
                        Name = "Встановлення дорожніх знаків",
                        Description = "Монтаж дорожніх знаків, вказівників та інформаційних табло. Включає розробку схеми розміщення, погодження з відповідними службами та професійний монтаж.",
                        PriceInfo = "від 3000 грн/знак",
                        ImagePath = "/images/services/sign-installation.jpg",
                        IsActive = true
                    }
                };

                await context.Services.AddRangeAsync(services);
            }

            // Add sample orders if none exist (for testing)
            if (!context.Orders.Any())
            {
                var testOrder = new RoadOrder
                {
                    CustomerName = "Тестовий Клієнт",
                    CustomerPhone = "+380501234567",
                    CustomerEmail = "test@example.com",
                    Comment = "Тестове замовлення",
                    TotalAmount = 1700.00m,
                    Status = OrderStatus.New,
                    CreatedAt = DateTime.UtcNow
                };

                context.Orders.Add(testOrder);
                await context.SaveChangesAsync();

                // Add order items
                var firstProduct = context.Products.First();
                var secondProduct = context.Products.Skip(1).First();

                var orderItems = new[]
                {
                    new OrderItem
                    {
                        OrderId = testOrder.Id,
                        ProductId = firstProduct.Id,
                        Quantity = 1,
                        Price = firstProduct.Price
                    },
                    new OrderItem
                    {
                        OrderId = testOrder.Id,
                        ProductId = secondProduct.Id,
                        Quantity = 1,
                        Price = secondProduct.Price
                    }
                };

                await context.OrderItems.AddRangeAsync(orderItems);
            }

            await context.SaveChangesAsync();
        }
    }
}