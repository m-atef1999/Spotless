import React, { useState, useEffect } from 'react';
import { Search, Plus, Edit2, Trash2, Package, DollarSign, Clock, Star } from 'lucide-react';
import { DashboardLayout } from '../../layouts/DashboardLayout';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { apiClient } from '../../lib/api';
import { ServiceDto } from '../../lib/apiClient';

export const ServiceManagementPage: React.FC = () => {
    const [services, setServices] = useState<ServiceDto[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [searchTerm, setSearchTerm] = useState('');

    useEffect(() => {
        const fetchServices = async () => {
            try {
                const response = await apiClient.services(undefined, 1, 100);
                if (response.data) {
                    setServices(response.data);
                }
            } catch (error) {
                console.error('Failed to fetch services', error);
            } finally {
                setIsLoading(false);
            }
        };

        fetchServices();
    }, []);

    const filteredServices = services.filter(service =>
        service.name?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        service.description?.toLowerCase().includes(searchTerm.toLowerCase())
    );

    const handleCreateService = () => {
        // Mock create service
        const name = window.prompt('Enter service name:');
        if (!name) return;
        const price = parseFloat(window.prompt('Enter base price:', '0') || '0');

        const newService = new ServiceDto({
            id: Math.random().toString(36).substr(2, 9),
            name,
            basePrice: price,
            description: 'New service description',
            estimatedDurationHours: 2,
            isActive: true,
            isFeatured: false
        });

        setServices([...services, newService]);
        alert('Service created successfully (Mock)!');
    };

    const handleEditService = (id: string) => {
        const service = services.find(s => s.id === id);
        if (!service) return;

        const newName = window.prompt('Edit service name:', service.name);
        if (newName) {
            setServices(services.map(s => s.id === id ? new ServiceDto({ ...s, name: newName }) : s));
        }
    };

    const handleDeleteService = (id: string) => {
        if (window.confirm('Are you sure you want to delete this service?')) {
            setServices(services.filter(s => s.id !== id));
        }
    };

    return (
        <DashboardLayout role="Admin">
            <div className="space-y-8">
                <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
                    <div>
                        <h1 className="text-2xl font-bold text-slate-900 dark:text-white">
                            Service Management
                        </h1>
                        <p className="text-slate-500 dark:text-slate-400 mt-1">
                            Manage service offerings and pricing.
                        </p>
                    </div>
                    <div className="flex gap-2 w-full sm:w-auto">
                        <div className="w-full sm:w-64">
                            <Input
                                placeholder="Search services..."
                                icon={<Search className="w-5 h-5" />}
                                value={searchTerm}
                                onChange={(e) => setSearchTerm(e.target.value)}
                            />
                        </div>
                        <Button onClick={handleCreateService} className="shrink-0 bg-cyan-600 hover:bg-cyan-700 text-white">
                            <Plus className="w-4 h-4 mr-2" />
                            Add Service
                        </Button>
                    </div>
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                    {isLoading ? (
                        // Loading skeletons
                        Array.from({ length: 6 }).map((_, i) => (
                            <div key={i} className="bg-white dark:bg-slate-900 rounded-xl p-6 border border-slate-200 dark:border-slate-800 animate-pulse">
                                <div className="h-6 bg-slate-200 dark:bg-slate-800 rounded w-3/4 mb-4"></div>
                                <div className="h-4 bg-slate-200 dark:bg-slate-800 rounded w-full mb-2"></div>
                                <div className="h-4 bg-slate-200 dark:bg-slate-800 rounded w-2/3"></div>
                            </div>
                        ))
                    ) : filteredServices.length === 0 ? (
                        <div className="col-span-full text-center py-12 text-slate-500">
                            No services found.
                        </div>
                    ) : (
                        filteredServices.map((service) => (
                            <div key={service.id} className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 p-6 hover:shadow-md transition-shadow relative group">
                                {service.isFeatured && (
                                    <div className="absolute top-4 right-4 text-yellow-500">
                                        <Star className="w-5 h-5 fill-current" />
                                    </div>
                                )}
                                <div className="flex items-start justify-between mb-4">
                                    <div className="p-3 bg-cyan-50 dark:bg-cyan-900/20 rounded-lg text-cyan-600 dark:text-cyan-400">
                                        <Package className="w-6 h-6" />
                                    </div>
                                </div>

                                <h3 className="text-lg font-bold text-slate-900 dark:text-white mb-2">
                                    {service.name}
                                </h3>
                                <p className="text-sm text-slate-500 dark:text-slate-400 mb-4 line-clamp-2">
                                    {service.description}
                                </p>

                                <div className="flex items-center gap-4 text-sm text-slate-600 dark:text-slate-300 mb-6">
                                    <div className="flex items-center gap-1">
                                        <DollarSign className="w-4 h-4 text-slate-400" />
                                        <span className="font-medium">{service.basePrice?.toFixed(2)}</span>
                                    </div>
                                    <div className="flex items-center gap-1">
                                        <Clock className="w-4 h-4 text-slate-400" />
                                        <span>{service.estimatedDurationHours}h</span>
                                    </div>
                                </div>

                                <div className="flex items-center gap-2 pt-4 border-t border-slate-100 dark:border-slate-800">
                                    <Button
                                        size="sm"
                                        variant="outline"
                                        className="flex-1"
                                        onClick={() => handleEditService(service.id!)}
                                    >
                                        <Edit2 className="w-4 h-4 mr-2" />
                                        Edit
                                    </Button>
                                    <Button
                                        size="sm"
                                        variant="outline"
                                        className="text-red-600 hover:bg-red-50 border-red-200"
                                        onClick={() => handleDeleteService(service.id!)}
                                    >
                                        <Trash2 className="w-4 h-4" />
                                    </Button>
                                </div>
                            </div>
                        ))
                    )}
                </div>
            </div>
        </DashboardLayout>
    );
};
