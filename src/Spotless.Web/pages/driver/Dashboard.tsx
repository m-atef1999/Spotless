import React, { useEffect, useState } from 'react';
import { motion } from 'framer-motion';
import { MapPin, Calendar, DollarSign, Package, Loader2, CheckCircle } from 'lucide-react';
import { DashboardLayout } from '../../layouts/DashboardLayout';
import { Button } from '../../components/ui/Button';
import { DriversService, type OrderDto, OrderStatus } from '../../lib/api';
import { useAuthStore } from '../../store/authStore';
import { usePolling } from '../../hooks/usePolling';

export const DriverDashboard: React.FC = () => {
    const { user } = useAuthStore();
    const [activeJobs, setActiveJobs] = useState<OrderDto[]>([]);
    const [availableJobs, setAvailableJobs] = useState<OrderDto[]>([]);
    const [isLoading, setIsLoading] = useState(true);

    const fetchData = async () => {
        try {
            const [myOrders, openOrders] = await Promise.all([
                DriversService.getApiDriversOrders(), // Jobs assigned to this driver
                DriversService.getApiDriversAvailable() // Jobs available for pickup
            ]);
            setActiveJobs(myOrders || []);
            setAvailableJobs(openOrders || []);
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

    const handleAcceptJob = async (orderId: string) => {
        try {
            await DriversService.postApiDriversApply({ orderId });
            // Refresh data
            fetchData();
        } catch (error) {
            console.error('Failed to accept job', error);
        }

    };

    const handleCompleteJob = async (orderId: string) => {
        try {
            // Optimistic update
            setActiveJobs(prev => prev.map(job =>
                job.id === orderId ? { ...job, status: OrderStatus.Delivered } : job
            ));
            console.log(`Completing job ${orderId}`);
            // TODO: Call API to complete job when endpoint is available
        } catch (error) {
            console.error('Failed to complete job', error);
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
    const completedJobsCount = activeJobs.filter(j => j.status === OrderStatus.Delivered).length; // 7 = Delivered
    const activeJobsCount = activeJobs.filter(j => j.status === OrderStatus.InCleaning || j.status === OrderStatus.PickedUp || j.status === OrderStatus.OutForDelivery).length;
    // Mock earnings for now
    const earnings = activeJobs.reduce((acc, job) => acc + (job.totalPrice || 0) * 0.2, 0); // Assuming 20% commission

    const stats = [
        { label: 'Active Jobs', value: activeJobsCount.toString(), icon: Package, color: 'text-blue-500', bg: 'bg-blue-50 dark:bg-blue-900/20' },
        { label: 'Est. Earnings', value: `$${earnings.toFixed(2)}`, icon: DollarSign, color: 'text-green-500', bg: 'bg-green-50 dark:bg-green-900/20' },
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
                            Welcome, {user?.name || 'Driver'}. You are currently <strong>Online</strong>.
                        </p>
                    </div>
                    <div className="flex gap-3">
                        <Button variant="outline">Go Offline</Button>
                        <Button onClick={() => window.location.reload()}>Refresh</Button>
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
                <div className="bg-white dark:bg-slate-900 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-800 mb-8">
                    <div className="p-6 border-b border-slate-100 dark:border-slate-800">
                        <h2 className="text-lg font-bold text-slate-900 dark:text-white">
                            Active Jobs
                        </h2>
                    </div>
                    <div className="divide-y divide-slate-100 dark:divide-slate-800">
                        {activeJobs.filter(j => j.status !== OrderStatus.Delivered && j.status !== OrderStatus.Cancelled).length === 0 ? (
                            <div className="p-6 text-center text-slate-500">
                                You have no active jobs. Accept a job below to get started!
                            </div>
                        ) : (
                            activeJobs.filter(j => j.status !== OrderStatus.Delivered && j.status !== OrderStatus.Cancelled).map((job) => (
                                <div key={job.id} className="p-6 flex flex-col sm:flex-row sm:items-center justify-between gap-4 hover:bg-slate-50 dark:hover:bg-slate-800/50 transition-colors">
                                    <div className="flex items-start gap-4">
                                        <div className={`p-3 rounded-xl bg-cyan-50 text-cyan-600`}>
                                            <Package className="w-6 h-6" />
                                        </div>
                                        <div>
                                            <div className="flex items-center gap-2">
                                                <h3 className="font-semibold text-slate-900 dark:text-white">
                                                    Order #{job.id?.substring(0, 8)}
                                                </h3>
                                                <span className="text-xs font-medium px-2 py-0.5 rounded-full bg-cyan-100 text-cyan-700 dark:bg-cyan-900/30 dark:text-cyan-400">
                                                    In Progress
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
                                        <Button size="sm" onClick={() => job.id && handleCompleteJob(job.id)}>
                                            Mark Completed
                                        </Button>
                                    </div>
                                </div>
                            ))
                        )}
                    </div>
                </div>

                {/* Available Jobs */}
                <div className="bg-white dark:bg-slate-900 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-800">
                    <div className="p-6 border-b border-slate-100 dark:border-slate-800">
                        <h2 className="text-lg font-bold text-slate-900 dark:text-white">
                            Available Jobs Nearby
                        </h2>
                    </div>
                    <div className="divide-y divide-slate-100 dark:divide-slate-800">
                        {availableJobs.length === 0 ? (
                            <div className="p-6 text-center text-slate-500">
                                No jobs available at the moment. Check back later!
                            </div>
                        ) : (
                            availableJobs.map((job) => (
                                <div key={job.id} className="p-6 flex flex-col sm:flex-row sm:items-center justify-between gap-4 hover:bg-slate-50 dark:hover:bg-slate-800/50 transition-colors">
                                    <div className="flex items-start gap-4">
                                        <div className={`p-3 rounded-xl bg-blue-50 text-blue-600`}>
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
                                                    {/* Address would be here if in DTO, using placeholder for now */}
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
                                                ${((job.totalPrice || 0) * 0.2).toFixed(2)}
                                            </p>
                                            <p className="text-xs text-slate-500">Est. Earnings</p>
                                        </div>
                                        <Button size="sm" onClick={() => job.id && handleAcceptJob(job.id)}>
                                            Accept Job
                                        </Button>
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
