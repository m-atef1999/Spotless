import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { motion, useScroll, useTransform } from 'framer-motion';
import { ArrowRight, Menu, X } from 'lucide-react';
import { Button } from '../components/ui/Button';
import { ThemeToggle } from '../components/ui/ThemeToggle';
import { CategoriesService, ServicesService, type CategoryDto, type ServiceDto, type PagedResponse } from '../lib/api';
import { useAuthStore } from '../store/authStore';
import logo from '../assets/logo.png';
import { AiChatWidget } from '../components/ai/AiChatWidget';
import { BackToTop } from '../components/ui/BackToTop';
import { getCached, setCache, CACHE_KEYS, CACHE_TTL } from '../utils/cacheUtils';

import {
    HeroSection,
    ServiceCard,
    CategoryCard,
    WhyChooseUs,
    TestimonialCarousel,
    CTASection,
    Footer,
} from '../components/landing';

export const MainPage: React.FC = () => {
    const navigate = useNavigate();
    const { role, token } = useAuthStore();
    const [categories, setCategories] = useState<CategoryDto[]>([]);
    const [services, setServices] = useState<ServiceDto[]>([]);
    // Store ALL services/categories for instant local search filtering
    const [allServices, setAllServices] = useState<ServiceDto[]>([]);
    const [allCategories, setAllCategories] = useState<CategoryDto[]>([]);
    const [searchQuery, setSearchQuery] = useState('');
    const [suggestions, setSuggestions] = useState<ServiceDto[]>([]);
    const [showSuggestions, setShowSuggestions] = useState(false);
    const [isLoading, setIsLoading] = useState(true);
    const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

    // Scroll-based navbar effects
    const { scrollY } = useScroll();
    const headerShadow = useTransform(
        scrollY,
        [0, 100],
        ['0 0 0 0 rgba(0,0,0,0)', '0 4px 30px rgba(0, 0, 0, 0.1)']
    );

    useEffect(() => {
        const fetchData = async () => {
            // First, try to show cached data immediately for instant perceived loading
            const cachedCategories = getCached<CategoryDto[]>(CACHE_KEYS.CATEGORIES_ALL);
            const cachedServices = getCached<ServiceDto[]>(CACHE_KEYS.SERVICES_ALL);

            if (cachedCategories && cachedServices) {
                setCategories(cachedCategories.slice(0, 4));
                setServices(cachedServices.slice(0, 8));
                // Store full lists for instant search
                setAllCategories(cachedCategories);
                setAllServices(cachedServices);
                setIsLoading(false);
            }

            // Then fetch fresh data in background
            try {
                const [categoriesData, servicesData] = await Promise.all([
                    CategoriesService.getApiCategories({ pageNumber: 1, pageSize: 50 }),
                    ServicesService.getApiServices({ pageNumber: 1, pageSize: 100 })
                ]);

                const freshCategories = (categoriesData as PagedResponse).data || [];
                const freshServices = (servicesData as PagedResponse).data || [];

                // Cache the full results
                setCache(CACHE_KEYS.CATEGORIES_ALL, freshCategories, CACHE_TTL.CATEGORIES);
                setCache(CACHE_KEYS.SERVICES_ALL, freshServices, CACHE_TTL.SERVICES);

                // Update UI with fresh data (limited for display)
                setCategories(freshCategories.slice(0, 4));
                setServices(freshServices.slice(0, 8));
                // Store full lists for instant search
                setAllCategories(freshCategories);
                setAllServices(freshServices);
            } catch (error) {
                console.error('Failed to fetch data:', error);
                // If we had cached data, we're fine. If not, show error state.
            } finally {
                setIsLoading(false);
            }
        };
        fetchData();
    }, []);

    // OPTIMIZED: Filter locally from cached data - no API calls needed!
    useEffect(() => {
        if (searchQuery.length > 1) {
            const searchLower = searchQuery.toLowerCase();

            // First, search services by name
            let results = allServices.filter(s =>
                s.name?.toLowerCase().includes(searchLower)
            );

            // If no direct service matches, check if search matches a category name
            if (results.length === 0) {
                const matchedCategory = allCategories.find(c =>
                    c.name?.toLowerCase().includes(searchLower)
                );

                if (matchedCategory) {
                    // Show services from that category
                    results = allServices.filter(s => s.categoryId === matchedCategory.id);
                }
            }

            // Limit to 5 suggestions for UI
            setSuggestions(results.slice(0, 5));
            setShowSuggestions(true);
        } else {
            setSuggestions([]);
            setShowSuggestions(false);
        }
    }, [searchQuery, allServices, allCategories]);

    const handleSearch = (e: React.FormEvent) => {
        e.preventDefault();
        if (searchQuery.trim()) {
            navigate(`/services?search=${encodeURIComponent(searchQuery)}`);
        }
    };

    const handleSearchChange = (value: string) => {
        setSearchQuery(value);
    };

    const handleSearchFocus = () => {
        if (searchQuery.length > 1) {
            setShowSuggestions(true);
        }
    };

    const handleSuggestionClick = (service: ServiceDto) => {
        setSearchQuery(service.name || '');
        setShowSuggestions(false);
        navigate(`/services?search=${encodeURIComponent(service.name || '')}`);
    };

    const handleCategoryClick = (categoryId: string, categoryName: string) => {
        navigate(`/services?categoryId=${categoryId}&categoryName=${encodeURIComponent(categoryName)}`);
    };

    const handleServiceClick = (service: ServiceDto) => {
        navigate(`/services?search=${encodeURIComponent(service.name || '')}`);
    };

    const handleBookNow = (e: React.MouseEvent, serviceId: string) => {
        e.stopPropagation(); // Prevent triggering the card click
        if (token) {
            navigate(`/customer/new-order?serviceId=${serviceId}`);
        } else {
            navigate('/login');
        }
    };

    // Close suggestions when clicking outside
    useEffect(() => {
        const handleClickOutside = () => setShowSuggestions(false);
        if (showSuggestions) {
            document.addEventListener('click', handleClickOutside);
            return () => document.removeEventListener('click', handleClickOutside);
        }
    }, [showSuggestions]);

    return (
        <div className="min-h-screen bg-slate-50 dark:bg-slate-900 transition-colors duration-300">
            {/* Sticky Header with scroll effects */}
            <motion.header
                className="sticky top-0 z-50 backdrop-blur-xl border-b border-slate-200/50 dark:border-slate-800/50"
                style={{
                    boxShadow: headerShadow,
                }}
            >
                <div className="absolute inset-0 bg-white/80 dark:bg-slate-900/90 backdrop-blur-xl" />
                <div className="container mx-auto px-4 h-20 flex items-center justify-between relative z-10">
                    {/* Logo */}
                    <motion.div
                        className="flex items-center gap-3"
                        whileHover={{ scale: 1.02 }}
                    >
                        <Link to="/" className="flex items-center gap-3">
                            <img src={logo} alt="Spotless Logo" className="h-10 w-auto" />
                            <span className="text-2xl font-bold gradient-text-primary">
                                Spotless
                            </span>
                        </Link>
                    </motion.div>

                    {/* Desktop Navigation */}
                    <nav className="hidden md:flex items-center gap-8">
                        {[
                            { href: '#services', label: 'Services' },
                            { href: '#why-choose-us', label: 'Why Us' },
                            { href: '#testimonials', label: 'Reviews' },
                        ].map((link) => (
                            <a
                                key={link.href}
                                href={link.href}
                                className="relative text-slate-600 dark:text-slate-300 hover:text-cyan-600 dark:hover:text-cyan-400 font-medium transition-colors group"
                            >
                                {link.label}
                                <span className="absolute -bottom-1 left-0 w-0 h-0.5 bg-gradient-to-r from-cyan-500 to-blue-500 group-hover:w-full transition-all duration-300" />
                            </a>
                        ))}
                    </nav>

                    {/* Right side actions */}
                    <div className="flex items-center gap-4">
                        <ThemeToggle />

                        {/* Desktop Auth buttons */}
                        <div className="hidden md:flex items-center gap-3">
                            {token ? (
                                <Link to={
                                    role === 'Driver' ? '/driver/dashboard' :
                                        role === 'Admin' ? '/admin/dashboard' :
                                            '/customer/dashboard'
                                }>
                                    <Button className="shadow-lg">
                                        Dashboard
                                    </Button>
                                </Link>
                            ) : (
                                <>
                                    <Link to="/login">
                                        <Button variant="ghost">Sign In</Button>
                                    </Link>
                                    <Link to="/register">
                                        <Button className="shadow-lg">Get Started</Button>
                                    </Link>
                                </>
                            )}
                        </div>

                        {/* Mobile menu button */}
                        <button
                            className="md:hidden p-2 rounded-lg hover:bg-slate-100 dark:hover:bg-slate-800 transition-colors"
                            onClick={() => setMobileMenuOpen(!mobileMenuOpen)}
                            aria-label="Toggle menu"
                        >
                            {mobileMenuOpen ? (
                                <X className="w-6 h-6 text-slate-600 dark:text-slate-300" />
                            ) : (
                                <Menu className="w-6 h-6 text-slate-600 dark:text-slate-300" />
                            )}
                        </button>
                    </div>
                </div>

                {/* Mobile menu */}
                {mobileMenuOpen && (
                    <motion.div
                        initial={{ opacity: 0, height: 0 }}
                        animate={{ opacity: 1, height: 'auto' }}
                        exit={{ opacity: 0, height: 0 }}
                        className="md:hidden border-t border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900"
                    >
                        <div className="container mx-auto px-4 py-4 space-y-4">
                            <a href="#services" className="block py-2 text-slate-600 dark:text-slate-300 hover:text-cyan-600" onClick={() => setMobileMenuOpen(false)}>Services</a>
                            <a href="#why-choose-us" className="block py-2 text-slate-600 dark:text-slate-300 hover:text-cyan-600" onClick={() => setMobileMenuOpen(false)}>Why Us</a>
                            <a href="#testimonials" className="block py-2 text-slate-600 dark:text-slate-300 hover:text-cyan-600" onClick={() => setMobileMenuOpen(false)}>Reviews</a>
                            <div className="pt-4 border-t border-slate-200 dark:border-slate-800">
                                {token ? (
                                    <Link to={role === 'Driver' ? '/driver/dashboard' : role === 'Admin' ? '/admin/dashboard' : '/customer/dashboard'} onClick={() => setMobileMenuOpen(false)}>
                                        <Button className="w-full">Dashboard</Button>
                                    </Link>
                                ) : (
                                    <div className="space-y-2">
                                        <Link to="/login" onClick={() => setMobileMenuOpen(false)}>
                                            <Button variant="outline" className="w-full">Sign In</Button>
                                        </Link>
                                        <Link to="/register" onClick={() => setMobileMenuOpen(false)}>
                                            <Button className="w-full">Get Started</Button>
                                        </Link>
                                    </div>
                                )}
                            </div>
                        </div>
                    </motion.div>
                )}
            </motion.header>

            {/* Hero Section */}
            <HeroSection
                searchQuery={searchQuery}
                onSearchChange={handleSearchChange}
                onSearchSubmit={handleSearch}
                onSearchFocus={handleSearchFocus}
                showSuggestions={showSuggestions}
                suggestions={suggestions}
                onSuggestionClick={handleSuggestionClick}
            />

            {/* Popular Services Section */}
            <section className="py-24 bg-white dark:bg-slate-900" id="services">
                <div className="container mx-auto px-4">
                    <motion.div
                        initial={{ opacity: 0, y: 20 }}
                        whileInView={{ opacity: 1, y: 0 }}
                        viewport={{ once: true }}
                        transition={{ duration: 0.6 }}
                        className="text-center max-w-3xl mx-auto mb-16"
                    >
                        <span className="inline-block text-cyan-600 dark:text-cyan-400 font-semibold text-sm uppercase tracking-wider mb-4">
                            Popular Services
                        </span>
                        <h2 className="text-4xl md:text-5xl font-bold text-slate-900 dark:text-white mb-6">
                            Most <span className="gradient-text-primary">Requested</span> Services
                        </h2>
                        <p className="text-lg text-slate-600 dark:text-slate-400">
                            From delicate fabrics to everyday wear, we handle it all with care and expertise.
                        </p>
                    </motion.div>

                    {/* Services grid */}
                    <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-8">
                        {isLoading ? (
                            Array(4).fill(0).map((_, i) => (
                                <div key={i} className="h-80 bg-slate-200 dark:bg-slate-800 rounded-2xl animate-pulse" />
                            ))
                        ) : (
                            services.map((service, index) => (
                                <ServiceCard
                                    key={service.id}
                                    service={service}
                                    onBookNow={handleBookNow}
                                    onClick={() => handleServiceClick(service)}
                                    index={index}
                                />
                            ))
                        )}
                    </div>

                    {/* View all button */}
                    <motion.div
                        initial={{ opacity: 0, y: 20 }}
                        whileInView={{ opacity: 1, y: 0 }}
                        viewport={{ once: true }}
                        transition={{ delay: 0.4 }}
                        className="text-center mt-12"
                    >
                        <Link to="/services">
                            <Button size="lg" className="rounded-full px-8 shadow-xl glow-cyan">
                                View All Services <ArrowRight className="w-5 h-5 ml-2" />
                            </Button>
                        </Link>
                    </motion.div>
                </div>
            </section>

            {/* Categories Section */}
            <section className="py-24 bg-slate-50 dark:bg-slate-800/50" id="categories">
                <div className="container mx-auto px-4">
                    <motion.div
                        initial={{ opacity: 0, y: 20 }}
                        whileInView={{ opacity: 1, y: 0 }}
                        viewport={{ once: true }}
                        transition={{ duration: 0.6 }}
                        className="flex flex-col md:flex-row justify-between items-start md:items-end gap-4 mb-12"
                    >
                        <div>
                            <span className="inline-block text-cyan-600 dark:text-cyan-400 font-semibold text-sm uppercase tracking-wider mb-4">
                                Categories
                            </span>
                            <h2 className="text-4xl md:text-5xl font-bold text-slate-900 dark:text-white mb-4">
                                Popular <span className="gradient-text-primary">Categories</span>
                            </h2>
                            <p className="text-lg text-slate-600 dark:text-slate-400">
                                Everything you need, cleaned to perfection.
                            </p>
                        </div>
                        <Link to="/services">
                            <Button variant="ghost" className="gap-2 group">
                                View All <ArrowRight className="w-4 h-4 group-hover:translate-x-1 transition-transform" />
                            </Button>
                        </Link>
                    </motion.div>

                    {/* Categories grid */}
                    <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-6">
                        {isLoading ? (
                            Array(4).fill(0).map((_, i) => (
                                <div key={i} className="h-72 bg-slate-200 dark:bg-slate-800 rounded-2xl animate-pulse" />
                            ))
                        ) : (
                            categories.map((category, index) => (
                                <CategoryCard
                                    key={category.id}
                                    category={category}
                                    onClick={() => handleCategoryClick(category.id || '', category.name || '')}
                                    index={index}
                                />
                            ))
                        )}
                    </div>
                </div>
            </section>

            {/* Why Choose Us Section */}
            <section id="why-choose-us" className="bg-white dark:bg-slate-900">
                <WhyChooseUs />
            </section>

            {/* Testimonials Section */}
            <section id="testimonials">
                <TestimonialCarousel />
            </section>

            {/* CTA Section */}
            <CTASection token={token} />

            {/* Footer */}
            <Footer />

            {/* Floating components */}
            <AiChatWidget />
            <BackToTop />
        </div>
    );
};
