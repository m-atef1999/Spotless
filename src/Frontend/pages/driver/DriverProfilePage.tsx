import React, { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { User, Mail, Phone, Car, Save, Loader2 } from 'lucide-react';
import { DashboardLayout } from '../../layouts/DashboardLayout';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { apiClient } from '../../lib/api';
import { DriverUpdateRequest, DriverDto } from '../../lib/apiClient';
import { useAuthStore } from '../../store/authStore';

const driverProfileSchema = z.object({
    name: z.string().min(2, 'Name must be at least 2 characters'),
    phone: z.string().min(10, 'Phone number must be at least 10 digits'),
    vehicleInfo: z.string().min(5, 'Vehicle info is required'),
});

type DriverProfileFormValues = z.infer<typeof driverProfileSchema>;

export const DriverProfilePage: React.FC = () => {
    const [isLoading, setIsLoading] = useState(true);
    const [isSaving, setIsSaving] = useState(false);
    const [profile, setProfile] = useState<DriverDto | null>(null);
    const [successMessage, setSuccessMessage] = useState<string | null>(null);
    const [error, setError] = useState<string | null>(null);
    const user = useAuthStore((state) => state.user);

    const {
        register,
        handleSubmit,
        setValue,
        formState: { errors },
    } = useForm<DriverProfileFormValues>({
        resolver: zodResolver(driverProfileSchema),
    });

    useEffect(() => {
        const fetchProfile = async () => {
            try {
                const data = await apiClient.profileGET();
                setProfile(data);

                // Pre-fill form
                setValue('name', data.name || '');
                setValue('phone', data.phone || '');
                setValue('vehicleInfo', data.vehicleInfo || '');
            } catch (err) {
                console.error('Failed to fetch driver profile', err);
                setError('Failed to load profile data.');
            } finally {
                setIsLoading(false);
            }
        };

        fetchProfile();
    }, [setValue]);

    const onSubmit = async (data: DriverProfileFormValues) => {
        setIsSaving(true);
        setSuccessMessage(null);
        setError(null);

        try {
            const command = new DriverUpdateRequest({
                name: data.name,
                phone: data.phone,
                vehicleInfo: data.vehicleInfo,
            });

            await apiClient.profilePUT(command);
            setSuccessMessage('Profile updated successfully!');

            // Refresh profile data
            const updatedProfile = await apiClient.profileGET();
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
            <DashboardLayout role="Driver">
                <div className="flex justify-center py-12">
                    <Loader2 className="w-8 h-8 animate-spin text-cyan-500" />
                </div>
            </DashboardLayout>
        );
    }

    return (
        <DashboardLayout role="Driver">
            <div className="max-w-4xl mx-auto space-y-8">
                <h1 className="text-2xl font-bold text-slate-900 dark:text-white">Driver Profile</h1>

                <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
                    {/* Sidebar / Summary */}
                    <div className="space-y-6">
                        <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 p-6 text-center">
                            <div className="w-24 h-24 bg-cyan-100 dark:bg-cyan-900/30 rounded-full flex items-center justify-center mx-auto mb-4 text-cyan-600 dark:text-cyan-400 text-2xl font-bold">
                                {profile?.name?.charAt(0).toUpperCase() || user?.email?.charAt(0).toUpperCase()}
                            </div>
                            <h2 className="text-lg font-semibold text-slate-900 dark:text-white">
                                {profile?.name || 'Driver'}
                            </h2>
                            <p className="text-slate-500 dark:text-slate-400 text-sm mb-4">
                                {profile?.email || user?.email}
                            </p>
                            <div className="inline-flex items-center px-3 py-1 rounded-full text-xs font-medium bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400">
                                {profile?.status || 'Active'}
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
                                <div className="grid grid-cols-1 gap-6">
                                    <div>
                                        <Input
                                            label="Full Name"
                                            icon={<User className="w-5 h-5" />}
                                            error={errors.name?.message}
                                            {...register('name')}
                                        />
                                    </div>

                                    <div>
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

                                    <div>
                                        <Input
                                            label="Phone Number"
                                            icon={<Phone className="w-5 h-5" />}
                                            error={errors.phone?.message}
                                            {...register('phone')}
                                        />
                                    </div>

                                    <div>
                                        <Input
                                            label="Vehicle Information"
                                            icon={<Car className="w-5 h-5" />}
                                            error={errors.vehicleInfo?.message}
                                            {...register('vehicleInfo')}
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
                    </div>
                </div>
            </div>
        </DashboardLayout>
    );
};
