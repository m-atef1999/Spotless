import React from 'react';
import { motion } from 'framer-motion';
import type { CategoryDto } from '../../lib/api';

interface CategoryCardProps {
    category: CategoryDto;
    onClick: () => void;
    index?: number;
}

export const CategoryCard: React.FC<CategoryCardProps> = ({
    category,
    onClick,
    index = 0,
}) => {
    return (
        <motion.div
            initial={{ opacity: 0, scale: 0.9 }}
            whileInView={{ opacity: 1, scale: 1 }}
            viewport={{ once: true, amount: 0.2 }}
            transition={{ duration: 0.5, delay: index * 0.1 }}
            whileHover={{ scale: 1.02 }}
            className="group relative h-72 rounded-2xl overflow-hidden cursor-pointer shadow-xl"
            onClick={onClick}
        >
            {/* Background image or gradient */}
            {category.imageUrl ? (
                <motion.img
                    src={category.imageUrl}
                    alt={category.name || ''}
                    className="absolute inset-0 w-full h-full object-cover"
                    whileHover={{ scale: 1.1 }}
                    transition={{ duration: 0.6 }}
                />
            ) : (
                <div className="absolute inset-0 bg-gradient-to-br from-cyan-600 to-blue-700" />
            )}

            {/* Gradient overlay */}
            <div className="absolute inset-0 bg-gradient-to-t from-black/80 via-black/40 to-transparent" />

            {/* Hover glow effect */}
            <div className="absolute inset-0 opacity-0 group-hover:opacity-100 transition-opacity duration-500">
                <div className="absolute inset-0 bg-gradient-to-t from-cyan-600/30 via-transparent to-transparent" />
            </div>

            {/* Content */}
            <div className="absolute bottom-0 left-0 right-0 p-6">
                <motion.h3
                    className="text-2xl font-bold text-white mb-2 group-hover:text-cyan-300 transition-colors duration-300"
                    initial={{ y: 0 }}
                    whileHover={{ y: -4 }}
                >
                    {category.name}
                </motion.h3>

                {/* Description - slides up on hover */}
                <motion.p
                    className="text-slate-200 text-sm leading-relaxed opacity-0 group-hover:opacity-100 transition-all duration-300 transform translate-y-4 group-hover:translate-y-0"
                >
                    {category.description || 'Professional cleaning service tailored to your needs.'}
                </motion.p>

                {/* Explore button */}
                <motion.div
                    className="mt-4 opacity-0 group-hover:opacity-100 transition-all duration-300 transform translate-y-4 group-hover:translate-y-0"
                >
                    <span className="inline-flex items-center gap-2 text-cyan-300 font-medium text-sm">
                        Explore category
                        <svg className="w-4 h-4 group-hover:translate-x-2 transition-transform" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 8l4 4m0 0l-4 4m4-4H3" />
                        </svg>
                    </span>
                </motion.div>
            </div>

            {/* Corner accent */}
            <div className="absolute top-4 right-4 w-12 h-12 rounded-full bg-white/10 backdrop-blur-sm flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity duration-300">
                <svg className="w-6 h-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
                </svg>
            </div>
        </motion.div>
    );
};
