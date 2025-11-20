import React from 'react';
import { motion } from 'framer-motion';

interface AuthLayoutProps {
    children: React.ReactNode;
    title: string;
    subtitle: string;
}

export const AuthLayout: React.FC<AuthLayoutProps> = ({ children, title, subtitle }) => {
    return (
        <div className="min-h-screen flex w-full bg-slate-50 dark:bg-slate-950">
            {/* Left Side - Form */}
            <div className="flex-1 flex items-center justify-center p-8 sm:p-12 lg:p-16 relative overflow-hidden">
                {/* Background Blobs for Left Side */}
                <div className="absolute top-0 left-0 w-full h-full overflow-hidden pointer-events-none">
                    <div className="absolute -top-[20%] -left-[10%] w-[50%] h-[50%] rounded-full bg-cyan-400/10 blur-3xl animate-blob"></div>
                    <div className="absolute top-[40%] -right-[10%] w-[40%] h-[40%] rounded-full bg-teal-400/10 blur-3xl animate-blob animation-delay-2000"></div>
                </div>

                <motion.div
                    initial={{ opacity: 0, y: 20 }}
                    animate={{ opacity: 1, y: 0 }}
                    transition={{ duration: 0.5 }}
                    className="w-full max-w-md relative z-10"
                >
                    {/* Logo */}
                    <div className="flex justify-center mb-8">
                        <div className="relative group">
                            <div className="absolute inset-0 bg-gradient-to-r from-cyan-400 to-teal-400 rounded-2xl blur-xl opacity-40 group-hover:opacity-60 transition-opacity duration-500"></div>
                            <div className="relative p-4 bg-white dark:bg-slate-900 rounded-2xl shadow-xl shadow-cyan-500/20 border border-cyan-100 dark:border-cyan-900/50">
                                <img src="/logos/spotless_logo.png" alt="Spotless Logo" className="w-10 h-10 object-contain" />
                            </div>
                        </div>
                    </div>

                    {/* Title */}
                    <div className="text-center mb-8">
                        <h1 className="text-4xl font-bold text-slate-900 dark:text-white mb-3 tracking-tight">
                            {title}
                        </h1>
                        <p className="text-slate-500 dark:text-slate-400 text-base">
                            {subtitle}
                        </p>
                    </div>

                    {/* Form Card */}
                    <motion.div
                        initial={{ opacity: 0 }}
                        animate={{ opacity: 1 }}
                        transition={{ delay: 0.2, duration: 0.5 }}
                        className="bg-white/80 dark:bg-slate-900/80 backdrop-blur-xl rounded-3xl shadow-xl shadow-slate-200/50 dark:shadow-slate-950/50 p-8 border border-white/50 dark:border-slate-800"
                    >
                        {children}
                    </motion.div>

                    {/* Footer */}
                    <p className="text-center text-sm text-slate-400 dark:text-slate-500 mt-8 font-medium">
                        © 2024 Spotless. Clean. Simple. Spotless.
                    </p>
                </motion.div>
            </div>

            {/* Right Side - Branding */}
            <div className="hidden lg:flex relative flex-1 bg-slate-900 items-center justify-center overflow-hidden">
                {/* Image Background with Overlay */}
                <div className="absolute inset-0 z-0">
                    <img
                        src="https://images.unsplash.com/photo-1581578731117-104f2a8d275d?q=80&w=1920&auto=format&fit=crop"
                        alt="Clean Laundry"
                        className="w-full h-full object-cover opacity-40"
                    />
                    <div className="absolute inset-0 bg-gradient-to-br from-cyan-900/90 to-teal-900/90 mix-blend-multiply"></div>
                </div>

                {/* Animated Background Orbs */}
                <div className="absolute top-20 left-20 w-96 h-96 bg-cyan-500 rounded-full mix-blend-overlay filter blur-[100px] opacity-40 animate-blob"></div>
                <div className="absolute bottom-20 right-20 w-96 h-96 bg-teal-500 rounded-full mix-blend-overlay filter blur-[100px] opacity-40 animate-blob animation-delay-2000"></div>

                {/* Content */}
                <div className="relative z-10 text-center text-white px-12 max-w-xl">
                    <motion.div
                        initial={{ opacity: 0, scale: 0.95 }}
                        animate={{ opacity: 1, scale: 1 }}
                        transition={{ duration: 0.8, ease: "easeOut" }}
                    >
                        {/* Logo */}
                        <div className="mb-10 flex justify-center">
                            <div className="p-6 bg-white/10 backdrop-blur-md rounded-full border border-white/20 shadow-2xl ring-4 ring-white/5">
                                <img src="/logos/spotless_logo.png" alt="Spotless Logo" className="w-20 h-20 object-contain" />
                            </div>
                        </div>

                        {/* Text */}
                        <h2 className="text-5xl font-bold mb-6 leading-tight tracking-tight">
                            Experience the <br />
                            <span className="text-transparent bg-clip-text bg-gradient-to-r from-cyan-200 to-teal-200">Purest Clean</span>
                        </h2>
                        <p className="text-xl text-cyan-100/80 leading-relaxed mb-10 font-light">
                            Clean. Simple. Spotless.
                        </p>

                        {/* Stats */}
                        <div className="flex items-center justify-center gap-12 p-8 bg-white/5 backdrop-blur-sm rounded-3xl border border-white/10">
                            <div className="text-center">
                                <div className="text-3xl font-bold text-white">10K+</div>
                                <div className="text-sm text-cyan-200/70 font-medium uppercase tracking-wider mt-1">Happy Users</div>
                            </div>
                            <div className="w-px h-12 bg-white/10"></div>
                            <div className="text-center">
                                <div className="text-3xl font-bold text-white">4.9</div>
                                <div className="text-sm text-cyan-200/70 font-medium uppercase tracking-wider mt-1">Top Rated</div>
                            </div>
                        </div>
                    </motion.div>
                </div>
            </div>
        </div>
    );
};
