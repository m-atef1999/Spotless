import { useState, useEffect } from 'react';
import { DriversService, type OrderDto } from '../../lib/api';
import { useToast } from '../../components/ui/Toast';

export function AvailableOrdersPage() {
    const [orders, setOrders] = useState<OrderDto[]>([]);
    const [loading, setLoading] = useState(true);
    const { addToast } = useToast();

    useEffect(() => {
        fetchAvailableOrders();
    }, []);

    const fetchAvailableOrders = async () => {
        try {
            setLoading(true);
            const data = await DriversService.getApiDriversAvailable();
            setOrders(data || []);
        } catch (error) {
            addToast('Failed to load available orders', 'error');
            console.error('Error fetching available orders:', error);
        } finally {
            setLoading(false);
        }
    };

    const handleAcceptOrder = async (orderId: string) => {
        try {
            await DriversService.postApiDriversApply({ orderId });
            addToast('Order accepted successfully!', 'success');
            fetchAvailableOrders();
        } catch (error) {
            console.error('Error accepting order:', error);
            addToast('Failed to accept order', 'error');
        }
    };

    return (
        <div className="min-h-screen bg-gray-50 p-6">
            <div className="max-w-7xl mx-auto">
                <div className="mb-8">
                    <h1 className="text-3xl font-bold text-gray-900">Available Orders</h1>
                    <p className="text-gray-600 mt-2">Browse and accept new delivery orders</p>
                </div>

                {loading ? (
                    <div className="flex items-center justify-center h-64">
                        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
                    </div>
                ) : orders.length === 0 ? (
                    <div className="bg-white rounded-lg shadow-sm p-12 text-center">
                        <p className="text-gray-500 text-lg">No available orders at the moment</p>
                        <p className="text-gray-400 mt-2">Check back later for new delivery opportunities</p>
                    </div>
                ) : (
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                        {orders.map((order) => (
                            <div key={order.id} className="bg-white rounded-lg shadow-sm p-6">
                                <div className="mb-4">
                                    <h3 className="text-lg font-semibold text-gray-900">Order #{order.id}</h3>
                                    <p className="text-gray-600 text-sm mt-1">{order.pickupAddress}</p>
                                </div>
                                <div className="space-y-2 mb-4">
                                    <div className="flex justify-between text-sm">
                                        <span className="text-gray-600">Distance:</span>
                                        <span className="font-medium">5.2 km</span>
                                    </div>
                                    <div className="flex justify-between text-sm">
                                        <span className="text-gray-600">Estimated Time:</span>
                                        <span className="font-medium">15 min</span>
                                    </div>
                                    <div className="flex justify-between text-sm">
                                        <span className="text-gray-600">Payment:</span>
                                        <span className="font-medium text-green-600">{order.totalPrice} EGP</span>
                                    </div>
                                </div>
                                <button
                                    onClick={() => handleAcceptOrder(order.id!)}
                                    className="w-full bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors"
                                >
                                    Accept Order
                                </button>
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </div>
    );
}
