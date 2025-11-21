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

    const handleToggleStatus = async () => {
        const newStatus = status === 'Online' ? 'Offline' : 'Online';
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

    const isOnline = status === 'Online';

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
