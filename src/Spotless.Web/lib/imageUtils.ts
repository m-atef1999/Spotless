// Map of category names to Image URLs
export const CATEGORY_IMAGES: Record<string, string> = {
  Cleaning: "https://images.pexels.com/photos/4239091/pexels-photo-4239091.jpeg?auto=compress&cs=tinysrgb&w=800",
  Laundry: "https://images.pexels.com/photos/5591664/pexels-photo-5591664.jpeg?auto=compress&cs=tinysrgb&w=800",
  Repair: "https://images.unsplash.com/photo-1581092918056-0c4c3acd3789?auto=format&fit=crop&q=80&w=800",
  Painting: "https://images.unsplash.com/photo-1562259949-e8e7689d7828?auto=format&fit=crop&q=80&w=800",
  Plumbing: "https://images.unsplash.com/photo-1607472586893-edb57bdc0e39?auto=format&fit=crop&q=80&w=800",
  Electrical: "https://images.unsplash.com/photo-1621905251189-08b45d6a269e?auto=format&fit=crop&q=80&w=800",
  Moving: "https://images.unsplash.com/photo-1600518464441-9154a4dea21b?auto=format&fit=crop&q=80&w=800",
  Gardening: "https://images.unsplash.com/photo-1617576683096-00fc8eecb3af?auto=format&fit=crop&q=80&w=800",
};

// Fallback image
export const DEFAULT_SERVICE_IMAGE =
  "https://images.pexels.com/photos/5591581/pexels-photo-5591581.jpeg?auto=compress&cs=tinysrgb&w=800";

// Helper function to get image source for a service
// Priority: imageData (base64) > imageUrl > name-based fallback
export const getServiceImage = (
  service: { name?: string | null; imageUrl?: string | null; imageData?: string | null; categoryId?: string } | string,
  categoryName?: string
): string => {
  // Handle legacy calls where just a string name was passed
  if (typeof service === 'string') {
    return getServiceImageByName(service, categoryName);
  }

  // Priority 1: Base64 image data from backend
  if (service.imageData && service.imageData.trim()) {
    return service.imageData;
  }

  // Priority 2: Image URL from backend
  if (service.imageUrl && service.imageUrl.trim()) {
    return service.imageUrl;
  }

  // Priority 3: Fallback to name-based matching
  return getServiceImageByName(service.name || '', categoryName);
};

// Original name-based matching function (kept for fallback)
const getServiceImageByName = (
  serviceName: string,
  categoryName?: string
): string => {
  const lowerName = serviceName.toLowerCase();

  // Deep cleaning / house cleaning
  if (lowerName.includes("deep clean") || lowerName.includes("house") || lowerName.includes("apartment") || lowerName.includes("شقة"))
    return "https://images.pexels.com/photos/4239091/pexels-photo-4239091.jpeg?auto=compress&cs=tinysrgb&w=800";
  if (lowerName.includes("carpet") || lowerName.includes("سجاد") || lowerName.includes("موكيت"))
    return "https://images.pexels.com/photos/6195125/pexels-photo-6195125.jpeg?auto=compress&cs=tinysrgb&w=800";
  if (lowerName.includes("window") || lowerName.includes("شبابيك"))
    return "https://images.pexels.com/photos/6195280/pexels-photo-6195280.jpeg?auto=compress&cs=tinysrgb&w=800";
  if (lowerName.includes("car") || lowerName.includes("سيارة"))
    return "https://images.pexels.com/photos/6873089/pexels-photo-6873089.jpeg?auto=compress&cs=tinysrgb&w=800";
  if (lowerName.includes("sofa") || lowerName.includes("upholstery") || lowerName.includes("كنب"))
    return "https://images.unsplash.com/photo-1555041469-a586c61ea9bc?w=800&q=80";
  if (lowerName.includes("iron") || lowerName.includes("كي") || lowerName.includes("كوي"))
    return "https://images.pexels.com/photos/4107120/pexels-photo-4107120.jpeg?auto=compress&cs=tinysrgb&w=800";
  if (lowerName.includes("room") || lowerName.includes("غرفة"))
    return "https://images.pexels.com/photos/6195673/pexels-photo-6195673.jpeg?auto=compress&cs=tinysrgb&w=800";
  if (lowerName.includes("kitchen") || lowerName.includes("مطبخ"))
    return "https://images.pexels.com/photos/6195122/pexels-photo-6195122.jpeg?auto=compress&cs=tinysrgb&w=800";
  if (lowerName.includes("bathroom") || lowerName.includes("حمام"))
    return "https://images.pexels.com/photos/6197117/pexels-photo-6197117.jpeg?auto=compress&cs=tinysrgb&w=800";
  if (lowerName.includes("disinfect") || lowerName.includes("sanitiz") || lowerName.includes("تعقيم") || lowerName.includes("تطهير"))
    return "https://images.pexels.com/photos/4099467/pexels-photo-4099467.jpeg?auto=compress&cs=tinysrgb&w=800";
  if (lowerName.includes("mattress") || lowerName.includes("مراتب"))
    return "https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=800&q=80";

  // Laundry related
  if (lowerName.includes("dry") || lowerName.includes("جاف"))
    return "https://images.pexels.com/photos/5591581/pexels-photo-5591581.jpeg?auto=compress&cs=tinysrgb&w=800";
  if (lowerName.includes("wash") || lowerName.includes("laundry") || lowerName.includes("غسيل"))
    return "https://images.pexels.com/photos/5591664/pexels-photo-5591664.jpeg?auto=compress&cs=tinysrgb&w=800";
  if (lowerName.includes("curtain") || lowerName.includes("ستائر"))
    return "https://images.unsplash.com/photo-1513694203232-719a280e022f?w=800&q=80";
  if (lowerName.includes("towel") || lowerName.includes("مناشف"))
    return "https://images.unsplash.com/photo-1616627547584-bf28ceeecdb4?w=800&q=80";
  if (lowerName.includes("blanket") || lowerName.includes("بطانيات"))
    return "https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=800&q=80";
  if (lowerName.includes("sheet") || lowerName.includes("ملاءات"))
    return "https://images.unsplash.com/photo-1522771753035-4a50354b6b00?w=800&q=80";
  if (lowerName.includes("pillow") || lowerName.includes("مخدات") || lowerName.includes("وسائد"))
    return "https://images.unsplash.com/photo-1584100936595-c0654b55a2e6?w=800&q=80";

  // Clothing
  if (lowerName.includes("suit") || lowerName.includes("بدلة"))
    return "https://images.unsplash.com/photo-1594938298603-c8148c472997?w=800&q=80";
  if (lowerName.includes("shirt") || lowerName.includes("قميص") || lowerName.includes("بلوزة"))
    return "https://images.unsplash.com/photo-1604654894610-df63bc536371?w=800&q=80";
  if (lowerName.includes("jacket") || lowerName.includes("جاكيت") || lowerName.includes("coat") || lowerName.includes("معطف"))
    return "https://images.unsplash.com/photo-1591047139829-d91aecb6caea?w=800&q=80";
  if (lowerName.includes("dress") || lowerName.includes("فستان"))
    return "https://images.unsplash.com/photo-1539008835657-9e8e9680c956?w=800&q=80";
  if (lowerName.includes("gown") || lowerName.includes("evening") || lowerName.includes("سهرة"))
    return "https://images.unsplash.com/photo-1566174053879-31528523f8ae?w=800&q=80";
  if (lowerName.includes("pants") || lowerName.includes("بنطال"))
    return "https://images.unsplash.com/photo-1624378439575-d8705ad7ae80?w=800&q=80";
  if (lowerName.includes("sweater") || lowerName.includes("كنزة") || lowerName.includes("صوف"))
    return "https://images.unsplash.com/photo-1620799140408-ed5341cd2431?w=800&q=80";
  if (lowerName.includes("tie") || lowerName.includes("ربطة"))
    return "https://images.unsplash.com/photo-1589756823695-278bc35616b5?w=800&q=80";
  if (lowerName.includes("scarf") || lowerName.includes("شال") || lowerName.includes("وشاح"))
    return "https://images.unsplash.com/photo-1584030373081-f37b7bb4fa8e?w=800&q=80";
  if (lowerName.includes("sport") || lowerName.includes("رياضي"))
    return "https://images.unsplash.com/photo-1556906781-9a412961c28c?w=800&q=80";
  if (lowerName.includes("kid") || lowerName.includes("أطفال"))
    return "https://images.pexels.com/photos/5560020/pexels-photo-5560020.jpeg?auto=compress&cs=tinysrgb&w=800";

  // Fallback to category image
  if (categoryName && CATEGORY_IMAGES[categoryName]) {
    return CATEGORY_IMAGES[categoryName];
  }

  // Try to match category by name keywords
  if (lowerName.includes("clean")) return CATEGORY_IMAGES["Cleaning"];
  if (lowerName.includes("wash") || lowerName.includes("laundry"))
    return CATEGORY_IMAGES["Laundry"];
  if (lowerName.includes("fix") || lowerName.includes("repair"))
    return CATEGORY_IMAGES["Repair"];
  if (lowerName.includes("paint")) return CATEGORY_IMAGES["Painting"];
  if (lowerName.includes("pipe") || lowerName.includes("leak"))
    return CATEGORY_IMAGES["Plumbing"];
  if (lowerName.includes("wire") || lowerName.includes("electric"))
    return CATEGORY_IMAGES["Electrical"];
  if (lowerName.includes("garden") || lowerName.includes("lawn"))
    return CATEGORY_IMAGES["Gardening"];

  return DEFAULT_SERVICE_IMAGE;
};
