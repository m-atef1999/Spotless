import { useState, useEffect } from 'react';
import { DriversService, ServicesService, type OrderDto, type ServiceDto } from '../../lib/api';
import { OrderStatus } from '../../lib/constants';
import { useToast } from '../../components/ui/Toast';
import { useAuthStore } from '../../store/authStore';
import { DashboardLayout } from '../../layouts/DashboardLayout';
import { Button } from '../../components/ui/Button';
import { X, MapPin, Clock, Package, DollarSign, Scale } from 'lucide-react';

export function OrderHistoryPage() {
    const [orders, setOrders] = useState<OrderDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [selectedOrder, setSelectedOrder] = useState<OrderDto | null>(null);
    const [servicesMap, setServicesMap] = useState<Record<string, ServiceDto>>({});
    const { user } = useAuthStore();
    const { addToast } = useToast();

    useEffect(() => {
        const fetchData = async () => {
            try {
                setLoading(true);
                // Fetch orders and services in parallel
                const [ordersData, servicesData] = await Promise.all([
                    DriversService.getApiDriversOrders(),
                    ServicesService.getApiServices({ pageNumber: 1, pageSize: 100 })
                ]);
                setOrders(ordersData || []);

                // Build services map for lookups
                const map: Record<string, ServiceDto> = {};
                if (servicesData?.data) {
                    servicesData.data.forEach(s => {
                        if (s.id) map[s.id] = s;
                    });
                }
                setServicesMap(map);
            } catch (error) {
                addToast('Failed to load order history', 'error');
                console.error('Error fetching order history:', error);
            } finally {
                setLoading(false);
            }
        };

        // Check if user is a driver
        if (user && 'vehicleInfo' in user) {
            fetchData();
        }
    }, [user, addToast]);


    const getStatusColor = (status?: number | string) => {
        if (status === undefined || status === null) return 'bg-gray-100 text-gray-800';

        // Handle string status
        if (typeof status === 'string') {
            const statusLower = status.toLowerCase();
            if (statusLower === 'delivered') return 'bg-green-100 text-green-800';
            if (statusLower === 'cancelled') return 'bg-red-100 text-red-800';
            if (statusLower === 'requested') return 'bg-blue-100 text-blue-800';
            if (statusLower === 'confirmed') return 'bg-green-100 text-green-800';
            if (statusLower === 'driverassigned') return 'bg-purple-100 text-purple-800';
            if (statusLower === 'pickedup') return 'bg-indigo-100 text-indigo-800';
            if (statusLower === 'incleaning') return 'bg-yellow-100 text-yellow-800';
            if (statusLower === 'outfordelivery') return 'bg-cyan-100 text-cyan-800';
            return 'bg-gray-100 text-gray-800';
        }

        switch (status) {
            case OrderStatus.Delivered:
                return 'bg-green-100 text-green-800';
            case OrderStatus.Cancelled:
                return 'bg-red-100 text-red-800';
            default:
                return 'bg-gray-100 text-gray-800';
        }
    };

    const getStatusLabel = (status?: number | string) => {
        if (status === undefined || status === null) return 'Unknown';

        // Handle string status (API may return string names)
        if (typeof status === 'string') {
            // Insert space before capital letters
            return status.replace(/([A-Z])/g, ' $1').trim();
        }

        switch (status) {
            case OrderStatus.PaymentFailed: return 'Payment Failed';
            case OrderStatus.Requested: return 'Requested';
            case OrderStatus.Confirmed: return 'Confirmed';
            case OrderStatus.DriverAssigned: return 'Driver Assigned';
            case OrderStatus.PickedUp: return 'Picked Up';
            case OrderStatus.InCleaning: return 'In Cleaning';
            case OrderStatus.OutForDelivery: return 'Out for Delivery';
            case OrderStatus.Delivered: return 'Delivered';
            case OrderStatus.Cancelled: return 'Cancelled';
            default: return 'Unknown';
        }
    };

    // Helper to compare status that handles both string and number values
    const isStatus = (orderStatus: number | string | undefined, targetStatus: number): boolean => {
        if (orderStatus === undefined || orderStatus === null) return false;
        if (typeof orderStatus === 'number') return orderStatus === targetStatus;
        // Map string to status name (handle both formats)
        const statusNames: Record<number, string[]> = {
            [OrderStatus.DriverAssigned]: ['DriverAssigned', 'driverassigned', 'Driver Assigned'],
            [OrderStatus.PickedUp]: ['PickedUp', 'pickedup', 'Picked Up'],
            [OrderStatus.InCleaning]: ['InCleaning', 'incleaning', 'In Cleaning'],
            [OrderStatus.OutForDelivery]: ['OutForDelivery', 'outfordelivery', 'Out For Delivery', 'Out for Delivery'],
            [OrderStatus.Delivered]: ['Delivered', 'delivered'],
            [OrderStatus.Cancelled]: ['Cancelled', 'cancelled'],
            [OrderStatus.Requested]: ['Requested', 'requested'],
            [OrderStatus.Confirmed]: ['Confirmed', 'confirmed'],
        };
        const names = statusNames[targetStatus];
        return names ? names.some(n => n.toLowerCase() === orderStatus.toLowerCase()) : false;
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
                                        <th className="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Actions</th>
                                    </tr>
                                </thead>
                                <tbody className="bg-white dark:bg-slate-900 divide-y divide-slate-100 dark:divide-slate-800">
                                    {orders.map((order) => (
                                        <tr key={order.id} className="hover:bg-slate-50 dark:hover:bg-slate-800/50 transition-colors cursor-pointer" onClick={() => setSelectedOrder(order)}>
                                            <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-cyan-600 dark:text-cyan-400 underline">
                                                #{order.id?.substring(0, 8)}
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap text-sm text-slate-500 dark:text-slate-400">
                                                {order.orderDate ? new Date(order.orderDate).toLocaleDateString() : 'N/A'}
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap text-sm text-slate-900 dark:text-white font-medium">
                                                {order.totalPrice ? `${order.totalPrice} ${order.currency || 'EGP'}` : 'N/A'}
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap">
                                                <div className="flex items-center gap-2">
                                                    <span className={`px-2 py-1 inline-flex text-xs leading-5 font-semibold rounded-full ${getStatusColor(order.status)}`}>
                                                        {getStatusLabel(order.status)}
                                                    </span>
                                                    {isStatus(order.status, OrderStatus.DriverAssigned) && (
                                                        <Button
                                                            size="sm"
                                                            onClick={async (e) => {
                                                                e.stopPropagation();
                                                                try {
                                                                    await DriversService.putApiDriversOrdersStatus({
                                                                        orderId: order.id!,
                                                                        requestBody: OrderStatus.PickedUp
                                                                    });
                                                                    const data = await DriversService.getApiDriversOrders();
                                                                    setOrders(data || []);
                                                                    addToast('Order picked up', 'success');
                                                                } catch (e) {
                                                                    console.error(e);
                                                                    addToast('Failed to update status', 'error');
                                                                }
                                                            }}
                                                        >
                                                            Picked Up
                                                        </Button>
                                                    )}
                                                    {isStatus(order.status, OrderStatus.PickedUp) && (
                                                        <Button
                                                            size="sm"
                                                            onClick={async (e) => {
                                                                e.stopPropagation();
                                                                try {
                                                                    await DriversService.putApiDriversOrdersStatus({
                                                                        orderId: order.id!,
                                                                        requestBody: OrderStatus.InCleaning
                                                                    });
                                                                    const data = await DriversService.getApiDriversOrders();
                                                                    setOrders(data || []);
                                                                    addToast('Delivered to laundry', 'success');
                                                                } catch (e) {
                                                                    console.error(e);
                                                                    addToast('Failed to update status', 'error');
                                                                }
                                                            }}
                                                        >
                                                            Delivered to Laundry
                                                        </Button>
                                                    )}
                                                    {isStatus(order.status, OrderStatus.InCleaning) && (
                                                        <span className="text-sm text-slate-500 dark:text-slate-400 italic">Cleaning in progress...</span>
                                                    )}
                                                    {isStatus(order.status, OrderStatus.OutForDelivery) && (
                                                        <Button
                                                            size="sm"
                                                            onClick={async (e) => {
                                                                e.stopPropagation();
                                                                try {
                                                                    await DriversService.putApiDriversOrdersStatus({
                                                                        orderId: order.id!,
                                                                        requestBody: OrderStatus.Delivered
                                                                    });
                                                                    const data = await DriversService.getApiDriversOrders();
                                                                    setOrders(data || []);
                                                                    addToast('Order delivered!', 'success');
                                                                } catch (e) {
                                                                    console.error(e);
                                                                    addToast('Failed to update status', 'error');
                                                                }
                                                            }}
                                                        >
                                                            Delivered to Customer
                                                        </Button>
                                                    )}
                                                </div>
                                            </td>
                                            <td className="px-6 py-4 whitespace-nowrap" onClick={(e) => e.stopPropagation()}>
                                                <button
                                                    onClick={() => setSelectedOrder(order)}
                                                    className="text-xs bg-slate-100 text-slate-700 px-3 py-1 rounded hover:bg-slate-200 dark:bg-slate-700 dark:text-slate-300"
                                                >
                                                    View Details
                                                </button>
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    </div>
                )}
            </div>

            {/* Order Details Modal */}
            {selectedOrder && (
                <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4" onClick={() => setSelectedOrder(null)}>
                    <div className="bg-white dark:bg-slate-900 rounded-2xl shadow-xl max-w-lg w-full max-h-[90vh] overflow-auto" onClick={(e) => e.stopPropagation()}>
                        <div className="flex items-center justify-between p-6 border-b border-slate-100 dark:border-slate-800">
                            <h2 className="text-xl font-bold text-slate-900 dark:text-white">Order Details</h2>
                            <button onClick={() => setSelectedOrder(null)} className="text-slate-400 hover:text-slate-600 dark:hover:text-slate-300">
                                <X className="w-6 h-6" />
                            </button>
                        </div>
                        <div className="p-6 space-y-6">
                            <div className="flex items-center justify-between">
                                <span className="text-slate-500 dark:text-slate-400">Order ID</span>
                                <span className="font-mono text-slate-900 dark:text-white">#{selectedOrder.id?.substring(0, 8)}</span>
                            </div>
                            <div className="flex items-center justify-between">
                                <span className="text-slate-500 dark:text-slate-400">Status</span>
                                <span className={`px-2 py-1 text-xs font-semibold rounded-full ${getStatusColor(selectedOrder.status)}`}>
                                    {getStatusLabel(selectedOrder.status)}
                                </span>
                            </div>

                            <div className="space-y-2">
                                <div className="flex items-center gap-2 text-slate-600 dark:text-slate-400">
                                    <MapPin className="w-4 h-4" />
                                    <span className="font-medium">Pickup Address</span>
                                </div>
                                <p className="text-slate-900 dark:text-white pl-6">
                                    {selectedOrder.pickupAddress || `${selectedOrder.pickupLatitude?.toFixed(4)}, ${selectedOrder.pickupLongitude?.toFixed(4)}` || 'Not specified'}
                                </p>
                            </div>

                            <div className="space-y-2">
                                <div className="flex items-center gap-2 text-slate-600 dark:text-slate-400">
                                    <MapPin className="w-4 h-4" />
                                    <span className="font-medium">Delivery Address</span>
                                </div>
                                <p className="text-slate-900 dark:text-white pl-6">
                                    {selectedOrder.deliveryAddress || `${selectedOrder.deliveryLatitude?.toFixed(4)}, ${selectedOrder.deliveryLongitude?.toFixed(4)}` || 'Not specified'}
                                </p>
                            </div>

                            <div className="flex items-center justify-between">
                                <div className="flex items-center gap-2 text-slate-500 dark:text-slate-400">
                                    <Clock className="w-4 h-4" />
                                    <span>Scheduled Time</span>
                                </div>
                                <span className="text-slate-900 dark:text-white">
                                    {selectedOrder.scheduledDate ? new Date(selectedOrder.scheduledDate).toLocaleDateString() : 'N/A'}
                                    {selectedOrder.startTime && ` at ${selectedOrder.startTime}`}
                                </span>
                            </div>

                            <div className="flex items-center justify-between">
                                <div className="flex items-center gap-2 text-slate-500 dark:text-slate-400">
                                    <Clock className="w-4 h-4" />
                                    <span>Estimated Duration</span>
                                </div>
                                <span className="text-slate-900 dark:text-white">
                                    {selectedOrder.estimatedDurationHours ? `${selectedOrder.estimatedDurationHours} hours` : 'N/A'}
                                </span>
                            </div>

                            <div className="flex items-center justify-between">
                                <div className="flex items-center gap-2 text-slate-500 dark:text-slate-400">
                                    <Package className="w-4 h-4" />
                                    <span>Service</span>
                                </div>
                                <span className="text-slate-900 dark:text-white">
                                    {selectedOrder.serviceName || 'N/A'}
                                </span>
                            </div>

                            {selectedOrder.items && selectedOrder.items.length > 0 && (
                                <div className="space-y-2">
                                    <span className="text-slate-500 dark:text-slate-400 font-medium">Items</span>
                                    <ul className="space-y-3 pl-4">
                                        {selectedOrder.items.map((item, idx) => {
                                            const service = item.serviceId ? servicesMap[item.serviceId] : null;
                                            return (
                                                <li key={idx} className="text-slate-900 dark:text-white text-sm border-b border-slate-100 dark:border-slate-800 pb-2 last:border-0">
                                                    <div className="flex justify-between items-start">
                                                        <div>
                                                            <span className="font-medium">{item.serviceName || service?.name || 'Service'}</span>
                                                            <span className="text-slate-500"> x{item.quantity || 1}</span>
                                                        </div>
                                                        <span className="text-cyan-600 dark:text-cyan-400 font-medium">
                                                            {((item.priceAmount || 0) * (item.quantity || 1)).toFixed(0)} EGP
                                                        </span>
                                                    </div>
                                                    {service && (
                                                        <div className="text-xs text-slate-500 dark:text-slate-400 mt-1 flex gap-4">
                                                            {service.estimatedDurationHours && (
                                                                <span className="flex items-center gap-1">
                                                                    <Clock className="w-3 h-3" />
                                                                    Est. {service.estimatedDurationHours}h
                                                                </span>
                                                            )}
                                                            {service.maxWeightKg && (
                                                                <span className="flex items-center gap-1">
                                                                    <Scale className="w-3 h-3" />
                                                                    Max {service.maxWeightKg} kg
                                                                </span>
                                                            )}
                                                        </div>
                                                    )}
                                                </li>
                                            );
                                        })}
                                    </ul>
                                </div>
                            )}

                            <div className="flex items-center justify-between pt-4 border-t border-slate-100 dark:border-slate-800">
                                <div className="flex items-center gap-2 text-slate-500 dark:text-slate-400">
                                    <DollarSign className="w-4 h-4" />
                                    <span className="font-medium">Total</span>
                                </div>
                                <span className="text-xl font-bold text-cyan-600 dark:text-cyan-400">
                                    {selectedOrder.totalPrice} {selectedOrder.currency || 'EGP'}
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
            )}
        </DashboardLayout>
    );
}
