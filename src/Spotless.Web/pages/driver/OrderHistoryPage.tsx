import { useState, useEffect } from 'react';
import { DriversService, type OrderDto } from '../../lib/api';
import { OrderStatus } from '../../lib/constants';
import { useToast } from '../../components/ui/Toast';
import { useAuthStore } from '../../store/authStore';

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
        <div className="min-h-screen bg-gray-50 p-6">
            <div className="max-w-7xl mx-auto">
                <div className="mb-8">
                    <h1 className="text-3xl font-bold text-gray-900">Order History</h1>
                    <p className="text-gray-600 mt-2">View your completed deliveries</p>
                </div>

                {loading ? (
                    <div className="flex items-center justify-center h-64">
                        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
                    </div>
                ) : orders.length === 0 ? (
                    <div className="bg-white rounded-lg shadow-sm p-12 text-center">
                        <p className="text-gray-500 text-lg">No completed orders yet</p>
                    </div>
                ) : (
                    <div className="bg-white rounded-lg shadow-sm overflow-hidden">
                        <table className="min-w-full divide-y divide-gray-200">
                            <thead className="bg-gray-50">
                                <tr>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Order ID</th>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Date</th>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Pickup</th>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Delivery</th>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Amount</th>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Status</th>
                                </tr>
                            </thead>
                            <tbody className="bg-white divide-y divide-gray-200">
                                {orders.map((order) => (
                                    <tr key={order.id} className="hover:bg-gray-50">
                                        <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                                            #{order.id}
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                                            {order.orderDate ? new Date(order.orderDate).toLocaleDateString() : 'N/A'}
                                        </td>
                                        <td className="px-6 py-4 text-sm text-gray-500">
                                            {order.pickupLatitude && order.pickupLongitude
                                                ? `${order.pickupLatitude.toFixed(4)}, ${order.pickupLongitude.toFixed(4)}`
                                                : 'N/A'}
                                        </td>
                                        <td className="px-6 py-4 text-sm text-gray-500">
                                            {order.deliveryLatitude && order.deliveryLongitude
                                                ? `${order.deliveryLatitude.toFixed(4)}, ${order.deliveryLongitude.toFixed(4)}`
                                                : 'N/A'}
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                                            {order.totalPrice ? `${order.totalPrice} ${order.currency || 'EGP'}` : 'N/A'}
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap">
                                            <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${getStatusColor(order.status)}`}>
                                                {getStatusLabel(order.status)}
                                            </span>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                )}
            </div>
        </div>
    );
}
