import React, { useState, useEffect } from 'react';
import { Power, CheckCircle, XCircle } from 'lucide-react';
import { DashboardLayout } from '../../layouts/DashboardLayout';
import { Button } from '../../components/ui/Button';
import { DriversService } from '../../lib/api';

export const DriverStatusPage: React.FC = () => {
    const [status, setStatus] = useState<string>('Offline');
    const [isLoading, setIsLoading] = useState(true);
    const [isUpdating, setIsUpdating] = useState(false);

    useEffect(() => {
        const fetchProfile = async () => {
            try {
                const profile = await DriversService.getApiDriversProfile();
                setStatus(profile.status || 'Offline');
            } catch (error) {
                console.error('Failed to fetch driver profile', error);
            } finally {
                setIsLoading(false);
            }
        };
        fetchProfile();
    }, []);

    // Real-time status updates (optional, gracefully handle failures)
    useEffect(() => {
        const apiUrl = import.meta.env.VITE_API_URL || 'https://spotless.runasp.net';
        const baseUrl = apiUrl.endsWith('/') ? apiUrl.slice(0, -1) : apiUrl;
        const token = localStorage.getItem('token');

        // Skip SignalR if no token available
        if (!token) {
            return;
        }

        let connection: any = null;

        // Import dynamically to avoid issues if not installed, though it should be
        import('@microsoft/signalr').then(signalR => {
            connection = new signalR.HubConnectionBuilder()
                .withUrl(`${baseUrl}/driverHub`, {
                    accessTokenFactory: () => token
                })
                .withAutomaticReconnect()
                .configureLogging(signalR.LogLevel.None) // Suppress logs
                .build();

            connection.start()
                .then(() => {
                    // Successfully connected
                    connection.on('DriverAvailabilityUpdated', (_userId: string, newStatus: string) => {
                        setStatus(newStatus);
                    });
                })
                .catch(() => {
                    // Silently fail - SignalR is optional for this page
                    // The page still works with manual refresh
                });
        }).catch(() => {
            // SignalR module not available, that's fine
        });

        return () => {
            if (connection) {
                connection.stop().catch(() => { });
            }
        };
    }, []);

    const handleToggleStatus = async () => {
        const newStatus = status === 'Available' ? 'Offline' : 'Available';
        setIsUpdating(true);
        try {
            await DriversService.putApiDriversStatus({
                requestBody: { status: newStatus }
            });
            // Optimistic update, but real-time will confirm
            setStatus(newStatus);
        } catch (error) {
            console.error('Failed to update status', error);
            alert('Failed to update status. Please try again.');
        } finally {
            setIsUpdating(false);
        }
    };

    const handleSetStatus = async (newStatus: string) => {
        if (newStatus === status) return;
        setIsUpdating(true);
        try {
            await DriversService.putApiDriversStatus({
                requestBody: { status: newStatus }
            });
            setStatus(newStatus);
        } catch (error) {
            console.error('Failed to update status', error);
            alert('Failed to update status. Please try again.');
        } finally {
            setIsUpdating(false);
        }
    };

    const availableStatuses = [
        { value: 'Available', label: 'Available', description: 'Ready to accept new orders', color: 'green' },
        { value: 'OnRoute', label: 'On Route', description: 'Currently on a pickup/delivery', color: 'blue' },
        { value: 'Busy', label: 'Busy', description: 'Working but cannot take new orders', color: 'orange' },
        { value: 'Offline', label: 'Offline', description: 'Not accepting any orders', color: 'slate' },
    ];

    const isOnline = status === 'Available' || status === 'Online';

    return (
        <DashboardLayout role="Driver">
            <div className="max-w-md mx-auto space-y-8">
                <div>
                    <h1 className="text-2xl font-bold text-slate-900 dark:text-white">
                        Driver Status
                    </h1>
                    <p className="text-slate-500 dark:text-slate-400 mt-1">
                        Manage your availability to receive orders.
                    </p>
                </div>

                <div className="bg-white dark:bg-slate-900 p-8 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-800 space-y-8">
                    <div className={`mx-auto w-24 h-24 rounded-full flex items-center justify-center transition-colors ${status === 'Available' ? 'bg-green-100 text-green-600 dark:bg-green-900/30 dark:text-green-400' :
                            status === 'OnRoute' ? 'bg-blue-100 text-blue-600 dark:bg-blue-900/30 dark:text-blue-400' :
                                status === 'Busy' ? 'bg-orange-100 text-orange-600 dark:bg-orange-900/30 dark:text-orange-400' :
                                    'bg-slate-100 text-slate-400 dark:bg-slate-800 dark:text-slate-500'
                        }`}>
                        <Power className="w-12 h-12" />
                    </div>

                    <div className="text-center">
                        <h2 className="text-xl font-semibold text-slate-900 dark:text-white">
                            You are currently {status}
                        </h2>
                        <p className="text-slate-500 dark:text-slate-400 mt-2">
                            {availableStatuses.find(s => s.value === status)?.description || 'Select a status below'}
                        </p>
                    </div>

                    {/* Status Options Grid */}
                    <div className="grid grid-cols-2 gap-3">
                        {availableStatuses.map((statusOption) => (
                            <button
                                key={statusOption.value}
                                onClick={() => handleSetStatus(statusOption.value)}
                                disabled={isUpdating || isLoading}
                                className={`p-4 rounded-xl border-2 transition-all ${status === statusOption.value
                                        ? statusOption.color === 'green' ? 'border-green-500 bg-green-50 dark:bg-green-900/20' :
                                            statusOption.color === 'blue' ? 'border-blue-500 bg-blue-50 dark:bg-blue-900/20' :
                                                statusOption.color === 'orange' ? 'border-orange-500 bg-orange-50 dark:bg-orange-900/20' :
                                                    'border-slate-500 bg-slate-50 dark:bg-slate-900/20'
                                        : 'border-slate-200 dark:border-slate-700 hover:border-slate-300 dark:hover:border-slate-600'
                                    }`}
                            >
                                <div className={`w-3 h-3 rounded-full mx-auto mb-2 ${statusOption.color === 'green' ? 'bg-green-500' :
                                        statusOption.color === 'blue' ? 'bg-blue-500' :
                                            statusOption.color === 'orange' ? 'bg-orange-500' :
                                                'bg-slate-400'
                                    }`} />
                                <div className="font-medium text-slate-900 dark:text-white text-sm">
                                    {statusOption.label}
                                </div>
                                <div className="text-xs text-slate-500 dark:text-slate-400 mt-1">
                                    {statusOption.description}
                                </div>
                            </button>
                        ))}
                    </div>

                    {/* Quick Toggle for convenience */}
                    <Button
                        size="lg"
                        onClick={handleToggleStatus}
                        isLoading={isUpdating || isLoading}
                        className={`w-full py-4 text-base ${isOnline
                            ? 'bg-red-500 hover:bg-red-600 text-white'
                            : 'bg-green-500 hover:bg-green-600 text-white'
                            }`}
                    >
                        {isOnline ? (
                            <>
                                <XCircle className="w-5 h-5 mr-2" />
                                Quick: Go Offline
                            </>
                        ) : (
                            <>
                                <CheckCircle className="w-5 h-5 mr-2" />
                                Quick: Go Online
                            </>
                        )}
                    </Button>
                </div>
            </div>
        </DashboardLayout>
    );
};
