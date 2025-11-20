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
            using var serviceScope = serviceProvider.CreateScope();
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
            await SeedAdminsAsync(context);
            await SeedCustomersAsync(context);
            await SeedDriversAsync(context);
            await SeedCategoriesAndServicesAsync(context);
            await SeedTimeSlotsAsync(context);
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

        private static async Task SeedAdminsAsync(ApplicationDbContext context)
        {
            if (await context.Admins.AnyAsync())
            {
                return;
            }

            var admins = new List<Admin>
            {
                new("Ahmed Hassan", "ahmed.hassan@spotless.com", Domain.Enums.AdminRole.SuperAdmin),
                new("Mohamed Ali", "mohamed.ali@spotless.com", Domain.Enums.AdminRole.Manager),
                new("Sara Ibrahim", "sara.ibrahim@spotless.com", Domain.Enums.AdminRole.Manager),
                new("Youssef Mahmoud", "youssef.mahmoud@spotless.com", Domain.Enums.AdminRole.Dispatcher),
                new("Nour Mohamed", "nour.mohamed@spotless.com", Domain.Enums.AdminRole.Dispatcher),
                new("Fatma Ahmed", "fatma.ahmed@spotless.com", Domain.Enums.AdminRole.Support),
                new("Omar Khaled", "omar.khaled@spotless.com", Domain.Enums.AdminRole.Support),
                new("Mona Samir", "mona.samir@spotless.com", Domain.Enums.AdminRole.Support),
                new("Karim Yasser", "karim.yasser@spotless.com", Domain.Enums.AdminRole.Dispatcher),
                new("Layla Hassan", "layla.hassan@spotless.com", Domain.Enums.AdminRole.Manager)
            };

            await context.Admins.AddRangeAsync(admins);
            await context.SaveChangesAsync();
        }

        private static async Task SeedCustomersAsync(ApplicationDbContext context)
        {
            if (await context.Customers.AnyAsync())
            {
                return;
            }

            var customers = new List<Customer>
            {
                // Individual Customers
                new(
                    adminId: null,
                    name: "Youssef Ahmed",
                    phone: "+201012345678",
                    email: "youssef.ahmed@gmail.com",
                    address: new Address("15 Tahrir Street", "Cairo", "Egypt", "11511"),
                    type: Domain.Enums.CustomerType.Individual
                ),
                new(
                    adminId: null,
                    name: "Nour Mohamed",
                    phone: "+201098765432",
                    email: "nour.mohamed@gmail.com",
                    address: new Address("23 Pyramids Road", "Giza", "Egypt", "12556"),
                    type: Domain.Enums.CustomerType.Individual
                ),
                new(
                    adminId: null,
                    name: "Hana Khaled",
                    phone: "+201123456789",
                    email: "hana.khaled@gmail.com",
                    address: new Address("45 Salah Salem Street", "Nasr City", "Egypt", "11371"),
                    type: Domain.Enums.CustomerType.Individual
                ),
                new(
                    adminId: null,
                    name: "Ali Hassan",
                    phone: "+201234567890",
                    email: "ali.hassan@gmail.com",
                    address: new Address("67 Ramses Street", "Downtown Cairo", "Egypt", "11511"),
                    type: Domain.Enums.CustomerType.Individual
                ),
                new(
                    adminId: null,
                    name: "Mariam Samir",
                    phone: "+201156789012",
                    email: "mariam.samir@gmail.com",
                    address: new Address("89 Makram Ebeid Street", "Heliopolis", "Egypt", "11361"),
                    type: Domain.Enums.CustomerType.Individual
                ),
                new(
                    adminId: null,
                    name: "Amr Yasser",
                    phone: "+201187654321",
                    email: "amr.yasser@gmail.com",
                    address: new Address("12 El Haram Street", "Giza", "Egypt", "12556"),
                    type: Domain.Enums.CustomerType.Individual
                ),

                // Business Customers
                new(
                    adminId: null,
                    name: "Cairo Cleaning Company",
                    phone: "+20225551234",
                    email: "info@cairocleaning.com",
                    address: new Address("34 Mohandiseen Street", "Mohandiseen", "Egypt", "12411"),
                    type: Domain.Enums.CustomerType.Business
                ),
                new(
                    adminId: null,
                    name: "Giza Services Ltd",
                    phone: "+20235554321",
                    email: "contact@gizaservices.com",
                    address: new Address("56 Dokki Street", "Dokki", "Egypt", "12311"),
                    type: Domain.Enums.CustomerType.Business
                ),
                new(
                    adminId: null,
                    name: "Nile Hospitality Group",
                    phone: "+20227778888",
                    email: "operations@nilehospitality.com",
                    address: new Address("78 Corniche El Nile", "Maadi", "Egypt", "11431"),
                    type: Domain.Enums.CustomerType.Business
                ),
                new(
                    adminId: null,
                    name: "Alexandria Hotels Co",
                    phone: "+20234445555",
                    email: "info@alexhotels.com",
                    address: new Address("90 Zamalek Street", "Zamalek", "Egypt", "11211"),
                    type: Domain.Enums.CustomerType.Business
                )
            };

            await context.Customers.AddRangeAsync(customers);
            await context.SaveChangesAsync();
        }

        private static async Task SeedDriversAsync(ApplicationDbContext context)
        {
            if (await context.Drivers.AnyAsync())
            {
                return;
            }

            var drivers = new List<Driver>
            {
                new(
                    adminId: null,
                    name: "Mahmoud Ibrahim",
                    email: "mahmoud.ibrahim@spotless.com",
                    phone: "+201011112222",
                    vehicleInfo: "Toyota Corolla 2020 - White - ABC 1234"
                ),
                new(
                    adminId: null,
                    name: "Tarek Mostafa",
                    email: "tarek.mostafa@spotless.com",
                    phone: "+201022223333",
                    vehicleInfo: "Hyundai Accent 2019 - Silver - DEF 5678"
                ),
                new(
                    adminId: null,
                    name: "Hossam Fathy",
                    email: "hossam.fathy@spotless.com",
                    phone: "+201033334444",
                    vehicleInfo: "Nissan Sunny 2021 - Black - GHI 9012"
                ),
                new(
                    adminId: null,
                    name: "Sherif Adel",
                    email: "sherif.adel@spotless.com",
                    phone: "+201044445555",
                    vehicleInfo: "Kia Cerato 2020 - Blue - JKL 3456"
                ),
                new(
                    adminId: null,
                    name: "Waleed Samy",
                    email: "waleed.samy@spotless.com",
                    phone: "+201055556666",
                    vehicleInfo: "Chevrolet Optra 2018 - Red - MNO 7890"
                ),
                new(
                    adminId: null,
                    name: "Khaled Nabil",
                    email: "khaled.nabil@spotless.com",
                    phone: "+201066667777",
                    vehicleInfo: "Peugeot 301 2019 - White - PQR 1234"
                ),
                new(
                    adminId: null,
                    name: "Essam Gamal",
                    email: "essam.gamal@spotless.com",
                    phone: "+201077778888",
                    vehicleInfo: "Renault Logan 2020 - Gray - STU 5678"
                ),
                new(
                    adminId: null,
                    name: "Ramy Ashraf",
                    email: "ramy.ashraf@spotless.com",
                    phone: "+201088889999",
                    vehicleInfo: "Suzuki Ciaz 2021 - Silver - VWX 9012"
                ),
                new(
                    adminId: null,
                    name: "Tamer Magdy",
                    email: "tamer.magdy@spotless.com",
                    phone: "+201099990000",
                    vehicleInfo: "Mitsubishi Lancer 2019 - Black - YZA 3456"
                ),
                new(
                    adminId: null,
                    name: "Wael Hamdy",
                    email: "wael.hamdy@spotless.com",
                    phone: "+201000001111",
                    vehicleInfo: "Skoda Rapid 2020 - White - BCD 7890"
                )
            };

            await context.Drivers.AddRangeAsync(drivers);
            await context.SaveChangesAsync();
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
                // Dry Cleaning Services (10 items)
                new(dryCleaning.Id, "بلوزة / قميص - Shirt Dry Clean", "غسيل جاف وكي للقمصان والبلوزات", new Money(50, Currency), 0.5m),
                new(dryCleaning.Id, "بنطال - Pants Dry Clean", "غسيل جاف وكي للبنطلونات", new Money(60, Currency), 0.5m),
                new(dryCleaning.Id, "جاكيت - Jacket Dry Clean", "غسيل جاف متخصص للجاكيت والمعاطف", new Money(100, Currency), 1.0m),
                new(dryCleaning.Id, "فستان - Dress Dry Clean", "غسيل جاف دقيق للفساتين", new Money(120, Currency), 1.0m),
                new(dryCleaning.Id, "بدلة كاملة - Full Suit Dry Clean", "غسيل جاف للبدلة الكاملة (جاكيت + بنطال)", new Money(150, Currency), 1.5m),
                new(dryCleaning.Id, "معطف شتوي - Winter Coat Dry Clean", "غسيل جاف للمعاطف الشتوية الثقيلة", new Money(180, Currency), 2.0m),
                new(dryCleaning.Id, "فستان سهرة - Evening Gown Dry Clean", "غسيل جاف فاخر لفساتين السهرة", new Money(200, Currency), 2.0m),
                new(dryCleaning.Id, "كنزة صوف - Wool Sweater Dry Clean", "غسيل جاف للكنزات الصوفية", new Money(70, Currency), 0.75m),
                new(dryCleaning.Id, "ربطة عنق - Tie Dry Clean", "غسيل جاف لربطات العنق", new Money(30, Currency), 0.25m),
                new(dryCleaning.Id, "شال / وشاح - Scarf/Shawl Dry Clean", "غسيل جاف للشالات والأوشحة", new Money(40, Currency), 0.5m),

                // Laundry & Ironing Services (10 items)
                new(laundry.Id, "غسيل وكي قطعة واحدة - Wash & Iron (Single Item)", "غسيل وكي للقطع الفردية", new Money(25, Currency), 0.25m),
                new(laundry.Id, "كوي فقط - Ironing Only", "خدمة كي فقط للقطعة", new Money(15, Currency), 0.15m),
                new(laundry.Id, "غسيل ملابس يومية - Daily Clothes Wash", "غسيل للملابس اليومية (بالكيلو)", new Money(40, Currency), 1.0m),
                new(laundry.Id, "غسيل ملابس الأطفال - Kids Clothes Laundry", "غسيل لطيف لملابس الأطفال", new Money(50, Currency), 1.0m),
                new(laundry.Id, "غسيل ملاءات السرير - Bed Sheets Laundry", "غسيل وكي ملاءات وأغطية السرير", new Money(60, Currency), 1.5m),
                new(laundry.Id, "غسيل مناشف - Towels Laundry", "غسيل وتجفيف المناشف", new Money(35, Currency), 0.75m),
                new(laundry.Id, "غسيل ستائر - Curtains Laundry", "غسيل وكي الستائر", new Money(100, Currency), 2.0m),
                new(laundry.Id, "كوي ملابس رسمية - Formal Wear Ironing", "كي احترافي للملابس الرسمية", new Money(30, Currency), 0.5m),
                new(laundry.Id, "غسيل ملابس رياضية - Sports Wear Laundry", "غسيل متخصص للملابس الرياضية", new Money(45, Currency), 0.75m),
                new(laundry.Id, "غسيل بطانيات - Blankets Laundry", "غسيل البطانيات والأغطية الثقيلة", new Money(80, Currency), 2.0m),

                // Home Cleaning Services (10 items)
                new(homeCleaning.Id, "تنظيف غرفة - Room Cleaning", "تنظيف وترتيب الغرفة", new Money(150, Currency), 2.0m),
                new(homeCleaning.Id, "تنظيف شقة كاملة - Full Apartment Cleaning", "تنظيف شامل لكل غرف الشقة", new Money(500, Currency), 5.0m),
                new(homeCleaning.Id, "تنظيف مطبخ - Kitchen Deep Clean", "تنظيف عميق للمطبخ وإزالة الدهون", new Money(250, Currency), 3.0m),
                new(homeCleaning.Id, "تنظيف حمام - Bathroom Deep Clean", "تنظيف وتعقيم الحمام", new Money(100, Currency), 1.5m),
                new(homeCleaning.Id, "تنظيف شبابيك - Windows Cleaning", "تنظيف الشبابيك من الداخل والخارج", new Money(120, Currency), 2.0m),
                new(homeCleaning.Id, "تنظيف بلكونة - Balcony Cleaning", "تنظيف البلكونة والتراس", new Money(80, Currency), 1.0m),
                new(homeCleaning.Id, "تنظيف مكيفات - AC Cleaning", "تنظيف وصيانة المكيفات", new Money(150, Currency), 1.5m),
                new(homeCleaning.Id, "تنظيف ثلاجة - Refrigerator Deep Clean", "تنظيف عميق للثلاجة من الداخل", new Money(100, Currency), 1.0m),
                new(homeCleaning.Id, "تنظيف فرن - Oven Deep Clean", "تنظيف الفرن وإزالة الدهون المتراكمة", new Money(120, Currency), 1.5m),
                new(homeCleaning.Id, "تنظيف بعد التشطيب - Post-Construction Cleaning", "تنظيف شامل بعد التشطيب أو التجديد", new Money(800, Currency), 8.0m),

                // Carpet & Upholstery Services (10 items)
                new(carpet.Id, "غسيل سجادة - Carpet Cleaning", "غسيل سجاد بالبخار (بالمتر المربع)", new Money(80, Currency), 1.5m),
                new(carpet.Id, "تنظيف كنب - Sofa/Upholstery Cleaning", "تنظيف الكنب وإزالة البقع (للمقعد)", new Money(120, Currency), 2.0m),
                new(carpet.Id, "تنظيف مراتب - Mattress Cleaning", "تنظيف وتعقيم المراتب", new Money(200, Currency), 2.5m),
                new(carpet.Id, "تنظيف موكيت - Wall-to-Wall Carpet Cleaning", "تنظيف الموكيت المثبت (بالمتر المربع)", new Money(60, Currency), 1.0m),
                new(carpet.Id, "إزالة البقع - Stain Removal", "إزالة البقع الصعبة من السجاد والكنب", new Money(100, Currency), 1.0m),
                new(carpet.Id, "تنظيف كراسي - Chair Upholstery Cleaning", "تنظيف كراسي الطعام والمكتب", new Money(80, Currency), 1.0m),
                new(carpet.Id, "تنظيف سجاد فارسي - Persian Rug Cleaning", "تنظيف متخصص للسجاد الفارسي والشرقي", new Money(150, Currency), 2.5m),
                new(carpet.Id, "تنظيف ستائر معلقة - Hanging Curtains Cleaning", "تنظيف الستائر دون فكها", new Money(120, Currency), 2.0m),
                new(carpet.Id, "تنظيف مخدات - Cushions & Pillows Cleaning", "تنظيف المخدات والوسائد", new Money(50, Currency), 0.75m),
                new(carpet.Id, "تعطير وتعقيم - Deodorizing & Sanitizing", "تعطير وتعقيم السجاد والمفروشات", new Money(70, Currency), 1.0m),

                // Disinfection & Sanitization Services (10 items)
                new(disinfection.Id, "تعقيم منزل - Home Disinfection", "تعقيم المنزل بالكامل ضد الفيروسات والبكتيريا", new Money(400, Currency), 2.0m),
                new(disinfection.Id, "تعقيم مكتب - Office Disinfection", "خدمة تعقيم للمكاتب والشركات", new Money(600, Currency), 3.0m),
                new(disinfection.Id, "تعقيم غرف الأطفال - Kids Room Sanitization", "تعقيم آمن لغرف الأطفال", new Money(200, Currency), 1.0m),
                new(disinfection.Id, "تعقيم سيارة - Car Disinfection", "تعقيم شامل للسيارة من الداخل", new Money(150, Currency), 1.0m),
                new(disinfection.Id, "تعقيم مطبخ - Kitchen Sanitization", "تعقيم المطبخ والأسطح", new Money(180, Currency), 1.5m),
                new(disinfection.Id, "تعقيم حمامات - Bathrooms Sanitization", "تعقيم عميق للحمامات", new Money(150, Currency), 1.5m),
                new(disinfection.Id, "تعقيم بعد المرض - Post-Illness Disinfection", "تعقيم شامل بعد حالات المرض", new Money(500, Currency), 3.0m),
                new(disinfection.Id, "تعقيم مدارس - School Disinfection", "خدمة تعقيم للمدارس والحضانات", new Money(1000, Currency), 5.0m),
                new(disinfection.Id, "تعقيم عيادات - Clinic Disinfection", "تعقيم طبي للعيادات والمراكز الصحية", new Money(800, Currency), 4.0m),
                new(disinfection.Id, "رش مبيدات - Pest Control Disinfection", "رش المبيدات ومكافحة الحشرات", new Money(350, Currency), 2.5m)
            };

            // Set all services as featured
            foreach (var service in services)
            {
                service.SetFeatured(true);
            }

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
                new(
                    "الفترة الصباحية (9 - 11)",
                    new TimeSpan(9, 0, 0),
                    new TimeSpan(11, 0, 0),
                    5,
                    workingDays
                ),
                new(
                    "فترة الظهيرة (11 - 1)",
                    new TimeSpan(11, 0, 0),
                    new TimeSpan(13, 0, 0),
                    5,
                    workingDays
                ),
                new(
                    "فترة بعد الظهيرة (1 - 3)",
                    new TimeSpan(13, 0, 0),
                    new TimeSpan(15, 0, 0),
                    5,
                    workingDays
                ),
                new(
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
