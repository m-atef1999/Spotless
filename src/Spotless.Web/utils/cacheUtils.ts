/**
 * Local Storage Cache Utility
 * 
 * Provides a thin caching layer on top of localStorage for API responses.
 * This helps with perceived performance by showing cached data immediately
 * while fetching fresh data in the background.
 * 
 * Features:
 * - TTL (time-to-live) support for cache expiration
 * - Type-safe with generics
 * - Graceful fallback if localStorage is unavailable
 * - Version key to invalidate cache on app updates
 */

const CACHE_VERSION = 'v1'; // Bump this to invalidate all caches on deploy
const CACHE_PREFIX = `spotless_cache_${CACHE_VERSION}_`;

interface CacheEntry<T> {
    data: T;
    timestamp: number;
    expiresAt: number;
}

/**
 * Check if localStorage is available (may not be in private browsing)
 */
const isLocalStorageAvailable = (): boolean => {
    try {
        const testKey = '__test__';
        localStorage.setItem(testKey, testKey);
        localStorage.removeItem(testKey);
        return true;
    } catch {
        return false;
    }
};

/**
 * Get cached data if available and not expired
 * @param key - Cache key
 * @returns Cached data or null if not found/expired
 */
export const getCached = <T>(key: string): T | null => {
    if (!isLocalStorageAvailable()) return null;

    try {
        const cached = localStorage.getItem(CACHE_PREFIX + key);
        if (!cached) return null;

        const entry: CacheEntry<T> = JSON.parse(cached);

        // Check if expired
        if (Date.now() > entry.expiresAt) {
            localStorage.removeItem(CACHE_PREFIX + key);
            return null;
        }

        return entry.data;
    } catch {
        // If parsing fails, remove corrupted cache
        localStorage.removeItem(CACHE_PREFIX + key);
        return null;
    }
};

/**
 * Set cache data with TTL
 * @param key - Cache key
 * @param data - Data to cache
 * @param ttlMinutes - Time to live in minutes (default: 5 minutes)
 */
export const setCache = <T>(key: string, data: T, ttlMinutes: number = 5): void => {
    if (!isLocalStorageAvailable()) return;

    try {
        const entry: CacheEntry<T> = {
            data,
            timestamp: Date.now(),
            expiresAt: Date.now() + (ttlMinutes * 60 * 1000)
        };
        localStorage.setItem(CACHE_PREFIX + key, JSON.stringify(entry));
    } catch (error) {
        // localStorage might be full, clear old caches
        clearOldCaches();
        console.warn('Cache set failed:', error);
    }
};

/**
 * Remove a specific cache entry
 * @param key - Cache key to remove
 */
export const removeCache = (key: string): void => {
    if (!isLocalStorageAvailable()) return;
    localStorage.removeItem(CACHE_PREFIX + key);
};

/**
 * Clear all Spotless caches
 */
export const clearAllCaches = (): void => {
    if (!isLocalStorageAvailable()) return;

    const keysToRemove: string[] = [];
    for (let i = 0; i < localStorage.length; i++) {
        const key = localStorage.key(i);
        if (key?.startsWith('spotless_cache_')) {
            keysToRemove.push(key);
        }
    }
    keysToRemove.forEach(key => localStorage.removeItem(key));
};

/**
 * Clear old/expired caches to free up space
 */
const clearOldCaches = (): void => {
    if (!isLocalStorageAvailable()) return;

    const keysToRemove: string[] = [];
    for (let i = 0; i < localStorage.length; i++) {
        const key = localStorage.key(i);
        if (key?.startsWith('spotless_cache_')) {
            try {
                const cached = localStorage.getItem(key);
                if (cached) {
                    const entry = JSON.parse(cached);
                    if (Date.now() > entry.expiresAt) {
                        keysToRemove.push(key);
                    }
                }
            } catch {
                keysToRemove.push(key);
            }
        }
    }
    keysToRemove.forEach(key => localStorage.removeItem(key));
};

// =====================
// Pre-defined Cache Keys
// =====================

export const CACHE_KEYS = {
    SERVICES_ALL: 'services_all',
    SERVICES_FEATURED: 'services_featured',
    CATEGORIES_ALL: 'categories_all',
} as const;

// =====================
// Cache TTL Defaults (in minutes)
// =====================

export const CACHE_TTL = {
    SERVICES: 5,      // 5 minutes - services don't change often
    CATEGORIES: 10,   // 10 minutes - categories rarely change
    FEATURED: 5,      // 5 minutes
} as const;
