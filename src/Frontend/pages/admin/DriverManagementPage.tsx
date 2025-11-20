import React, { useState, useEffect } from 'react';
import { Check, X, Search, MoreVertical, Truck } from 'lucide-react';
import { DashboardLayout } from '../../layouts/DashboardLayout';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { DriverDto } from '../../lib/apiClient';

// Mock data since API endpoint for listing drivers is missing
const MOCK_DRIVERS: DriverDto[] = [
    new DriverDto({
        id: '1',
        name: 'John Smith',
        email: 'john.smith@example.com',
        phone: '+1 (555) 123-4567',
        vehicleInfo: '2019 Toyota Prius',
        status: 'Active',
    }),
    new DriverDto({
        id: '2',
        name: 'Sarah Johnson',
        email: 'sarah.j@example.com',
        phone: '+1 (555) 987-6543',
        vehicleInfo: '2021 Honda Civic',
        status: 'Pending',
    }),
    new DriverDto({
        id: '3',
        name: 'Michael Brown',
        email: 'm.brown@example.com',
        phone: '+1 (555) 456-7890',
        vehicleInfo: '2020 Ford Fusion',
        status: 'Active',
    }),
];

export const DriverManagementPage: React.FC = () => {
    const [drivers, setDrivers] = useState<DriverDto[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [searchTerm, setSearchTerm] = useState('');
    const [processingId, setProcessingId] = useState<string | null>(null);

    useEffect(() => {
        // Simulate API fetch
        setTimeout(() => {
            setDrivers(MOCK_DRIVERS);
            setIsLoading(false);
        }, 1000);
    }, []);

    const handleApprove = async (driverId: string) => {
        if (!driverId) return;
        setProcessingId(driverId);
        try {
            // In a real scenario, we would call the API
            // await apiClient.approve(driverId, new ApproveDriverRequest({ password: 'DefaultPassword123!' }));

            // Update local state to reflect change
            setDrivers(prev => prev.map(d =>
                d.id === driverId ? new DriverDto({ ...d, status: 'Active' }) : d
            ));
        } catch (error) {
            console.error('Failed to approve driver', error);
        } finally {
            setProcessingId(null);
        }
    };

    const handleReject = async (driverId: string) => {
        // Implement reject logic
        console.log('Reject driver', driverId);
    };

    const filteredDrivers = drivers.filter(driver =>
        driver.name?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        driver.email?.toLowerCase().includes(searchTerm.toLowerCase())
    );

    return (
        <DashboardLayout role="Admin">
            <div className="space-y-8">
                <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
                    <div>
                        <h1 className="text-2xl font-bold text-slate-900 dark:text-white">
                            Driver Management
                        </h1>
                        <p className="text-slate-500 dark:text-slate-400 mt-1">
                            Manage driver applications and active fleet.
                        </p>
                    </div>
                    <div className="w-full sm:w-72">
                        <Input
                            placeholder="Search drivers..."
                            icon={<Search className="w-5 h-5" />}
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                        />
                    </div>
                </div>

                <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 overflow-hidden">
                    <div className="overflow-x-auto">
                        <table className="w-full text-left border-collapse">
                            <thead>
                                <tr className="bg-slate-50 dark:bg-slate-800/50 border-b border-slate-200 dark:border-slate-800">
                                    <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">Driver</th>
                                    <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">Contact</th>
                                    <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">Vehicle</th>
                                    <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">Status</th>
                                    <th className="px-6 py-4 text-xs font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider text-right">Actions</th>
                                </tr>
                            </thead>
                            <tbody className="divide-y divide-slate-200 dark:divide-slate-800">
                                {isLoading ? (
                                    <tr>
                                        <td colSpan={5} className="px-6 py-8 text-center text-slate-500">
                                            Loading drivers...
                                        </td>
                                    </tr>
                                ) : filteredDrivers.length === 0 ? (
                                    <tr>
                                        <td colSpan={5} className="px-6 py-8 text-center text-slate-500">
                                            No drivers found.
                                        </td>
                                    </tr>
                                ) : (
                                    filteredDrivers.map((driver) => (
                                        <tr key={driver.id} className="hover:bg-slate-50 dark:hover:bg-slate-800/50 transition-colors">
                                            <td className="px-6 py-4">
                                                <div className="flex items-center gap-3">
                                                    <div className="w-10 h-10 bg-cyan-100 dark:bg-cyan-900/30 rounded-full flex items-center justify-center text-cyan-600 dark:text-cyan-400 font-bold">
                                                        {driver.name?.charAt(0)}
                                                    </div>
                                                    <div>
                                                        <div className="font-medium text-slate-900 dark:text-white">{driver.name}</div>
                                                        <div className="text-xs text-slate-500">ID: {driver.id}</div>
                                                    </div>
                                                </div>
                                            </td>
                                            <td className="px-6 py-4">
                                                <div className="text-sm text-slate-600 dark:text-slate-300">{driver.email}</div>
                                                <div className="text-sm text-slate-500">{driver.phone}</div>
                                            </td>
                                            <td className="px-6 py-4">
                                                <div className="flex items-center gap-2 text-sm text-slate-600 dark:text-slate-300">
                                                    <Truck className="w-4 h-4 text-slate-400" />
                                                    {driver.vehicleInfo}
                                                </div>
                                            </td>
                                            <td className="px-6 py-4">
                                                <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${driver.status === 'Active'
                                                    ? 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400'
                                                    : 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400'
                                                    }`}>
                                                    {driver.status}
                                                </span>
                                            </td>
                                            <td className="px-6 py-4 text-right">
                                                {driver.status === 'Pending' ? (
                                                    <div className="flex items-center justify-end gap-2">
                                                        <Button
                                                            size="sm"
                                                            onClick={() => handleApprove(driver.id!)}
                                                            isLoading={processingId === driver.id}
                                                            className="bg-green-600 hover:bg-green-700 text-white"
                                                        >
                                                            <Check className="w-4 h-4 mr-1" />
                                                            Approve
                                                        </Button>
                                                        <Button
                                                            size="sm"
                                                            variant="outline"
                                                            onClick={() => handleReject(driver.id!)}
                                                            className="text-red-600 hover:bg-red-50 border-red-200"
                                                        >
                                                            <X className="w-4 h-4" />
                                                        </Button>
                                                    </div>
                                                ) : (
                                                    <Button size="sm" variant="ghost">
                                                        <MoreVertical className="w-4 h-4" />
                                                    </Button>
                                                )}
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
