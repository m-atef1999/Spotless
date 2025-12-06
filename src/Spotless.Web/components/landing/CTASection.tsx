import React from 'react';
import { motion } from 'framer-motion';
import { Link } from 'react-router-dom';
import { Button } from '../ui/Button';

interface CTASectionProps {
    token: string | null;
}

export const CTASection: React.FC<CTASectionProps> = ({ token }) => {
    return (
        <section className="py-24 relative overflow-hidden">
            {/* Background image with overlay */}
            <div
                className="absolute inset-0 bg-cover bg-center"
                style={{
                    backgroundImage: "url('https://images.unsplash.com/photo-1545173168-9f1947eebb8f?auto=format&fit=crop&q=80')",
                }}
            />
            <div className="absolute inset-0 bg-gradient-to-br from-slate-900/95 via-slate-900/90 to-cyan-900/80" />

            {/* Decorative elements */}
            <div className="absolute top-20 left-20 w-40 h-40 bg-cyan-500/20 rounded-full blur-3xl" />
            <div className="absolute bottom-20 right-20 w-40 h-40 bg-purple-500/20 rounded-full blur-3xl" />

            <div className="container mx-auto px-4 relative z-10">
                <motion.div
                    initial={{ opacity: 0, y: 30 }}
                    whileInView={{ opacity: 1, y: 0 }}
                    viewport={{ once: true }}
                    transition={{ duration: 0.6 }}
                    className="max-w-3xl mx-auto text-center"
                >
                    {/* Badge */}
                    <motion.div
                        initial={{ opacity: 0, scale: 0.9 }}
                        whileInView={{ opacity: 1, scale: 1 }}
                        viewport={{ once: true }}
                        transition={{ delay: 0.2 }}
                        className="inline-block mb-6"
                    >
                        <span className="px-4 py-2 rounded-full bg-cyan-500/20 text-cyan-300 text-sm font-medium border border-cyan-500/30">
                            ✨ Limited Time Offer
                        </span>
                    </motion.div>

                    <h2 className="text-4xl md:text-6xl font-bold text-white mb-6 leading-tight">
                        Ready for a{' '}
                        <span className="bg-gradient-to-r from-cyan-400 to-blue-400 bg-clip-text text-transparent">
                            Spotless
                        </span>{' '}
                        Experience?
                    </h2>

                    <p className="text-xl text-slate-300 mb-10 leading-relaxed">
                        Join thousands of satisfied customers who trust us with their garments.
                        <br />
                        <span className="text-cyan-400 font-semibold">First order gets 20% off!</span>
                    </p>

                    <motion.div
                        initial={{ opacity: 0, y: 20 }}
                        whileInView={{ opacity: 1, y: 0 }}
                        viewport={{ once: true }}
                        transition={{ delay: 0.4 }}
                        className="flex flex-col sm:flex-row gap-4 justify-center"
                    >
                        {token ? (
                            <Link to="/customer/new-order">
                                <Button
                                    size="lg"
                                    className="text-lg px-10 py-6 bg-gradient-to-r from-cyan-500 to-blue-500 hover:from-cyan-600 hover:to-blue-600 border-0 shadow-2xl shadow-cyan-500/30 glow-cyan"
                                >
                                    Place an Order
                                </Button>
                            </Link>
                        ) : (
                            <>
                                <Link to="/register">
                                    <Button
                                        size="lg"
                                        className="text-lg px-10 py-6 bg-gradient-to-r from-cyan-500 to-blue-500 hover:from-cyan-600 hover:to-blue-600 border-0 shadow-2xl shadow-cyan-500/30 glow-cyan"
                                    >
                                        Get Started Now
                                    </Button>
                                </Link>
                                <Link to="/services">
                                    <Button
                                        size="lg"
                                        variant="outline"
                                        className="text-lg px-10 py-6 border-2 border-slate-500 text-white hover:bg-white/10 hover:border-slate-400"
                                    >
                                        View Pricing
                                    </Button>
                                </Link>
                            </>
                        )}
                    </motion.div>

                    {/* Trust indicators */}
                    <motion.div
                        initial={{ opacity: 0 }}
                        whileInView={{ opacity: 1 }}
                        viewport={{ once: true }}
                        transition={{ delay: 0.6 }}
                        className="mt-12 flex flex-wrap justify-center gap-8 text-slate-400"
                    >
                        <div className="flex items-center gap-2">
                            <svg className="w-5 h-5 text-green-400" fill="currentColor" viewBox="0 0 20 20">
                                <path fillRule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clipRule="evenodd" />
                            </svg>
                            <span>Free Pickup & Delivery</span>
                        </div>
                        <div className="flex items-center gap-2">
                            <svg className="w-5 h-5 text-green-400" fill="currentColor" viewBox="0 0 20 20">
                                <path fillRule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clipRule="evenodd" />
                            </svg>
                            <span>24-Hour Turnaround</span>
                        </div>
                        <div className="flex items-center gap-2">
                            <svg className="w-5 h-5 text-green-400" fill="currentColor" viewBox="0 0 20 20">
                                <path fillRule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clipRule="evenodd" />
                            </svg>
                            <span>Satisfaction Guaranteed</span>
                        </div>
                    </motion.div>
                </motion.div>
            </div>
        </section>
    );
};
