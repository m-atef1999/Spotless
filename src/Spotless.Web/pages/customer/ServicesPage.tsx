import React, { useEffect, useState, useCallback } from 'react';
import { Search, Clock, DollarSign, Sparkles, Loader2, AlertCircle, ShoppingCart, Zap } from 'lucide-react';
import { DashboardLayout } from '../../layouts/DashboardLayout';
import { Input } from '../../components/ui/Input';
import { Button } from '../../components/ui/Button';
import { useToast } from '../../components/ui/Toast';

import { ServicesService, CartsService, type ServiceDto, type PagedResponse } from '../../lib/api';
import { useNavigate } from 'react-router-dom';
import { getServiceImage } from '../../lib/imageUtils';

export const ServicesPage: React.FC = () => {
    const navigate = useNavigate();
    const { addToast } = useToast();
    const [services, setServices] = useState<ServiceDto[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [searchTerm, setSearchTerm] = useState('');
    const [debouncedSearchTerm, setDebouncedSearchTerm] = useState('');
    const [addingToCart, setAddingToCart] = useState<string | null>(null);

    // Debounce search term
    useEffect(() => {
        const timer = setTimeout(() => {
            setDebouncedSearchTerm(searchTerm);
        }, 500);
        return () => clearTimeout(timer);
    }, [searchTerm]);

    const fetchServices = useCallback(async () => {
        setIsLoading(true);
        setError(null);
        try {
            // Fetch services with search term, page 1, 50 items
            const response: PagedResponse = await ServicesService.getApiServices({ nameSearchTerm: debouncedSearchTerm, pageNumber: 1, pageSize: 50 });
            setServices(response.data || []);
        } catch (err) {
            console.error('Failed to fetch services', err);
            setError('Failed to load services. Please try again later.');
        } finally {
            setIsLoading(false);
        }
    }, [debouncedSearchTerm]);

    useEffect(() => {
        fetchServices();
    }, [fetchServices]);

    const handleAddToCart = async (serviceId: string, serviceName: string) => {
        try {
            setAddingToCart(serviceId);
            await CartsService.postApiCustomersCartItems({
                requestBody: {
                    serviceId,
                    quantity: 1
                }
            });
            addToast(`Added ${serviceName} to cart`, 'success');
        } catch (error) {
            console.error('Failed to add to cart:', error);
            addToast('Failed to add to cart', 'error');
        } finally {
            setAddingToCart(null);
        }
    };

    const handleBuyNow = (_serviceId: string) => {
        // For now, redirect to new order page with service pre-selected (if supported)
        // Or we could implement immediate checkout flow
        navigate('/customer/new-order');
    };

    return (
        <DashboardLayout role="Customer">
            <div className="max-w-6xl mx-auto space-y-8">
                <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                    <div>
                        <h1 className="text-2xl font-bold text-slate-900 dark:text-white flex items-center gap-3">
                            <Sparkles className="w-8 h-8 text-cyan-500" />
                            Our Services
                        </h1>
                        <p className="text-slate-500 dark:text-slate-400 mt-1">
                            Choose from our wide range of professional cleaning services.
                        </p>
                    </div>
                    <div className="w-full md:w-96">
                        <Input
                            placeholder="Search services..."
                            icon={<Search className="w-5 h-5" />}
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                        />
                    </div>
                </div>

                {isLoading ? (
                    <div className="flex justify-center py-20">
                        <Loader2 className="w-10 h-10 animate-spin text-cyan-500" />
                    </div>
                ) : error ? (
                    <div className="flex flex-col items-center justify-center py-20 text-center">
                        <div className="w-16 h-16 bg-red-100 text-red-600 rounded-full flex items-center justify-center mb-4">
                            <AlertCircle className="w-8 h-8" />
                        </div>
                        <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-2">
                            Error Loading Services
                        </h3>
                        <p className="text-slate-500 dark:text-slate-400 mb-6 max-w-md">
                            {error}
                        </p>
                        <Button onClick={fetchServices} variant="outline">
                            Try Again
                        </Button>
                    </div>
                ) : services.length === 0 ? (
                    <div className="flex flex-col items-center justify-center py-20 text-center">
                        <div className="w-16 h-16 bg-slate-100 text-slate-400 rounded-full flex items-center justify-center mb-4">
                            <Search className="w-8 h-8" />
                        </div>
                        <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-2">
                            No Services Found
                        </h3>
                        <p className="text-slate-500 dark:text-slate-400 max-w-md">
                            We couldn't find any services matching "{searchTerm}". Try adjusting your search terms.
                        </p>
                    </div>
                ) : (
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                        {services.map((service) => (
                            <div
                                key={service.id}
                                className="group bg-white dark:bg-slate-900 rounded-2xl border border-slate-200 dark:border-slate-800 p-6 hover:shadow-xl hover:border-cyan-200 dark:hover:border-cyan-900/50 transition-all duration-300 flex flex-col"
                            >
                                <div
                                    className="mb-4 cursor-pointer"
                                    onClick={() => navigate(`/customer/services/${service.id}`)}
                                >
                                    <div className="relative h-48 mb-4 rounded-xl overflow-hidden group-hover:shadow-md transition-all duration-300">
                                        <img
                                            src={getServiceImage(service.name || '')}
                                            alt={service.name || ''}
                                            className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
                                        />
                                        <div className="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent opacity-60" />
                                        <div className="absolute bottom-3 left-3 text-white">
                                            <div className="p-1.5 bg-white/20 backdrop-blur-md rounded-lg inline-flex">
                                                <Sparkles className="w-4 h-4 text-white" />
                                            </div>
                                        </div>
                                    </div>
                                    <h3 className="text-xl font-bold text-slate-900 dark:text-white mb-2 group-hover:text-cyan-600 dark:group-hover:text-cyan-400 transition-colors">
                                        {service.name}
                                    </h3>
                                    <p className="text-slate-500 dark:text-slate-400 text-sm line-clamp-3">
                                        {service.description || 'No description available.'}
                                    </p>
                                </div>

                                <div className="mt-auto pt-6 border-t border-slate-100 dark:border-slate-800">
                                    <div className="flex items-center justify-between mb-4">
                                        <div className="flex items-center gap-1.5 text-sm text-slate-500 dark:text-slate-400">
                                            <Clock className="w-4 h-4" />
                                            {service.estimatedDurationHours}h
                                        </div>
                                        <div className="flex items-center gap-1.5 text-lg font-bold text-slate-900 dark:text-white">
                                            <DollarSign className="w-5 h-5 text-cyan-500" />
                                            {service.basePrice?.toFixed(2)}
                                        </div>
                                    </div>

                                    <div className="grid grid-cols-2 gap-3">
                                        <Button
                                            onClick={() => service.id && handleAddToCart(service.id, service.name || 'Service')}
                                            variant="outline"
                                            className="w-full"
                                            disabled={addingToCart === service.id}
                                        >
                                            {addingToCart === service.id ? (
                                                <Loader2 className="w-4 h-4 animate-spin" />
                                            ) : (
                                                <>
                                                    <ShoppingCart className="w-4 h-4 mr-2" />
                                                    Add
                                                </>
                                            )}
                                        </Button>
                                        <Button
                                            onClick={() => service.id && handleBuyNow(service.id)}
                                            className="w-full shadow-lg shadow-cyan-500/20"
                                        >
                                            <Zap className="w-4 h-4 mr-2" />
                                            Buy Now
                                        </Button>
                                    </div>
                                </div>
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </DashboardLayout>
    );
};
