import React, { useEffect, useState } from 'react';
import { motion } from 'framer-motion';
import { MapPin, Calendar, Package, Loader2 } from 'lucide-react';
import { DashboardLayout } from '../../layouts/DashboardLayout';
import { Button } from '../../components/ui/Button';
import { DriversService, type OrderDto, OrderStatus } from '../../lib/api';
import { usePolling } from '../../hooks/usePolling';

export const AvailableOrdersPage: React.FC = () => {
    const [availableJobs, setAvailableJobs] = useState<OrderDto[]>([]);
    const [isLoading, setIsLoading] = useState(true);

    const fetchAvailableJobs = async () => {
        try {
            const openOrders = await DriversService.getApiDriversAvailable();
            setAvailableJobs(openOrders || []);
        } catch (error) {
            console.error('Failed to fetch available jobs', error);
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        fetchAvailableJobs();
    }, []);

    usePolling(fetchAvailableJobs, 15000);

    const handleAcceptJob = async (orderId: string) => {
        console.log('Accepting job:', orderId);
        try {
            await DriversService.postApiDriversAccept({ orderId });
            // Refresh data
            fetchAvailableJobs();
            alert('Order accepted successfully!');
        } catch (error: any) {
            console.error('Failed to accept job', error);
            const errorMessage = error?.body?.message || error?.message || 'Unknown error';
            alert(`Failed to accept order: ${errorMessage}`);
        }
    };

    if (isLoading) {
        return (
            <DashboardLayout role="Driver">
                <div className="flex items-center justify-center h-96">
                    <Loader2 className="w-8 h-8 animate-spin text-cyan-500" />
                </div>
            </DashboardLayout>
        );
    }

    return (
        <DashboardLayout role="Driver">
            <div className="space-y-8">
                <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
                    <div>
                        <h1 className="text-2xl font-bold text-slate-900 dark:text-white">
                            Available Orders
                        </h1>
                        <p className="text-slate-500 dark:text-slate-400 mt-1">
                            Browse and accept jobs available in your area.
                        </p>
                    </div>
                    <Button onClick={() => fetchAvailableJobs()} variant="outline">
                        Refresh List
                    </Button>
                </div>

                <div className="bg-white dark:bg-slate-900 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-800">
                    <div className="divide-y divide-slate-100 dark:divide-slate-800">
                        {availableJobs.length === 0 ? (
                            <div className="p-12 text-center">
                                <div className="w-16 h-16 bg-slate-100 dark:bg-slate-800 rounded-full flex items-center justify-center mx-auto mb-4">
                                    <Package className="w-8 h-8 text-slate-400" />
                                </div>
                                <h3 className="text-lg font-medium text-slate-900 dark:text-white">No jobs available</h3>
                                <p className="text-slate-500 dark:text-slate-400 mt-1">
                                    There are no available jobs at the moment. Please check back later.
                                </p>
                            </div>
                        ) : (
                            availableJobs.map((job) => (
                                <motion.div
                                    key={job.id}
                                    initial={{ opacity: 0 }}
                                    animate={{ opacity: 1 }}
                                    className="p-6 flex flex-col sm:flex-row sm:items-center justify-between gap-4 hover:bg-slate-50 dark:hover:bg-slate-800/50 transition-colors"
                                >
                                    <div className="flex items-start gap-4">
                                        <div className={`p-3 rounded-xl bg-cyan-50 dark:bg-cyan-900/20 text-cyan-600 dark:text-cyan-400`}>
                                            <Package className="w-6 h-6" />
                                        </div>
                                        <div>
                                            <div className="flex items-center gap-2">
                                                <h3 className="font-semibold text-slate-900 dark:text-white">
                                                    Order #{job.id?.substring(0, 8)}
                                                </h3>
                                                <span className="text-xs font-medium px-2 py-0.5 rounded-full bg-slate-100 text-slate-600 dark:bg-slate-800 dark:text-slate-400">
                                                    {job.status === OrderStatus.Requested ? 'Pending' : 'Available'}
                                                </span>
                                            </div>
                                            <div className="flex items-center gap-4 mt-2 text-sm text-slate-500 dark:text-slate-400">
                                                <div className="flex items-center gap-1">
                                                    <MapPin className="w-4 h-4" />
                                                    Customer Location
                                                </div>
                                                <div className="flex items-center gap-1">
                                                    <Calendar className="w-4 h-4" />
                                                    {job.orderDate ? new Date(job.orderDate).toLocaleDateString() : 'Today'}
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div className="flex items-center gap-4">
                                        <div className="text-right hidden sm:block">
                                            <p className="text-lg font-bold text-slate-900 dark:text-white">
                                                {((job.totalPrice || 0) * 0.2).toFixed(2)} EGP
                                            </p>
                                            <p className="text-xs text-slate-500">Est. Earnings</p>
                                        </div>
                                        <Button onClick={() => job.id && handleAcceptJob(job.id)}>
                                            Accept Job
                                        </Button>
                                    </div>
                                </motion.div>
                            ))
                        )}
                    </div>
                </div>
            </div>
        </DashboardLayout>
    );
};
