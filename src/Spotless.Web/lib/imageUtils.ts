// Map of category names to Unsplash Image URLs
export const CATEGORY_IMAGES: Record<string, string> = {
  Cleaning:
    "https://plus.unsplash.com/premium_photo-1663011218145-c1d0c3ba3542?auto=format&fit=crop&q=80&w=800",
  Laundry:
    "https://plus.unsplash.com/premium_photo-1664372899525-d99a419fd21a?q=80&w=694?auto=format&fit=crop&q=80&w=800",
  Repair:
    "https://images.unsplash.com/photo-1581092918056-0c4c3acd3789?auto=format&fit=crop&q=80&w=800",
  Painting:
    "https://plus.unsplash.com/premium_photo-1723662253911-db2eaa3324be?q=80&w=1493?auto=format&fit=crop&q=80&w=800",
  Plumbing:
    "https://images.unsplash.com/photo-1607472586893-edb57bdc0e39?auto=format&fit=crop&q=80&w=800",
  Electrical:
    "https://images.unsplash.com/photo-1621905251189-08b45d6a269e?auto=format&fit=crop&q=80&w=800",
  Moving:
    "https://images.unsplash.com/photo-1600518464441-9154a4dea21b?auto=format&fit=crop&q=80&w=800",
  Gardening:
    "https://images.unsplash.com/photo-1617576683096-00fc8eecb3af?auto=format&fit=crop&q=80&w=800",
};

// Fallback image if no category match
export const DEFAULT_SERVICE_IMAGE =
  "https://plus.unsplash.com/premium_photo-1664372899525-d99a419fd21a?q=80&w=694?auto=format&fit=crop&q=80&w=800";

export const getServiceImage = (
  serviceName: string,
  categoryName?: string
): string => {
  // Try to match by specific service name keywords
  const lowerName = serviceName.toLowerCase();

  if (lowerName.includes("deep clean") || lowerName.includes("house"))
    return "https://images.unsplash.com/photo-1686178827149-6d55c72d81df?auto=format&fit=crop&q=80&w=800";
  if (lowerName.includes("carpet"))
    return "https://bluepaisley.com/cdn/shop/articles/Rug_Cleaning_Toronto_ccab6cca-deb7-4ac2-8c59-eb433f09c8e3_800x.jpg?v=1556793184";
  if (lowerName.includes("window"))
    return "https://plus.unsplash.com/premium_photo-1663047393808-0f93f3780641?q=80&w=687?auto=format&fit=crop&q=80&w=800";
  if (lowerName.includes("car"))
    return "https://images.unsplash.com/photo-1543857182-68106299b6b2?q=80&w=1171?auto=format&fit=crop&q=80&w=800";
  if (lowerName.includes("sofa") || lowerName.includes("upholstery"))
    return "https://plus.unsplash.com/premium_photo-1679775636300-4ee7c2eab501?q=80&w=687?auto=format&fit=crop&q=80&w=800";
  if (lowerName.includes("iron"))
    return "https://plus.unsplash.com/premium_photo-1683134123155-20499f471baa?q=80&w=1170?auto=format&fit=crop&q=80&w=800";

  // Fallback to category image
  if (categoryName && CATEGORY_IMAGES[categoryName]) {
    return CATEGORY_IMAGES[categoryName];
  }

  // Try to match category by name keywords if categoryName is not provided or not found
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
