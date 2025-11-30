import React, { useState, useEffect } from 'react';
import { X, Calendar, MapPin, Clock, User, Package, Truck } from 'lucide-react';
import { OrdersService, OrderStatus, type OrderDto } from '../../lib/api';

interface OrderDetailsModalProps {
    orderId: string;
    onClose: () => void;
    onStatusChange?: () => void;
}

export const OrderDetailsModal: React.FC<OrderDetailsModalProps> = ({ orderId, onClose, onStatusChange }) => {
    const [order, setOrder] = useState<OrderDto | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const [isUpdating, setIsUpdating] = useState(false);

    useEffect(() => {
        const fetchOrder = async () => {
            try {
                const response = await OrdersService.getApiOrders({ id: orderId });
                setOrder(response);
            } catch (error) {
                console.error('Failed to fetch order details', error);
            } finally {
                setIsLoading(false);
            }
        };

        if (orderId) {
            fetchOrder();
        }
    }, [orderId]);

    const handleStatusChange = async (newStatus: number) => {
        if (!order || !order.id) return;
        setIsUpdating(true);
        try {
            await OrdersService.putApiOrdersStatus({
                id: order.id,
                status: newStatus
            });
            // @ts-ignore - casting number to OrderStatus enum
            setOrder({ ...order, status: newStatus });
            if (onStatusChange) onStatusChange();
        } catch (error) {
            console.error('Failed to update status', error);
            alert('Failed to update status');
        } finally {
            setIsUpdating(false);
        }
    };

    const getStatusBadge = (status: number | undefined) => {
        if (status === undefined) return null;
        switch (status) {
            case OrderStatus.Requested: return <span className="px-3 py-1 rounded-full text-sm font-medium bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400">Pending</span>;
            case OrderStatus.Confirmed: return <span className="px-3 py-1 rounded-full text-sm font-medium bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400">Confirmed</span>;
            case OrderStatus.DriverAssigned: return <span className="px-3 py-1 rounded-full text-sm font-medium bg-indigo-100 text-indigo-800 dark:bg-indigo-900/30 dark:text-indigo-400">Driver Assigned</span>;
            case OrderStatus.PickedUp: return <span className="px-3 py-1 rounded-full text-sm font-medium bg-orange-100 text-orange-800 dark:bg-orange-900/30 dark:text-orange-400">Picked Up</span>;
            case OrderStatus.InCleaning: return <span className="px-3 py-1 rounded-full text-sm font-medium bg-purple-100 text-purple-800 dark:bg-purple-900/30 dark:text-purple-400">In Cleaning</span>;
            case OrderStatus.OutForDelivery: return <span className="px-3 py-1 rounded-full text-sm font-medium bg-teal-100 text-teal-800 dark:bg-teal-900/30 dark:text-teal-400">Out for Delivery</span>;
            case OrderStatus.Delivered: return <span className="px-3 py-1 rounded-full text-sm font-medium bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400">Completed</span>;
            case OrderStatus.Cancelled: return <span className="px-3 py-1 rounded-full text-sm font-medium bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400">Cancelled</span>;
            default: return <span className="px-3 py-1 rounded-full text-sm font-medium bg-slate-100 text-slate-800 dark:bg-slate-800 dark:text-slate-400">Unknown</span>;
        }
    };

    if (isLoading) {
        return (
            <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/50 backdrop-blur-sm">
                <div className="bg-white dark:bg-slate-900 rounded-2xl p-8 shadow-2xl">
                    <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
                </div>
            </div>
        );
    }

    if (!order) return null;

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/50 backdrop-blur-sm overflow-y-auto">
            <div className="bg-white dark:bg-slate-900 rounded-2xl shadow-2xl w-full max-w-3xl my-8 border border-slate-200 dark:border-slate-800 animate-fade-in-up">
                <div className="p-6 border-b border-slate-100 dark:border-slate-800 flex justify-between items-center sticky top-0 bg-white dark:bg-slate-900 z-10 rounded-t-2xl">
                    <div>
                        <h2 className="text-xl font-bold text-slate-900 dark:text-white flex items-center gap-2">
                            Order #{order.id?.substring(0, 8) || 'Unknown'}
                            {getStatusBadge(order.status)}
                        </h2>
                        <p className="text-sm text-slate-500 mt-1">
                            Placed on {order.createdAt ? new Date(order.createdAt).toLocaleDateString() : 'N/A'} at {order.createdAt ? new Date(order.createdAt).toLocaleTimeString() : ''}
                        </p>
                    </div>
                    <button
                        onClick={onClose}
                        className="text-slate-400 hover:text-slate-500 dark:hover:text-slate-300 transition-colors p-2 hover:bg-slate-100 dark:hover:bg-slate-800 rounded-full"
                    >
                        <X className="w-6 h-6" />
                    </button>
                </div>

                <div className="p-6 space-y-8">
                    {/* Status Control */}
                    <div className="bg-slate-50 dark:bg-slate-800/50 p-4 rounded-xl border border-slate-200 dark:border-slate-700">
                        <label className="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                            Update Status (Admin Override)
                        </label>
                        <div className="flex flex-wrap gap-2">
                            {[
                                OrderStatus.Requested,
                                OrderStatus.Confirmed,
                                OrderStatus.DriverAssigned,
                                OrderStatus.PickedUp,
                                OrderStatus.InCleaning,
                                OrderStatus.OutForDelivery,
                                OrderStatus.Delivered,
                                OrderStatus.Cancelled
                            ].map((status) => (
                                <button
                                    key={status}
                                    onClick={() => handleStatusChange(status)}
                                    disabled={isUpdating || order.status === status}
                                    className={`px-3 py-1.5 text-xs font-medium rounded-lg transition-colors border ${order.status === status
                                        ? 'bg-blue-600 text-white border-blue-600'
                                        : 'bg-white dark:bg-slate-800 text-slate-600 dark:text-slate-300 border-slate-200 dark:border-slate-700 hover:border-blue-400 dark:hover:border-blue-500'
                                        } disabled:opacity-50 disabled:cursor-not-allowed`}
                                >
                                    {/* @ts-ignore */}
                                    {getStatusBadge(status)?.props.children || 'Unknown'}
                                </button>
                            ))}
                        </div>
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                        {/* Customer Info */}
                        <div className="space-y-4">
                            <h3 className="text-lg font-semibold text-slate-900 dark:text-white flex items-center gap-2">
                                <User className="w-5 h-5 text-blue-500" />
                                Customer Details
                            </h3>
                            <div className="bg-slate-50 dark:bg-slate-800/50 rounded-xl p-4 border border-slate-100 dark:border-slate-800 space-y-3">
                                <div>
                                    <div className="text-xs text-slate-500 uppercase tracking-wider font-semibold">Customer ID</div>
                                    <div className="text-slate-900 dark:text-white font-mono text-sm">{order.customerId}</div>
                                </div>
                                {/* Add more customer details if available in DTO */}
                            </div>
                        </div>

                        {/* Schedule Info */}
                        <div className="space-y-4">
                            <h3 className="text-lg font-semibold text-slate-900 dark:text-white flex items-center gap-2">
                                <Calendar className="w-5 h-5 text-purple-500" />
                                Schedule
                            </h3>
                            <div className="bg-slate-50 dark:bg-slate-800/50 rounded-xl p-4 border border-slate-100 dark:border-slate-800 space-y-3">
                                <div>
                                    <div className="text-xs text-slate-500 uppercase tracking-wider font-semibold">Scheduled Date</div>
                                    <div className="text-slate-900 dark:text-white font-medium">
                                        {order.scheduledDate ? new Date(order.scheduledDate).toLocaleDateString(undefined, { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' }) : 'N/A'}
                                    </div>
                                </div>
                                <div>
                                    <div className="text-xs text-slate-500 uppercase tracking-wider font-semibold">Time Slot</div>
                                    <div className="text-slate-900 dark:text-white flex items-center gap-2">
                                        <Clock className="w-4 h-4 text-slate-400" />
                                        {order.startTime ? `${order.startTime} - ${order.endTime}` : 'Scheduled Slot'}
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                        {/* Locations */}
                        <div className="space-y-4">
                            <h3 className="text-lg font-semibold text-slate-900 dark:text-white flex items-center gap-2">
                                <MapPin className="w-5 h-5 text-red-500" />
                                Locations
                            </h3>
                            <div className="space-y-4">
                                <div className="bg-slate-50 dark:bg-slate-800/50 rounded-xl p-4 border border-slate-100 dark:border-slate-800">
                                    <div className="text-xs text-slate-500 uppercase tracking-wider font-semibold mb-1">Pickup Address</div>
                                    <div className="text-slate-900 dark:text-white">{order.pickupAddress || 'No address provided'}</div>
                                </div>
                                <div className="bg-slate-50 dark:bg-slate-800/50 rounded-xl p-4 border border-slate-100 dark:border-slate-800">
                                    <div className="text-xs text-slate-500 uppercase tracking-wider font-semibold mb-1">Delivery Address</div>
                                    <div className="text-slate-900 dark:text-white">{order.deliveryAddress || 'No address provided'}</div>
                                </div>
                            </div>
                        </div>

                        {/* Driver Info */}
                        <div className="space-y-4">
                            <h3 className="text-lg font-semibold text-slate-900 dark:text-white flex items-center gap-2">
                                <Truck className="w-5 h-5 text-orange-500" />
                                Driver Assignment
                            </h3>
                            <div className="bg-slate-50 dark:bg-slate-800/50 rounded-xl p-4 border border-slate-100 dark:border-slate-800">
                                {order.driverId ? (
                                    <div>
                                        <div className="text-xs text-slate-500 uppercase tracking-wider font-semibold mb-1">Assigned Driver</div>
                                        <div className="text-slate-900 dark:text-white font-medium">Driver #{order.driverId.substring(0, 8)}</div>
                                        <div className="mt-2">
                                            <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400">
                                                Assigned
                                            </span>
                                        </div>
                                    </div>
                                ) : (
                                    <div className="text-center py-4">
                                        <div className="text-slate-500 mb-2">No driver assigned</div>
                                    </div>
                                )}
                            </div>
                        </div>
                    </div>

                    {/* Order Items */}
                    <div className="space-y-4">
                        <h3 className="text-lg font-semibold text-slate-900 dark:text-white flex items-center gap-2">
                            <Package className="w-5 h-5 text-blue-500" />
                            Order Items
                        </h3>
                        <div className="bg-slate-50 dark:bg-slate-800/50 rounded-xl border border-slate-100 dark:border-slate-800 overflow-hidden">
                            <table className="w-full text-left">
                                <thead className="bg-slate-100 dark:bg-slate-800 border-b border-slate-200 dark:border-slate-700">
                                    <tr>
                                        <th className="px-4 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wider">Service</th>
                                        <th className="px-4 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wider text-right">Quantity</th>
                                        <th className="px-4 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wider text-right">Price</th>
                                        <th className="px-4 py-3 text-xs font-semibold text-slate-500 uppercase tracking-wider text-right">Total</th>
                                    </tr>
                                </thead>
                                <tbody className="divide-y divide-slate-200 dark:divide-slate-700">
                                    {order.items && order.items.length > 0 ? (
                                        order.items.map((item, index) => (
                                            <tr key={index}>
                                                <td className="px-4 py-3 text-sm text-slate-900 dark:text-white">{item.serviceName}</td>
                                                <td className="px-4 py-3 text-sm text-slate-600 dark:text-slate-300 text-right">{item.quantity}</td>
                                                <td className="px-4 py-3 text-sm text-slate-600 dark:text-slate-300 text-right">${item.priceAmount?.toFixed(2) || '0.00'}</td>
                                                <td className="px-4 py-3 text-sm font-medium text-slate-900 dark:text-white text-right">${((item.priceAmount || 0) * (item.quantity || 0)).toFixed(2)}</td>
                                            </tr>
                                        ))
                                    ) : (
                                        <tr>
                                            <td colSpan={4} className="px-4 py-8 text-center text-slate-500">No items found</td>
                                        </tr>
                                    )}
                                </tbody>
                                <tfoot className="bg-slate-100 dark:bg-slate-800 border-t border-slate-200 dark:border-slate-700">
                                    <tr>
                                        <td colSpan={3} className="px-4 py-3 text-sm font-bold text-slate-900 dark:text-white text-right">Total Amount</td>
                                        <td className="px-4 py-3 text-sm font-bold text-slate-900 dark:text-white text-right">${order.totalPrice?.toFixed(2) || '0.00'}</td>
                                    </tr>
                                </tfoot>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};
