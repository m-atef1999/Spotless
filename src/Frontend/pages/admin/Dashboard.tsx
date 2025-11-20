import React, { useEffect, useState } from 'react';
import { motion } from 'framer-motion';
import { Users, ShoppingBag, TrendingUp, Activity, Loader2 } from 'lucide-react';
import { AreaChart, Area, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, PieChart, Pie, Cell, Legend } from 'recharts';
import { DashboardLayout } from '../../layouts/DashboardLayout';
import { apiClient } from '../../lib/api';
import { AdminDashboardDto } from '../../lib/apiClient';
import { usePolling } from '../../hooks/usePolling';

// Mock Data for Charts (since API doesn't provide historical data yet)
const REVENUE_DATA = [
    { name: 'Mon', revenue: 1200 },
    { name: 'Tue', revenue: 1900 },
    { name: 'Wed', revenue: 1500 },
    { name: 'Thu', revenue: 2100 },
    { name: 'Fri', revenue: 2800 },
    { name: 'Sat', revenue: 3200 },
    { name: 'Sun', revenue: 2500 },
];

const SERVICE_DATA = [
    { name: 'Standard Cleaning', value: 45 },
    { name: 'Deep Cleaning', value: 25 },
    { name: 'Move-in/out', value: 20 },
    { name: 'Carpet Cleaning', value: 10 },
];

const COLORS = ['#06b6d4', '#3b82f6', '#8b5cf6', '#ec4899'];

export const AdminDashboard: React.FC = () => {
    const [stats, setStats] = useState<AdminDashboardDto | null>(null);
    const [isLoading, setIsLoading] = useState(true);

    const fetchData = async () => {
        try {
            const data = await apiClient.dashboard(1, 10);
            setStats(data);
        } catch (error) {
            console.error('Failed to fetch admin dashboard', error);
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
            <DashboardLayout role="Admin">
                <div className="flex items-center justify-center h-96">
                    <Loader2 className="w-8 h-8 animate-spin text-cyan-500" />
                </div>
            </DashboardLayout>
        );
    }

    const statCards = [
        { label: 'New Users Today', value: stats?.newRegistrationsToday || 0, change: '+0%', icon: Users, color: 'text-blue-500', bg: 'bg-blue-50 dark:bg-blue-900/20' },
        { label: 'Orders Today', value: stats?.totalOrdersToday || 0, change: '+0%', icon: ShoppingBag, color: 'text-purple-500', bg: 'bg-purple-50 dark:bg-purple-900/20' },
        { label: 'Revenue Today', value: `$${stats?.revenueToday?.toFixed(2) || '0.00'}`, change: '+0%', icon: TrendingUp, color: 'text-green-500', bg: 'bg-green-50 dark:bg-green-900/20' },
        { label: 'Active Drivers', value: stats?.numberOfActiveCleaners || 0, change: '0%', icon: Activity, color: 'text-orange-500', bg: 'bg-orange-50 dark:bg-orange-900/20' },
    ];

    return (
        <DashboardLayout role="Admin">
            <div className="space-y-8">
                {/* Header */}
                <div>
                    <h1 className="text-2xl font-bold text-slate-900 dark:text-white">
                        Admin Overview
                    </h1>
                    <p className="text-slate-500 dark:text-slate-400 mt-1">
                        System performance and key metrics.
                    </p>
                </div>

                {/* Stats Grid */}
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
                    {statCards.map((stat, index) => (
                        <motion.div
                            key={index}
                            initial={{ opacity: 0, y: 20 }}
                            animate={{ opacity: 1, y: 0 }}
                            transition={{ delay: index * 0.1 }}
                            className="bg-white dark:bg-slate-900 p-6 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-800"
                        >
                            <div className="flex items-center justify-between mb-4">
                                <div className={`p-3 rounded-xl ${stat.bg}`}>
                                    <stat.icon className={`w-6 h-6 ${stat.color}`} />
                                </div>
                                {/* Change indicator - placeholder for now as API doesn't provide % change */}
                                <span className={`text-sm font-medium px-2 py-1 rounded-full bg-slate-100 text-slate-600 dark:bg-slate-800 dark:text-slate-400`}>
                                    {stat.change}
                                </span>
                            </div>
                            <div>
                                <p className="text-sm font-medium text-slate-500 dark:text-slate-400">
                                    {stat.label}
                                </p>
                                <p className="text-2xl font-bold text-slate-900 dark:text-white mt-1">
                                    {stat.value}
                                </p>
                            </div>
                        </motion.div>
                    ))}
                </div>

                {/* Recent Activity / Charts */}
                <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
                    {/* Revenue Trend */}
                    <motion.div
                        initial={{ opacity: 0, x: -20 }}
                        animate={{ opacity: 1, x: 0 }}
                        transition={{ delay: 0.4 }}
                        className="bg-white dark:bg-slate-900 p-6 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-800"
                    >
                        <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-6">Revenue Trends</h3>
                        <div className="h-80">
                            <ResponsiveContainer width="100%" height="100%">
                                <AreaChart data={REVENUE_DATA}>
                                    <defs>
                                        <linearGradient id="colorRevenue" x1="0" y1="0" x2="0" y2="1">
                                            <stop offset="5%" stopColor="#06b6d4" stopOpacity={0.1} />
                                            <stop offset="95%" stopColor="#06b6d4" stopOpacity={0} />
                                        </linearGradient>
                                    </defs>
                                    <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#e2e8f0" />
                                    <XAxis dataKey="name" axisLine={false} tickLine={false} tick={{ fill: '#64748b' }} />
                                    <YAxis axisLine={false} tickLine={false} tick={{ fill: '#64748b' }} />
                                    <Tooltip
                                        contentStyle={{ backgroundColor: '#fff', borderRadius: '8px', border: 'none', boxShadow: '0 4px 6px -1px rgb(0 0 0 / 0.1)' }}
                                        itemStyle={{ color: '#06b6d4' }}
                                    />
                                    <Area type="monotone" dataKey="revenue" stroke="#06b6d4" strokeWidth={3} fillOpacity={1} fill="url(#colorRevenue)" />
                                </AreaChart>
                            </ResponsiveContainer>
                        </div>
                    </motion.div>

                    {/* Service Popularity */}
                    <motion.div
                        initial={{ opacity: 0, x: 20 }}
                        animate={{ opacity: 1, x: 0 }}
                        transition={{ delay: 0.5 }}
                        className="bg-white dark:bg-slate-900 p-6 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-800"
                    >
                        <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-6">Service Popularity</h3>
                        <div className="h-80">
                            <ResponsiveContainer width="100%" height="100%">
                                <PieChart>
                                    <Pie
                                        data={SERVICE_DATA}
                                        cx="50%"
                                        cy="50%"
                                        innerRadius={80}
                                        outerRadius={110}
                                        paddingAngle={5}
                                        dataKey="value"
                                    >
                                        {SERVICE_DATA.map((_, index) => (
                                            <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                                        ))}
                                    </Pie>
                                    <Tooltip
                                        contentStyle={{ backgroundColor: '#fff', borderRadius: '8px', border: 'none', boxShadow: '0 4px 6px -1px rgb(0 0 0 / 0.1)' }}
                                    />
                                    <Legend verticalAlign="bottom" height={36} iconType="circle" />
                                </PieChart>
                            </ResponsiveContainer>
                        </div>
                    </motion.div>
                </div>
            </div>
        </DashboardLayout>
    );
};
