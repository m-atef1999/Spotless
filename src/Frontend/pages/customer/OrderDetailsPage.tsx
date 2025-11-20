import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { format } from 'date-fns';
import { ArrowLeft, Calendar, MapPin, CreditCard, Package, AlertCircle, Loader2, RotateCcw } from 'lucide-react';
import { DashboardLayout } from '../../layouts/DashboardLayout';
import { Button } from '../../components/ui/Button';
import { apiClient } from '../../lib/api';
import { OrderDto, OrderStatus, PaymentMethod } from '../../lib/apiClient';

export const OrderDetailsPage: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const [order, setOrder] = useState<OrderDto | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const [servicesMap, setServicesMap] = useState<Record<string, string>>({});

    useEffect(() => {
        const fetchData = async () => {
            try {
                const [orders, servicesResponse] = await Promise.all([
                    apiClient.orders(),
                    apiClient.services(undefined, 1, 100) // Fetch enough services
                ]);

                // Create services map
                const map: Record<string, string> = {};
                if (servicesResponse.data) {
                    servicesResponse.data.forEach(s => {
                        if (s.id) map[s.id] = s.name || 'Unknown Service';
                    });
                }
                setServicesMap(map);

                const foundOrder = orders.find(o => o.id === id);

                if (foundOrder) {
                    setOrder(foundOrder);
                } else {
                    setError('Order not found');
                }
            } catch (err) {
                console.error('Failed to fetch order details', err);
                setError('Failed to load order details. Please try again later.');
            } finally {
                setIsLoading(false);
            }
        };

        if (id) {
            fetchData();
        }
    }, [id]);

    const getStatusColor = (status?: OrderStatus) => {
        switch (status) {
            case OrderStatus._0: // Pending
                return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400';
            case OrderStatus._1: // Confirmed
                return 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400';
            case OrderStatus._2: // InProgress
                return 'bg-cyan-100 text-cyan-800 dark:bg-cyan-900/30 dark:text-cyan-400';
            case OrderStatus._3: // Completed
                return 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400';
            case OrderStatus._4: // Cancelled
                return 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400';
            default:
                return 'bg-slate-100 text-slate-800 dark:bg-slate-800 dark:text-slate-400';
        }
    };

    const getStatusLabel = (status?: OrderStatus) => {
        switch (status) {
            case OrderStatus._0: return 'Pending';
            case OrderStatus._1: return 'Confirmed';
            case OrderStatus._2: return 'In Progress';
            case OrderStatus._3: return 'Completed';
            case OrderStatus._4: return 'Cancelled';
            default: return 'Unknown';
        }
    };

    const getPaymentMethodLabel = (method?: PaymentMethod) => {
        switch (method) {
            case PaymentMethod._0: return 'Cash';
            case PaymentMethod._1: return 'Credit Card';
            default: return 'Unknown';
        }
    };

    const handleCancelOrder = async () => {
        if (!window.confirm('Are you sure you want to cancel this order?')) return;
        try {
            // Mock API call
            setOrder(prev => prev ? new OrderDto({ ...prev, status: OrderStatus._4 }) : null);
            // await apiClient.cancelOrder(id);
        } catch (error) {
            console.error('Failed to cancel order', error);
        }
    };

    const handleWriteReview = () => {
        // For now, just a prompt
        const rating = window.prompt('Rate this service (1-5):');
        if (!rating) return;

        const comment = window.prompt('Leave a comment:');
        if (comment) {
            alert('Review submitted successfully!');
        }
    };

    const handleReorder = () => {
        if (window.confirm('Start a new order based on this one?')) {
            navigate('/customer/new-order');
        }
    };

    if (isLoading) {
        return (
            <DashboardLayout role="Customer">
                <div className="flex justify-center py-12">
                    <Loader2 className="w-8 h-8 animate-spin text-cyan-500" />
                </div>
            </DashboardLayout>
        );
    }

    if (error || !order) {
        return (
            <DashboardLayout role="Customer">
                <div className="max-w-3xl mx-auto text-center py-12">
                    <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-red-100 text-red-600 mb-4">
                        <AlertCircle className="w-8 h-8" />
                    </div>
                    <h2 className="text-2xl font-bold text-slate-900 dark:text-white mb-2">
                        {error || 'Order not found'}
                    </h2>
                    <p className="text-slate-500 dark:text-slate-400 mb-6">
                        The order you are looking for could not be found or an error occurred.
                    </p>
                    <Button onClick={() => navigate('/customer/orders')}>
                        Back to Orders
                    </Button>
                </div>
            </DashboardLayout>
        );
    }

    return (
        <DashboardLayout role="Customer">
            <div className="max-w-4xl mx-auto space-y-6">
                {/* Header */}
                <div className="flex items-center gap-4 mb-6">
                    <button
                        onClick={() => navigate('/customer/orders')}
                        className="p-2 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-full transition-colors"
                    >
                        <ArrowLeft className="w-5 h-5 text-slate-500 dark:text-slate-400" />
                    </button>
                    <div>
                        <h1 className="text-2xl font-bold text-slate-900 dark:text-white flex items-center gap-3">
                            Order #{order.id?.slice(0, 8)}
                            <span className={`px-3 py-1 rounded-full text-sm font-medium ${getStatusColor(order.status)}`}>
                                {getStatusLabel(order.status)}
                            </span>
                        </h1>
                        <p className="text-slate-500 dark:text-slate-400 text-sm mt-1">
                            Placed on {order.orderDate ? format(new Date(order.orderDate), 'PPP p') : 'Unknown date'}
                        </p>
                    </div>

                    <div className="ml-auto flex gap-3">
                        {(order.status === OrderStatus._0 || order.status === OrderStatus._1) && (
                            <Button variant="outline" className="text-red-600 border-red-200 hover:bg-red-50" onClick={handleCancelOrder}>
                                Cancel Order
                            </Button>
                        )}
                        {order.status === OrderStatus._3 && (
                            <>
                                <Button variant="outline" onClick={handleReorder}>
                                    <RotateCcw className="w-4 h-4 mr-2" />
                                    Reorder
                                </Button>
                                <Button onClick={handleWriteReview}>
                                    Write Review
                                </Button>
                            </>
                        )}
                    </div>
                </div>

                <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                    {/* Main Content */}
                    <div className="md:col-span-2 space-y-6">
                        {/* Items */}
                        <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 p-6">
                            <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-4 flex items-center gap-2">
                                <Package className="w-5 h-5 text-cyan-500" />
                                Order Items
                            </h3>
                            <div className="space-y-4">
                                {order.items?.map((item, index) => (
                                    <div key={index} className="flex justify-between items-center py-3 border-b border-slate-100 dark:border-slate-800 last:border-0">
                                        <div>
                                            <div className="font-medium text-slate-900 dark:text-white">
                                                {item.serviceId && servicesMap[item.serviceId] ? servicesMap[item.serviceId] : 'Service Item'}
                                            </div>
                                            <div className="text-sm text-slate-500 dark:text-slate-400">
                                                Quantity: {item.quantity}
                                            </div>
                                        </div>
                                        <div className="font-medium text-slate-900 dark:text-white">
                                            ${((item.priceAmount || 0) * (item.quantity || 1)).toFixed(2)}
                                        </div>
                                    </div>
                                ))}
                            </div>
                            <div className="mt-4 pt-4 border-t border-slate-200 dark:border-slate-800 flex justify-between items-center">
                                <span className="font-semibold text-slate-900 dark:text-white">Total Amount</span>
                                <span className="text-xl font-bold text-cyan-600 dark:text-cyan-400">
                                    ${order.totalPrice?.toFixed(2)}
                                </span>
                            </div>
                        </div>
                    </div>

                    {/* Sidebar */}
                    <div className="space-y-6">
                        {/* Schedule */}
                        <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 p-6">
                            <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-4 flex items-center gap-2">
                                <Calendar className="w-5 h-5 text-cyan-500" />
                                Schedule
                            </h3>
                            <div className="space-y-1">
                                <p className="text-sm text-slate-500 dark:text-slate-400">Scheduled for</p>
                                <p className="font-medium text-slate-900 dark:text-white">
                                    {order.scheduledDate ? format(new Date(order.scheduledDate), 'PPP p') : 'Not scheduled'}
                                </p>
                            </div>
                        </div>

                        {/* Location */}
                        <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 p-6">
                            <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-4 flex items-center gap-2">
                                <MapPin className="w-5 h-5 text-cyan-500" />
                                Location
                            </h3>
                            <div className="space-y-1">
                                <p className="text-sm text-slate-500 dark:text-slate-400">Delivery Address</p>
                                <p className="font-medium text-slate-900 dark:text-white">
                                    {/* Address missing in DTO, placeholder */}
                                    123 Main St, Apt 4B<br />
                                    New York, NY 10001
                                </p>
                            </div>
                        </div>

                        {/* Payment */}
                        <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 p-6">
                            <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-4 flex items-center gap-2">
                                <CreditCard className="w-5 h-5 text-cyan-500" />
                                Payment
                            </h3>
                            <div className="space-y-1">
                                <p className="text-sm text-slate-500 dark:text-slate-400">Method</p>
                                <p className="font-medium text-slate-900 dark:text-white">
                                    {getPaymentMethodLabel(order.paymentMethod)}
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </DashboardLayout>
    );
};
