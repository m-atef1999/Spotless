import { useState, useEffect } from 'react';
import { DriversService, type OrderDto } from '../../lib/api';
import { OrderStatus } from '../../lib/constants';
import { useToast } from '../../components/ui/Toast';
import { useAuthStore } from '../../store/authStore';
import { DashboardLayout } from '../../layouts/DashboardLayout';

export function OrderHistoryPage() {
    const [orders, setOrders] = useState<OrderDto[]>([]);
    const [loading, setLoading] = useState(true);
    const { user } = useAuthStore();
    const { addToast } = useToast();

    useEffect(() => {
        const fetchOrderHistory = async () => {
            try {
                setLoading(true);
                const data = await DriversService.getApiDriversOrders();
                setOrders(data || []);
            } catch (error) {
                addToast('Failed to load order history', 'error');
                console.error('Error fetching order history:', error);
            } finally {
                setLoading(false);
            }
        };

        // Check if user is a driver
        if (user && 'vehicleInfo' in user) {
            fetchOrderHistory();
        }
    }, [user, addToast]);

    const getStatusColor = (status?: number) => {
        switch (status) {
            case OrderStatus.Delivered: // Completed
                return 'bg-green-100 text-green-800';
            default:
                return 'bg-gray-100 text-gray-800';
        }
    };

    const getStatusLabel = (status?: number) => {
        switch (status) {
            case OrderStatus.Requested: return 'Pending';
            case OrderStatus.Confirmed: return 'Confirmed';
            case OrderStatus.InCleaning: return 'In Progress';
            case OrderStatus.PickedUp: return 'Picked Up';
            case OrderStatus.Delivered: return 'Completed';
            case OrderStatus.Cancelled: return 'Cancelled';
            default: return 'Unknown';
        }
    };

    return (
        <DashboardLayout role="Driver">
            <div className="space-y-8">
                <div>
                    <h1 className="text-2xl font-bold text-slate-900 dark:text-white">Order History</h1>
                    <p className="text-slate-500 dark:text-slate-400 mt-1">View your completed deliveries</p>
                </div>

                {loading ? (
                    <div className="flex items-center justify-center h-64">
                        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-cyan-600"></div>
                    </div>
                ) : orders.length === 0 ? (
                    <div className="bg-white dark:bg-slate-900 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-800 p-12 text-center">
                        <p className="text-slate-500 dark:text-slate-400 text-lg">No completed orders yet</p>
                    </div>
                ) : (
                    <div className="bg-white dark:bg-slate-900 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-800 overflow-hidden">
                        <div className="overflow-x-auto">
                            <table className="min-w-full divide-y divide-slate-100 dark:divide-slate-800">
                                <thead className="bg-slate-50 dark:bg-slate-800/50">
                                    <tr>
                                        <th className="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Order ID</th>
                                        <th className="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Date</th>
                                        <th className="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Amount</th>
                                        <th className="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Status</th>
                                    </tr>
                                </thead>
                                <tbody className="bg-white dark:bg-slate-900 divide-y divide-slate-100 dark:divide-slate-800">
                                    {orders.map((order) => (
                                        <tr key={order.id} className="hover:bg-slate-50 dark:hover:bg-slate-800/50 transition-colors">
                                            <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-slate-900 dark:text-white">
                                                #{order.id?.substring(0, 8)}
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap text-sm text-slate-500 dark:text-slate-400">
                                                {order.orderDate ? new Date(order.orderDate).toLocaleDateString() : 'N/A'}
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap text-sm text-slate-900 dark:text-white font-medium">
                                                {order.totalPrice ? `${order.totalPrice} ${order.currency || 'EGP'}` : 'N/A'}
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap">
                                                <span className={`px-2 py-1 inline-flex text-xs leading-5 font-semibold rounded-full ${getStatusColor(order.status)}`}>
                                                    {getStatusLabel(order.status)}
                                                </span>
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    </div>
                )}
            </div>
        </DashboardLayout>
    );
}
