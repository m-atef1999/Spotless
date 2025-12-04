import React, { useState } from 'react';
import { Search, Plus, Edit2, Trash2, Package, DollarSign, Clock, Star, Image } from 'lucide-react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { DashboardLayout } from '../../layouts/DashboardLayout';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Modal } from '../../components/ui/Modal';
import { ServicesService, CategoriesService, type ServiceDto } from '../../lib/api';
import { useToast } from '../../components/ui/Toast';

interface ServiceFormData {
    name: string;
    description: string;
    price: number;
    categoryId: string;
    estimatedDurationHours: number;
    maxWeightKg: number;
    imageUrl: string;
}

export const ServiceManagementPage: React.FC = () => {
    const [searchTerm, setSearchTerm] = useState('');
    const [showModal, setShowModal] = useState(false);
    const [editingService, setEditingService] = useState<ServiceDto | null>(null);
    const [formData, setFormData] = useState<ServiceFormData>({
        name: '',
        description: '',
        price: 0,
        categoryId: '',
        estimatedDurationHours: 2,
        maxWeightKg: 50,
        imageUrl: '',
    });
    const queryClient = useQueryClient();
    const { addToast } = useToast();

    const { data: servicesData, isLoading } = useQuery({
        queryKey: ['services'],
        queryFn: async () => {
            const response = await ServicesService.getApiServices({ pageNumber: 1, pageSize: 100 });
            return response.data || [];
        },
        staleTime: 15000,
    });

    const { data: categoriesData } = useQuery({
        queryKey: ['categories'],
        queryFn: async () => {
            const response = await CategoriesService.getApiCategories({ pageNumber: 1, pageSize: 100 });
            return response.data || [];
        },
    });

    const services = servicesData || [];
    const categories = categoriesData || [];

    const filteredServices = services.filter((service: ServiceDto) =>
        service.name?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        service.description?.toLowerCase().includes(searchTerm.toLowerCase())
    );

    const createMutation = useMutation({
        mutationFn: async (data: ServiceFormData) => {
            return await ServicesService.postApiServices({
                requestBody: {
                    categoryId: data.categoryId,
                    name: data.name,
                    description: data.description,
                    pricePerUnitAmount: data.price,
                    pricePerUnitCurrency: 'EGP',
                    estimatedDurationHours: data.estimatedDurationHours,
                    maxWeightKg: data.maxWeightKg,
                    imageUrl: data.imageUrl || undefined,
                }
            });
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['services'] });
            addToast('Service created successfully!', 'success');
            setShowModal(false);
        },
        onError: () => {
            addToast('Failed to create service. Please try again.', 'error');
        }
    });

    const updateMutation = useMutation({
        mutationFn: async ({ id, data }: { id: string; data: ServiceFormData }) => {
            return await ServicesService.putApiServices({
                id,
                requestBody: {
                    name: data.name,
                    description: data.description,
                    pricePerUnitValue: data.price,
                    pricePerUnitCurrency: 'EGP',
                    estimatedDurationHours: data.estimatedDurationHours,
                    maxWeightKg: data.maxWeightKg,
                    categoryId: data.categoryId,
                    imageUrl: data.imageUrl || undefined,
                }
            });
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['services'] });
            addToast('Service updated successfully!', 'success');
            setShowModal(false);
        },
        onError: () => {
            addToast('Failed to update service. Please try again.', 'error');
        }
    });

    const deleteMutation = useMutation({
        mutationFn: async (id: string) => {
            return await ServicesService.deleteApiServices({ id });
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['services'] });
            addToast('Service deleted successfully!', 'success');
        },
        onError: () => {
            addToast('Failed to delete service. Please try again.', 'error');
        }
    });

    const handleCreate = () => {
        setEditingService(null);
        setFormData({
            name: '',
            description: '',
            price: 0,
            categoryId: categories.length > 0 ? categories[0].id : '',
            estimatedDurationHours: 2,
            maxWeightKg: 50,
            imageUrl: '',
        });
        if (categories.length === 0) {
            addToast('Please create a category first.', 'error');
            return;
        }
        setShowModal(true);
    };

    const handleEdit = (service: ServiceDto) => {
        setEditingService(service);
        setFormData({
            name: service.name || '',
            description: service.description || '',
            price: service.basePrice || 0,
            categoryId: service.categoryId || '',
            estimatedDurationHours: service.estimatedDurationHours || 2,
            maxWeightKg: service.maxWeightKg || 50,
            imageUrl: service.imageUrl || '',
        });
        setShowModal(true);
    };

    const handleSubmit = () => {
        if (!formData.name.trim()) {
            addToast('Service name is required', 'error');
            return;
        }
        if (editingService) {
            updateMutation.mutate({ id: editingService.id!, data: formData });
        } else {
            createMutation.mutate(formData);
        }
    };

    const handleDelete = (id: string) => {
        if (window.confirm('Are you sure you want to delete this service? This action cannot be undone.')) {
            deleteMutation.mutate(id);
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
                        <Button onClick={handleCreate} className="shrink-0 bg-cyan-600 hover:bg-cyan-700 text-white">
                            <Plus className="w-4 h-4 mr-2" />
                            Add Service
                        </Button>
                    </div>
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                    {isLoading ? (
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

                                {/* Service Image */}
                                {service.imageUrl && (
                                    <div className="mb-4 rounded-lg overflow-hidden h-32 bg-slate-100 dark:bg-slate-800">
                                        <img src={service.imageUrl} alt={service.name} className="w-full h-full object-cover" />
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
                                        <span className="font-medium">{service.basePrice?.toFixed(2)} EGP</span>
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
                                        onClick={() => handleEdit(service)}
                                    >
                                        <Edit2 className="w-4 h-4 mr-2" />
                                        Edit
                                    </Button>
                                    <Button
                                        size="sm"
                                        variant="outline"
                                        className="text-red-600 hover:bg-red-50 border-red-200"
                                        onClick={() => handleDelete(service.id!)}
                                    >
                                        <Trash2 className="w-4 h-4" />
                                    </Button>
                                </div>
                            </div>
                        ))
                    )}
                </div>
            </div>

            {/* Create/Edit Modal */}
            <Modal
                isOpen={showModal}
                onClose={() => setShowModal(false)}
                title={editingService ? 'Edit Service' : 'Create Service'}
                size="lg"
            >
                <div className="space-y-4">
                    <div>
                        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
                            Service Name *
                        </label>
                        <input
                            type="text"
                            value={formData.name}
                            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                            className="w-full px-4 py-2 rounded-lg border border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-800 text-slate-900 dark:text-white focus:ring-2 focus:ring-cyan-500"
                            placeholder="Enter service name"
                        />
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
                            Description
                        </label>
                        <textarea
                            value={formData.description}
                            onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                            className="w-full px-4 py-2 rounded-lg border border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-800 text-slate-900 dark:text-white focus:ring-2 focus:ring-cyan-500"
                            placeholder="Enter description"
                            rows={3}
                        />
                    </div>

                    <div className="grid grid-cols-2 gap-4">
                        <div>
                            <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
                                Price (EGP) *
                            </label>
                            <input
                                type="number"
                                value={formData.price}
                                onChange={(e) => setFormData({ ...formData, price: parseFloat(e.target.value) || 0 })}
                                className="w-full px-4 py-2 rounded-lg border border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-800 text-slate-900 dark:text-white focus:ring-2 focus:ring-cyan-500"
                                placeholder="0.00"
                                min="0"
                                step="0.01"
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
                                Duration (hours)
                            </label>
                            <input
                                type="number"
                                value={formData.estimatedDurationHours}
                                onChange={(e) => setFormData({ ...formData, estimatedDurationHours: parseInt(e.target.value) || 1 })}
                                className="w-full px-4 py-2 rounded-lg border border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-800 text-slate-900 dark:text-white focus:ring-2 focus:ring-cyan-500"
                                placeholder="2"
                                min="1"
                            />
                        </div>
                    </div>

                    <div className="grid grid-cols-2 gap-4">
                        <div>
                            <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
                                Max Weight (kg)
                            </label>
                            <input
                                type="number"
                                value={formData.maxWeightKg}
                                onChange={(e) => setFormData({ ...formData, maxWeightKg: parseInt(e.target.value) || 50 })}
                                className="w-full px-4 py-2 rounded-lg border border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-800 text-slate-900 dark:text-white focus:ring-2 focus:ring-cyan-500"
                                placeholder="50"
                                min="1"
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
                                Category *
                            </label>
                            <select
                                value={formData.categoryId}
                                onChange={(e) => setFormData({ ...formData, categoryId: e.target.value })}
                                className="w-full px-4 py-2 rounded-lg border border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-800 text-slate-900 dark:text-white focus:ring-2 focus:ring-cyan-500"
                            >
                                {categories.map((cat: any) => (
                                    <option key={cat.id} value={cat.id}>{cat.name}</option>
                                ))}
                            </select>
                        </div>
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
                            <Image className="w-4 h-4 inline mr-1" />
                            Image URL
                        </label>
                        <input
                            type="url"
                            value={formData.imageUrl}
                            onChange={(e) => setFormData({ ...formData, imageUrl: e.target.value })}
                            className="w-full px-4 py-2 rounded-lg border border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-800 text-slate-900 dark:text-white focus:ring-2 focus:ring-cyan-500"
                            placeholder="https://example.com/image.jpg"
                        />
                        {formData.imageUrl && (
                            <div className="mt-2 rounded-lg overflow-hidden h-24 w-24 bg-slate-100 dark:bg-slate-800">
                                <img src={formData.imageUrl} alt="Preview" className="w-full h-full object-cover" onError={(e) => (e.target as HTMLImageElement).style.display = 'none'} />
                            </div>
                        )}
                    </div>

                    <div className="flex justify-end gap-3 pt-4 border-t border-slate-200 dark:border-slate-700">
                        <Button variant="outline" onClick={() => setShowModal(false)}>
                            Cancel
                        </Button>
                        <Button
                            onClick={handleSubmit}
                            className="bg-cyan-600 hover:bg-cyan-700 text-white"
                            disabled={createMutation.isPending || updateMutation.isPending}
                        >
                            {createMutation.isPending || updateMutation.isPending ? 'Saving...' : (editingService ? 'Update' : 'Create')}
                        </Button>
                    </div>
                </div>
            </Modal>
        </DashboardLayout>
    );
};
