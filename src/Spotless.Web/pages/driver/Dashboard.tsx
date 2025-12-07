import React, { useEffect, useState } from 'react';
import { motion } from 'framer-motion';
import { MapPin, Calendar, DollarSign, Package, Loader2, CheckCircle, Power } from 'lucide-react';
import { DashboardLayout } from '../../layouts/DashboardLayout';
import { Button } from '../../components/ui/Button';
import { DriversService } from '../../lib/api';
import type { Spotless_Application_Dtos_Order_OrderDto as OrderDto } from '../../lib/models/Spotless_Application_Dtos_Order_OrderDto';
import { OrderStatus } from '../../lib/constants';
import { useAuthStore } from '../../store/authStore';
import { usePolling } from '../../hooks/usePolling';

export const DriverDashboard: React.FC = () => {
    const { user } = useAuthStore();
    const [activeJobs, setActiveJobs] = useState<OrderDto[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [isStatusLoading, setIsStatusLoading] = useState(false);
    const [driverStatus, setDriverStatus] = useState<string>('Offline');

    const fetchData = async () => {
        try {
            const [myOrders, profile] = await Promise.all([
                DriversService.getApiDriversOrders(),
                DriversService.getApiDriversProfile()
            ]);
            setActiveJobs(myOrders || []);
            setDriverStatus(profile.status || 'Offline');
        } catch (error) {
            console.error('Failed to fetch driver data', error);
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        fetchData();
    }, []);

    usePolling(fetchData, 30000);

    // Real-time status updates
    useEffect(() => {
        const apiUrl = import.meta.env.VITE_API_URL || 'https://spotless.runasp.net';
        const baseUrl = apiUrl.endsWith('/') ? apiUrl.slice(0, -1) : apiUrl;

        import('@microsoft/signalr').then(signalR => {
            const connection = new signalR.HubConnectionBuilder()
                .withUrl(`${baseUrl}/driverHub`, {
                    accessTokenFactory: () => localStorage.getItem('token') || ''
                })
                .withAutomaticReconnect()
                .build();

            connection.start()
                .then(() => {
                    console.log('DriverHub Connected (Dashboard)');
                    connection.on('DriverAvailabilityUpdated', (_userId: string, newStatus: string) => {
                        setDriverStatus(newStatus);
                    });
                })
                .catch(err => console.error('DriverHub Connection Error: ', err));

            return () => {
                connection.stop();
            };
        });
    }, []);

    const handleToggleStatus = async () => {
        const newStatus = driverStatus === 'Available' ? 'Offline' : 'Available';
        setIsStatusLoading(true);
        try {
            await DriversService.putApiDriversStatus({
                requestBody: { status: newStatus }
            });
            // Optimistic update
            setDriverStatus(newStatus);
        } catch (error) {
            console.error('Failed to update status', error);
        } finally {
            setIsStatusLoading(false);
        }
    };

    const handleSetStatus = async (newStatus: string) => {
        if (newStatus === driverStatus) return;
        setIsStatusLoading(true);
        try {
            await DriversService.putApiDriversStatus({
                requestBody: { status: newStatus }
            });
            setDriverStatus(newStatus);
        } catch (error) {
            console.error('Failed to update status', error);
        } finally {
            setIsStatusLoading(false);
        }
    };

    const availableStatuses = ['Available', 'OnRoute', 'Busy', 'Offline'];

    const handleUpdateStatus = async (orderId: string, newStatus: number) => {
        try {
            // Optimistic update
            setActiveJobs(prev => prev.map(job =>
                job.id === orderId ? { ...job, status: newStatus as any } : job
            ));

            // Call API to update status (use DriversService for driver role)
            await DriversService.putApiDriversOrdersStatus({
                orderId: orderId,
                requestBody: newStatus
            });
            fetchData();
        } catch (error) {
            console.error('Failed to update job status', error);
            fetchData(); // Revert on error
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

    // Calculate stats from active jobs
    // Fix: Cast to any to allow string comparisons (backend returns strings)
    const completedJobsCount = activeJobs.filter(j => (j.status as any) === OrderStatus.Delivered || (j.status as any) === 'Delivered' || (j.status as any) === 7).length;
    const currentActiveJobs = activeJobs.filter(j =>
        (j.status as any) === OrderStatus.InCleaning || (j.status as any) === 'InCleaning' || (j.status as any) === 5 ||
        (j.status as any) === OrderStatus.PickedUp || (j.status as any) === 'PickedUp' || (j.status as any) === 4 ||
        (j.status as any) === OrderStatus.OutForDelivery || (j.status as any) === 'OutForDelivery' || (j.status as any) === 6 ||
        (j.status as any) === OrderStatus.DriverAssigned || (j.status as any) === 'DriverAssigned' || (j.status as any) === 3
    );

    // Mock earnings for now
    const earnings = activeJobs.reduce((acc, job) => acc + (job.totalPrice || 0) * 0.2, 0); // Assuming 20% commission

    const stats = [
        { label: 'Active Jobs', value: currentActiveJobs.length.toString(), icon: Package, color: 'text-blue-500', bg: 'bg-blue-50 dark:bg-blue-900/20' },
        { label: 'Est. Earnings', value: `${earnings.toFixed(2)} EGP`, icon: DollarSign, color: 'text-green-500', bg: 'bg-green-50 dark:bg-green-900/20' },
        { label: 'Completed Jobs', value: completedJobsCount.toString(), icon: CheckCircle, color: 'text-purple-500', bg: 'bg-purple-50 dark:bg-purple-900/20' },
    ];

    return (
        <DashboardLayout role="Driver">
            <div className="space-y-8">
                {/* Header */}
                <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
                    <div>
                        <h1 className="text-2xl font-bold text-slate-900 dark:text-white">
                            Driver Dashboard
                        </h1>
                        <p className="text-slate-500 dark:text-slate-400 mt-1">
                            Welcome, {user?.name || 'Driver'}.
                        </p>
                    </div>
                    <div className="flex gap-3 items-center">
                        {/* Status Dropdown */}
                        <div className="relative">
                            <select
                                value={driverStatus}
                                onChange={(e) => handleSetStatus(e.target.value)}
                                disabled={isStatusLoading}
                                className={`appearance-none pl-4 pr-10 py-2 rounded-lg border text-sm font-medium cursor-pointer transition-colors ${driverStatus === 'Available' ? 'bg-green-50 border-green-300 text-green-700 dark:bg-green-900/30 dark:border-green-700 dark:text-green-400' :
                                        driverStatus === 'OnRoute' ? 'bg-blue-50 border-blue-300 text-blue-700 dark:bg-blue-900/30 dark:border-blue-700 dark:text-blue-400' :
                                            driverStatus === 'Busy' ? 'bg-orange-50 border-orange-300 text-orange-700 dark:bg-orange-900/30 dark:border-orange-700 dark:text-orange-400' :
                                                'bg-slate-100 border-slate-300 text-slate-700 dark:bg-slate-800 dark:border-slate-600 dark:text-slate-400'
                                    }`}
                            >
                                {availableStatuses.map((status) => (
                                    <option key={status} value={status}>{status}</option>
                                ))}
                            </select>
                            <div className="absolute right-3 top-1/2 -translate-y-1/2 pointer-events-none">
                                <svg className="w-4 h-4 text-current opacity-60" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                                </svg>
                            </div>
                        </div>
                        {/* Quick Toggle */}
                        {driverStatus === 'Available' ? (
                            <Button
                                variant="outline"
                                onClick={handleToggleStatus}
                                isLoading={isStatusLoading}
                                className="text-red-600 hover:text-red-700 hover:bg-red-50 border-red-200"
                            >
                                <Power className="w-4 h-4 mr-2" />
                                Go Offline
                            </Button>
                        ) : driverStatus === 'Offline' && (
                            <Button
                                onClick={handleToggleStatus}
                                isLoading={isStatusLoading}
                                className="bg-green-600 hover:bg-green-700 text-white"
                            >
                                <Power className="w-4 h-4 mr-2" />
                                Go Online
                            </Button>
                        )}
                        <Button onClick={() => fetchData()} variant="outline">Refresh</Button>
                    </div>
                </div>

                {/* Stats */}
                <div className="grid grid-cols-1 sm:grid-cols-3 gap-6">
                    {stats.map((stat, index) => (
                        <motion.div
                            key={index}
                            initial={{ opacity: 0, y: 20 }}
                            animate={{ opacity: 1, y: 0 }}
                            transition={{ delay: index * 0.1 }}
                            className="bg-white dark:bg-slate-900 p-6 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-800"
                        >
                            <div className="flex items-center justify-between">
                                <div>
                                    <p className="text-sm font-medium text-slate-500 dark:text-slate-400">
                                        {stat.label}
                                    </p>
                                    <p className="text-2xl font-bold text-slate-900 dark:text-white mt-1">
                                        {stat.value}
                                    </p>
                                </div>
                                <div className={`p-3 rounded-xl ${stat.bg}`}>
                                    <stat.icon className={`w-6 h-6 ${stat.color}`} />
                                </div>
                            </div>
                        </motion.div>
                    ))}
                </div>

                {/* Active Jobs Section */}
                <div className="bg-white dark:bg-slate-900 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-800">
                    <div className="p-6 border-b border-slate-100 dark:border-slate-800 flex justify-between items-center">
                        <h2 className="text-lg font-bold text-slate-900 dark:text-white">
                            Your Active Jobs
                        </h2>
                        <Button variant="ghost" size="sm" onClick={() => window.location.href = '/driver/available-orders'}>
                            Find More Jobs
                        </Button>
                    </div>
                    <div className="divide-y divide-slate-100 dark:divide-slate-800">
                        {currentActiveJobs.length === 0 ? (
                            <div className="p-12 text-center">
                                <div className="w-16 h-16 bg-slate-100 dark:bg-slate-800 rounded-full flex items-center justify-center mx-auto mb-4">
                                    <Package className="w-8 h-8 text-slate-400" />
                                </div>
                                <h3 className="text-lg font-medium text-slate-900 dark:text-white">No active jobs</h3>
                                <p className="text-slate-500 dark:text-slate-400 mt-1 mb-4">
                                    You don't have any active jobs right now.
                                </p>
                                <Button onClick={() => window.location.href = '/driver/available-orders'}>
                                    Browse Available Orders
                                </Button>
                            </div>
                        ) : (
                            currentActiveJobs.map((job) => (
                                <div key={job.id} className="p-6 flex flex-col sm:flex-row sm:items-center justify-between gap-4 hover:bg-slate-50 dark:hover:bg-slate-800/50 transition-colors">
                                    <div className="flex items-start gap-4">
                                        <div className={`p-3 rounded-xl bg-cyan-50 dark:bg-cyan-900/20 text-cyan-600 dark:text-cyan-400`}>
                                            <Package className="w-6 h-6" />
                                        </div>
                                        <div>
                                            <div className="flex items-center gap-2">
                                                <h3 className="font-semibold text-slate-900 dark:text-white">
                                                    Order #{job.id?.substring(0, 8)}
                                                </h3>
                                                <span className="text-xs font-medium px-2 py-0.5 rounded-full bg-cyan-100 text-cyan-700 dark:bg-cyan-900/30 dark:text-cyan-400">
                                                    {job.status}
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
                                        {((job.status as any) === 'DriverAssigned' || (job.status as any) === 3) && (
                                            <Button size="sm" onClick={() => job.id && handleUpdateStatus(job.id, OrderStatus.PickedUp)}>
                                                Picked Up
                                            </Button>
                                        )}
                                        {((job.status as any) === 'PickedUp' || (job.status as any) === 4) && (
                                            <Button size="sm" onClick={() => job.id && handleUpdateStatus(job.id, OrderStatus.InCleaning)}>
                                                Delivered to Laundry
                                            </Button>
                                        )}
                                        {((job.status as any) === 'InCleaning' || (job.status as any) === 5) && (
                                            <span className="text-sm text-slate-500 italic">Cleaning in progress...</span>
                                        )}
                                        {((job.status as any) === 'OutForDelivery' || (job.status as any) === 6) && (
                                            <Button size="sm" onClick={() => job.id && handleUpdateStatus(job.id, OrderStatus.Delivered)}>
                                                Delivered to Customer
                                            </Button>
                                        )}
                                    </div>
                                </div>
                            ))
                        )}
                    </div>
                </div>
            </div>
        </DashboardLayout>
    );
};
