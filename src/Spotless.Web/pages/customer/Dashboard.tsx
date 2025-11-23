import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { motion } from 'framer-motion';
import { Clock, CheckCircle, CreditCard, Loader2 } from 'lucide-react';
import { DashboardLayout } from '../../layouts/DashboardLayout';

import { CustomersService, OrdersService, type CustomerDashboardDto, type OrderDto, getPaymentMethodLabel, getPaymentStatusLabel } from '../../lib/api';

import { usePolling } from '../../hooks/usePolling';

export const CustomerDashboard: React.FC = () => {
    const [dashboardData, setDashboardData] = useState<CustomerDashboardDto | null>(null);
    const [recentOrders, setRecentOrders] = useState<OrderDto[]>([]);
    const [isLoading, setIsLoading] = useState(true);

    const fetchData = async () => {
        try {
            // Fetch dashboard stats and recent orders
            const [dashboard, orders] = await Promise.all([
                CustomersService.getApiCustomersDashboard({ pageNumber: 1, pageSize: 10 }),
                OrdersService.getApiOrdersCustomer({ pageNumber: 1, pageSize: 5 })
            ]);
            setDashboardData(dashboard);
            setRecentOrders(orders.data || []);
        } catch (error) {
            console.error('Failed to fetch dashboard data', error);
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        fetchData();
    }, []);

    usePolling(fetchData, 30000);

    if (isLoading) {
        return (
            <DashboardLayout role="Customer">
                <div className="flex items-center justify-center h-96">
                    <Loader2 className="w-8 h-8 animate-spin text-cyan-500" />
                </div>
            </DashboardLayout>
        );
    }

    const stats = [
        { label: 'Active Orders', value: dashboardData?.upcomingBookedServices || 0, icon: Clock, color: 'text-blue-500', bg: 'bg-blue-50 dark:bg-blue-900/20' },
        { label: 'Completed', value: dashboardData?.totalOrders || 0, icon: CheckCircle, color: 'text-teal-500', bg: 'bg-teal-50 dark:bg-teal-900/20' },
        { label: 'Wallet Balance', value: `${dashboardData?.walletBalance?.toFixed(2) || '0.00'} EGP`, icon: CreditCard, color: 'text-purple-500', bg: 'bg-purple-50 dark:bg-purple-900/20' },
    ];

    return (
        <DashboardLayout role="Customer">
            <div className="space-y-8">


                {/* Welcome Section */}
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

                {/* Upcoming Orders */}
                {dashboardData?.upcomingOrders && dashboardData.upcomingOrders.length > 0 && (
                    <div className="bg-white dark:bg-slate-900 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-800 overflow-hidden">
                        <div className="p-6 border-b border-slate-100 dark:border-slate-800">
                            <h2 className="text-lg font-bold text-slate-900 dark:text-white">
                                Upcoming Orders
                            </h2>
                        </div>
                        <div className="p-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
                            {dashboardData.upcomingOrders.map((order) => (
                                <div key={order.id} className="p-4 rounded-xl border border-slate-200 dark:border-slate-700 bg-slate-50 dark:bg-slate-800/50">
                                    <div className="flex justify-between items-start mb-2">
                                        <span className="text-sm font-mono text-slate-500">#{order.id?.substring(0, 8)}</span>
                                        <span className="px-2 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400">
                                            Upcoming
                                        </span>
                                    </div>
                                    <div className="mb-2">
                                        <p className="text-sm font-medium text-slate-900 dark:text-white">
                                            {order.scheduledDate ? new Date(order.scheduledDate).toLocaleDateString() : 'Date TBD'}
                                        </p>
                                        <p className="text-xs text-slate-500">
                                            {order.scheduledDate ? new Date(order.scheduledDate).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }) : ''}
                                        </p>
                                    </div>
                                    <div className="flex justify-between items-center mt-4">
                                        <span className="font-bold text-slate-900 dark:text-white">{order.totalPrice?.toFixed(2)} EGP</span>
                                        <Link to={`/customer/orders`} className="text-sm text-cyan-600 hover:text-cyan-700 font-medium">
                                            View Details
                                        </Link>
                                    </div>
                                </div>
                            ))}
                        </div>
                    </div>
                )}

                {/* Recent Orders */}
                <div className="bg-white dark:bg-slate-900 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-800 overflow-hidden">
                    <div className="p-6 border-b border-slate-100 dark:border-slate-800 flex items-center justify-between">
                        <h2 className="text-lg font-bold text-slate-900 dark:text-white">
                            Recent Orders
                        </h2>
                        <Link to="/customer/orders" className="text-sm font-medium text-cyan-600 hover:text-cyan-700 dark:text-cyan-400">
                            View All
                        </Link>
                    </div>
                    <div className="overflow-x-auto">
                        <table className="w-full">
                            <thead className="bg-slate-50 dark:bg-slate-950/50">
                                <tr>
                                    <th className="px-6 py-4 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Order ID</th>
                                    <th className="px-6 py-4 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Service</th>
                                    <th className="px-6 py-4 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Status</th>
                                    <th className="px-6 py-4 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Date</th>
                                    <th className="px-6 py-4 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Amount</th>
                                </tr>
                            </thead>
                            <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
                                {recentOrders.length === 0 ? (
                                    <tr>
                                        <td colSpan={5} className="px-6 py-8 text-center text-slate-500">
                                            No orders found. Start by creating a new order!
                                        </td>
                                    </tr>
                                ) : (
                                    recentOrders.map((order) => (
                                        <tr key={order.id} className="hover:bg-slate-50 dark:hover:bg-slate-800/50 transition-colors">
                                            <td className="px-6 py-4 text-sm font-medium text-slate-900 dark:text-white">
                                                #{order.id?.substring(0, 8)}
                                            </td>
                                            <td className="px-6 py-4 text-sm text-slate-600 dark:text-slate-300">
                                                Cleaning Service
                                                {order.items && order.items.length > 1 && ` +${order.items.length - 1} more`}
                                            </td>
                                            <td className="px-6 py-4">
                                                <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium
                                                    ${order.status === 2 ? 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400' :
                                                        order.status === 0 ? 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400' :
                                                            'bg-amber-100 text-amber-800 dark:bg-amber-900/30 dark:text-amber-400'}`}>
                                                    {order.status === 0 ? 'Pending' : order.status === 1 ? 'Confirmed' : order.status === 2 ? 'Completed' : 'Cancelled'}
                                                </span>
                                            </td>
                                            <td className="px-6 py-4 text-sm text-slate-500 dark:text-slate-400">
                                                {order.orderDate ? new Date(order.orderDate).toLocaleDateString() : '-'}
                                            </td>
                                            <td className="px-6 py-4 text-sm font-medium text-slate-900 dark:text-white">
                                                {order.totalPrice?.toFixed(2)} EGP
                                            </td>
                                        </tr>
                                    ))
                                )}
                            </tbody>
                        </table>
                    </div>
                </div>

                {/* Recent Transactions */}
                {dashboardData?.recentTransactions && dashboardData.recentTransactions.length > 0 && (
                    <div className="bg-white dark:bg-slate-900 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-800 overflow-hidden">
                        <div className="p-6 border-b border-slate-100 dark:border-slate-800">
                            <h2 className="text-lg font-bold text-slate-900 dark:text-white">
                                Recent Transactions
                            </h2>
                        </div>
                        <div className="overflow-x-auto">
                            <table className="w-full">
                                <thead className="bg-slate-50 dark:bg-slate-950/50">
                                    <tr>
                                        <th className="px-6 py-4 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Date</th>
                                        <th className="px-6 py-4 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Description</th>
                                        <th className="px-6 py-4 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Method</th>
                                        <th className="px-6 py-4 text-left text-xs font-semibold text-slate-500 uppercase tracking-wider">Status</th>
                                        <th className="px-6 py-4 text-right text-xs font-semibold text-slate-500 uppercase tracking-wider">Amount</th>
                                    </tr>
                                </thead>
                                <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
                                    {dashboardData.recentTransactions.map((tx) => (
                                        <tr key={tx.id} className="hover:bg-slate-50 dark:hover:bg-slate-800/50 transition-colors">
                                            <td className="px-6 py-4 text-sm text-slate-500 dark:text-slate-400">
                                                {tx.paymentDate ? new Date(tx.paymentDate).toLocaleDateString() : '-'}
                                            </td>
                                            <td className="px-6 py-4 text-sm text-slate-900 dark:text-white">
                                                {tx.orderId ? `Order #${tx.orderId.substring(0, 8)}` : 'Wallet Top-up'}
                                            </td>
                                            <td className="px-6 py-4 text-sm text-slate-600 dark:text-slate-300">
                                                {getPaymentMethodLabel(tx.method || 0)}
                                            </td>
                                            <td className="px-6 py-4">
                                                <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium
                                                    ${tx.status === 1 ? 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400' :
                                                        tx.status === 0 ? 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400' :
                                                            'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400'}`}>
                                                    {getPaymentStatusLabel(tx.status || 0)}
                                                </span>
                                            </td>
                                            <td className="px-6 py-4 text-sm font-medium text-right text-slate-900 dark:text-white">
                                                ${tx.amount?.toFixed(2)}
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
};
