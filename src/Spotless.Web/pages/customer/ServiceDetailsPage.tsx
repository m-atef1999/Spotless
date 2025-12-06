import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { ArrowLeft, Clock, DollarSign, Loader2, AlertCircle, Sparkles, Zap } from 'lucide-react';
import { DashboardLayout } from '../../layouts/DashboardLayout';
import { Button } from '../../components/ui/Button';
import { ServicesService, type ServiceDto } from '../../lib/api';
import { getServiceImage } from '../../lib/imageUtils';

export const ServiceDetailsPage: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const [service, setService] = useState<ServiceDto | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchService = async () => {
            if (!id) return;
            setIsLoading(true);
            try {
                const data = await ServicesService.getApiServices1({ id });
                setService(data);
            } catch (err) {
                console.error('Failed to fetch service details', err);
                setError('Failed to load service details.');
            } finally {
                setIsLoading(false);
            }
        };

        fetchService();
    }, [id]);

    const handleBookNow = () => {
        if (!service?.id) return;
        navigate(`/customer/new-order?serviceId=${service.id}`);
    };

    if (isLoading) {
        return (
            <DashboardLayout role="Customer">
                <div className="flex justify-center py-20">
                    <Loader2 className="w-10 h-10 animate-spin text-cyan-500" />
                </div>
            </DashboardLayout>
        );
    }

    if (error || !service) {
        return (
            <DashboardLayout role="Customer">
                <div className="flex flex-col items-center justify-center py-20 text-center">
                    <div className="w-16 h-16 bg-red-100 text-red-600 rounded-full flex items-center justify-center mb-4">
                        <AlertCircle className="w-8 h-8" />
                    </div>
                    <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-2">
                        Error Loading Service
                    </h3>
                    <p className="text-slate-500 dark:text-slate-400 mb-6 max-w-md">
                        {error || 'Service not found.'}
                    </p>
                    <Button onClick={() => navigate('/customer/services')} variant="outline">
                        Back to Services
                    </Button>
                </div>
            </DashboardLayout>
        );
    }

    return (
        <DashboardLayout role="Customer">
            <div className="max-w-4xl mx-auto">
                <Button
                    variant="ghost"
                    onClick={() => navigate('/customer/services')}
                    className="mb-6 hover:bg-slate-100 dark:hover:bg-slate-800"
                >
                    <ArrowLeft className="w-4 h-4 mr-2" />
                    Back to Services
                </Button>

                <div className="bg-white dark:bg-slate-900 rounded-2xl border border-slate-200 dark:border-slate-800 overflow-hidden shadow-xl">
                    <div className="relative h-64 md:h-80">
                        <img
                            src={getServiceImage(service)}
                            alt={service.name || ''}
                            className="w-full h-full object-cover"
                        />
                        <div className="absolute inset-0 bg-gradient-to-t from-black/70 via-black/30 to-transparent" />
                        <div className="absolute bottom-6 left-6 md:left-10 text-white">
                            <div className="flex items-center gap-2 mb-2">
                                <span className="px-3 py-1 bg-cyan-500/20 backdrop-blur-md border border-cyan-500/30 rounded-full text-xs font-medium text-cyan-100">
                                    Professional Service
                                </span>
                            </div>
                            <h1 className="text-3xl md:text-4xl font-bold mb-2">{service.name}</h1>
                        </div>
                    </div>

                    <div className="p-6 md:p-10">
                        <div className="grid md:grid-cols-3 gap-8">
                            <div className="md:col-span-2 space-y-6">
                                <div>
                                    <h2 className="text-xl font-semibold text-slate-900 dark:text-white mb-3 flex items-center gap-2">
                                        <Sparkles className="w-5 h-5 text-cyan-500" />
                                        Description
                                    </h2>
                                    <p className="text-slate-600 dark:text-slate-300 leading-relaxed">
                                        {service.description || 'No detailed description available for this service.'}
                                    </p>
                                </div>

                                <div className="grid grid-cols-2 gap-4">
                                    <div className="p-4 bg-slate-50 dark:bg-slate-800/50 rounded-xl border border-slate-100 dark:border-slate-800">
                                        <div className="flex items-center gap-2 text-slate-500 dark:text-slate-400 mb-1">
                                            <Clock className="w-4 h-4" />
                                            <span className="text-sm font-medium">Estimated Time</span>
                                        </div>
                                        <div className="text-lg font-semibold text-slate-900 dark:text-white">
                                            {service.estimatedDurationHours} Hours
                                        </div>
                                    </div>
                                    <div className="p-4 bg-slate-50 dark:bg-slate-800/50 rounded-xl border border-slate-100 dark:border-slate-800">
                                        <div className="flex items-center gap-2 text-slate-500 dark:text-slate-400 mb-1">
                                            <DollarSign className="w-4 h-4" />
                                            <span className="text-sm font-medium">Base Price</span>
                                        </div>
                                        <div className="text-lg font-semibold text-slate-900 dark:text-white">
                                            {service.basePrice?.toFixed(2)} EGP
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div className="space-y-4">
                                <div className="p-6 bg-slate-50 dark:bg-slate-800/50 rounded-xl border border-slate-100 dark:border-slate-800">
                                    <h3 className="font-semibold text-slate-900 dark:text-white mb-4">Actions</h3>
                                    <Button
                                        onClick={handleBookNow}
                                        className="w-full shadow-lg shadow-cyan-500/20"
                                    >
                                        <Zap className="w-4 h-4 mr-2" />
                                        Book Now
                                    </Button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </DashboardLayout>
    );
};
