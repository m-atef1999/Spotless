import React, { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { Link, useNavigate } from 'react-router-dom';
import { useGoogleLogin } from '@react-oauth/google';
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
    const [showVerification, setShowVerification] = useState(false);

    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm<RegisterFormValues>({
        resolver: zodResolver(registerSchema),
    });

    const googleLogin = useGoogleLogin({
        onSuccess: async (tokenResponse) => {
            try {
                // Use the store action to handle Google Login
                await useAuthStore.getState().loginWithGoogle(tokenResponse.access_token);
                navigate('/customer/dashboard');
            } catch (err: any) {
                console.error('Google login failed', err);
                setError('Google login failed. Please try again.');
            }
        },
        onError: () => {
            setError('Google login failed. Please try again.');
        }
    });

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
            <div className="space-y-4">
                <Button
                    type="button"
                    variant="outline"
                    className="w-full flex items-center justify-center gap-2 h-11 text-slate-700 dark:text-slate-200 border-slate-300 dark:border-slate-700 hover:bg-slate-50 dark:hover:bg-slate-800"
                    onClick={() => googleLogin()}
                >
                    <svg className="w-5 h-5" viewBox="0 0 24 24">
                        <path
                            d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"
                            fill="#4285F4"
                        />
                        <path
                            d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"
                            fill="#34A853"
                        />
                        <path
                            d="M5.84 14.11c-.22-.66-.35-1.36-.35-2.11s.13-1.45.35-2.11V7.05H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.95l3.66-2.84z"
                            fill="#FBBC05"
                        />
                        <path
                            d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.05l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"
                            fill="#EA4335"
                        />
                    </svg>
                    Sign up with Google
                </Button>

                <div className="relative">
                    <div className="absolute inset-0 flex items-center">
                        <span className="w-full border-t border-slate-300 dark:border-slate-700" />
                    </div>
                    <div className="relative flex justify-center text-xs uppercase">
                        <span className="bg-white dark:bg-slate-900 px-2 text-slate-500">Or continue with email</span>
                    </div>
                </div>

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
            </div>
        </AuthLayout>
    );
};
