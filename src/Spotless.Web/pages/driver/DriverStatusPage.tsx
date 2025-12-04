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

                <div className="bg-white dark:bg-slate-900 p-8 rounded-2xl shadow-sm border border-slate-100 dark:border-slate-800 text-center space-y-8">
                    <div className={`mx-auto w-24 h-24 rounded-full flex items-center justify-center transition-colors ${isOnline
                        ? 'bg-green-100 text-green-600 dark:bg-green-900/30 dark:text-green-400'
                        : 'bg-slate-100 text-slate-400 dark:bg-slate-800 dark:text-slate-500'
                        }`}>
                        <Power className="w-12 h-12" />
                    </div>

                    <div>
                        <h2 className="text-xl font-semibold text-slate-900 dark:text-white">
                            You are currently {status}
                        </h2>
                        <p className="text-slate-500 dark:text-slate-400 mt-2">
                            {isOnline
                                ? "You are visible to customers and can receive new orders."
                                : "You will not receive any new order notifications."}
                        </p>
                    </div>

                    <Button
                        size="lg"
                        onClick={handleToggleStatus}
                        isLoading={isUpdating || isLoading}
                        className={`w-full py-6 text-lg ${isOnline
                            ? 'bg-red-500 hover:bg-red-600 text-white'
                            : 'bg-green-500 hover:bg-green-600 text-white'
                            }`}
                    >
                        {isOnline ? (
                            <>
                                <XCircle className="w-5 h-5 mr-2" />
                                Go Offline
                            </>
                        ) : (
                            <>
                                <CheckCircle className="w-5 h-5 mr-2" />
                                Go Online
                            </>
                        )}
                    </Button>
                </div>
            </div>
        </DashboardLayout>
    );
};
