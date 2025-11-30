import { useState, useEffect } from 'react';
import { useToast } from '../../components/ui/Toast';
import { DriversService } from '../../lib/api';
import { DashboardLayout } from '../../layouts/DashboardLayout';

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
            const data = await DriversService.getApiDriversEarnings();
            setEarnings({
                totalEarnings: data.totalEarnings || 0,
                pendingPayments: data.pendingPayments || 0,
                completedOrders: data.completedOrders || 0,
                averageRating: data.averageRating || 0
            });
        } catch (error) {
            console.warn('Failed to load earnings data, using fallback:', error);
            // Fallback for when backend is not yet deployed with new endpoint
            setEarnings({
                totalEarnings: 0,
                pendingPayments: 0,
                completedOrders: 0,
                averageRating: 5.0
            });
            // Only show toast if it's not a 404 (which is expected if backend is old)
            // @ts-ignore
            if (error?.status !== 404) {
                addToast('Using offline earnings data', 'info');
            }
        } finally {
            setLoading(false);
        }
    };

    return (
        <DashboardLayout role="Driver">
            <div className="space-y-8">
                <div className="mb-8">
                    <h1 className="text-3xl font-bold text-gray-900 dark:text-white">Earnings & Payments</h1>
                    <p className="text-gray-600 dark:text-gray-400 mt-2">Track your earnings and payment history</p>
                </div>

                {loading ? (
                    <div className="flex items-center justify-center h-64">
                        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
                    </div>
                ) : (
                    <>
                        {/* Stats Cards */}
                        <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
                            <div className="bg-white dark:bg-slate-800 rounded-lg shadow-sm p-6 border border-gray-100 dark:border-slate-700">
                                <div className="text-sm text-gray-600 dark:text-gray-400 mb-2">Total Earnings</div>
                                <div className="text-3xl font-bold text-gray-900 dark:text-white">{earnings.totalEarnings.toFixed(2)} EGP</div>
                            </div>
                            <div className="bg-white dark:bg-slate-800 rounded-lg shadow-sm p-6 border border-gray-100 dark:border-slate-700">
                                <div className="text-sm text-gray-600 dark:text-gray-400 mb-2">Pending Payments</div>
                                <div className="text-3xl font-bold text-orange-600">{earnings.pendingPayments.toFixed(2)} EGP</div>
                            </div>
                            <div className="bg-white dark:bg-slate-800 rounded-lg shadow-sm p-6 border border-gray-100 dark:border-slate-700">
                                <div className="text-sm text-gray-600 dark:text-gray-400 mb-2">Completed Orders</div>
                                <div className="text-3xl font-bold text-green-600">{earnings.completedOrders}</div>
                            </div>
                            <div className="bg-white dark:bg-slate-800 rounded-lg shadow-sm p-6 border border-gray-100 dark:border-slate-700">
                                <div className="text-sm text-gray-600 dark:text-gray-400 mb-2">Average Rating</div>
                                <div className="text-3xl font-bold text-yellow-600">⭐ {earnings.averageRating}</div>
                            </div>
                        </div>

                        {/* Payment History */}
                        <div className="bg-white dark:bg-slate-800 rounded-lg shadow-sm p-6 border border-gray-100 dark:border-slate-700">
                            <h2 className="text-xl font-bold text-gray-900 dark:text-white mb-4">Payment History</h2>
                            <div className="text-center py-12 text-gray-500 dark:text-gray-400">
                                Payment history will be displayed here
                            </div>
                        </div>
                    </>
                )}
            </div>
        </DashboardLayout>
    );
}


