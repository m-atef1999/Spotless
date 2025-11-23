import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { format } from 'date-fns';
import { ArrowLeft, Calendar, MapPin, CreditCard, Package, AlertCircle, Loader2, RotateCcw } from 'lucide-react';
import { DashboardLayout } from '../../layouts/DashboardLayout';
import { Button } from '../../components/ui/Button';
import { useToast } from '../../components/ui/Toast';
import { OrdersService, ServicesService, type OrderDto, OrderStatus, PaymentMethod, getOrderStatusLabel, getOrderStatusColor } from '../../lib/api';

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
                if (!id) return;
                const [orderResponse, servicesResponse] = await Promise.all([
                    OrdersService.getApiOrders({ id }),
                    ServicesService.getApiServices({ pageNumber: 1, pageSize: 100 })
                ]);

                // Create services map
                const map: Record<string, string> = {};
                if (servicesResponse.data) {
                    servicesResponse.data.forEach(s => {
                        if (s.id) map[s.id] = s.name || 'Unknown Service';
                    });
                }
                setServicesMap(map);

                if (orderResponse) {
                    setOrder(orderResponse);
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

    const getPaymentMethodLabel = (method?: string | number) => {
        // PaymentMethod in DTO might be string or number depending on serialization
        if (typeof method === 'string') return method;
        switch (method) {
            case PaymentMethod.CreditCard: return 'Credit Card';
            case PaymentMethod.DebitCard: return 'Debit Card';
            case PaymentMethod.Fawry: return 'Fawry';
            case PaymentMethod.PayPal: return 'PayPal';
            case PaymentMethod.Wallet: return 'Wallet';
            case PaymentMethod.CashOnDelivery: return 'Cash On Delivery';
            case PaymentMethod.Paymob: return 'Paymob';
            case PaymentMethod.Instapay: return 'Instapay';
            default: return 'Unknown';
        }
    };

    const { addToast } = useToast(); // Assuming useToast is available or needs import

    const handleCancelOrder = async () => {
        if (!window.confirm('Are you sure you want to cancel this order?')) return;
        try {
            if (id) {
                await OrdersService.postApiOrdersCancel({ id });
                setOrder(prev => prev ? { ...prev, status: OrderStatus.Cancelled } : null);
                addToast('Order cancelled successfully', 'success');
            }
        } catch (error) {
            console.error('Failed to cancel order', error);
            addToast('Failed to cancel order', 'error');
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
                            <span className={`px-3 py-1 rounded-full text-sm font-medium ${getOrderStatusColor(order.status || 0)}`}>
                                {getOrderStatusLabel(order.status || 0)}
                            </span>
                        </h1>
                        <p className="text-slate-500 dark:text-slate-400 text-sm mt-1">
                            Placed on {order.createdAt ? format(new Date(order.createdAt.endsWith('Z') ? order.createdAt : order.createdAt + 'Z'), 'PPP p') : (order.orderDate ? format(new Date(order.orderDate), 'PPP p') : 'Unknown date')}
                        </p>
                    </div>

                    <div className="ml-auto flex gap-3">
                        {/* Check for both number and string status representations */}
                        {([OrderStatus.Requested, OrderStatus.Confirmed, OrderStatus.PaymentFailed].includes(order.status as any) ||
                            ['Requested', 'Confirmed', 'PaymentFailed'].includes(order.status as any)) && (
                                <Button variant="outline" className="text-red-600 border-red-200 hover:bg-red-50" onClick={handleCancelOrder}>
                                    Cancel Order
                                </Button>
                            )}
                        {order.status === OrderStatus.Delivered && (
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
                                                {item.serviceName || (item.serviceId && servicesMap[item.serviceId] ? servicesMap[item.serviceId] : 'Service Item')}
                                            </div>
                                            <div className="text-sm text-slate-500 dark:text-slate-400">
                                                Quantity: {item.quantity}
                                            </div>
                                        </div>
                                        <div className="font-medium text-slate-900 dark:text-white">
                                            {((item.priceAmount || 0) * (item.quantity || 1)).toFixed(2)} EGP
                                        </div>
                                    </div>
                                ))}
                            </div>
                            <div className="mt-4 pt-4 border-t border-slate-200 dark:border-slate-800 flex justify-between items-center">
                                <span className="font-semibold text-slate-900 dark:text-white">Total Amount</span>
                                <span className="text-xl font-bold text-cyan-600 dark:text-cyan-400">
                                    {order.totalPrice?.toFixed(2)} EGP
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
                            <div className="space-y-3">
                                <div>
                                    <p className="text-sm text-slate-500 dark:text-slate-400">Pickup Time</p>
                                    <div className="font-medium text-slate-900 dark:text-white">
                                        {order.scheduledDate && order.startTime ? (
                                            <>
                                                {format(new Date(order.scheduledDate), 'PPP')} at {format(new Date(`2000-01-01T${order.startTime}`), 'h:mm a')}
                                            </>
                                        ) : (
                                            'Not Scheduled'
                                        )}
                                    </div>
                                </div>

                                {order.estimatedDurationHours && order.estimatedDurationHours > 0 && (
                                    <div>
                                        <p className="text-sm text-slate-500 dark:text-slate-400">Total Estimated Hours</p>
                                        <div className="font-medium text-slate-900 dark:text-white">
                                            {order.estimatedDurationHours} hrs
                                        </div>
                                    </div>
                                )}
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
                                <p className="font-medium text-slate-900 dark:text-white break-words">
                                    {order.deliveryAddress || 'No address provided'}
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
