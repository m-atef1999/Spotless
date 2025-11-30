import React, { useState } from 'react';
import { User, Mail, Lock, Shield } from 'lucide-react';
import { DashboardLayout } from '../../layouts/DashboardLayout';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { AuthService } from '../../lib/api';
import { useAuthStore } from '../../store/authStore';

export const AdminSettingsPage: React.FC = () => {
    const { user } = useAuthStore();

    // Password Change State
    const [currentPassword, setCurrentPassword] = useState('');
    const [newPassword, setNewPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [passwordMessage, setPasswordMessage] = useState<{ type: 'success' | 'error', text: string } | null>(null);
    const [isChangingPassword, setIsChangingPassword] = useState(false);

    const handlePasswordChange = async () => {
        if (!currentPassword || !newPassword || !confirmPassword) {
            setPasswordMessage({ type: 'error', text: 'All password fields are required.' });
            return;
        }

        if (newPassword !== confirmPassword) {
            setPasswordMessage({ type: 'error', text: 'New passwords do not match.' });
            return;
        }

        setIsChangingPassword(true);
        setPasswordMessage(null);

        try {
            await AuthService.postApiAuthChangePassword({
                requestBody: {
                    currentPassword,
                    newPassword,
                    userId: user?.id || ''
                }
            });
            setPasswordMessage({ type: 'success', text: 'Password changed successfully.' });
            setCurrentPassword('');
            setNewPassword('');
            setConfirmPassword('');
        } catch (err: any) {
            console.error('Failed to change password', err);
            const msg = err.body?.message || 'Failed to change password. Check your current password.';
            setPasswordMessage({ type: 'error', text: msg });
        } finally {
            setIsChangingPassword(false);
        }
    };

    return (
        <DashboardLayout role="Admin">
            <div className="max-w-4xl mx-auto space-y-8">
                <h1 className="text-2xl font-bold text-slate-900 dark:text-white">Admin Settings</h1>

                <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
                    {/* Sidebar / Summary */}
                    <div className="space-y-6">
                        <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 p-6 text-center">
                            <div className="w-24 h-24 bg-purple-100 dark:bg-purple-900/30 rounded-full flex items-center justify-center mx-auto mb-4 text-purple-600 dark:text-purple-400 text-2xl font-bold">
                                <Shield className="w-12 h-12" />
                            </div>
                            <h2 className="text-lg font-semibold text-slate-900 dark:text-white">
                                {(user as any)?.name || 'Admin User'}
                            </h2>
                            <p className="text-slate-500 dark:text-slate-400 text-sm mb-4">
                                {user?.email}
                            </p>
                            <div className="inline-flex items-center px-3 py-1 rounded-full text-xs font-medium bg-purple-100 text-purple-800 dark:bg-purple-900/30 dark:text-purple-400">
                                Super Admin
                            </div>
                        </div>
                    </div>

                    {/* Main Form */}
                    <div className="md:col-span-2 space-y-8">
                        <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 p-6">
                            <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-6">
                                Profile Information
                            </h3>

                            <div className="space-y-6">
                                <div className="grid grid-cols-1 sm:grid-cols-2 gap-6">
                                    <div className="sm:col-span-2">
                                        <Input
                                            label="Full Name"
                                            icon={<User className="w-5 h-5" />}
                                            value={(user as any)?.name || ''}
                                            disabled
                                            className="bg-slate-50 dark:bg-slate-800 text-slate-500"
                                        />
                                        <p className="text-xs text-slate-400 mt-1 ml-1">Name cannot be changed</p>
                                    </div>

                                    <div className="sm:col-span-2">
                                        <Input
                                            label="Email Address"
                                            icon={<Mail className="w-5 h-5" />}
                                            type="email"
                                            value={user?.email || ''}
                                            disabled
                                            className="bg-slate-50 dark:bg-slate-800 text-slate-500"
                                        />
                                        <p className="text-xs text-slate-400 mt-1 ml-1">Email cannot be changed</p>
                                    </div>
                                </div>
                            </div>
                        </div>

                        {/* Security Section */}
                        <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 p-6">
                            <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-6 flex items-center gap-2">
                                <Lock className="w-5 h-5 text-purple-500" />
                                Security
                            </h3>

                            <div className="space-y-4">
                                {passwordMessage && (
                                    <div className={`p-4 rounded-lg border ${passwordMessage.type === 'success' ? 'bg-green-50 text-green-700 border-green-100' : 'bg-red-50 text-red-700 border-red-100'}`}>
                                        {passwordMessage.text}
                                    </div>
                                )}

                                <div className="grid grid-cols-1 sm:grid-cols-2 gap-6">
                                    <div className="sm:col-span-2">
                                        <Input
                                            label="Current Password"
                                            type="password"
                                            placeholder="••••••••"
                                            value={currentPassword}
                                            onChange={(e) => setCurrentPassword(e.target.value)}
                                        />
                                    </div>
                                    <div>
                                        <Input
                                            label="New Password"
                                            type="password"
                                            placeholder="••••••••"
                                            value={newPassword}
                                            onChange={(e) => setNewPassword(e.target.value)}
                                        />
                                    </div>
                                    <div>
                                        <Input
                                            label="Confirm New Password"
                                            type="password"
                                            placeholder="••••••••"
                                            value={confirmPassword}
                                            onChange={(e) => setConfirmPassword(e.target.value)}
                                        />
                                    </div>
                                </div>
                                <div className="pt-2">
                                    <Button
                                        variant="outline"
                                        onClick={handlePasswordChange}
                                        isLoading={isChangingPassword}
                                    >
                                        Update Password
                                    </Button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </DashboardLayout>
    );
};
