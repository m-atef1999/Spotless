import { useState, useEffect } from 'react';
import { useToast } from '../../components/ui/Toast';

interface EarningsData {
    totalEarnings: number;
    pendingPayments: number;
    completedOrders: number;
    averageRating: number;
}

export function EarningsPage() {
    const [earnings, setEarnings] = useState<EarningsData>({
        totalEarnings: 0,
        pendingPayments: 0,
        completedOrders: 0,
        averageRating: 0
    });
    const [loading, setLoading] = useState(true);
    const { addToast } = useToast();

    useEffect(() => {
        fetchEarnings();
    }, []);

    const fetchEarnings = async () => {
        try {
            setLoading(true);
            // Placeholder data - replace with actual API call when backend is ready
            setEarnings({
                totalEarnings: 5420.50,
                pendingPayments: 320.00,
                completedOrders: 47,
                averageRating: 4.8
            });
        } catch (error) {
            addToast('Failed to load earnings data', 'error');
            console.error('Error fetching earnings:', error);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-h-screen bg-gray-50 p-6">
            <div className="max-w-7xl mx-auto">
                <div className="mb-8">
                    <h1 className="text-3xl font-bold text-gray-900">Earnings & Payments</h1>
                    <p className="text-gray-600 mt-2">Track your earnings and payment history</p>
                </div>

                {loading ? (
                    <div className="flex items-center justify-center h-64">
                        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
                    </div>
                ) : (
                    <>
                        {/* Stats Cards */}
                        <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
                            <div className="bg-white rounded-lg shadow-sm p-6">
                                <div className="text-sm text-gray-600 mb-2">Total Earnings</div>
                                <div className="text-3xl font-bold text-gray-900">{earnings.totalEarnings.toFixed(2)} EGP</div>
                            </div>
                            <div className="bg-white rounded-lg shadow-sm p-6">
                                <div className="text-sm text-gray-600 mb-2">Pending Payments</div>
                                <div className="text-3xl font-bold text-orange-600">{earnings.pendingPayments.toFixed(2)} EGP</div>
                            </div>
                            <div className="bg-white rounded-lg shadow-sm p-6">
                                <div className="text-sm text-gray-600 mb-2">Completed Orders</div>
                                <div className="text-3xl font-bold text-green-600">{earnings.completedOrders}</div>
                            </div>
                            <div className="bg-white rounded-lg shadow-sm p-6">
                                <div className="text-sm text-gray-600 mb-2">Average Rating</div>
                                <div className="text-3xl font-bold text-yellow-600">⭐ {earnings.averageRating}</div>
                            </div>
                        </div>

                        {/* Payment History */}
                        <div className="bg-white rounded-lg shadow-sm p-6">
                            <h2 className="text-xl font-bold text-gray-900 mb-4">Payment History</h2>
                            <div className="text-center py-12 text-gray-500">
                                Payment history will be displayed here
                            </div>
                        </div>
                    </>
                )}
            </div>
        </div>
    );
}


