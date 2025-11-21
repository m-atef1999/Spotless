import React, { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { Link } from 'react-router-dom';
import { AuthLayout } from '../../layouts/AuthLayout';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import type { DriverApplicationRequest } from '../../lib/api';
import { useAuthStore } from '../../store/authStore';

const driverApplicationSchema = z.object({
    name: z.string().min(2, 'Name must be at least 2 characters'),
    email: z.string().email('Invalid email address'),
    phone: z.string().min(10, 'Phone number must be at least 10 digits'),
    vehicleInfo: z.string().min(5, 'Vehicle information is required (e.g., Make, Model, Year)'),
});

type DriverApplicationFormValues = z.infer<typeof driverApplicationSchema>;

export const DriverApplicationPage: React.FC = () => {
    const registerDriver = useAuthStore((state) => state.registerDriver);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState(false);

    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm<DriverApplicationFormValues>({
        resolver: zodResolver(driverApplicationSchema),
    });

    const onSubmit = async (data: DriverApplicationFormValues) => {
        setIsLoading(true);
        setError(null);

        try {
            const command: DriverApplicationRequest = {
                name: data.name,
                email: data.email,
                phone: data.phone,
                vehicleInfo: data.vehicleInfo,
            };

            await registerDriver(command);
            setSuccess(true);
        } catch (err) {
            console.error('Application failed', err);
            setError((err as Error).message || 'Application failed. Please try again.');
        } finally {
            setIsLoading(false);
        }
    };

    if (success) {
        return (
            <AuthLayout
                title="Application Submitted"
                subtitle="Thank you for applying to be a Spotless driver!"
            >
                <div className="text-center space-y-6">
                    <div className="p-4 bg-green-50 text-green-700 rounded-lg border border-green-100">
                        Your application has been received. Our team will review your details and contact you shortly.
                    </div>
                    <Link to="/login">
                        <Button className="w-full">Return to Login</Button>
                    </Link>
                </div>
            </AuthLayout>
        );
    }

    return (
        <AuthLayout
            title="Become a Driver"
            subtitle="Join our fleet and earn money on your schedule"
        >
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
                {error && (
                    <div className="p-3 text-sm text-red-500 bg-red-50 rounded-lg border border-red-100">
                        {error}
                    </div>
                )}

                <Input
                    label="Full Name"
                    type="text"
                    placeholder="Jane Doe"
                    error={errors.name?.message}
                    {...register('name')}
                />

                <Input
                    label="Email"
                    type="email"
                    placeholder="jane@example.com"
                    error={errors.email?.message}
                    {...register('email')}
                />

                <Input
                    label="Phone Number"
                    type="tel"
                    placeholder="+1 (555) 000-0000"
                    error={errors.phone?.message}
                    {...register('phone')}
                />

                <Input
                    label="Vehicle Information"
                    type="text"
                    placeholder="2020 Toyota Camry - Silver"
                    error={errors.vehicleInfo?.message}
                    {...register('vehicleInfo')}
                />

                <Button
                    type="submit"
                    className="w-full"
                    isLoading={isLoading}
                >
                    Submit Application
                </Button>

                <div className="text-center text-sm text-slate-600">
                    Already have an account?{' '}
                    <Link to="/login" className="font-medium text-cyan-600 hover:text-cyan-500">
                        Sign in
                    </Link>
                </div>
            </form>
        </AuthLayout>
    );
};
