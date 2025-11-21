import React, { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { Link, useNavigate } from 'react-router-dom';
import { AuthLayout } from '../../layouts/AuthLayout';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { type RegisterCustomerCommand, CustomerType } from '../../lib/api';
import { useAuthStore } from '../../store/authStore';

const registerSchema = z.object({
    name: z.string().min(2, 'Name must be at least 2 characters'),
    email: z.string().email('Invalid email address'),
    password: z.string().min(6, 'Password must be at least 6 characters'),
    confirmPassword: z.string(),
    phone: z.string().min(10, 'Phone number must be at least 10 digits'),
    street: z.string().min(5, 'Street address is required'),
    city: z.string().min(2, 'City is required'),
    zipCode: z.string().min(3, 'Zip code is required'),
    country: z.string().min(2, 'Country is required'),
}).refine((data) => data.password === data.confirmPassword, {
    message: "Passwords don't match",
    path: ["confirmPassword"],
});

type RegisterFormValues = z.infer<typeof registerSchema>;

export const RegisterPage: React.FC = () => {
    const navigate = useNavigate();
    const registerCustomer = useAuthStore((state) => state.registerCustomer);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm<RegisterFormValues>({
        resolver: zodResolver(registerSchema),
    });

    const [showVerification, setShowVerification] = useState(false);

    const onSubmit = async (data: RegisterFormValues) => {
        setIsLoading(true);
        setError(null);

        try {
            const command: RegisterCustomerCommand = {
                name: data.name,
                email: data.email,
                password: data.password,
                phone: data.phone,
                street: data.street,
                city: data.city,
                zipCode: data.zipCode,
                country: data.country,
                type: CustomerType.Regular, // Default to Regular (0)
            };

            await registerCustomer(command);
            setShowVerification(true); // Show verification modal instead of immediate navigation
        } catch (err) {
            console.error('Registration failed', err);
            setError((err as Error).message || 'Registration failed. Please try again.');
        } finally {
            setIsLoading(false);
        }
    };

    if (showVerification) {
        return (
            <AuthLayout
                title="Verify your Email"
                subtitle="We've sent a code to your email address"
            >
                <div className="text-center space-y-6">
                    <div className="bg-cyan-50 dark:bg-cyan-900/20 p-4 rounded-full w-16 h-16 mx-auto flex items-center justify-center">
                        <svg className="w-8 h-8 text-cyan-600 dark:text-cyan-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
                        </svg>
                    </div>

                    <p className="text-slate-600 dark:text-slate-400">
                        Please check your inbox and enter the verification code below to activate your account.
                    </p>

                    <div className="space-y-4">
                        <Input
                            placeholder="Enter 6-digit code"
                            className="text-center text-2xl tracking-widest"
                            maxLength={6}
                        />

                        <Button
                            className="w-full"
                            onClick={() => {
                                alert('Email verified successfully (Mock)');
                                navigate('/customer/dashboard');
                            }}
                        >
                            Verify Email
                        </Button>
                    </div>

                    <p className="text-sm text-slate-500">
                        Didn't receive the code?{' '}
                        <button className="text-cyan-600 font-medium hover:underline">
                            Resend
                        </button>
                    </p>
                </div>
            </AuthLayout>
        );
    }

    return (
        <AuthLayout
            title="Create an Account"
            subtitle="Join Spotless for premium cleaning services"
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
                    placeholder="John Doe"
                    error={errors.name?.message}
                    {...register('name')}
                />

                <Input
                    label="Email"
                    type="email"
                    placeholder="john@example.com"
                    error={errors.email?.message}
                    {...register('email')}
                />

                <div className="grid grid-cols-2 gap-4">
                    <Input
                        label="Password"
                        type="password"
                        placeholder="••••••••"
                        error={errors.password?.message}
                        {...register('password')}
                    />

                    <Input
                        label="Confirm Password"
                        type="password"
                        placeholder="••••••••"
                        error={errors.confirmPassword?.message}
                        {...register('confirmPassword')}
                    />
                </div>

                <Input
                    label="Phone Number"
                    type="tel"
                    placeholder="+1 (555) 000-0000"
                    error={errors.phone?.message}
                    {...register('phone')}
                />

                <Input
                    label="Street Address"
                    type="text"
                    placeholder="123 Main St"
                    error={errors.street?.message}
                    {...register('street')}
                />

                <div className="grid grid-cols-3 gap-4">
                    <Input
                        label="City"
                        type="text"
                        placeholder="New York"
                        error={errors.city?.message}
                        {...register('city')}
                    />
                    <Input
                        label="Zip Code"
                        type="text"
                        placeholder="10001"
                        error={errors.zipCode?.message}
                        {...register('zipCode')}
                    />
                    <Input
                        label="Country"
                        type="text"
                        placeholder="USA"
                        error={errors.country?.message}
                        {...register('country')}
                    />
                </div>

                <Button
                    type="submit"
                    className="w-full"
                    isLoading={isLoading}
                >
                    Create Account
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
