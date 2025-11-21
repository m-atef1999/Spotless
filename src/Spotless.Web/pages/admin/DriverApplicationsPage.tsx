import React, { useState, useEffect } from 'react';
import { Check, X, FileText, Truck, User } from 'lucide-react';
import { DashboardLayout } from '../../layouts/DashboardLayout';
import { Button } from '../../components/ui/Button';
import { DriversService, type DriverDto } from '../../lib/api';

// Mock data for pending drivers as there isn't a dedicated "get pending" endpoint visible in service yet
// In real implementation, we might filter getApiDrivers or use a specific endpoint
const MOCK_PENDING_DRIVERS: DriverDto[] = [
    {
        id: 'p1',
        name: 'Alex Johnson',
        email: 'alex.j@example.com',
        phone: '+1 (555) 111-2222',
        vehicleInfo: '2022 Tesla Model 3',
        status: 'Pending',
    },
    {
        id: 'p2',
        name: 'Sam Wilson',
        email: 'sam.w@example.com',
        phone: '+1 (555) 333-4444',
        vehicleInfo: '2020 Ford Transit',
        status: 'Pending',
    }
];

export const DriverApplicationsPage: React.FC = () => {
    const [applications, setApplications] = useState<DriverDto[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [processingId, setProcessingId] = useState<string | null>(null);

    useEffect(() => {
        // Simulate fetching pending applications
        // In future: await DriversService.getApiDrivers({ status: 'Pending' });
        setTimeout(() => {
            setApplications(MOCK_PENDING_DRIVERS);
            setIsLoading(false);
        }, 1000);
    }, []);

    const handleApprove = async (driverId: string) => {
        if (!driverId) return;
        setProcessingId(driverId);
        try {
            // Using the approve endpoint from DriversService
            // Note: applicationId might be different from driverId depending on backend, assuming driverId for now
            await DriversService.postApiDriversAdminApplicationsApprove({
                applicationId: driverId,
                requestBody: {
                    // Add any required fields for approval if needed
                }
            });

            setApplications(prev => prev.filter(app => app.id !== driverId));
            alert('Driver application approved successfully');
        } catch (error) {
            console.error('Failed to approve application', error);
            alert('Failed to approve application. Please try again.');
        } finally {
            setProcessingId(null);
        }
    };

    const handleReject = async (driverId: string) => {
        if (!confirm('Are you sure you want to reject this application?')) return;

        // Mock rejection for now as specific reject endpoint might be different
        setApplications(prev => prev.filter(app => app.id !== driverId));
    };

    return (
        <DashboardLayout role="Admin">
            <div className="space-y-8">
                <div>
                    <h1 className="text-2xl font-bold text-slate-900 dark:text-white">
                        Driver Applications
                    </h1>
                    <p className="text-slate-500 dark:text-slate-400 mt-1">
                        Review and approve new driver registrations.
                    </p>
                </div>

                <div className="grid grid-cols-1 gap-6">
                    {isLoading ? (
                        <div className="text-center py-12 text-slate-500">Loading applications...</div>
                    ) : applications.length === 0 ? (
                        <div className="text-center py-12 bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800">
                            <FileText className="w-12 h-12 text-slate-300 mx-auto mb-4" />
                            <h3 className="text-lg font-medium text-slate-900 dark:text-white">No Pending Applications</h3>
                            <p className="text-slate-500">All caught up! There are no new driver applications to review.</p>
                        </div>
                    ) : (
                        applications.map((app) => (
                            <div key={app.id} className="bg-white dark:bg-slate-900 p-6 rounded-xl border border-slate-200 dark:border-slate-800 shadow-sm flex flex-col md:flex-row md:items-center justify-between gap-6">
                                <div className="flex items-start gap-4">
                                    <div className="w-12 h-12 bg-orange-100 dark:bg-orange-900/30 rounded-full flex items-center justify-center text-orange-600 dark:text-orange-400 shrink-0">
                                        <User className="w-6 h-6" />
                                    </div>
                                    <div>
                                        <h3 className="text-lg font-semibold text-slate-900 dark:text-white">{app.name}</h3>
                                        <div className="space-y-1 mt-1">
                                            <p className="text-sm text-slate-600 dark:text-slate-300 flex items-center gap-2">
                                                <span className="font-medium">Email:</span> {app.email}
                                            </p>
                                            <p className="text-sm text-slate-600 dark:text-slate-300 flex items-center gap-2">
                                                <span className="font-medium">Phone:</span> {app.phone}
                                            </p>
                                            <p className="text-sm text-slate-600 dark:text-slate-300 flex items-center gap-2">
                                                <Truck className="w-4 h-4 text-slate-400" />
                                                {app.vehicleInfo}
                                            </p>
                                        </div>
                                    </div>
                                </div>

                                <div className="flex items-center gap-3 md:self-center self-end">
                                    <Button
                                        variant="outline"
                                        onClick={() => handleReject(app.id!)}
                                        className="text-red-600 hover:bg-red-50 border-red-200"
                                    >
                                        <X className="w-4 h-4 mr-2" />
                                        Reject
                                    </Button>
                                    <Button
                                        onClick={() => handleApprove(app.id!)}
                                        isLoading={processingId === app.id}
                                        className="bg-green-600 hover:bg-green-700 text-white"
                                    >
                                        <Check className="w-4 h-4 mr-2" />
                                        Approve Application
                                    </Button>
                                </div>
                            </div>
                        ))
                    )}
                </div>
            </div>
        </DashboardLayout>
    );
};
