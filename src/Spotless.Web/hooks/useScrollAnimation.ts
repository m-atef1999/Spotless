import { useRef } from 'react';
import { useInView, type UseInViewOptions, type Variants } from 'framer-motion';

interface ScrollAnimationOptions extends Omit<UseInViewOptions, 'once'> {
    once?: boolean;
    threshold?: number;
}

/**
 * Custom hook for scroll-triggered animations using Framer Motion's useInView
 * Returns a ref and inView state for triggering animations when elements enter viewport
 */
export function useScrollAnimation(options: ScrollAnimationOptions = {}) {
    const { once = true, threshold = 0.2, ...restOptions } = options;
    const ref = useRef<HTMLDivElement>(null);

    const isInView = useInView(ref, {
        once,
        amount: threshold,
        ...restOptions,
    });

    return { ref, isInView };
}

// Animation variants for Framer Motion
export const fadeInUp: Variants = {
    hidden: { opacity: 0, y: 30 },
    visible: {
        opacity: 1,
        y: 0,
        transition: { duration: 0.6, ease: 'easeOut' as const }
    }
};

export const fadeIn: Variants = {
    hidden: { opacity: 0 },
    visible: {
        opacity: 1,
        transition: { duration: 0.5, ease: 'easeOut' as const }
    }
};

export const scaleIn: Variants = {
    hidden: { opacity: 0, scale: 0.9 },
    visible: {
        opacity: 1,
        scale: 1,
        transition: { duration: 0.4, ease: 'easeOut' as const }
    }
};

export const slideInLeft: Variants = {
    hidden: { opacity: 0, x: -50 },
    visible: {
        opacity: 1,
        x: 0,
        transition: { duration: 0.5, ease: 'easeOut' as const }
    }
};

export const slideInRight: Variants = {
    hidden: { opacity: 0, x: 50 },
    visible: {
        opacity: 1,
        x: 0,
        transition: { duration: 0.5, ease: 'easeOut' as const }
    }
};

// Stagger container variant
export const staggerContainer: Variants = {
    hidden: { opacity: 0 },
    visible: {
        opacity: 1,
        transition: {
            staggerChildren: 0.1,
            delayChildren: 0.1
        }
    }
};

// Stagger item variant (for children)
export const staggerItem: Variants = {
    hidden: { opacity: 0, y: 20 },
    visible: {
        opacity: 1,
        y: 0,
        transition: { duration: 0.4, ease: 'easeOut' as const }
    }
};

// Hover scale animation
export const hoverScale = {
    scale: 1.02,
    transition: { duration: 0.2 }
};

// Tap scale animation
export const tapScale = {
    scale: 0.98
};
