import React, { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { Star, Quote, ChevronLeft, ChevronRight } from 'lucide-react';

interface Testimonial {
    id: number;
    name: string;
    role: string;
    avatar: string;
    rating: number;
    text: string;
}

const testimonials: Testimonial[] = [
    {
        id: 1,
        name: 'Sarah Ahmed',
        role: 'Regular Customer',
        avatar: 'https://randomuser.me/api/portraits/women/44.jpg',
        rating: 5,
        text: "Absolutely amazing service! My clothes come back looking brand new every time. The pickup and delivery is so convenient for my busy schedule.",
    },
    {
        id: 2,
        name: 'Mohamed Hassan',
        role: 'Business Professional',
        avatar: 'https://randomuser.me/api/portraits/men/32.jpg',
        rating: 5,
        text: "I trust Spotless with all my suits and formal wear. The quality is exceptional, and they never miss a delivery window.",
    },
    {
        id: 3,
        name: 'Nour El-Din',
        role: 'Loyal Customer',
        avatar: 'https://randomuser.me/api/portraits/women/68.jpg',
        rating: 5,
        text: "The best laundry service I've ever used. Their attention to detail and customer service is top-notch. Highly recommend!",
    },
    {
        id: 4,
        name: 'Ahmed Mostafa',
        role: 'Premium Member',
        avatar: 'https://randomuser.me/api/portraits/men/75.jpg',
        rating: 5,
        text: "Switched to Spotless 6 months ago and never looked back. The app is easy to use and the quality is consistently excellent.",
    },
];

export const TestimonialCarousel: React.FC = () => {
    const [current, setCurrent] = useState(0);
    const [isAutoPlaying, setIsAutoPlaying] = useState(true);

    const next = () => {
        setCurrent((prev) => (prev + 1) % testimonials.length);
    };

    const prev = () => {
        setCurrent((prev) => (prev - 1 + testimonials.length) % testimonials.length);
    };

    // Auto-advance carousel
    useEffect(() => {
        if (!isAutoPlaying) return;
        const interval = setInterval(next, 5000);
        return () => clearInterval(interval);
    }, [isAutoPlaying]);

    return (
        <section className="py-24 bg-gradient-to-b from-slate-50 to-white dark:from-slate-900 dark:to-slate-950 relative overflow-hidden">
            {/* Background decoration */}
            <div className="absolute top-20 left-10 w-20 h-20 text-cyan-200 dark:text-cyan-800 opacity-50">
                <Quote className="w-full h-full" />
            </div>
            <div className="absolute bottom-20 right-10 w-20 h-20 text-cyan-200 dark:text-cyan-800 opacity-50 rotate-180">
                <Quote className="w-full h-full" />
            </div>

            <div className="container mx-auto px-4 relative z-10">
                {/* Section header */}
                <motion.div
                    initial={{ opacity: 0, y: 20 }}
                    whileInView={{ opacity: 1, y: 0 }}
                    viewport={{ once: true }}
                    transition={{ duration: 0.6 }}
                    className="text-center max-w-3xl mx-auto mb-16"
                >
                    <span className="inline-block text-cyan-600 dark:text-cyan-400 font-semibold text-sm uppercase tracking-wider mb-4">
                        Testimonials
                    </span>
                    <h2 className="text-4xl md:text-5xl font-bold text-slate-900 dark:text-white mb-6">
                        What Our <span className="gradient-text-primary">Customers Say</span>
                    </h2>
                    <p className="text-lg text-slate-600 dark:text-slate-400">
                        Join thousands of satisfied customers who trust us with their garments.
                    </p>
                </motion.div>

                {/* Carousel */}
                <div
                    className="max-w-4xl mx-auto relative"
                    onMouseEnter={() => setIsAutoPlaying(false)}
                    onMouseLeave={() => setIsAutoPlaying(true)}
                >
                    <AnimatePresence mode="wait">
                        <motion.div
                            key={current}
                            initial={{ opacity: 0, x: 50 }}
                            animate={{ opacity: 1, x: 0 }}
                            exit={{ opacity: 0, x: -50 }}
                            transition={{ duration: 0.4 }}
                            className="glass-card rounded-3xl p-8 md:p-12"
                        >
                            <div className="flex flex-col md:flex-row items-center gap-8">
                                {/* Avatar */}
                                <div className="relative shrink-0">
                                    <div className="w-24 h-24 md:w-32 md:h-32 rounded-full overflow-hidden ring-4 ring-cyan-500/20 shadow-xl">
                                        <img
                                            src={testimonials[current].avatar}
                                            alt={testimonials[current].name}
                                            className="w-full h-full object-cover"
                                        />
                                    </div>
                                    {/* Quote icon */}
                                    <div className="absolute -bottom-2 -right-2 w-10 h-10 bg-gradient-to-br from-cyan-500 to-blue-600 rounded-full flex items-center justify-center shadow-lg">
                                        <Quote className="w-5 h-5 text-white" />
                                    </div>
                                </div>

                                {/* Content */}
                                <div className="flex-1 text-center md:text-left">
                                    {/* Stars */}
                                    <div className="flex justify-center md:justify-start gap-1 mb-4">
                                        {[...Array(5)].map((_, i) => (
                                            <Star
                                                key={i}
                                                className={`w-5 h-5 ${i < testimonials[current].rating
                                                        ? 'text-amber-400 fill-amber-400'
                                                        : 'text-slate-300'
                                                    }`}
                                            />
                                        ))}
                                    </div>

                                    {/* Quote */}
                                    <p className="text-lg md:text-xl text-slate-700 dark:text-slate-300 leading-relaxed mb-6 italic">
                                        "{testimonials[current].text}"
                                    </p>

                                    {/* Author */}
                                    <div>
                                        <h4 className="font-bold text-slate-900 dark:text-white text-lg">
                                            {testimonials[current].name}
                                        </h4>
                                        <p className="text-cyan-600 dark:text-cyan-400 font-medium">
                                            {testimonials[current].role}
                                        </p>
                                    </div>
                                </div>
                            </div>
                        </motion.div>
                    </AnimatePresence>

                    {/* Navigation buttons */}
                    <button
                        onClick={prev}
                        className="absolute left-0 top-1/2 -translate-y-1/2 -translate-x-4 md:-translate-x-12 w-12 h-12 rounded-full glass flex items-center justify-center text-slate-600 dark:text-slate-300 hover:text-cyan-600 dark:hover:text-cyan-400 hover:scale-110 transition-all shadow-lg"
                        aria-label="Previous testimonial"
                    >
                        <ChevronLeft className="w-6 h-6" />
                    </button>
                    <button
                        onClick={next}
                        className="absolute right-0 top-1/2 -translate-y-1/2 translate-x-4 md:translate-x-12 w-12 h-12 rounded-full glass flex items-center justify-center text-slate-600 dark:text-slate-300 hover:text-cyan-600 dark:hover:text-cyan-400 hover:scale-110 transition-all shadow-lg"
                        aria-label="Next testimonial"
                    >
                        <ChevronRight className="w-6 h-6" />
                    </button>

                    {/* Dots */}
                    <div className="flex justify-center gap-2 mt-8">
                        {testimonials.map((_, index) => (
                            <button
                                key={index}
                                onClick={() => setCurrent(index)}
                                className={`w-3 h-3 rounded-full transition-all duration-300 ${index === current
                                        ? 'bg-cyan-500 w-8'
                                        : 'bg-slate-300 dark:bg-slate-600 hover:bg-cyan-400'
                                    }`}
                                aria-label={`Go to testimonial ${index + 1}`}
                            />
                        ))}
                    </div>
                </div>
            </div>
        </section>
    );
};
