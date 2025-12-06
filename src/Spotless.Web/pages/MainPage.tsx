import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Search, ArrowRight, Star, Clock, Shield, Sparkles, MapPin, Phone, Mail } from 'lucide-react';
import { Button } from '../components/ui/Button';
import { ThemeToggle } from '../components/ui/ThemeToggle';
import { CategoriesService, ServicesService, type CategoryDto, type ServiceDto, type PagedResponse } from '../lib/api';
import { useAuthStore } from '../store/authStore';
import logo from '../assets/logo.png';
import { AiChatWidget } from '../components/ai/AiChatWidget';
import { BackToTop } from '../components/ui/BackToTop';
import { getCached, setCache, CACHE_KEYS, CACHE_TTL } from '../utils/cacheUtils';

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

    const handleCategoryClick = (categoryId: string, categoryName: string) => {
        navigate(`/services?categoryId=${categoryId}&categoryName=${encodeURIComponent(categoryName)}`);
    };

    const handleBookNow = (e: React.MouseEvent, serviceId: string) => {
        e.stopPropagation(); // Prevent triggering the card click
        if (token) {
            navigate(`/customer/new-order?serviceId=${serviceId}`);
        } else {
            navigate('/login');
        }
    };

    return (
        <div className="min-h-screen bg-slate-50 dark:bg-slate-900 transition-colors duration-300">
            {/* Header */}
            <header className="sticky top-0 z-50 bg-white/80 dark:bg-slate-900/80 backdrop-blur-md border-b border-slate-200 dark:border-slate-800">
                <div className="container mx-auto px-4 h-20 flex items-center justify-between">
                    <div className="flex items-center gap-3">
                        <img src={logo} alt="Spotless Logo" className="h-10 w-auto" />
                        <span className="text-2xl font-bold bg-gradient-to-r from-cyan-600 to-blue-600 bg-clip-text text-transparent">
                            Spotless
                        </span>
                    </div>

                    <nav className="hidden md:flex items-center gap-8">
                        <a href="#services" className="text-slate-600 dark:text-slate-300 hover:text-cyan-600 dark:hover:text-cyan-400 font-medium transition-colors">Services</a>
                        <a href="#how-it-works" className="text-slate-600 dark:text-slate-300 hover:text-cyan-600 dark:hover:text-cyan-400 font-medium transition-colors">How it Works</a>
                        <a href="#services" className="text-slate-600 dark:text-slate-300 hover:text-cyan-600 dark:hover:text-cyan-400 font-medium transition-colors">Pricing</a>
                    </nav>

                    <div className="flex items-center gap-4">
                        <ThemeToggle />
                        {token ? (
                            <Link to={
                                role === 'Driver' ? '/driver/dashboard' :
                                    role === 'Admin' ? '/admin/dashboard' :
                                        '/customer/dashboard'
                            }>
                                <Button>Dashboard</Button>
                            </Link>
                        ) : (
                            <>
                                <Link to="/login">
                                    <Button variant="ghost" className="hidden sm:flex">Sign In</Button>
                                </Link>
                                <Link to="/register">
                                    <Button>Get Started</Button>
                                </Link>
                            </>
                        )}
                    </div>
                </div>
            </header>

            {/* Hero Section */}
            <section className="relative pt-20 pb-32">
                <div className="absolute inset-0 bg-grid-slate-200/50 dark:bg-grid-slate-800/50 [mask-image:linear-gradient(0deg,white,rgba(255,255,255,0.6))] dark:[mask-image:linear-gradient(0deg,rgba(0,0,0,0.2),rgba(0,0,0,0.5))]" />
                <div className="container mx-auto px-4 relative">
                    <div className="max-w-4xl mx-auto text-center space-y-8">
                        <div className="inline-flex items-center gap-2 px-4 py-2 rounded-full bg-cyan-50 dark:bg-cyan-900/30 text-cyan-700 dark:text-cyan-300 text-sm font-medium border border-cyan-100 dark:border-cyan-800 animate-fade-in-up">
                            <Sparkles className="w-4 h-4" />
                            <span>Premium Laundry & Dry Cleaning Service</span>
                        </div>

                        <h1 className="text-5xl md:text-7xl font-bold text-slate-900 dark:text-white tracking-tight leading-tight animate-fade-in-up delay-100">
                            Fresh Clothes, <br />
                            <span className="bg-gradient-to-r from-cyan-500 to-blue-600 bg-clip-text text-transparent">
                                Zero Hassle
                            </span>
                        </h1>

                        <p className="text-xl text-slate-600 dark:text-slate-400 max-w-2xl mx-auto animate-fade-in-up delay-200">
                            Experience the ultimate convenience with our door-to-door laundry service.
                            Professional care for your garments, delivered back to you fresh and clean.
                        </p>

                        {/* Search Bar */}
                        <div className="max-w-2xl mx-auto relative animate-fade-in-up delay-300 z-20">
                            <form onSubmit={handleSearch} className="relative group">
                                <div className="absolute inset-0 bg-gradient-to-r from-cyan-500 to-blue-600 rounded-2xl blur opacity-20 group-hover:opacity-30 transition-opacity" />
                                <div className="relative flex items-center bg-white dark:bg-slate-800 rounded-2xl shadow-xl border border-slate-200 dark:border-slate-700 p-2">
                                    <Search className="w-6 h-6 text-slate-400 ml-4" />
                                    <input
                                        type="text"
                                        placeholder="What needs cleaning? (e.g., Suits, Shirts, Bedding)"
                                        className="flex-1 bg-transparent border-none focus:ring-0 text-lg px-4 text-slate-900 dark:text-white placeholder:text-slate-400"
                                        value={searchQuery}
                                        onChange={(e) => setSearchQuery(e.target.value)}
                                        onFocus={() => searchQuery.length > 1 && setShowSuggestions(true)}
                                    />
                                    <Button size="lg" className="rounded-xl px-8" type="submit">
                                        Search
                                    </Button>
                                </div>
                            </form>

                            {/* Search Suggestions */}
                            {showSuggestions && suggestions.length > 0 && (
                                <div className="absolute top-full left-0 right-0 mt-2 bg-white dark:bg-slate-800 rounded-xl shadow-2xl border border-slate-200 dark:border-slate-700 overflow-hidden z-30">
                                    {suggestions.map((service) => (
                                        <div
                                            key={service.id}
                                            className="flex items-center gap-4 p-4 hover:bg-slate-50 dark:hover:bg-slate-700/50 cursor-pointer transition-colors border-b border-slate-100 dark:border-slate-700 last:border-0"
                                            onClick={() => {
                                                setSearchQuery(service.name || '');
                                                setShowSuggestions(false);
                                                navigate(`/services?search=${encodeURIComponent(service.name || '')}`);
                                            }}
                                        >
                                            {service.imageUrl && (
                                                <img
                                                    src={service.imageUrl}
                                                    alt={service.name || ''}
                                                    className="w-12 h-12 rounded-lg object-cover"
                                                />
                                            )}
                                            <div>
                                                <h4 className="font-medium text-slate-900 dark:text-white">{service.name}</h4>
                                                <p className="text-sm text-cyan-600 dark:text-cyan-400 font-medium">
                                                    {service.basePrice !== undefined && service.basePrice !== null ? `${service.basePrice.toFixed(0)} ${service.currency || 'EGP'}` : 'Price on Request'}
                                                </p>
                                            </div>
                                        </div>
                                    ))}
                                </div>
                            )}
                        </div>
                    </div>
                </div>
            </section>

            {/* Popular Services */}
            <section className="py-24 bg-white dark:bg-slate-900">
                <div className="container mx-auto px-4">
                    <div className="text-center max-w-3xl mx-auto mb-16">
                        <h2 className="text-3xl font-bold text-slate-900 dark:text-white mb-4">Most Requested Services</h2>
                        <p className="text-slate-600 dark:text-slate-400">
                            From delicate fabrics to everyday wear, we handle it all with care.
                        </p>
                    </div>

                    <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-8">
                        {isLoading ? (
                            Array(4).fill(0).map((_, i) => (
                                <div key={i} className="h-80 bg-slate-200 dark:bg-slate-800 rounded-2xl animate-pulse" />
                            ))
                        ) : (
                            services.map((service) => (
                                <div
                                    key={service.id}
                                    className="bg-slate-50 dark:bg-slate-800 rounded-2xl overflow-hidden hover:shadow-lg transition-shadow duration-300 border border-slate-100 dark:border-slate-700 cursor-pointer group relative flex flex-col"
                                    onClick={() => navigate(`/services?search=${encodeURIComponent(service.name || '')}`)}
                                >
                                    {service.imageUrl && (
                                        <div className="relative h-48 overflow-hidden">
                                            <img
                                                src={service.imageUrl}
                                                alt={service.name || ''}
                                                className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
                                            />
                                        </div>
                                    )}
                                    <div className="p-6 flex-1 flex flex-col">
                                        <div className="flex justify-between items-start mb-2">
                                            <h3 className="font-bold text-slate-900 dark:text-white line-clamp-2 pr-2">{service.name}</h3>
                                            <div className="flex flex-col items-end gap-1 shrink-0">
                                                <span className="bg-cyan-100 dark:bg-cyan-900/30 text-cyan-700 dark:text-cyan-300 text-xs font-bold px-2 py-1 rounded-full whitespace-nowrap">
                                                    {service.basePrice !== undefined && service.basePrice !== null ? `${service.basePrice.toFixed(0)} ${service.currency || 'EGP'}` : 'Price on Request'}
                                                </span>
                                                {service.maxWeightKg !== undefined && (
                                                    <span className="text-xs font-medium text-slate-500 dark:text-slate-400">
                                                        Max {service.maxWeightKg} KG
                                                    </span>
                                                )}
                                            </div>
                                        </div>
                                        <p className="text-sm text-slate-600 dark:text-slate-400 mb-4 line-clamp-2 flex-1">
                                            {service.description || 'No description available'}
                                        </p>
                                        <div className="flex items-center justify-between mt-auto">
                                            <div className="flex items-center gap-1 text-amber-400">
                                                <Star className="w-4 h-4 fill-current" />
                                                <span className="text-sm font-medium text-slate-700 dark:text-slate-300">4.9</span>
                                            </div>
                                            <Button
                                                size="sm"
                                                variant="outline"
                                                className="hover:bg-cyan-50 dark:hover:bg-slate-700"
                                                onClick={(e) => handleBookNow(e, service.id!)}
                                            >
                                                Book <ArrowRight className="w-3 h-3 ml-1" />
                                            </Button>
                                        </div>
                                    </div>
                                </div>
                            ))
                        )}
                    </div>

                    <div className="text-center mt-12">
                        <Link to="/services">
                            <Button size="lg" className="rounded-full px-8">
                                View All Services
                            </Button>
                        </Link>
                    </div>
                </div>
            </section>

            {/* Categories Section */}
            <section className="py-24 bg-slate-50 dark:bg-slate-800/50" id="services">
                <div className="container mx-auto px-4">
                    <div className="flex justify-between items-end mb-12">
                        <div>
                            <h2 className="text-3xl font-bold text-slate-900 dark:text-white mb-4">Popular Categories</h2>
                            <p className="text-slate-600 dark:text-slate-400">Everything you need, cleaned to perfection.</p>
                        </div>
                        <Link to="/services">
                            <Button variant="ghost" className="hidden md:flex gap-2">
                                View All <ArrowRight className="w-4 h-4" />
                            </Button>
                        </Link>
                    </div>

                    <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-6">
                        {isLoading ? (
                            Array(4).fill(0).map((_, i) => (
                                <div key={i} className="h-64 bg-slate-200 dark:bg-slate-800 rounded-2xl animate-pulse" />
                            ))
                        ) : (
                            categories.map((category) => (
                                <div
                                    key={category.id}
                                    className="group relative h-64 rounded-2xl overflow-hidden cursor-pointer bg-gradient-to-br from-cyan-600 to-blue-700"
                                    onClick={() => handleCategoryClick(category.id || '', category.name || '')}
                                >
                                    {category.imageUrl && (
                                        <img
                                            src={category.imageUrl}
                                            alt={category.name || ''}
                                            className="absolute inset-0 w-full h-full object-cover transition-transform duration-500 group-hover:scale-110"
                                        />
                                    )}
                                    <div className="absolute inset-0 bg-gradient-to-t from-black/80 via-black/40 to-transparent" />
                                    <div className="absolute bottom-0 left-0 p-6">
                                        <h3 className="text-xl font-bold text-white mb-2">{category.name}</h3>
                                        <p className="text-slate-200 text-sm opacity-0 group-hover:opacity-100 transition-opacity transform translate-y-2 group-hover:translate-y-0">
                                            {category.description || 'Professional cleaning service'}
                                        </p>
                                    </div>
                                </div>
                            ))
                        )}
                    </div>
                </div>
            </section>

            {/* Features Grid */}
            <section className="py-24 bg-white dark:bg-slate-900" id="how-it-works">
                <div className="container mx-auto px-4">
                    <div className="grid md:grid-cols-3 gap-12">
                        {[
                            {
                                icon: Clock,
                                title: "Quick Turnaround",
                                description: "Get your clothes back in as little as 24 hours. We value your time."
                            },
                            {
                                icon: Shield,
                                title: "Premium Care",
                                description: "Expert handling of all fabric types using eco-friendly cleaning solutions."
                            },
                            {
                                icon: MapPin,
                                title: "Doorstep Service",
                                description: "Free pickup and delivery at your scheduled time and location."
                            }
                        ].map((feature, index) => (
                            <div key={index} className="text-center space-y-4 p-6 rounded-2xl hover:bg-slate-50 dark:hover:bg-slate-800/50 transition-colors duration-300">
                                <div className="w-16 h-16 mx-auto bg-cyan-100 dark:bg-cyan-900/30 rounded-2xl flex items-center justify-center text-cyan-600 dark:text-cyan-400">
                                    <feature.icon className="w-8 h-8" />
                                </div>
                                <h3 className="text-xl font-bold text-slate-900 dark:text-white">{feature.title}</h3>
                                <p className="text-slate-600 dark:text-slate-400 leading-relaxed">{feature.description}</p>
                            </div>
                        ))}
                    </div>
                </div>
            </section>

            {/* CTA Section */}
            <section className="py-24 bg-slate-900 relative overflow-hidden">
                <div className="absolute inset-0 bg-[url('https://images.unsplash.com/photo-1545173168-9f1947eebb8f?auto=format&fit=crop&q=80')] bg-cover bg-center opacity-10" />
                <div className="container mx-auto px-4 relative z-10 text-center">
                    <h2 className="text-4xl md:text-5xl font-bold text-white mb-6">
                        Ready for a Spotless Experience?
                    </h2>
                    <p className="text-xl text-slate-300 mb-10 max-w-2xl mx-auto">
                        Join thousands of satisfied customers who trust us with their garments.
                        First order gets 20% off!
                    </p>
                    <div className="flex flex-col sm:flex-row gap-4 justify-center">
                        <Link to="/register">
                            <Button size="lg" className="w-full sm:w-auto text-lg px-8 py-6 bg-cyan-500 hover:bg-cyan-600 text-white border-none">
                                Get Started Now
                            </Button>
                        </Link>
                        <Link to="/services">
                            <Button size="lg" variant="outline" className="w-full sm:w-auto text-lg px-8 py-6 border-slate-600 text-white hover:bg-slate-800 hover:text-white">
                                View Pricing
                            </Button>
                        </Link>
                    </div>
                </div>
            </section>

            {/* Footer */}
            <footer className="bg-slate-50 dark:bg-slate-950 border-t border-slate-200 dark:border-slate-800 pt-16 pb-8">
                <div className="container mx-auto px-4">
                    <div className="grid md:grid-cols-4 gap-12 mb-12">
                        <div className="space-y-4">
                            <div className="flex items-center gap-3">
                                <img src={logo} alt="Spotless Logo" className="h-8 w-auto" />
                                <span className="text-xl font-bold text-slate-900 dark:text-white">Spotless</span>
                            </div>
                            <p className="text-slate-600 dark:text-slate-400">
                                Premium laundry and dry cleaning service delivered to your doorstep.
                            </p>
                        </div>

                        <div>
                            <h4 className="font-bold text-slate-900 dark:text-white mb-4">Services</h4>
                            <ul className="space-y-2 text-slate-600 dark:text-slate-400">
                                <li><a href="#" className="hover:text-cyan-600 dark:hover:text-cyan-400">Dry Cleaning</a></li>
                                <li><a href="#" className="hover:text-cyan-600 dark:hover:text-cyan-400">Wash & Fold</a></li>
                                <li><a href="#" className="hover:text-cyan-600 dark:hover:text-cyan-400">Ironing</a></li>
                                <li><a href="#" className="hover:text-cyan-600 dark:hover:text-cyan-400">Alterations</a></li>
                            </ul>
                        </div>

                        <div>
                            <h4 className="font-bold text-slate-900 dark:text-white mb-4">Company</h4>
                            <ul className="space-y-2 text-slate-600 dark:text-slate-400">
                                <li><a href="#" className="hover:text-cyan-600 dark:hover:text-cyan-400">About Us</a></li>
                                <li><a href="#" className="hover:text-cyan-600 dark:hover:text-cyan-400">Careers</a></li>
                                <li><a href="#" className="hover:text-cyan-600 dark:hover:text-cyan-400">Privacy Policy</a></li>
                                <li><a href="#" className="hover:text-cyan-600 dark:hover:text-cyan-400">Terms of Service</a></li>
                            </ul>
                        </div>

                        <div>
                            <h4 className="font-bold text-slate-900 dark:text-white mb-4">Contact</h4>
                            <ul className="space-y-4 text-slate-600 dark:text-slate-400">
                                <li className="flex items-center gap-3">
                                    <MapPin className="w-5 h-5 text-cyan-600" />
                                    <span>123 Clean Street, NY 10001</span>
                                </li>
                                <li className="flex items-center gap-3">
                                    <Phone className="w-5 h-5 text-cyan-600" />
                                    <span>+1 (555) 123-4567</span>
                                </li>
                                <li className="flex items-center gap-3">
                                    <Mail className="w-5 h-5 text-cyan-600" />
                                    <span>support@spotless.com</span>
                                </li>
                            </ul>
                        </div>
                    </div>

                    <div className="border-t border-slate-200 dark:border-slate-800 pt-8 text-center text-slate-600 dark:text-slate-400">
                        <p>&copy; {new Date().getFullYear()} Spotless. All rights reserved.</p>
                    </div>
                </div>
            </footer>

            <AiChatWidget />
            <BackToTop />
        </div>
    );
};
