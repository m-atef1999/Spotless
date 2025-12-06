import React, { useState } from 'react';
import { motion } from 'framer-motion';
import { Search, Sparkles, ChevronDown, Shirt, Droplets, Wind, Scissors, SprayCan, Brush } from 'lucide-react';
import { Button } from '../ui/Button';
import type { ServiceDto } from '../../lib/api';

interface HeroSectionProps {
    searchQuery: string;
    onSearchChange: (value: string) => void;
    onSearchSubmit: (e: React.FormEvent) => void;
    onSearchFocus: () => void;
    showSuggestions: boolean;
    suggestions: ServiceDto[];
    onSuggestionClick: (service: ServiceDto) => void;
}

// Floating cleaning icons configuration
const floatingIcons = [
    { Icon: Shirt, x: '10%', y: '15%', size: 48, delay: 0, rotation: -15, color: 'cyan' },
    { Icon: Droplets, x: '85%', y: '20%', size: 40, delay: 0.5, rotation: 10, color: 'blue' },
    { Icon: Wind, x: '15%', y: '70%', size: 36, delay: 1, rotation: 5, color: 'purple' },
    { Icon: Scissors, x: '80%', y: '65%', size: 42, delay: 1.5, rotation: -10, color: 'teal' },
    { Icon: SprayCan, x: '5%', y: '45%', size: 38, delay: 2, rotation: 20, color: 'indigo' },
    { Icon: Brush, x: '92%', y: '45%', size: 44, delay: 2.5, rotation: -25, color: 'sky' },
    // Additional scattered icons
    { Icon: Shirt, x: '25%', y: '85%', size: 32, delay: 0.3, rotation: 12, color: 'emerald' },
    { Icon: Droplets, x: '70%', y: '10%', size: 28, delay: 0.8, rotation: -8, color: 'violet' },
    { Icon: Wind, x: '95%', y: '80%', size: 34, delay: 1.3, rotation: 15, color: 'cyan' },
    { Icon: Scissors, x: '3%', y: '25%', size: 30, delay: 1.8, rotation: -20, color: 'blue' },
];

// Emoji bubbles for extra flair
const emojiBubbles = [
    { emoji: '🧺', x: '20%', y: '30%', size: 32, delay: 0.2 },
    { emoji: '👔', x: '75%', y: '35%', size: 28, delay: 0.7 },
    { emoji: '🧼', x: '88%', y: '75%', size: 26, delay: 1.2 },
    { emoji: '✨', x: '12%', y: '60%', size: 24, delay: 1.7 },
    { emoji: '🫧', x: '65%', y: '80%', size: 30, delay: 2.2 },
    { emoji: '🧴', x: '30%', y: '12%', size: 28, delay: 0.4 },
    { emoji: '👕', x: '55%', y: '5%', size: 26, delay: 0.9 },
    { emoji: '🧹', x: '40%', y: '90%', size: 24, delay: 1.4 },
];

const colorMap: Record<string, string> = {
    cyan: 'from-cyan-400/20 to-cyan-500/30 text-cyan-500 dark:text-cyan-400',
    blue: 'from-blue-400/20 to-blue-500/30 text-blue-500 dark:text-blue-400',
    purple: 'from-purple-400/20 to-purple-500/30 text-purple-500 dark:text-purple-400',
    teal: 'from-teal-400/20 to-teal-500/30 text-teal-500 dark:text-teal-400',
    indigo: 'from-indigo-400/20 to-indigo-500/30 text-indigo-500 dark:text-indigo-400',
    sky: 'from-sky-400/20 to-sky-500/30 text-sky-500 dark:text-sky-400',
    emerald: 'from-emerald-400/20 to-emerald-500/30 text-emerald-500 dark:text-emerald-400',
    violet: 'from-violet-400/20 to-violet-500/30 text-violet-500 dark:text-violet-400',
};

// Floating Icon Component
const FloatingIcon: React.FC<{
    Icon: React.ElementType;
    x: string;
    y: string;
    size: number;
    delay: number;
    rotation: number;
    color: string;
}> = ({ Icon, x, y, size, delay, rotation, color }) => {
    const [isHovered, setIsHovered] = useState(false);

    return (
        <motion.div
            className="absolute cursor-pointer hidden md:block"
            style={{ left: x, top: y }}
            initial={{ opacity: 0, scale: 0 }}
            animate={{ opacity: 1, scale: 1 }}
            transition={{ delay: delay + 0.5, duration: 0.5, type: 'spring' }}
        >
            <motion.div
                className={`relative p-4 rounded-2xl backdrop-blur-md bg-gradient-to-br ${colorMap[color]} border border-white/20 dark:border-slate-700/30 shadow-xl`}
                animate={{
                    y: isHovered ? 0 : [0, -10, 0],
                    rotate: isHovered ? 0 : rotation,
                    scale: isHovered ? 1.2 : 1,
                }}
                transition={{
                    y: { duration: 3 + delay, repeat: Infinity, ease: 'easeInOut' },
                    rotate: { duration: 0.3 },
                    scale: { duration: 0.3, type: 'spring', stiffness: 300 },
                }}
                onMouseEnter={() => setIsHovered(true)}
                onMouseLeave={() => setIsHovered(false)}
                whileHover={{
                    boxShadow: '0 20px 40px rgba(6, 182, 212, 0.3)',
                }}
            >
                <Icon style={{ width: size, height: size }} />

                {/* Sparkle effect on hover */}
                {isHovered && (
                    <motion.div
                        className="absolute -top-1 -right-1"
                        initial={{ scale: 0, rotate: 0 }}
                        animate={{ scale: 1, rotate: 180 }}
                        transition={{ duration: 0.3 }}
                    >
                        <Sparkles className="w-4 h-4 text-yellow-400" />
                    </motion.div>
                )}
            </motion.div>
        </motion.div>
    );
};

// Emoji Bubble Component
const EmojiBubble: React.FC<{
    emoji: string;
    x: string;
    y: string;
    size: number;
    delay: number;
}> = ({ emoji, x, y, size, delay }) => {
    const [isHovered, setIsHovered] = useState(false);

    return (
        <motion.div
            className="absolute cursor-pointer select-none hidden sm:block"
            style={{ left: x, top: y, fontSize: size }}
            initial={{ opacity: 0, scale: 0, rotate: -180 }}
            animate={{ opacity: 0.7, scale: 1, rotate: 0 }}
            transition={{ delay: delay + 0.8, duration: 0.6, type: 'spring' }}
            onMouseEnter={() => setIsHovered(true)}
            onMouseLeave={() => setIsHovered(false)}
        >
            <motion.div
                animate={{
                    y: isHovered ? -15 : [0, -8, 0],
                    scale: isHovered ? 1.5 : 1,
                    rotate: isHovered ? [0, -10, 10, 0] : 0,
                }}
                transition={{
                    y: { duration: 2.5 + delay * 0.5, repeat: isHovered ? 0 : Infinity, ease: 'easeInOut' },
                    scale: { duration: 0.2, type: 'spring', stiffness: 400 },
                    rotate: { duration: 0.4 },
                }}
                className="filter drop-shadow-lg"
            >
                <span className="block" style={{ filter: 'drop-shadow(0 4px 8px rgba(0,0,0,0.2))' }}>
                    {emoji}
                </span>
            </motion.div>
        </motion.div>
    );
};

export const HeroSection: React.FC<HeroSectionProps> = ({
    searchQuery,
    onSearchChange,
    onSearchSubmit,
    onSearchFocus,
    showSuggestions,
    suggestions,
    onSuggestionClick,
}) => {
    const scrollToServices = () => {
        document.getElementById('services')?.scrollIntoView({ behavior: 'smooth' });
    };

    return (
        <section className="relative min-h-[90vh] flex items-center justify-center overflow-hidden">
            {/* Animated background blobs */}
            <div className="absolute inset-0 overflow-hidden">
                <div className="absolute -top-40 -right-40 w-80 h-80 bg-cyan-400/30 dark:bg-cyan-500/20 rounded-full blur-3xl animate-blob" />
                <div className="absolute top-40 -left-40 w-96 h-96 bg-blue-400/30 dark:bg-blue-500/20 rounded-full blur-3xl animate-blob animation-delay-2000" />
                <div className="absolute -bottom-40 right-1/3 w-72 h-72 bg-purple-400/20 dark:bg-purple-500/15 rounded-full blur-3xl animate-blob animation-delay-4000" />
            </div>

            {/* Grid pattern overlay */}
            <div className="absolute inset-0 bg-grid-pattern opacity-50" />

            {/* Floating Cleaning Icons Layer */}
            <div className="absolute inset-0 pointer-events-none">
                <div className="relative w-full h-full pointer-events-auto">
                    {floatingIcons.map((iconProps, index) => (
                        <FloatingIcon key={`icon-${index}`} {...iconProps} />
                    ))}
                    {emojiBubbles.map((bubbleProps, index) => (
                        <EmojiBubble key={`emoji-${index}`} {...bubbleProps} />
                    ))}
                </div>
            </div>

            {/* Gradient overlay */}
            <div className="absolute inset-0 bg-gradient-to-b from-transparent via-transparent to-background/80" />

            <div className="container mx-auto px-4 relative z-10">
                <div className="max-w-4xl mx-auto text-center space-y-8">
                    {/* Badge */}
                    <motion.div
                        initial={{ opacity: 0, y: 20 }}
                        animate={{ opacity: 1, y: 0 }}
                        transition={{ duration: 0.5 }}
                        className="inline-flex items-center gap-2 px-5 py-2.5 rounded-full glass-card text-cyan-700 dark:text-cyan-300 text-sm font-medium"
                    >
                        <Sparkles className="w-4 h-4" />
                        <span>Premium Laundry & Dry Cleaning Service</span>
                    </motion.div>

                    {/* Main heading */}
                    <motion.h1
                        initial={{ opacity: 0, y: 30 }}
                        animate={{ opacity: 1, y: 0 }}
                        transition={{ duration: 0.6, delay: 0.1 }}
                        className="text-5xl md:text-7xl font-bold text-slate-900 dark:text-white tracking-tight leading-tight text-shadow-lg"
                    >
                        Fresh Clothes,{' '}
                        <br className="hidden sm:block" />
                        <span className="gradient-text-primary">Zero Hassle</span>
                    </motion.h1>

                    {/* Subtitle */}
                    <motion.p
                        initial={{ opacity: 0, y: 30 }}
                        animate={{ opacity: 1, y: 0 }}
                        transition={{ duration: 0.6, delay: 0.2 }}
                        className="text-xl md:text-2xl text-slate-600 dark:text-slate-300 max-w-2xl mx-auto leading-relaxed"
                    >
                        Experience the ultimate convenience with our door-to-door laundry service.
                        Professional care for your garments, delivered back to you fresh and clean.
                    </motion.p>

                    {/* Search Bar */}
                    <motion.div
                        initial={{ opacity: 0, y: 30 }}
                        animate={{ opacity: 1, y: 0 }}
                        transition={{ duration: 0.6, delay: 0.3 }}
                        className="max-w-2xl mx-auto relative z-20"
                    >
                        <form onSubmit={onSearchSubmit} className="relative group">
                            {/* Glow effect */}
                            <div className="absolute inset-0 bg-gradient-to-r from-cyan-500 to-blue-600 rounded-2xl blur-xl opacity-20 group-hover:opacity-30 group-focus-within:opacity-40 transition-opacity duration-300" />

                            {/* Search input container */}
                            <div className="relative flex items-center glass-strong rounded-2xl shadow-2xl p-2">
                                <Search className="w-6 h-6 text-slate-400 ml-4 flex-shrink-0" />
                                <input
                                    type="text"
                                    placeholder="What needs cleaning? (e.g., Suits, Shirts, Bedding)"
                                    className="flex-1 bg-transparent border-none focus:ring-0 focus:outline-none text-lg px-4 text-slate-900 dark:text-white placeholder:text-slate-400"
                                    value={searchQuery}
                                    onChange={(e) => onSearchChange(e.target.value)}
                                    onFocus={onSearchFocus}
                                />
                                <Button size="lg" className="rounded-xl px-8 shadow-lg" type="submit">
                                    Search
                                </Button>
                            </div>
                        </form>

                        {/* Search Suggestions Dropdown */}
                        {showSuggestions && suggestions.length > 0 && (
                            <motion.div
                                initial={{ opacity: 0, y: -10 }}
                                animate={{ opacity: 1, y: 0 }}
                                exit={{ opacity: 0, y: -10 }}
                                className="absolute top-full left-0 right-0 mt-2 glass-strong rounded-xl shadow-2xl overflow-hidden z-30"
                            >
                                {suggestions.map((service, index) => (
                                    <motion.div
                                        key={service.id}
                                        initial={{ opacity: 0, x: -20 }}
                                        animate={{ opacity: 1, x: 0 }}
                                        transition={{ delay: index * 0.05 }}
                                        className="flex items-center gap-4 p-4 hover:bg-cyan-50/50 dark:hover:bg-slate-700/50 cursor-pointer transition-colors border-b border-slate-100 dark:border-slate-700/50 last:border-0"
                                        onClick={() => onSuggestionClick(service)}
                                    >
                                        {service.imageUrl && (
                                            <img
                                                src={service.imageUrl}
                                                alt={service.name || ''}
                                                className="w-12 h-12 rounded-lg object-cover shadow-md"
                                            />
                                        )}
                                        <div className="flex-1">
                                            <h4 className="font-semibold text-slate-900 dark:text-white">{service.name}</h4>
                                            <p className="text-sm text-cyan-600 dark:text-cyan-400 font-medium">
                                                {service.basePrice !== undefined && service.basePrice !== null
                                                    ? `${service.basePrice.toFixed(0)} ${service.currency || 'EGP'}`
                                                    : 'Price on Request'}
                                            </p>
                                        </div>
                                    </motion.div>
                                ))}
                            </motion.div>
                        )}
                    </motion.div>

                    {/* CTA buttons */}
                    <motion.div
                        initial={{ opacity: 0, y: 30 }}
                        animate={{ opacity: 1, y: 0 }}
                        transition={{ duration: 0.6, delay: 0.4 }}
                        className="flex flex-col sm:flex-row gap-4 justify-center pt-4"
                    >
                        <Button
                            size="lg"
                            className="text-lg px-8 py-4 shadow-xl glow-cyan"
                            onClick={scrollToServices}
                        >
                            Explore Services
                        </Button>
                    </motion.div>
                </div>
            </div>

            {/* Scroll indicator */}
            <motion.div
                initial={{ opacity: 0 }}
                animate={{ opacity: 1 }}
                transition={{ delay: 1 }}
                className="absolute bottom-8 left-1/2 -translate-x-1/2 cursor-pointer z-20"
                onClick={scrollToServices}
            >
                <div className="flex flex-col items-center gap-2 text-slate-400 dark:text-slate-500 animate-bounce-slow">
                    <span className="text-sm font-medium">Scroll to explore</span>
                    <ChevronDown className="w-6 h-6" />
                </div>
            </motion.div>
        </section>
    );
};
