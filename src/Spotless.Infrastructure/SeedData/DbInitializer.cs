using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Spotless.Domain.Entities;
using Spotless.Domain.ValueObjects;
using Spotless.Infrastructure.Context;
using Spotless.Infrastructure.Identity;


namespace Spotless.Infrastructure.SeedData
{
    public static class DbInitializer
    {

        private const string Currency = "EGP";

        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using (var serviceScope = serviceProvider.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                var userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();


                var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole<Guid>>>();


                if (context == null || userManager == null || roleManager == null)
                {
                    throw new InvalidOperationException("Could not resolve required services for seeding.");
                }

                await context.Database.MigrateAsync();


                await SeedRolesAsync(roleManager);
                await SeedAdminUserAsync(userManager);
                await SeedCategoriesAndServicesAsync(context);
                await SeedTimeSlotsAsync(context);
            }
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("Customer"))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>("Customer"));
            }
            if (!await roleManager.RoleExistsAsync("Driver"))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>("Driver"));
            }
        }

        private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
        {
            if (await userManager.FindByEmailAsync("admin@spotless.com") == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin@spotless.com",
                    Email = "admin@spotless.com",
                    EmailConfirmed = true,
                    IsActive = true,
                    LastLoginDate = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(adminUser, "P@ssw0rd123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }



        private static async Task SeedCategoriesAndServicesAsync(ApplicationDbContext context)
        {

            if (await context.Categories.AnyAsync())
            {
                return;
            }


            var dryCleaning = new Category("الغسيل الجاف - Dry Cleaning", new Money(50, Currency));
            var laundry = new Category("الغسيل و الكي - Laundry & Ironing", new Money(30, Currency));
            var homeCleaning = new Category("تنظيف المنزل - Home Cleaning", new Money(200, Currency));
            var carpet = new Category("تنظيف السجاد والمفروشات - Carpet & Upholstery", new Money(150, Currency));
            var disinfection = new Category("التعقيم والتطهير - Disinfection & Sanitization", new Money(250, Currency));

            await context.Categories.AddRangeAsync(dryCleaning, laundry, homeCleaning, carpet, disinfection);
            await context.SaveChangesAsync();


            if (await context.Services.AnyAsync())
            {
                return;
            }

            var services = new List<Service>
            {

                new Service(dryCleaning.Id, "بلوزة / قميص - Shirt Dry Clean", "غسيل جاف وكي للقمصان والبلوزات", new Money(50, Currency), 0.5m),
                new Service(dryCleaning.Id, "بنطال - Pants Dry Clean", "غسيل جاف وكي للبنطلونات", new Money(60, Currency), 0.5m),
                new Service(dryCleaning.Id, "جاكيت - Jacket Dry Clean", "غسيل جاف متخصص للجاكيت والمعاطف", new Money(100, Currency), 1.0m),
                new Service(dryCleaning.Id, "فستان - Dress Dry Clean", "غسيل جاف دقيق للفساتين", new Money(120, Currency), 1.0m),


                new Service(laundry.Id, "غسيل وكي قطعة واحدة - Wash & Iron (Single Item)", "غسيل وكي للقطع الفردية", new Money(25, Currency), 0.25m),
                new Service(laundry.Id, "كوي فقط - Ironing Only", "خدمة كي فقط للقطعة", new Money(15, Currency), 0.15m),
                new Service(laundry.Id, "غسيل ملابس يومية - Daily Clothes Wash", "غسيل للملابس اليومية (بالكيلو)", new Money(40, Currency), 1.0m),
                new Service(laundry.Id, "غسيل ملابس الأطفال - Kids Clothes Laundry", "غسيل لطيف لملابس الأطفال", new Money(50, Currency), 1.0m),


                new Service(homeCleaning.Id, "تنظيف غرفة - Room Cleaning", "تنظيف وترتيب الغرفة", new Money(150, Currency), 2.0m),
                new Service(homeCleaning.Id, "تنظيف شقة كاملة - Full Apartment Cleaning", "تنظيف شامل لكل غرف الشقة", new Money(500, Currency), 5.0m),
                new Service(homeCleaning.Id, "تنظيف مطبخ - Kitchen Deep Clean", "تنظيف عميق للمطبخ وإزالة الدهون", new Money(250, Currency), 3.0m),
                new Service(homeCleaning.Id, "تنظيف حمام - Bathroom Deep Clean", "تنظيف وتعقيم الحمام", new Money(100, Currency), 1.5m),


                new Service(carpet.Id, "غسيل سجادة - Carpet Cleaning", "غسيل سجاد بالبخار (بالمتر المربع)", new Money(80, Currency), 1.5m),
                new Service(carpet.Id, "تنظيف كنب - Sofa/Upholstery Cleaning", "تنظيف الكنب وإزالة البقع (للمقعد)", new Money(120, Currency), 2.0m),
                new Service(carpet.Id, "تنظيف مراتب - Mattress Cleaning", "تنظيف وتعقيم المراتب", new Money(200, Currency), 2.5m),


                new Service(disinfection.Id, "تعقيم منزل - Home Disinfection", "تعقيم المنزل بالكامل ضد الفيروسات والبكتيريا", new Money(400, Currency), 2.0m),
                new Service(disinfection.Id, "تعقيم مكتب - Office Disinfection", "خدمة تعقيم للمكاتب والشركات", new Money(600, Currency), 3.0m),
                new Service(disinfection.Id, "تعقيم غرف الأطفال - Kids Room Sanitization", "تعقيم آمن لغرف الأطفال", new Money(200, Currency), 1.0m)
            };

            await context.Services.AddRangeAsync(services);
            await context.SaveChangesAsync();
        }


        private static async Task SeedTimeSlotsAsync(ApplicationDbContext context)
        {

            if (await context.TimeSlots.AnyAsync())
            {
                return;
            }


            string workingDays = "Sat,Sun,Mon,Tue,Wed,Thu";

            var slots = new List<TimeSlot>
            {
                new TimeSlot(
                    "الفترة الصباحية (9 - 11)",
                    new TimeSpan(9, 0, 0),
                    new TimeSpan(11, 0, 0),
                    5,
                    workingDays
                ),
                new TimeSlot(
                    "فترة الظهيرة (11 - 1)",
                    new TimeSpan(11, 0, 0),
                    new TimeSpan(13, 0, 0),
                    5,
                    workingDays
                ),
                new TimeSlot(
                    "فترة بعد الظهيرة (1 - 3)",
                    new TimeSpan(13, 0, 0),
                    new TimeSpan(15, 0, 0),
                    5,
                    workingDays
                ),
                new TimeSlot(
                    "الفترة المسائية (3 - 5)",
                    new TimeSpan(15, 0, 0),
                    new TimeSpan(17, 0, 0),
                    5,
                    workingDays
                )
            };

            await context.TimeSlots.AddRangeAsync(slots);
            await context.SaveChangesAsync();
        }
    }
}