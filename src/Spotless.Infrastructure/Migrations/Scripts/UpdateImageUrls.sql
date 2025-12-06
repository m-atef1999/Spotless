-- ============================================================
-- Spotless Database - Image URL Update Script
-- ============================================================
-- This script sets appropriate image URLs for all categories and services
-- based on their names. Uses Pexels and Unsplash free image URLs.
-- ============================================================

-- ============================================================
-- CATEGORIES
-- ============================================================

-- الغسيل الجاف - Dry Cleaning
UPDATE Categories 
SET ImageUrl = 'https://images.pexels.com/photos/5591581/pexels-photo-5591581.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Dry Cleaning%' OR Name LIKE '%الغسيل الجاف%';

-- الغسيل و الكي - Laundry & Ironing
UPDATE Categories 
SET ImageUrl = 'https://images.pexels.com/photos/5591664/pexels-photo-5591664.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Laundry%' OR Name LIKE '%الغسيل و الكي%';

-- تنظيف المنزل - Home Cleaning
UPDATE Categories 
SET ImageUrl = 'https://images.pexels.com/photos/4239091/pexels-photo-4239091.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Home Cleaning%' OR Name LIKE '%تنظيف المنزل%';

-- تنظيف السجاد والمفروشات - Carpet & Upholstery
UPDATE Categories 
SET ImageUrl = 'https://images.pexels.com/photos/6195125/pexels-photo-6195125.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Carpet%' OR Name LIKE '%السجاد%';

-- التعقيم والتطهير - Disinfection & Sanitization
UPDATE Categories 
SET ImageUrl = 'https://images.pexels.com/photos/4099467/pexels-photo-4099467.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Disinfection%' OR Name LIKE '%التعقيم%';


-- ============================================================
-- DRY CLEANING SERVICES (10 items)
-- ============================================================

-- بلوزة / قميص - Shirt Dry Clean
UPDATE Services 
SET ImageUrl = 'https://images.unsplash.com/photo-1604654894610-df63bc536371?w=800&q=80'
WHERE Name LIKE '%Shirt Dry Clean%' OR Name LIKE '%بلوزة / قميص%';

-- بنطال - Pants Dry Clean
UPDATE Services 
SET ImageUrl = 'https://images.unsplash.com/photo-1624378439575-d8705ad7ae80?w=800&q=80'
WHERE Name LIKE '%Pants Dry Clean%' OR Name LIKE '%بنطال%';

-- جاكيت - Jacket Dry Clean
UPDATE Services 
SET ImageUrl = 'https://images.unsplash.com/photo-1591047139829-d91aecb6caea?w=800&q=80'
WHERE Name LIKE '%Jacket Dry Clean%' OR Name LIKE '%جاكيت%';

-- فستان - Dress Dry Clean
UPDATE Services 
SET ImageUrl = 'https://images.unsplash.com/photo-1539008835657-9e8e9680c956?w=800&q=80'
WHERE Name LIKE '%Dress Dry Clean%' OR Name LIKE '%فستان - Dress%';

-- بدلة كاملة - Full Suit Dry Clean
UPDATE Services 
SET ImageUrl = 'https://images.unsplash.com/photo-1594938298603-c8148c472997?w=800&q=80'
WHERE Name LIKE '%Suit Dry Clean%' OR Name LIKE '%بدلة كاملة%';

-- معطف شتوي - Winter Coat Dry Clean
UPDATE Services 
SET ImageUrl = 'https://images.unsplash.com/photo-1539533018447-63fcce2678e3?w=800&q=80'
WHERE Name LIKE '%Winter Coat%' OR Name LIKE '%معطف شتوي%';

-- فستان سهرة - Evening Gown Dry Clean
UPDATE Services 
SET ImageUrl = 'https://images.unsplash.com/photo-1566174053879-31528523f8ae?w=800&q=80'
WHERE Name LIKE '%Evening Gown%' OR Name LIKE '%فستان سهرة%';

-- كنزة صوف - Wool Sweater Dry Clean
UPDATE Services 
SET ImageUrl = 'https://images.unsplash.com/photo-1620799140408-ed5341cd2431?w=800&q=80'
WHERE Name LIKE '%Wool Sweater%' OR Name LIKE '%كنزة صوف%';

-- ربطة عنق - Tie Dry Clean
UPDATE Services 
SET ImageUrl = 'https://images.unsplash.com/photo-1589756823695-278bc35616b5?w=800&q=80'
WHERE Name LIKE '%Tie Dry Clean%' OR Name LIKE '%ربطة عنق%';

-- شال / وشاح - Scarf/Shawl Dry Clean
UPDATE Services 
SET ImageUrl = 'https://images.unsplash.com/photo-1584030373081-f37b7bb4fa8e?w=800&q=80'
WHERE Name LIKE '%Scarf%' OR Name LIKE '%Shawl%' OR Name LIKE '%شال%' OR Name LIKE '%وشاح%';


-- ============================================================
-- LAUNDRY & IRONING SERVICES (10 items)
-- ============================================================

-- غسيل وكي قطعة واحدة - Wash & Iron (Single Item)
UPDATE Services 
SET ImageUrl = 'https://images.pexels.com/photos/5591664/pexels-photo-5591664.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Wash & Iron%' OR Name LIKE '%غسيل وكي قطعة%';

-- كوي فقط - Ironing Only
UPDATE Services 
SET ImageUrl = 'https://images.pexels.com/photos/4107120/pexels-photo-4107120.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Ironing Only%' OR Name LIKE '%كوي فقط%';

-- غسيل ملابس يومية - Daily Clothes Wash
UPDATE Services 
SET ImageUrl = 'https://images.pexels.com/photos/5591664/pexels-photo-5591664.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Daily Clothes Wash%' OR Name LIKE '%غسيل ملابس يومية%';

-- غسيل ملابس الأطفال - Kids Clothes Laundry
UPDATE Services 
SET ImageUrl = 'https://images.pexels.com/photos/5560020/pexels-photo-5560020.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Kids Clothes%' OR Name LIKE '%ملابس الأطفال%';

-- غسيل ملاءات السرير - Bed Sheets Laundry
UPDATE Services 
SET ImageUrl = 'https://images.unsplash.com/photo-1522771753035-4a50354b6b00?w=800&q=80'
WHERE Name LIKE '%Bed Sheets%' OR Name LIKE '%ملاءات السرير%';

-- غسيل مناشف - Towels Laundry
UPDATE Services 
SET ImageUrl = 'https://images.unsplash.com/photo-1616627547584-bf28ceeecdb4?w=800&q=80'
WHERE Name LIKE '%Towels Laundry%' OR Name LIKE '%غسيل مناشف%';

-- غسيل ستائر - Curtains Laundry
UPDATE Services 
SET ImageUrl = 'https://images.unsplash.com/photo-1513694203232-719a280e022f?w=800&q=80'
WHERE Name LIKE '%Curtains Laundry%' OR Name LIKE '%غسيل ستائر%';

-- كوي ملابس رسمية - Formal Wear Ironing
UPDATE Services 
SET ImageUrl = 'https://images.unsplash.com/photo-1594938298603-c8148c472997?w=800&q=80'
WHERE Name LIKE '%Formal Wear Ironing%' OR Name LIKE '%كوي ملابس رسمية%';

-- غسيل ملابس رياضية - Sports Wear Laundry
UPDATE Services 
SET ImageUrl = 'https://images.unsplash.com/photo-1556906781-9a412961c28c?w=800&q=80'
WHERE Name LIKE '%Sports Wear%' OR Name LIKE '%ملابس رياضية%';

-- غسيل بطانيات - Blankets Laundry
UPDATE Services 
SET ImageUrl = 'https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=800&q=80'
WHERE Name LIKE '%Blankets Laundry%' OR Name LIKE '%غسيل بطانيات%';


-- ============================================================
-- HOME CLEANING SERVICES (10 items)
-- ============================================================

-- تنظيف غرفة - Room Cleaning
UPDATE Services 
SET ImageUrl = 'https://images.pexels.com/photos/6195673/pexels-photo-6195673.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Room Cleaning%' OR Name LIKE '%تنظيف غرفة%';

-- تنظيف شقة كاملة - Full Apartment Cleaning
UPDATE Services 
SET ImageUrl = 'https://images.pexels.com/photos/4239091/pexels-photo-4239091.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Apartment Cleaning%' OR Name LIKE '%تنظيف شقة%';

-- تنظيف مطبخ - Kitchen Deep Clean
UPDATE Services 
SET ImageUrl = 'https://images.pexels.com/photos/6195122/pexels-photo-6195122.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Kitchen%' OR Name LIKE '%تنظيف مطبخ%';

-- تنظيف حمام - Bathroom Deep Clean
UPDATE Services 
SET ImageUrl = 'https://images.pexels.com/photos/6197117/pexels-photo-6197117.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Bathroom%' OR Name LIKE '%تنظيف حمام%';

-- تنظيف شبابيك - Windows Cleaning
UPDATE Services 
SET ImageUrl = 'https://images.pexels.com/photos/6195280/pexels-photo-6195280.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Windows Cleaning%' OR Name LIKE '%تنظيف شبابيك%';

-- تنظيف بلكونة - Balcony Cleaning
UPDATE Services 
SET ImageUrl = 'https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=800&q=80'
WHERE Name LIKE '%Balcony%' OR Name LIKE '%بلكونة%';

-- تنظيف مكيفات - AC Cleaning
UPDATE Services 
SET ImageUrl = 'https://images.pexels.com/photos/5824519/pexels-photo-5824519.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%AC Cleaning%' OR Name LIKE '%مكيفات%';

-- تنظيف ثلاجة - Refrigerator Deep Clean
UPDATE Services 
SET ImageUrl = 'https://images.pexels.com/photos/5824883/pexels-photo-5824883.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Refrigerator%' OR Name LIKE '%ثلاجة%';

-- تنظيف فرن - Oven Deep Clean
UPDATE Services 
SET ImageUrl = 'https://images.pexels.com/photos/6996087/pexels-photo-6996087.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Oven%' OR Name LIKE '%فرن%';

-- تنظيف بعد التشطيب - Post-Construction Cleaning
UPDATE Services 
SET ImageUrl = 'https://images.pexels.com/photos/8961065/pexels-photo-8961065.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Post-Construction%' OR Name LIKE '%بعد التشطيب%';


-- ============================================================
-- CARPET & UPHOLSTERY SERVICES (10 items)
-- ============================================================

-- غسيل سجادة - Carpet Cleaning
UPDATE Services 
SET ImageUrl = 'https://images.pexels.com/photos/6195125/pexels-photo-6195125.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Carpet Cleaning%' OR Name LIKE '%غسيل سجادة%';

-- تنظيف كنب - Sofa/Upholstery Cleaning
UPDATE Services 
SET ImageUrl = 'https://images.unsplash.com/photo-1555041469-a586c61ea9bc?w=800&q=80'
WHERE Name LIKE '%Sofa%' OR Name LIKE '%Upholstery%' OR Name LIKE '%تنظيف كنب%';

-- تنظيف مراتب - Mattress Cleaning
UPDATE Services 
SET ImageUrl = 'https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=800&q=80'
WHERE Name LIKE '%Mattress%' OR Name LIKE '%مراتب%';

-- تنظيف موكيت - Wall-to-Wall Carpet Cleaning
UPDATE Services 
SET ImageUrl = 'https://images.pexels.com/photos/6195125/pexels-photo-6195125.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Wall-to-Wall%' OR Name LIKE '%موكيت%';

-- إزالة البقع - Stain Removal
UPDATE Services 
SET ImageUrl = 'https://images.pexels.com/photos/6195125/pexels-photo-6195125.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Stain Removal%' OR Name LIKE '%إزالة البقع%';

-- تنظيف كراسي - Chair Upholstery Cleaning
UPDATE Services 
SET ImageUrl = 'https://images.unsplash.com/photo-1506439773649-6e0eb8cfb237?w=800&q=80'
WHERE Name LIKE '%Chair Upholstery%' OR Name LIKE '%تنظيف كراسي%';

-- تنظيف سجاد فارسي - Persian Rug Cleaning
UPDATE Services 
SET ImageUrl = 'https://images.pexels.com/photos/276583/pexels-photo-276583.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Persian Rug%' OR Name LIKE '%سجاد فارسي%';

-- تنظيف ستائر معلقة - Hanging Curtains Cleaning
UPDATE Services 
SET ImageUrl = 'https://images.unsplash.com/photo-1513694203232-719a280e022f?w=800&q=80'
WHERE Name LIKE '%Hanging Curtains%' OR Name LIKE '%ستائر معلقة%';

-- تنظيف مخدات - Cushions & Pillows Cleaning
UPDATE Services 
SET ImageUrl = 'https://images.unsplash.com/photo-1584100936595-c0654b55a2e6?w=800&q=80'
WHERE Name LIKE '%Cushions%' OR Name LIKE '%Pillows%' OR Name LIKE '%مخدات%';

-- تعطير وتعقيم - Deodorizing & Sanitizing
UPDATE Services 
SET ImageUrl = 'https://images.pexels.com/photos/6197117/pexels-photo-6197117.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Deodorizing%' OR Name LIKE '%تعطير وتعقيم%';


-- ============================================================
-- DISINFECTION & SANITIZATION SERVICES (10 items)
-- ============================================================

-- تعقيم منزل - Home Disinfection
UPDATE Services 
SET ImageUrl = 'https://images.pexels.com/photos/4099467/pexels-photo-4099467.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Home Disinfection%' OR Name LIKE '%تعقيم منزل%';

-- تعقيم مكتب - Office Disinfection
UPDATE Services 
SET ImageUrl = 'https://images.pexels.com/photos/4239036/pexels-photo-4239036.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Office Disinfection%' OR Name LIKE '%تعقيم مكتب%';

-- تعقيم غرف الأطفال - Kids Room Sanitization
UPDATE Services 
SET ImageUrl = 'https://images.pexels.com/photos/5560020/pexels-photo-5560020.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Kids Room Sanitization%' OR Name LIKE '%تعقيم غرف الأطفال%';

-- تعقيم سيارة - Car Disinfection
UPDATE Services 
SET ImageUrl = 'https://images.pexels.com/photos/6873089/pexels-photo-6873089.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Car Disinfection%' OR Name LIKE '%تعقيم سيارة%';

-- تعقيم مطبخ - Kitchen Sanitization
UPDATE Services 
SET ImageUrl = 'https://images.pexels.com/photos/6195122/pexels-photo-6195122.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Kitchen Sanitization%' OR Name LIKE '%تعقيم مطبخ%';

-- تعقيم حمامات - Bathrooms Sanitization
UPDATE Services 
SET ImageUrl = 'https://images.pexels.com/photos/6197117/pexels-photo-6197117.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Bathrooms Sanitization%' OR Name LIKE '%تعقيم حمامات%';

-- تعقيم بعد المرض - Post-Illness Disinfection
UPDATE Services 
SET ImageUrl = 'https://images.pexels.com/photos/3985163/pexels-photo-3985163.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Post-Illness%' OR Name LIKE '%بعد المرض%';

-- تعقيم مدارس - School Disinfection
UPDATE Services 
SET ImageUrl = 'https://images.pexels.com/photos/8617981/pexels-photo-8617981.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%School Disinfection%' OR Name LIKE '%تعقيم مدارس%';

-- تعقيم عيادات - Clinic Disinfection
UPDATE Services 
SET ImageUrl = 'https://images.pexels.com/photos/4386467/pexels-photo-4386467.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Clinic Disinfection%' OR Name LIKE '%تعقيم عيادات%';

-- رش مبيدات - Pest Control Disinfection
UPDATE Services 
SET ImageUrl = 'https://images.pexels.com/photos/5824901/pexels-photo-5824901.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE Name LIKE '%Pest Control%' OR Name LIKE '%رش مبيدات%';


-- ============================================================
-- FALLBACK: Set default image for any services without ImageUrl
-- ============================================================
UPDATE Services 
SET ImageUrl = 'https://images.pexels.com/photos/5591581/pexels-photo-5591581.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE ImageUrl IS NULL OR ImageUrl = '';

UPDATE Categories 
SET ImageUrl = 'https://images.pexels.com/photos/5591581/pexels-photo-5591581.jpeg?auto=compress&cs=tinysrgb&w=800'
WHERE ImageUrl IS NULL OR ImageUrl = '';


-- ============================================================
-- VERIFICATION QUERIES (run these to check results)
-- ============================================================
-- SELECT Name, ImageUrl FROM Categories;
-- SELECT Name, ImageUrl FROM Services;
