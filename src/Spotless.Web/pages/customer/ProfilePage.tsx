import React, { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { User, Mail, Phone, MapPin, Save, Loader2, Wallet, Lock } from 'lucide-react';
import { DashboardLayout } from '../../layouts/DashboardLayout';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';

import { CustomersService, type CustomerUpdateRequest, type CustomerDto } from '../../lib/api';
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

export const ProfilePage: React.FC = () => {
    const [isLoading, setIsLoading] = useState(true);
    const [isSaving, setIsSaving] = useState(false);
    const [profile, setProfile] = useState<CustomerDto | null>(null);
    const [successMessage, setSuccessMessage] = useState<string | null>(null);
    const [error, setError] = useState<string | null>(null);
    const user = useAuthStore((state) => state.user);

    const {
        register,
        handleSubmit,
        setValue,
        formState: { errors },
    } = useForm<ProfileFormValues>({
        resolver: zodResolver(profileSchema),
    });

    useEffect(() => {
        const fetchProfile = async () => {
            try {
                const data = await CustomersService.getApiCustomersMe();
                setProfile(data);

                // Pre-fill form
                setValue('name', data.name || '');
                setValue('phone', data.phone || '');
                setValue('street', data.street || '');
                setValue('city', data.city || '');
                setValue('country', data.country || '');
                setValue('zipCode', data.zipCode || '');
            } catch (err) {
                console.error('Failed to fetch profile', err);
                setError('Failed to load profile data.');
            } finally {
                setIsLoading(false);
            }
        };

        fetchProfile();
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
                <h1 className="text-2xl font-bold text-slate-900 dark:text-white">Profile & Settings</h1>

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
                                Balance: ${profile?.walletBalance?.toFixed(2) || '0.00'}
                            </div>
                        </div>
                    </div>

                    {/* Main Form */}
                    <div className="md:col-span-2">
                        <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 p-6">
                            <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-6">
                                Personal Information
                            </h3>

                            {successMessage && (
                                <div className="mb-6 p-4 bg-green-50 text-green-700 rounded-lg border border-green-100">
                                    {successMessage}
                                </div>
                            )}

                            {error && (
                                <div className="mb-6 p-4 bg-red-50 text-red-700 rounded-lg border border-red-100">
                                    {error}
                                </div>
                            )}

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

                        {/* Security Section */}
                        <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 p-6 mt-8">
                            <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-6 flex items-center gap-2">
                                <Lock className="w-5 h-5 text-cyan-500" />
                                Security
                            </h3>

                            <div className="space-y-4">
                                <div className="grid grid-cols-1 sm:grid-cols-2 gap-6">
                                    <div className="sm:col-span-2">
                                        <Input
                                            label="Current Password"
                                            type="password"
                                            placeholder="••••••••"
                                        />
                                    </div>
                                    <div>
                                        <Input
                                            label="New Password"
                                            type="password"
                                            placeholder="••••••••"
                                        />
                                    </div>
                                    <div>
                                        <Input
                                            label="Confirm New Password"
                                            type="password"
                                            placeholder="••••••••"
                                        />
                                    </div>
                                </div>
                                <div className="pt-2">
                                    <Button
                                        variant="outline"
                                        onClick={() => alert('Password update functionality is currently mocked.')}
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
