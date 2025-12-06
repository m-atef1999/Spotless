import { useRef } from 'react';
import { useInView, type UseInViewOptions } from 'framer-motion';

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
export const fadeInUp = {
    hidden: { opacity: 0, y: 30 },
    visible: {
        opacity: 1,
        y: 0,
        transition: { duration: 0.6, ease: 'easeOut' }
    }
};

export const fadeIn = {
    hidden: { opacity: 0 },
    visible: {
        opacity: 1,
        transition: { duration: 0.5, ease: 'easeOut' }
    }
};

export const scaleIn = {
    hidden: { opacity: 0, scale: 0.9 },
    visible: {
        opacity: 1,
        scale: 1,
        transition: { duration: 0.4, ease: 'easeOut' }
    }
};

export const slideInLeft = {
    hidden: { opacity: 0, x: -50 },
    visible: {
        opacity: 1,
        x: 0,
        transition: { duration: 0.5, ease: 'easeOut' }
    }
};

export const slideInRight = {
    hidden: { opacity: 0, x: 50 },
    visible: {
        opacity: 1,
        x: 0,
        transition: { duration: 0.5, ease: 'easeOut' }
    }
};

// Stagger container variant
export const staggerContainer = {
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
export const staggerItem = {
    hidden: { opacity: 0, y: 20 },
    visible: {
        opacity: 1,
        y: 0,
        transition: { duration: 0.4, ease: 'easeOut' }
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
