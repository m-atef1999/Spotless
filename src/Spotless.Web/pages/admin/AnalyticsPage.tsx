import React, { useState, useEffect } from 'react';
import { BarChart, Bar, PieChart, Pie, Cell, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';
import { AnalyticsService, type AnalyticsDashboardDto } from '../../lib/api';

interface AnalyticsData {
    revenue: {
        total: number;
        thisMonth: number;
        lastMonth: number;
        growth: number;
    };
    orders: {
        total: number;
        completed: number;
        pending: number;
        cancelled: number;
    };
    users: {
        totalCustomers: number;
        totalDrivers: number;
        activeCustomers: number;
        activeDrivers: number;
    };
    monthlyRevenue: Array<{ month: string; revenue: number }>;
    ordersByStatus: Array<{ name: string; value: number }>;
    topServices: Array<{ name: string; orders: number; revenue: number }>;
}

const AnalyticsPage: React.FC = () => {
    const [analytics, setAnalytics] = useState<AnalyticsData | null>(null);
    const [loading, setLoading] = useState(true);
    const [timeRange, setTimeRange] = useState<'week' | 'month' | 'year'>('month');

    useEffect(() => {
        fetchAnalytics();
    }, [timeRange]);

    const fetchAnalytics = async () => {
        setLoading(true);
        try {
            const data: AnalyticsDashboardDto = await AnalyticsService.getApiAnalyticsDashboard();

            const transformedData: AnalyticsData = {
                revenue: {
                    total: data.totalRevenue || 0,
                    thisMonth: data.monthlyRevenue || 0,
                    lastMonth: 0, // Missing from DTO
                    growth: 0 // Missing from DTO
                },
                orders: {
                    total: data.totalOrders || 0,
                    completed: data.completedOrders || 0,
                    pending: data.pendingOrders || 0,
                    cancelled: data.cancelledOrders || 0
                },
                users: {
                    totalCustomers: data.totalCustomers || 0,
                    totalDrivers: data.totalDrivers || 0,
                    activeCustomers: 0, // Missing from DTO
                    activeDrivers: data.activeDrivers || 0
                },
                monthlyRevenue: [], // Missing from DTO
                ordersByStatus: [
                    { name: 'Completed', value: data.completedOrders || 0 },
                    { name: 'Pending', value: data.pendingOrders || 0 },
                    { name: 'Cancelled', value: data.cancelledOrders || 0 }
                ],
                topServices: [] // Missing from DTO
            };
            setAnalytics(transformedData);
        } catch (error) {
            console.error('Failed to fetch analytics:', error);
        } finally {
            setLoading(false);
        }
    };

    const COLORS = ['#10b981', '#f59e0b', '#ef4444', '#3b82f6', '#8b5cf6'];

    if (loading) {
        return (
            <div className="flex items-center justify-center min-h-screen">
                <div className="text-center">
                    <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
                    <p className="mt-4 text-gray-600">Loading analytics...</p>
                </div>
            </div>
        );
    }

    if (!analytics) {
        return (
            <div className="p-6">
                <div className="bg-red-50 border border-red-200 rounded-lg p-4">
                    <p className="text-red-800">Failed to load analytics data.</p>
                </div>
            </div>
        );
    }

    return (
        <div className="p-6 space-y-6">
            <div className="flex justify-between items-center">
                <h1 className="text-3xl font-bold text-gray-900">Analytics Dashboard</h1>
                <div className="flex gap-2">
                    <button
                        onClick={() => setTimeRange('week')}
                        className={`px-4 py-2 rounded-lg ${timeRange === 'week' ? 'bg-blue-600 text-white' : 'bg-gray-200 text-gray-700'}`}
                    >
                        Week
                    </button>
                    <button
                        onClick={() => setTimeRange('month')}
                        className={`px-4 py-2 rounded-lg ${timeRange === 'month' ? 'bg-blue-600 text-white' : 'bg-gray-200 text-gray-700'}`}
                    >
                        Month
                    </button>
                    <button
                        onClick={() => setTimeRange('year')}
                        className={`px-4 py-2 rounded-lg ${timeRange === 'year' ? 'bg-blue-600 text-white' : 'bg-gray-200 text-gray-700'}`}
                    >
                        Year
                    </button>
                </div>
            </div>

            {/* Key Metrics */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
                <div className="bg-white rounded-lg shadow p-6">
                    <h3 className="text-sm font-medium text-gray-500">Total Revenue</h3>
                    <p className="text-3xl font-bold text-gray-900 mt-2">${analytics.revenue.total.toLocaleString()}</p>
                    <p className="text-sm text-green-600 mt-2">+{analytics.revenue.growth}% from last month</p>
                </div>

                <div className="bg-white rounded-lg shadow p-6">
                    <h3 className="text-sm font-medium text-gray-500">Total Orders</h3>
                    <p className="text-3xl font-bold text-gray-900 mt-2">{analytics.orders.total.toLocaleString()}</p>
                    <p className="text-sm text-gray-600 mt-2">{analytics.orders.completed} completed</p>
                </div>

                <div className="bg-white rounded-lg shadow p-6">
                    <h3 className="text-sm font-medium text-gray-500">Active Customers</h3>
                    <p className="text-3xl font-bold text-gray-900 mt-2">{analytics.users.activeCustomers}</p>
                    <p className="text-sm text-gray-600 mt-2">of {analytics.users.totalCustomers} total</p>
                </div>

                <div className="bg-white rounded-lg shadow p-6">
                    <h3 className="text-sm font-medium text-gray-500">Active Drivers</h3>
                    <p className="text-3xl font-bold text-gray-900 mt-2">{analytics.users.activeDrivers}</p>
                    <p className="text-sm text-gray-600 mt-2">of {analytics.users.totalDrivers} total</p>
                </div>
            </div>

            {/* Charts */}
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                {/* Monthly Revenue Chart */}
                <div className="bg-white rounded-lg shadow p-6">
                    <h2 className="text-xl font-semibold text-gray-900 mb-4">Monthly Revenue</h2>
                    <ResponsiveContainer width="100%" height={300}>
                        <PieChart>
                            <Pie
                                data={analytics.ordersByStatus}
                                cx="50%"
                                cy="50%"
                                labelLine={false}
                                label={({ name, percent }) => `${name}: ${((percent ?? 0) * 100).toFixed(0)}%`}
                                outerRadius={100}
                                fill="#8884d8"
                                dataKey="value"
                            >
                                {analytics.ordersByStatus.map((_, index) => (
                                    <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                                ))}
                            </Pie>
                            <Tooltip />
                        </PieChart>
                    </ResponsiveContainer>
                </div>
            </div>

            {/* Top Services */}
            <div className="bg-white rounded-lg shadow p-6">
                <h2 className="text-xl font-semibold text-gray-900 mb-4">Top Services</h2>
                <ResponsiveContainer width="100%" height={300}>
                    <BarChart data={analytics.topServices}>
                        <CartesianGrid strokeDasharray="3 3" />
                        <XAxis dataKey="name" />
                        <YAxis yAxisId="left" orientation="left" stroke="#3b82f6" />
                        <YAxis yAxisId="right" orientation="right" stroke="#10b981" />
                        <Tooltip />
                        <Legend />
                        <Bar yAxisId="left" dataKey="orders" fill="#3b82f6" name="Orders" />
                        <Bar yAxisId="right" dataKey="revenue" fill="#10b981" name="Revenue ($)" />
                    </BarChart>
                </ResponsiveContainer>
            </div>
        </div >
    );
};

export default AnalyticsPage;
