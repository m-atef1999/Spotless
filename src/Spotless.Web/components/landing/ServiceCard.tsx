import React from 'react';
import { motion } from 'framer-motion';
import { Star, ArrowRight } from 'lucide-react';
import { Button } from '../ui/Button';
import type { ServiceDto } from '../../lib/api';

interface ServiceCardProps {
    service: ServiceDto;
    onBookNow: (e: React.MouseEvent, serviceId: string) => void;
    onClick: () => void;
    index?: number;
}

export const ServiceCard: React.FC<ServiceCardProps> = ({
    service,
    onBookNow,
    onClick,
    index = 0,
}) => {
    return (
        <motion.div
            initial={{ opacity: 0, y: 30 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true, amount: 0.2 }}
            transition={{ duration: 0.5, delay: index * 0.1 }}
            whileHover={{ y: -8 }}
            className="group relative glass-card rounded-2xl overflow-hidden cursor-pointer hover:shadow-2xl hover:shadow-cyan-500/10 transition-all duration-300"
            onClick={onClick}
        >
            {/* Image container with zoom effect */}
            {service.imageUrl && (
                <div className="relative h-48 overflow-hidden">
                    <motion.img
                        src={service.imageUrl}
                        alt={service.name || ''}
                        className="w-full h-full object-cover"
                        whileHover={{ scale: 1.1 }}
                        transition={{ duration: 0.5 }}
                    />
                    {/* Gradient overlay on image */}
                    <div className="absolute inset-0 bg-gradient-to-t from-black/40 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300" />
                </div>
            )}

            {/* Card content */}
            <div className="p-6 flex flex-col">
                {/* Title and price */}
                <div className="flex justify-between items-start mb-3">
                    <h3 className="font-bold text-slate-900 dark:text-white text-lg line-clamp-2 pr-2 group-hover:text-cyan-600 dark:group-hover:text-cyan-400 transition-colors">
                        {service.name}
                    </h3>
                    <div className="flex flex-col items-end gap-1 shrink-0">
                        <span className="bg-gradient-to-r from-cyan-500 to-blue-500 text-white text-xs font-bold px-3 py-1.5 rounded-full whitespace-nowrap shadow-lg">
                            {service.basePrice !== undefined && service.basePrice !== null
                                ? `${service.basePrice.toFixed(0)} ${service.currency || 'EGP'}`
                                : 'Price on Request'}
                        </span>
                        {service.maxWeightKg !== undefined && (
                            <span className="text-xs font-medium text-slate-500 dark:text-slate-400">
                                Max {service.maxWeightKg} KG
                            </span>
                        )}
                    </div>
                </div>

                {/* Description */}
                <p className="text-sm text-slate-600 dark:text-slate-400 mb-4 line-clamp-2 flex-1">
                    {service.description || 'Professional cleaning service for your garments.'}
                </p>

                {/* Footer with rating and book button */}
                <div className="flex items-center justify-between mt-auto pt-3 border-t border-slate-100 dark:border-slate-700/50">
                    <div className="flex items-center gap-1.5">
                        <div className="flex">
                            {[...Array(5)].map((_, i) => (
                                <Star
                                    key={i}
                                    className={`w-4 h-4 ${i < 4 ? 'text-amber-400 fill-amber-400' : 'text-slate-300 dark:text-slate-600'}`}
                                />
                            ))}
                        </div>
                        <span className="text-sm font-medium text-slate-600 dark:text-slate-400">4.9</span>
                    </div>
                    <Button
                        size="sm"
                        variant="outline"
                        className="group-hover:bg-cyan-500 group-hover:text-white group-hover:border-cyan-500 transition-all duration-300"
                        onClick={(e) => onBookNow(e, service.id!)}
                    >
                        Book <ArrowRight className="w-3 h-3 ml-1 group-hover:translate-x-1 transition-transform" />
                    </Button>
                </div>
            </div>

            {/* Decorative corner accent */}
            <div className="absolute top-0 right-0 w-20 h-20 overflow-hidden">
                <div className="absolute -top-10 -right-10 w-20 h-20 bg-gradient-to-br from-cyan-500/20 to-blue-500/20 rotate-45 opacity-0 group-hover:opacity-100 transition-opacity duration-300" />
            </div>
        </motion.div>
    );
};
