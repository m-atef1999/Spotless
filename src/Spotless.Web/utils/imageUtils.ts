// Helper function to get image source for a service
// Returns imageUrl if set, otherwise empty string (no fallback)
export const getServiceImage = (
    service: { name?: string | null; imageUrl?: string | null } | string
): string => {
    // Handle legacy calls where just a string name was passed
    if (typeof service === 'string') {
        return '';
    }

    // Return imageUrl if set, otherwise empty
    if (service.imageUrl && service.imageUrl.trim()) {
        return service.imageUrl;
    }

    return '';
};

// Helper function to get image source for a category
// Returns imageUrl if set, otherwise empty string (no fallback)
export const getCategoryImage = (
    category: { name?: string | null; imageUrl?: string | null } | string
): string => {
    // Handle legacy calls where just a string name was passed
    if (typeof category === 'string') {
        return '';
    }

    // Return imageUrl if set, otherwise empty
    if (category.imageUrl && category.imageUrl.trim()) {
        return category.imageUrl;
    }

    return '';
};
