import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { format } from 'date-fns';
import { Package, Calendar, MapPin, ChevronRight, Loader2, AlertCircle } from 'lucide-react';
import { DashboardLayout } from '../../layouts/DashboardLayout';
import { Button } from '../../components/ui/Button';
import { useToast } from '../../components/ui/Toast';

import {
    OrdersService,
    type OrderDto,
    OrderStatus,
    getOrderStatusLabel,
    getOrderStatusColor
} from '../../lib/api';

export const MyOrdersPage: React.FC = () => {
    const [orders, setOrders] = useState<OrderDto[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const { addToast } = useToast();

    useEffect(() => {
        fetchOrders();
    }, []);

    const fetchOrders = async () => {
        try {
            setIsLoading(true);
            const response = await OrdersService.getApiOrdersCustomer({ pageNumber: 1, pageSize: 100 });
            const data = (response.data as unknown as OrderDto[]) || [];
            // Sort by date descending
            const sorted = data.sort((a, b) =>
                new Date(b.scheduledDate || 0).getTime() - new Date(a.scheduledDate || 0).getTime()
            );
            setOrders(sorted);
        } catch (err) {
            console.error('Failed to fetch orders', err);
            setError('Failed to load your orders. Please try again later.');
        } finally {
            setIsLoading(false);
        }
    };

    const handleCancelOrder = async (orderId: string) => {
        if (!confirm('Are you sure you want to cancel this order?')) return;

        try {
            await OrdersService.postApiOrdersCancel({ id: orderId });
            addToast('Order cancelled successfully', 'success');
            // Refresh orders
            fetchOrders();
        } catch (err) {
            console.error('Failed to cancel order', err);
            addToast('Failed to cancel order', 'error');
        }
    };

    const canCancel = (status?: number) => {
        return status === OrderStatus.Requested || status === OrderStatus.Confirmed;
    };

    return (
        <DashboardLayout role="Customer">
            <div className="max-w-5xl mx-auto space-y-8">
                <div className="flex justify-between items-center">
                    <h1 className="text-2xl font-bold text-slate-900 dark:text-white">My Orders</h1>
                    <Link to="/customer/new-order">
                        <Button>New Order</Button>
                    </Link>
                </div>

                {isLoading ? (
                    <div className="flex justify-center py-12">
                        <Loader2 className="w-8 h-8 animate-spin text-cyan-500" />
                    </div>
                ) : error ? (
                    <div className="p-4 bg-red-50 text-red-600 rounded-xl flex items-center gap-2">
                        <AlertCircle className="w-5 h-5" />
                        {error}
                    </div>
                ) : orders.length === 0 ? (
                    <div className="text-center py-16 bg-white dark:bg-slate-900 rounded-2xl border border-slate-200 dark:border-slate-800">
                        <div className="w-16 h-16 bg-slate-100 dark:bg-slate-800 rounded-full flex items-center justify-center mx-auto mb-4">
                            <Package className="w-8 h-8 text-slate-400" />
                        </div>
                        <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-2">No orders yet</h3>
                        <p className="text-slate-500 dark:text-slate-400 mb-6 max-w-sm mx-auto">
                            You haven't placed any orders yet. Start by creating your first cleaning request.
                        </p>
                        <Link to="/customer/new-order">
                            <Button>Create First Order</Button>
                        </Link>
                    </div>
                ) : (
                    <div className="space-y-4">
                        {orders.map((order) => (
                            <div
                                key={order.id}
                                className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 p-4 sm:p-6 hover:border-cyan-300 dark:hover:border-cyan-700 transition-all group"
                            >
                                <div className="flex flex-col sm:flex-row justify-between gap-4">
                                    <div className="space-y-1">
                                        <div className="flex items-center gap-3">
                                            <span className="font-mono text-sm text-slate-500">#{order.id?.slice(0, 8)}</span>
                                            <span className={`px-2.5 py-0.5 rounded-full text-xs font-medium ${getOrderStatusColor(order.status || 0)}`}>
                                                {getOrderStatusLabel(order.status || 0)}
                                            </span>
                                        </div>
                                        <div className="flex items-center gap-2 text-slate-900 dark:text-white font-medium">
                                            <Calendar className="w-4 h-4 text-slate-400" />
                                            {order.scheduledDate ? format(new Date(order.scheduledDate), 'PPP p') : 'Not scheduled'}
                                        </div>
                                        <div className="flex items-center gap-2 text-sm text-slate-500 dark:text-slate-400">
                                            <MapPin className="w-4 h-4" />
                                            Delivery Address
                                        </div>
                                    </div>

                                    <div className="flex flex-col sm:items-end justify-between gap-4">
                                        <div className="text-lg font-bold text-cyan-600 dark:text-cyan-400">
                                            ${order.totalPrice?.toFixed(2)}
                                        </div>
                                        <div className="flex gap-2">
                                            {canCancel(order.status) && order.id && (
                                                <Button
                                                    variant="outline"
                                                    size="sm"
                                                    onClick={() => handleCancelOrder(order.id!)}
                                                    className="text-red-600 hover:bg-red-50 hover:text-red-700 border-red-200"
                                                >
                                                    Cancel
                                                </Button>
                                            )}
                                            <Button variant="outline" size="sm" className="group-hover:bg-cyan-50 dark:group-hover:bg-cyan-900/20 group-hover:text-cyan-700 dark:group-hover:text-cyan-300 group-hover:border-cyan-200 dark:group-hover:border-cyan-800">
                                                View Details
                                                <ChevronRight className="w-4 h-4 ml-1" />
                                            </Button>
                                        </div>
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
