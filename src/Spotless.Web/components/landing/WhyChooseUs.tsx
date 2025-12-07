import React from 'react';
import { motion } from 'framer-motion';
import { Clock, Shield, MapPin, Sparkles, Leaf } from 'lucide-react';
import { staggerContainer, staggerItem } from '../../hooks/useScrollAnimation';

const features = [
    {
        icon: Clock,
        title: 'Quick Turnaround',
        description: 'Get your clothes back in as little as 24 hours. We value your time as much as you do.',
        color: 'from-cyan-500 to-cyan-600',
    },
    {
        icon: Shield,
        title: 'Premium Care',
        description: 'Expert handling of all fabric types using top-quality, eco-friendly cleaning solutions.',
        color: 'from-blue-500 to-blue-600',
    },
    {
        icon: MapPin,
        title: 'Doorstep Service',
        description: 'Free pickup and delivery at your scheduled time and preferred location.',
        color: 'from-purple-500 to-purple-600',
    },
    {
        icon: Sparkles,
        title: 'Stain Specialists',
        description: 'Our experts tackle even the toughest stains with specialized treatments.',
        color: 'from-amber-500 to-orange-500',
    },
    {
        icon: Leaf,
        title: 'Eco-Friendly',
        description: 'Sustainable cleaning practices that are gentle on your clothes and the environment.',
        color: 'from-emerald-500 to-teal-500',
    },
];

export const WhyChooseUs: React.FC = () => {
    return (
        <section className="py-24 relative overflow-hidden">
            {/* Background decorations */}
            <div className="absolute top-1/2 left-1/4 w-64 h-64 bg-cyan-200/30 dark:bg-cyan-800/20 rounded-full blur-3xl -translate-y-1/2" />
            <div className="absolute top-1/2 right-1/4 w-64 h-64 bg-purple-200/30 dark:bg-purple-800/20 rounded-full blur-3xl -translate-y-1/2" />

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
                        Why Choose Us
                    </span>
                    <h2 className="text-4xl md:text-5xl font-bold text-slate-900 dark:text-white mb-6">
                        The Spotless <span className="gradient-text-primary">Difference</span>
                    </h2>
                    <p className="text-lg text-slate-600 dark:text-slate-400">
                        We go above and beyond to deliver exceptional service that keeps you coming back.
                    </p>
                </motion.div>

                {/* Features grid */}
                <motion.div
                    variants={staggerContainer}
                    initial="hidden"
                    whileInView="visible"
                    viewport={{ once: true, amount: 0.2 }}
                    className="grid md:grid-cols-2 lg:grid-cols-3 gap-8"
                >
                    {features.map((feature, index) => (
                        <motion.div
                            key={index}
                            variants={staggerItem}
                            whileHover={{ y: -8, scale: 1.02 }}
                            className="group relative glass-card rounded-2xl p-8 hover:shadow-2xl hover:shadow-cyan-500/10 transition-all duration-300"
                        >
                            {/* Icon */}
                            <div className={`w-16 h-16 rounded-2xl bg-gradient-to-br ${feature.color} flex items-center justify-center mb-6 shadow-lg group-hover:scale-110 transition-transform duration-300`}>
                                <feature.icon className="w-8 h-8 text-white" />
                            </div>

                            {/* Content */}
                            <h3 className="text-xl font-bold text-slate-900 dark:text-white mb-3 group-hover:text-cyan-600 dark:group-hover:text-cyan-400 transition-colors">
                                {feature.title}
                            </h3>
                            <p className="text-slate-600 dark:text-slate-400 leading-relaxed">
                                {feature.description}
                            </p>

                            {/* Hover accent line */}
                            <div className="absolute bottom-0 left-0 right-0 h-1 bg-gradient-to-r from-transparent via-cyan-500 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300 rounded-b-2xl" />
                        </motion.div>
                    ))}
                </motion.div>
            </div>
        </section>
    );
};
