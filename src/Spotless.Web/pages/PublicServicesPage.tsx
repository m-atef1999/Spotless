import React, { useState, useEffect } from 'react';
import { useSearchParams, Link, useNavigate } from 'react-router-dom';
import { Search, Filter, LogIn, User, Activity } from 'lucide-react';
import { Button } from '../components/ui/Button';
import { ThemeToggle } from '../components/ui/ThemeToggle';
import { ServicesService, type ServiceDto } from '../lib/api';
import logo from '../assets/logo.png';
import { getServiceImage } from '../utils/imageUtils';
import { useAuthStore } from '../store/authStore';
import { BackToTop } from '../components/ui/BackToTop';

export const PublicServicesPage: React.FC = () => {
    const [searchParams, setSearchParams] = useSearchParams();
    const initialSearch = searchParams.get('search') || '';
    const navigate = useNavigate();
    const { token } = useAuthStore();

    const [services, setServices] = useState<ServiceDto[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [searchQuery, setSearchQuery] = useState(initialSearch);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        setSearchQuery(searchParams.get('search') || '');
    }, [searchParams]);

    useEffect(() => {
        const fetchServices = async () => {
            setIsLoading(true);
            setError(null);
            try {
                const response = await ServicesService.getApiServices({
                    pageNumber: 1,
                    pageSize: 50,
                    nameSearchTerm: searchQuery || undefined
                });
                setServices(response.data || []);
            } catch (err) {
                console.error('Failed to fetch services:', err);
                setError('Failed to load services. Please try again later.');
            } finally {
                setIsLoading(false);
            }
        };

        const timer = setTimeout(() => {
            fetchServices();
        }, 300);

        return () => clearTimeout(timer);
    }, [searchQuery]);

    const handleSearch = (e: React.FormEvent) => {
        e.preventDefault();
        setSearchParams(searchQuery ? { search: searchQuery } : {});
    };

    const handleBookNow = (serviceId?: string) => {
        if (token) {
            navigate(`/customer/new-order${serviceId ? `?serviceId=${serviceId}` : ''}`);
        } else {
            navigate('/login');
        }
    };

    return (
        <div className="min-h-screen bg-slate-50 dark:bg-slate-900 transition-colors duration-300">
            {/* Header */}
            <header className="sticky top-0 z-50 bg-white/80 dark:bg-slate-900/80 backdrop-blur-md border-b border-slate-200 dark:border-slate-800">
                <div className="container mx-auto px-4 h-20 flex items-center justify-between">
                    <Link to="/" className="flex items-center gap-3">
                        <img src={logo} alt="Spotless Logo" className="h-10 w-auto" />
                        <span className="text-2xl font-bold bg-gradient-to-r from-cyan-600 to-blue-600 bg-clip-text text-transparent">
                            Spotless
                        </span>
                    </Link>

                    <div className="flex items-center gap-4">
                        <ThemeToggle />
                        {token ? (
                            <Link to="/customer/dashboard">
                                <Button>Dashboard</Button>
                            </Link>
                        ) : (
                            <>
                                <Link to="/login">
                                    <Button variant="ghost" className="hidden sm:flex gap-2">
                                        <LogIn className="w-4 h-4" /> Sign In
                                    </Button>
                                </Link>
                                <Link to="/register">
                                    <Button className="gap-2">
                                        <User className="w-4 h-4" /> Get Started
                                    </Button>
                                </Link>
                            </>
                        )}
                    </div>
                </div>
            </header>

            <main className="container mx-auto px-4 py-12">
                {/* Page Header */}
                <div className="max-w-4xl mx-auto text-center mb-16">
                    <h1 className="text-4xl md:text-5xl font-bold text-slate-900 dark:text-white mb-6">
                        Our Services
                    </h1>
                    <p className="text-xl text-slate-600 dark:text-slate-400 mb-10">
                        Professional cleaning solutions tailored to your needs.
                    </p>

                    {/* Search Bar */}
                    <form onSubmit={handleSearch} className="relative max-w-2xl mx-auto">
                        <div className="absolute inset-0 bg-gradient-to-r from-cyan-500 to-blue-600 rounded-2xl blur opacity-20" />
                        <div className="relative flex items-center bg-white dark:bg-slate-800 rounded-2xl shadow-xl border border-slate-200 dark:border-slate-700 p-2">
                            <Search className="w-6 h-6 text-slate-400 ml-4" />
                            <input
                                type="text"
                                placeholder="Search services (e.g., Suits, Bedding...)"
                                className="flex-1 bg-transparent border-none focus:ring-0 text-lg px-4 text-slate-900 dark:text-white placeholder:text-slate-400"
                                value={searchQuery}
                                onChange={(e) => setSearchQuery(e.target.value)}
                            />
                            <Button size="lg" className="rounded-xl px-8" type="submit">
                                Search
                            </Button>
                        </div>
                    </form>
                </div>

                {/* Services Grid */}
                {isLoading ? (
                    <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-8">
                        {Array(6).fill(0).map((_, i) => (
                            <div key={i} className="h-96 bg-slate-200 dark:bg-slate-800 rounded-2xl animate-pulse" />
                        ))}
                    </div>
                ) : error ? (
                    <div className="text-center py-20">
                        <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-red-100 dark:bg-red-900/30 text-red-600 dark:text-red-400 mb-4">
                            <Filter className="w-8 h-8" />
                        </div>
                        <h3 className="text-xl font-bold text-slate-900 dark:text-white mb-2">Unable to load services</h3>
                        <p className="text-slate-600 dark:text-slate-400 mb-6">{error}</p>
                        <Button onClick={() => window.location.reload()}>Try Again</Button>
                    </div>
                ) : services.length === 0 ? (
                    <div className="text-center py-20">
                        <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-slate-100 dark:bg-slate-800 text-slate-400 mb-4">
                            <Search className="w-8 h-8" />
                        </div>
                        <h3 className="text-xl font-bold text-slate-900 dark:text-white mb-2">No services found</h3>
                        <p className="text-slate-600 dark:text-slate-400">
                            Try adjusting your search terms or browse all services.
                        </p>
                    </div>
                ) : (
                    <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-8">
                        {services.map((service) => (
                            <div
                                key={service.id}
                                className="group bg-white dark:bg-slate-800 rounded-2xl overflow-hidden shadow-sm hover:shadow-xl transition-all duration-300 border border-slate-100 dark:border-slate-700 flex flex-col"
                            >
                                <div className="relative h-56 overflow-hidden">
                                    <img
                                        src={getServiceImage(service.name || '')}
                                        alt={service.name || ''}
                                        className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
                                    />
                                    <div className="absolute top-4 right-4 bg-white/90 dark:bg-slate-900/90 backdrop-blur px-3 py-1 rounded-full text-sm font-bold text-cyan-600 dark:text-cyan-400 shadow-sm opacity-0">
                                        {/* Hidden but kept for potential future use or layout stability if needed, effectively removed from view */}
                                    </div>
                                </div>

                                <div className="p-6 flex-1 flex flex-col">
                                    <div className="flex justify-between items-start mb-2">
                                        <h3 className="text-xl font-bold text-slate-900 dark:text-white line-clamp-2 pr-2">
                                            {service.name}
                                        </h3>
                                        <div className="flex flex-col items-end gap-1 shrink-0">
                                            <span className="bg-slate-100 dark:bg-slate-700 text-slate-600 dark:text-slate-300 text-xs font-bold px-2 py-1 rounded-full whitespace-nowrap">
                                                {service.basePrice !== undefined && service.basePrice !== null ? `${service.basePrice.toFixed(0)} ${service.currency || 'EGP'}` : 'Price on Request'}
                                            </span>
                                            {service.maxWeightKg !== undefined && (
                                                <span className="text-xs font-medium text-slate-500 dark:text-slate-400">
                                                    Max {service.maxWeightKg} KG
                                                </span>
                                            )}
                                        </div>
                                    </div>
                                    <p className="text-slate-600 dark:text-slate-400 text-sm mb-6 line-clamp-2 flex-1">
                                        {service.description || 'Professional cleaning with premium care.'}
                                    </p>

                                    <div className="flex items-center justify-between mt-auto pt-4 border-t border-slate-100 dark:border-slate-700">
                                        <span className="text-sm text-slate-500 dark:text-slate-400">
                                            {service.estimatedDurationHours ? `Est. Time: ${service.estimatedDurationHours}h` : 'Est. Time: 24h'}
                                        </span>
                                        <Button
                                            onClick={() => handleBookNow(service.id)}
                                            className="shadow-lg shadow-cyan-500/20"
                                        >
                                            <Activity className="w-4 h-4 mr-2" />
                                            Book Now
                                        </Button>
                                    </div>
                                </div>
                            </div>
                        ))}
                    </div>
                )}
            </main>
            <BackToTop />
        </div>
    );
};
