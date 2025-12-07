import React, { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { User, Mail, Phone, MapPin, Save, Loader2, Wallet, Lock, CheckCircle, Clock } from 'lucide-react';
import { DashboardLayout } from '../../layouts/DashboardLayout';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';

import { CustomersService, AuthService, DriversService, OpenAPI, type CustomerUpdateRequest, type CustomerDto, type DriverDto } from '../../lib/api';
import { useAuthStore } from '../../store/authStore';

const profileSchema = z.object({
    name: z.string().min(2, 'Name must be at least 2 characters'),
    phone: z.string().min(10, 'Phone number must be at least 10 digits'),
    street: z.string().optional(),
    city: z.string().optional(),
    country: z.string().optional(),
    zipCode: z.string().optional(),
});

type ProfileFormValues = z.infer<typeof profileSchema>;

// Type for driver application response
interface DriverApplicationStatus {
    id: string;
    status: string;
    rejectionReason?: string;
    updatedAt?: string;
}

export const ProfilePage: React.FC = () => {
    const [isLoading, setIsLoading] = useState(true);
    const [isSaving, setIsSaving] = useState(false);
    const [profile, setProfile] = useState<CustomerDto | null>(null);
    const [driverProfile, setDriverProfile] = useState<DriverDto | null>(null);
    const [applicationStatus, setApplicationStatus] = useState<DriverApplicationStatus | null>(null);
    const [successMessage, setSuccessMessage] = useState<string | null>(null);
    const [error, setError] = useState<string | null>(null);
    const user = useAuthStore((state) => state.user);

    // Password Change State
    const [currentPassword, setCurrentPassword] = useState('');
    const [newPassword, setNewPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [passwordMessage, setPasswordMessage] = useState<{ type: 'success' | 'error', text: string } | null>(null);
    const [isChangingPassword, setIsChangingPassword] = useState(false);

    const {
        register,
        handleSubmit,
        setValue,
        formState: { errors },
    } = useForm<ProfileFormValues>({
        resolver: zodResolver(profileSchema),
    });

    useEffect(() => {
        const fetchData = async () => {
            try {
                const [customerData, driverData] = await Promise.allSettled([
                    CustomersService.getApiCustomersMe(),
                    DriversService.getApiDriversProfile()
                ]);

                if (customerData.status === 'fulfilled') {
                    const data = customerData.value;
                    setProfile(data);
                    // Pre-fill form
                    setValue('name', data.name || '');
                    setValue('phone', data.phone || '');
                    setValue('street', data.street || '');
                    setValue('city', data.city || '');
                    setValue('country', data.country || '');
                    setValue('zipCode', data.zipCode || '');
                }

                if (driverData.status === 'fulfilled') {
                    setDriverProfile(driverData.value);
                }

                // Also fetch driver application status (for pending applications)
                try {
                    const token = typeof OpenAPI.TOKEN === 'function' ? await OpenAPI.TOKEN({} as any) : OpenAPI.TOKEN;
                    const response = await fetch(`${OpenAPI.BASE}/api/Drivers/my-application`, {
                        headers: {
                            'Authorization': `Bearer ${token}`,
                            'Content-Type': 'application/json',
                        },
                    });
                    if (response.ok) {
                        const appData = await response.json();
                        setApplicationStatus(appData);
                    }
                } catch {
                    // No application found, which is fine
                }

            } catch (err) {
                console.error('Failed to fetch profile', err);
                setError('Failed to load profile data.');
            } finally {
                setIsLoading(false);
            }
        };

        fetchData();
    }, [setValue]);

    const onSubmit = async (data: ProfileFormValues) => {
        setIsSaving(true);
        setSuccessMessage(null);
        setError(null);

        try {
            const command: CustomerUpdateRequest = {
                name: data.name,
                phone: data.phone,
                street: data.street,
                city: data.city,
                country: data.country,
                zipCode: data.zipCode,
            };

            await CustomersService.putApiCustomersMe({ requestBody: command });
            setSuccessMessage('Profile updated successfully!');

            // Refresh profile data to ensure sync
            const updatedProfile = await CustomersService.getApiCustomersMe();
            setProfile(updatedProfile);
        } catch (err) {
            console.error('Failed to update profile', err);
            setError('Failed to update profile. Please try again.');
        } finally {
            setIsSaving(false);
        }
    };

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
                    userId: user?.id || '' // Although backend extracts from token, some DTOs might require it or it's ignored
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

    if (isLoading) {
        return (
            <DashboardLayout role="Customer">
                <div className="flex justify-center py-12">
                    <Loader2 className="w-8 h-8 animate-spin text-cyan-500" />
                </div>
            </DashboardLayout>
        );
    }

    return (
        <DashboardLayout role="Customer">
            <div className="max-w-4xl mx-auto space-y-8">
                <h1 className="text-2xl font-bold text-slate-900 dark:text-white">Settings</h1>

                <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
                    {/* Sidebar / Summary */}
                    <div className="space-y-6">
                        <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 p-6 text-center">
                            <div className="w-24 h-24 bg-cyan-100 dark:bg-cyan-900/30 rounded-full flex items-center justify-center mx-auto mb-4 text-cyan-600 dark:text-cyan-400 text-2xl font-bold">
                                {profile?.name?.charAt(0).toUpperCase() || user?.email?.charAt(0).toUpperCase()}
                            </div>
                            <h2 className="text-lg font-semibold text-slate-900 dark:text-white">
                                {profile?.name || 'User'}
                            </h2>
                            <p className="text-slate-500 dark:text-slate-400 text-sm mb-4">
                                {profile?.email || user?.email}
                            </p>
                            <div className="flex items-center justify-center gap-2 text-sm font-medium text-slate-700 dark:text-slate-300 bg-slate-50 dark:bg-slate-800 py-2 rounded-lg">
                                <Wallet className="w-4 h-4 text-cyan-500" />
                                Balance: {profile?.walletBalance?.toFixed(2) || '0.00'} EGP
                            </div>
                        </div>
                    </div>

                    {/* Main Form */}
                    <div className="md:col-span-2 space-y-8">
                        <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 p-6">
                            <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-6">
                                Personal Information
                            </h3>



                            <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
                                <div className="grid grid-cols-1 sm:grid-cols-2 gap-6">
                                    <div className="sm:col-span-2">
                                        <Input
                                            label="Full Name"
                                            icon={<User className="w-5 h-5" />}
                                            error={errors.name?.message}
                                            {...register('name')}
                                        />
                                    </div>

                                    <div className="sm:col-span-2">
                                        <Input
                                            label="Email Address"
                                            icon={<Mail className="w-5 h-5" />}
                                            type="email"
                                            disabled
                                            value={profile?.email || ''}
                                            className="bg-slate-50 dark:bg-slate-800 text-slate-500"
                                        />
                                        <p className="text-xs text-slate-400 mt-1 ml-1">Email cannot be changed</p>
                                    </div>

                                    <div className="sm:col-span-2">
                                        <Input
                                            label="Phone Number"
                                            icon={<Phone className="w-5 h-5" />}
                                            error={errors.phone?.message}
                                            {...register('phone')}
                                        />
                                    </div>

                                    <div className="sm:col-span-2 pt-4 border-t border-slate-100 dark:border-slate-800">
                                        <h4 className="text-sm font-medium text-slate-900 dark:text-white mb-4 flex items-center gap-2">
                                            <MapPin className="w-4 h-4 text-cyan-500" />
                                            Address Details
                                        </h4>
                                    </div>

                                    <div className="sm:col-span-2">
                                        <Input
                                            label="Street Address"
                                            placeholder="123 Main St"
                                            error={errors.street?.message}
                                            {...register('street')}
                                        />
                                    </div>

                                    <div>
                                        <Input
                                            label="City"
                                            placeholder="New York"
                                            error={errors.city?.message}
                                            {...register('city')}
                                        />
                                    </div>

                                    <div>
                                        <Input
                                            label="ZIP Code"
                                            placeholder="10001"
                                            error={errors.zipCode?.message}
                                            {...register('zipCode')}
                                        />
                                    </div>

                                    <div className="sm:col-span-2">
                                        <Input
                                            label="Country"
                                            placeholder="United States"
                                            error={errors.country?.message}
                                            {...register('country')}
                                        />
                                    </div>
                                </div>

                                {successMessage && (
                                    <div className="p-4 bg-green-600 text-white rounded-lg shadow-sm">
                                        <div className="flex items-center gap-2">
                                            <CheckCircle className="w-5 h-5" />
                                            <span className="font-medium">{successMessage}</span>
                                        </div>
                                    </div>
                                )}

                                {error && (
                                    <div className="p-4 bg-red-50 text-red-700 rounded-lg border border-red-100">
                                        {error}
                                    </div>
                                )}

                                <div className="pt-4">
                                    <Button
                                        type="submit"
                                        isLoading={isSaving}
                                        className="w-full sm:w-auto"
                                    >
                                        <Save className="w-4 h-4 mr-2" />
                                        Save Changes
                                    </Button>
                                </div>
                            </form>
                        </div>

                        {/* Driver Application Section */}
                        {(user as any)?.role !== 'Driver' && (
                            <div className="bg-gradient-to-br from-slate-900 to-slate-800 rounded-xl p-6 text-white overflow-hidden relative">
                                <div className="absolute top-0 right-0 w-64 h-64 bg-cyan-500/10 rounded-full blur-3xl -translate-y-1/2 translate-x-1/2"></div>

                                <div className="relative z-10">
                                    <h3 className="text-xl font-bold mb-2">Become a Driver</h3>
                                    <p className="text-slate-300 mb-6 max-w-lg">
                                        Join our fleet of professional drivers and earn money on your own schedule.
                                        Apply now to start your journey with Spotless.
                                    </p>

                                    <DriverApplicationForm
                                        initialName={profile?.name || ''}
                                        initialEmail={profile?.email || ''}
                                        initialPhone={profile?.phone || ''}
                                        driverProfile={driverProfile}
                                        applicationStatus={applicationStatus}
                                    />
                                </div>
                            </div>
                        )}

                        {/* Security Section */}
                        <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 p-6">
                            <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-6 flex items-center gap-2">
                                <Lock className="w-5 h-5 text-cyan-500" />
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
        </DashboardLayout >
    );
};

// Internal Component for Driver Application Form
const DriverApplicationForm: React.FC<{
    initialName: string;
    initialEmail: string;
    initialPhone: string;
    driverProfile: DriverDto | null;
    applicationStatus: DriverApplicationStatus | null;
}> = ({ initialName, initialEmail, initialPhone, driverProfile, applicationStatus }) => {
    const [isExpanded, setIsExpanded] = useState(false);
    const [isLoading, setIsLoading] = useState(false);
    const [submitStatus, setSubmitStatus] = useState<'idle' | 'success' | 'error'>('idle');
    const [errorMessage, setErrorMessage] = useState('');
    const registerDriver = useAuthStore((state) => state.registerDriver);
    const switchRole = useAuthStore((state) => state.switchRole);

    const [vehicleInfo, setVehicleInfo] = useState('');

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!vehicleInfo.trim()) return;

        setIsLoading(true);
        setSubmitStatus('idle');

        try {
            await registerDriver({
                name: initialName,
                email: initialEmail,
                phone: initialPhone,
                vehicleInfo: vehicleInfo
            });
            setSubmitStatus('success');
            setIsExpanded(false);
        } catch (err: any) {
            console.error(err);
            setSubmitStatus('error');
            setErrorMessage(err.message || 'Application failed');
        } finally {
            setIsLoading(false);
        }
    };

    // Check for pending application status from backend (persisted state)
    if (applicationStatus && applicationStatus.status === 'Submitted') {
        return (
            <div className="bg-yellow-500/20 border border-yellow-500/30 rounded-lg p-4 text-yellow-200 flex items-center gap-3">
                <div className="w-8 h-8 rounded-full bg-yellow-500 flex items-center justify-center shrink-0">
                    <Clock className="w-4 h-4 text-white" />
                </div>
                <div>
                    <p className="font-semibold">Application Under Review</p>
                    <p className="text-sm opacity-80">Your application is currently being reviewed by our team.</p>
                </div>
            </div>
        );
    }

    if (driverProfile) {
        if (driverProfile.status === 'PendingApproval') {
            return (
                <div className="bg-yellow-500/20 border border-yellow-500/30 rounded-lg p-4 text-yellow-200 flex items-center gap-3">
                    <div className="w-8 h-8 rounded-full bg-yellow-500 flex items-center justify-center shrink-0">
                        <Clock className="w-4 h-4 text-white" />
                    </div>
                    <div>
                        <p className="font-semibold">Application Under Review</p>
                        <p className="text-sm opacity-80">Your application is currently being reviewed by our team.</p>
                    </div>
                </div>
            );
        }

        if (driverProfile.status === 'Rejected') {
            const rejectionDate = driverProfile.updatedAt ? new Date(driverProfile.updatedAt) : new Date();
            const now = new Date();
            const diffTime = Math.abs(now.getTime() - rejectionDate.getTime());
            const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
            const remainingDays = 30 - diffDays;

            if (remainingDays > 0) {
                return (
                    <div className="bg-red-500/20 border border-red-500/30 rounded-lg p-4 text-red-200 flex items-center gap-3">
                        <div className="w-8 h-8 rounded-full bg-red-500 flex items-center justify-center shrink-0">
                            <Lock className="w-4 h-4 text-white" />
                        </div>
                        <div>
                            <p className="font-semibold">Application Rejected</p>
                            <p className="text-sm opacity-80">
                                You can apply again in {remainingDays} days.
                            </p>
                        </div>
                    </div>
                );
            }
            // If cooldown passed, show apply button (fall through)
            // If cooldown passed, show apply button (fall through)
        } else {
            // All other statuses mean approved/active (Offline, Available, Busy, etc.)
            return (
                <div className="bg-green-500/20 border border-green-500/30 rounded-lg p-4 text-green-200 flex flex-col sm:flex-row items-start sm:items-center gap-4 justify-between">
                    <div className="flex items-center gap-3">
                        <div className="w-8 h-8 rounded-full bg-green-500 flex items-center justify-center shrink-0">
                            <CheckCircle className="w-4 h-4 text-white" />
                        </div>
                        <div>
                            <p className="font-semibold">Application Approved</p>
                            <p className="text-sm opacity-80">You are now a registered driver.</p>
                        </div>
                    </div>
                    <Button
                        onClick={() => switchRole()}
                        className="bg-green-600 hover:bg-green-500 text-white border-none whitespace-nowrap"
                    >
                        Go to Dashboard
                    </Button>
                </div>
            );
        }
    }

    if (submitStatus === 'success') {
        return (
            <div className="bg-green-500/20 border border-green-500/30 rounded-lg p-4 text-green-200 flex items-center gap-3">
                <div className="w-8 h-8 rounded-full bg-green-500 flex items-center justify-center shrink-0">
                    <Save className="w-4 h-4 text-white" />
                </div>
                <div>
                    <p className="font-semibold">Application Submitted!</p>
                    <p className="text-sm opacity-80">We will review your application and contact you shortly.</p>
                </div>
            </div>
        );
    }

    if (!isExpanded) {
        return (
            <Button
                onClick={() => setIsExpanded(true)}
                className="w-full sm:w-auto"
            >
                {applicationStatus?.status === 'Rejected' ? 'Apply Again' : 'Apply Now'}
            </Button>
        );
    }

    return (
        <form onSubmit={handleSubmit} className="bg-white/5 rounded-lg p-4 space-y-4 border border-white/10">
            <div>
                <label className="block text-sm font-medium text-slate-300 mb-1">Vehicle Information</label>
                <Input
                    placeholder="e.g. 2020 Toyota Camry - Silver"
                    value={vehicleInfo}
                    onChange={(e) => setVehicleInfo(e.target.value)}
                    className="bg-white/10 border-white/20 text-white placeholder:text-slate-400 focus:border-cyan-500 focus:ring-cyan-500/20"
                />
                <p className="text-xs text-slate-400 mt-1">Please provide Make, Model, Year, and Color.</p>
            </div>

            {submitStatus === 'error' && (
                <p className="text-red-400 text-sm">{errorMessage}</p>
            )}

            <div className="flex gap-3">
                <Button
                    type="submit"
                    isLoading={isLoading}
                    disabled={!vehicleInfo.trim()}
                    className="bg-cyan-500 hover:bg-cyan-400 text-white border-none"
                >
                    Submit Application
                </Button>
                <Button
                    type="button"
                    variant="ghost"
                    onClick={() => setIsExpanded(false)}
                    className="text-slate-300 hover:text-white hover:bg-white/10"
                >
                    Cancel
                </Button>
            </div>
        </form>
    );
};
