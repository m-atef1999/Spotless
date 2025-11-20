
// Map of category names to Unsplash Image URLs
export const CATEGORY_IMAGES: Record<string, string> = {
    'Cleaning': 'https://images.unsplash.com/photo-1581578731117-104f2a8d46a8?auto=format&fit=crop&q=80&w=800',
    'Laundry': 'https://images.unsplash.com/photo-1545173168-9f1947eebb8f?auto=format&fit=crop&q=80&w=800',
    'Repair': 'https://images.unsplash.com/photo-1581092918056-0c4c3acd3789?auto=format&fit=crop&q=80&w=800',
    'Painting': 'https://images.unsplash.com/photo-1589939705384-5185137a7f0f?auto=format&fit=crop&q=80&w=800',
    'Plumbing': 'https://images.unsplash.com/photo-1607472586893-edb57bdc0e39?auto=format&fit=crop&q=80&w=800',
    'Electrical': 'https://images.unsplash.com/photo-1621905251189-08b45d6a269e?auto=format&fit=crop&q=80&w=800',
    'Moving': 'https://images.unsplash.com/photo-1600518464441-9154a4dea21b?auto=format&fit=crop&q=80&w=800',
    'Gardening': 'https://images.unsplash.com/photo-1617576683096-00fc8eecb3af?auto=format&fit=crop&q=80&w=800',
};

// Fallback image if no category match
export const DEFAULT_SERVICE_IMAGE = 'https://images.unsplash.com/photo-1556911220-e15b29be8c8f?auto=format&fit=crop&q=80&w=800';

export const getServiceImage = (serviceName: string, categoryName?: string): string => {
    // Try to match by specific service name keywords
    const lowerName = serviceName.toLowerCase();

    if (lowerName.includes('deep clean') || lowerName.includes('house')) return 'https://images.unsplash.com/photo-1527515637-62c99099d967?auto=format&fit=crop&q=80&w=800';
    if (lowerName.includes('carpet')) return 'https://images.unsplash.com/photo-1558317374-a354d5f6d4da?auto=format&fit=crop&q=80&w=800';
    if (lowerName.includes('window')) return 'https://images.unsplash.com/photo-1504328345606-18bbc8c9d7d1?auto=format&fit=crop&q=80&w=800';
    if (lowerName.includes('car')) return 'https://images.unsplash.com/photo-1601362840469-51e4d8d58785?auto=format&fit=crop&q=80&w=800';
    if (lowerName.includes('sofa') || lowerName.includes('upholstery')) return 'https://images.unsplash.com/photo-1540574163026-643ea20ade25?auto=format&fit=crop&q=80&w=800';
    if (lowerName.includes('iron')) return 'https://images.unsplash.com/photo-1489274495757-95c7c83700c0?auto=format&fit=crop&q=80&w=800';

    // Fallback to category image
    if (categoryName && CATEGORY_IMAGES[categoryName]) {
        return CATEGORY_IMAGES[categoryName];
    }

    // Try to match category by name keywords if categoryName is not provided or not found
    if (lowerName.includes('clean')) return CATEGORY_IMAGES['Cleaning'];
    if (lowerName.includes('wash') || lowerName.includes('laundry')) return CATEGORY_IMAGES['Laundry'];
    if (lowerName.includes('fix') || lowerName.includes('repair')) return CATEGORY_IMAGES['Repair'];
    if (lowerName.includes('paint')) return CATEGORY_IMAGES['Painting'];
    if (lowerName.includes('pipe') || lowerName.includes('leak')) return CATEGORY_IMAGES['Plumbing'];
    if (lowerName.includes('wire') || lowerName.includes('electric')) return CATEGORY_IMAGES['Electrical'];
    if (lowerName.includes('garden') || lowerName.includes('lawn')) return CATEGORY_IMAGES['Gardening'];

    return DEFAULT_SERVICE_IMAGE;
};
