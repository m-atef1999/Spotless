import React from 'react';
import { motion } from 'framer-motion';
import { Link } from 'react-router-dom';
import { MapPin, Phone, Mail, Facebook, Twitter, Instagram, Linkedin } from 'lucide-react';
import logo from '../../assets/logo.png';

const socialLinks = [
    { icon: Facebook, href: '#', label: 'Facebook' },
    { icon: Twitter, href: '#', label: 'Twitter' },
    { icon: Instagram, href: '#', label: 'Instagram' },
    { icon: Linkedin, href: '#', label: 'LinkedIn' },
];

const footerLinks = {
    services: [
        { label: 'Dry Cleaning', href: '/services' },
        { label: 'Wash & Fold', href: '/services' },
        { label: 'Ironing', href: '/services' },
        { label: 'Alterations', href: '/services' },
    ],
    company: [
        { label: 'About Us', href: '#' },
        { label: 'Careers', href: '#' },
        { label: 'Privacy Policy', href: '#' },
        { label: 'Terms of Service', href: '#' },
    ],
};

export const Footer: React.FC = () => {
    return (
        <footer className="relative bg-slate-900 pt-20 pb-8 overflow-hidden">
            {/* Top gradient accent */}
            <div className="absolute top-0 left-0 right-0 h-1 bg-gradient-to-r from-cyan-500 via-blue-500 to-purple-500" />

            {/* Background decoration */}
            <div className="absolute top-20 right-10 w-64 h-64 bg-cyan-500/5 rounded-full blur-3xl" />
            <div className="absolute bottom-20 left-10 w-64 h-64 bg-purple-500/5 rounded-full blur-3xl" />

            <div className="container mx-auto px-4 relative z-10">
                <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-12 mb-16">
                    {/* Brand column */}
                    <motion.div
                        initial={{ opacity: 0, y: 20 }}
                        whileInView={{ opacity: 1, y: 0 }}
                        viewport={{ once: true }}
                        transition={{ duration: 0.5 }}
                        className="lg:col-span-1"
                    >
                        <div className="flex items-center gap-3 mb-6">
                            <img src={logo} alt="Spotless Logo" className="h-10 w-auto" />
                            <span className="text-2xl font-bold text-white">Spotless</span>
                        </div>
                        <p className="text-slate-400 mb-6 leading-relaxed">
                            Premium laundry and dry cleaning service delivered to your doorstep.
                            Quality care for your garments.
                        </p>
                        {/* Social links */}
                        <div className="flex gap-4">
                            {socialLinks.map((social, index) => (
                                <motion.a
                                    key={index}
                                    href={social.href}
                                    whileHover={{ scale: 1.1, y: -2 }}
                                    className="w-10 h-10 rounded-full bg-slate-800 flex items-center justify-center text-slate-400 hover:bg-cyan-500 hover:text-white transition-colors duration-300"
                                    aria-label={social.label}
                                >
                                    <social.icon className="w-5 h-5" />
                                </motion.a>
                            ))}
                        </div>
                    </motion.div>

                    {/* Services column */}
                    <motion.div
                        initial={{ opacity: 0, y: 20 }}
                        whileInView={{ opacity: 1, y: 0 }}
                        viewport={{ once: true }}
                        transition={{ duration: 0.5, delay: 0.1 }}
                    >
                        <h4 className="text-white font-bold text-lg mb-6">Services</h4>
                        <ul className="space-y-3">
                            {footerLinks.services.map((link, index) => (
                                <li key={index}>
                                    <Link
                                        to={link.href}
                                        className="text-slate-400 hover:text-cyan-400 transition-colors duration-300 inline-flex items-center group"
                                    >
                                        <span className="w-0 group-hover:w-2 h-0.5 bg-cyan-400 mr-0 group-hover:mr-2 transition-all duration-300" />
                                        {link.label}
                                    </Link>
                                </li>
                            ))}
                        </ul>
                    </motion.div>

                    {/* Company column */}
                    <motion.div
                        initial={{ opacity: 0, y: 20 }}
                        whileInView={{ opacity: 1, y: 0 }}
                        viewport={{ once: true }}
                        transition={{ duration: 0.5, delay: 0.2 }}
                    >
                        <h4 className="text-white font-bold text-lg mb-6">Company</h4>
                        <ul className="space-y-3">
                            {footerLinks.company.map((link, index) => (
                                <li key={index}>
                                    <a
                                        href={link.href}
                                        className="text-slate-400 hover:text-cyan-400 transition-colors duration-300 inline-flex items-center group"
                                    >
                                        <span className="w-0 group-hover:w-2 h-0.5 bg-cyan-400 mr-0 group-hover:mr-2 transition-all duration-300" />
                                        {link.label}
                                    </a>
                                </li>
                            ))}
                        </ul>
                    </motion.div>

                    {/* Contact column */}
                    <motion.div
                        initial={{ opacity: 0, y: 20 }}
                        whileInView={{ opacity: 1, y: 0 }}
                        viewport={{ once: true }}
                        transition={{ duration: 0.5, delay: 0.3 }}
                    >
                        <h4 className="text-white font-bold text-lg mb-6">Contact</h4>
                        <ul className="space-y-4">
                            <li className="flex items-start gap-3">
                                <div className="w-10 h-10 rounded-lg bg-slate-800 flex items-center justify-center shrink-0">
                                    <MapPin className="w-5 h-5 text-cyan-400" />
                                </div>
                                <span className="text-slate-400">123 Clean Street, Cairo, Egypt</span>
                            </li>
                            <li className="flex items-start gap-3">
                                <div className="w-10 h-10 rounded-lg bg-slate-800 flex items-center justify-center shrink-0">
                                    <Phone className="w-5 h-5 text-cyan-400" />
                                </div>
                                <span className="text-slate-400">+20 123 456 7890</span>
                            </li>
                            <li className="flex items-start gap-3">
                                <div className="w-10 h-10 rounded-lg bg-slate-800 flex items-center justify-center shrink-0">
                                    <Mail className="w-5 h-5 text-cyan-400" />
                                </div>
                                <span className="text-slate-400">support@spotless.com</span>
                            </li>
                        </ul>
                    </motion.div>
                </div>

                {/* Bottom bar */}
                <div className="border-t border-slate-800 pt-8">
                    <div className="flex flex-col md:flex-row justify-between items-center gap-4">
                        <p className="text-slate-500 text-sm">
                            © {new Date().getFullYear()} Spotless. All rights reserved.
                        </p>
                        <div className="flex gap-6 text-sm text-slate-500">
                            <a href="#" className="hover:text-cyan-400 transition-colors">Privacy</a>
                            <a href="#" className="hover:text-cyan-400 transition-colors">Terms</a>
                            <a href="#" className="hover:text-cyan-400 transition-colors">Cookies</a>
                        </div>
                    </div>
                </div>
            </div>
        </footer>
    );
};
