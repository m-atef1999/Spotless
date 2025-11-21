import React, { useState, useEffect } from 'react';
import { Search, Filter, Eye, Calendar, User } from 'lucide-react';
import { DashboardLayout } from '../../layouts/DashboardLayout';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { OrderStatus, DriversService } from '../../lib/api';

// Extended interface for mock data
interface MockOrder {
    id: string;
    serviceName: string;
    totalAmount: number;
    status: number;
    scheduledDate: Date;
    address: string;
    customerName: string;
    assignedDriver?: string;
}



// Mock data
const MOCK_ORDERS: MockOrder[] = [
    {
        id: '1001',
        serviceName: 'Deep Cleaning - 3 Bedroom',
        totalAmount: 150.00,
        status: OrderStatus.Confirmed, // Confirmed
        scheduledDate: new Date('2023-11-25T10:00:00'),
        address: '123 Main St, Springfield',
        customerName: 'Alice Walker',
    },
    {
        id: '1002',
        serviceName: 'Carpet Cleaning',
        totalAmount: 85.50,
        status: OrderStatus.Requested, // Pending
        scheduledDate: new Date('2023-11-26T14:00:00'),
        address: '456 Elm St, Springfield',
        customerName: 'Bob Martin',
    },
    {
        id: '1003',
        serviceName: 'Move-out Cleaning',
        totalAmount: 320.00,
        status: OrderStatus.InCleaning, // InProgress
        scheduledDate: new Date('2023-11-24T09:00:00'),
        address: '789 Oak Ave, Springfield',
        customerName: 'Charlie Davis',
    },
];

export const OrderManagementPage: React.FC = () => {
    const [orders, setOrders] = useState<MockOrder[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [searchTerm, setSearchTerm] = useState('');

    const handleAssignDriver = async (orderId: string) => {
        const driverId = window.prompt('Enter Driver ID to assign (e.g., d1, d2):');
        if (!driverId) return;

        try {
            await DriversService.postApiDriversAdminAssign({
                requestBody: {
                    orderId,
                    driverId,
                    // adminId is likely handled by backend from token, or we can omit if optional
                }
            });

            setOrders(prev => prev.map(o =>
                o.id === orderId ? { ...o, status: OrderStatus.Confirmed, assignedDriver: `Driver ${driverId}` } : o
            ));
            alert(`Driver ${driverId} assigned to order #${orderId} successfully!`);
        } catch (error) {
            console.error('Failed to assign driver:', error);
            alert('Failed to assign driver. Please try again.');
        }
    };

    useEffect(() => {
        // Simulate API fetch
        setTimeout(() => {
            setOrders(MOCK_ORDERS);
            setIsLoading(false);
        }, 1000);
    }, []);

    const getStatusBadge = (status: number) => {
        switch (status) {
            case OrderStatus.Requested: // Pending
                return <span className="px-2 py-1 rounded-full text-xs font-medium bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400">Pending</span>;
            case OrderStatus.Confirmed: // Confirmed
                return <span className="px-2 py-1 rounded-full text-xs font-medium bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400">Confirmed</span>;
            case OrderStatus.InCleaning: // InProgress
                return <span className="px-2 py-1 rounded-full text-xs font-medium bg-purple-100 text-purple-800 dark:bg-purple-900/30 dark:text-purple-400">In Progress</span>;
            case OrderStatus.Delivered: // Completed
                return <span className="px-2 py-1 rounded-full text-xs font-medium bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400">Completed</span>;
            case OrderStatus.Cancelled: // Cancelled
                return <span className="px-2 py-1 rounded-full text-xs font-medium bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400">Cancelled</span>;
            default:
                return <span className="px-2 py-1 rounded-full text-xs font-medium bg-slate-100 text-slate-800 dark:bg-slate-800 dark:text-slate-400">Unknown</span>;
        }
    };

    const filteredOrders = orders.filter(order =>
        order.id.includes(searchTerm) ||
        order.customerName.toLowerCase().includes(searchTerm.toLowerCase()) ||
        order.serviceName.toLowerCase().includes(searchTerm.toLowerCase())
    );

    return (
        <DashboardLayout role="Admin">
            <div className="space-y-8">
                <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
                    <div>
                        <h1 className="text-2xl font-bold text-slate-900 dark:text-white">
                            Order Management
                        </h1>
                        <p className="text-slate-500 dark:text-slate-400 mt-1">
                            View and manage customer bookings.
                        </p>
                    </div>
                    <div className="flex gap-2 w-full sm:w-auto">
                        <div className="w-full sm:w-64">
                            <Input
                                placeholder="Search orders..."
                                icon={<Search className="w-5 h-5" />}
                                value={searchTerm}
                                onChange={(e) => setSearchTerm(e.target.value)}
                            />
                        </div>
                        <Button variant="outline" className="shrink-0">
                            <Filter className="w-4 h-4 mr-2" />
                            Filter
                        </Button>
                    </div>
                </div>

                <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 overflow-hidden">
                    <div className="overflow-x-auto">
                        <table className="w-full text-left border-collapse">
                            <thead>
                                <tr className="bg-slate-50 dark:bg-slate-800/50 border-b border-slate-200 dark:border-slate-800">
                                    <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">Order ID</th>
                                    <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">Customer</th>
                                    <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">Service</th>
                                    <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">Date</th>
                                    <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">Status</th>
                                    <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">Assigned To</th>
                                    <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">Amount</th>
                                    <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider text-right">Actions</th>
                                </tr>
                            </thead>
                            <tbody className="divide-y divide-slate-200 dark:divide-slate-800">
                                {isLoading ? (
                                    <tr>
                                        <td colSpan={7} className="px-6 py-8 text-center text-slate-500">
                                            Loading orders...
                                        </td>
                                    </tr>
                                ) : filteredOrders.length === 0 ? (
                                    <tr>
                                        <td colSpan={7} className="px-6 py-8 text-center text-slate-500">
                                            No orders found.
                                        </td>
                                    </tr>
                                ) : (
                                    filteredOrders.map((order) => (
                                        <tr key={order.id} className="hover:bg-slate-50 dark:hover:bg-slate-800/50 transition-colors">
                                            <td className="px-6 py-4 font-mono text-sm text-slate-500">
                                                #{order.id}
                                            </td>
                                            <td className="px-6 py-4">
                                                <div className="flex items-center gap-2">
                                                    <User className="w-4 h-4 text-slate-400" />
                                                    <span className="font-medium text-slate-900 dark:text-white">{order.customerName}</span>
                                                </div>
                                            </td>
                                            <td className="px-6 py-4 text-sm text-slate-600 dark:text-slate-300">
                                                {order.serviceName}
                                            </td>
                                            <td className="px-6 py-4">
                                                <div className="flex items-center gap-2 text-sm text-slate-500">
                                                    <Calendar className="w-4 h-4" />
                                                    {order.scheduledDate.toLocaleDateString()}
                                                </div>
                                            </td>
                                            <td className="px-6 py-4">
                                                {getStatusBadge(order.status)}
                                            </td>
                                            <td className="px-6 py-4 text-sm text-slate-500">
                                                {order.assignedDriver || <span className="text-slate-400 italic">Unassigned</span>}
                                            </td>
                                            <td className="px-6 py-4 font-medium text-slate-900 dark:text-white">
                                                ${order.totalAmount.toFixed(2)}
                                            </td>
                                            <td className="px-6 py-4 text-right">
                                                <div className="flex justify-end gap-2">
                                                    {!order.assignedDriver && order.status !== OrderStatus.Delivered && order.status !== OrderStatus.Cancelled && (
                                                        <Button size="sm" variant="outline" onClick={() => handleAssignDriver(order.id)}>
                                                            Assign
                                                        </Button>
                                                    )}
                                                    <Button size="sm" variant="ghost">
                                                        <Eye className="w-4 h-4" />
                                                    </Button>
                                                </div>
                                            </td>
                                        </tr>
                                    ))
                                )}
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </DashboardLayout>
    );
};
