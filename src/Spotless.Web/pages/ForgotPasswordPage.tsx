import React, { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { Link } from 'react-router-dom';
import { AuthLayout } from '../layouts/AuthLayout';
import { Button } from '../components/ui/Button';
import { Input } from '../components/ui/Input';
import { AuthService } from '../lib/api';

const forgotPasswordSchema = z.object({
    email: z.string().email('Invalid email address'),
});

type ForgotPasswordFormValues = z.infer<typeof forgotPasswordSchema>;

export const ForgotPasswordPage: React.FC = () => {
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState(false);

    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm<ForgotPasswordFormValues>({
        resolver: zodResolver(forgotPasswordSchema),
    });

    const onSubmit = async (data: ForgotPasswordFormValues) => {
        setIsLoading(true);
        setError(null);

        try {
            await AuthService.postApiAuthForgotPassword({ requestBody: { email: data.email } });
            setSuccess(true);
        } catch (err) {
            console.error('Forgot password failed', err);
            // Don't reveal if email exists or not for security, but for now we might show error
            // Or just show success anyway
            setSuccess(true);
        } finally {
            setIsLoading(false);
        }
    };

    if (success) {
        return (
            <AuthLayout
                title="Check your inbox"
                subtitle="We've sent you instructions to reset your password."
            >
                <div className="text-center space-y-6">
                    <div className="p-4 bg-green-50 text-green-700 rounded-lg border border-green-100">
                        If an account exists for that email, you will receive a password reset link shortly.
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
            title="Forgot Password"
            subtitle="Enter your email to reset your password"
        >
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
                {error && (
                    <div className="p-3 text-sm text-red-500 bg-red-50 rounded-lg border border-red-100">
                        {error}
                    </div>
                )}

                <Input
                    label="Email"
                    type="email"
                    placeholder="john@example.com"
                    error={errors.email?.message}
                    {...register('email')}
                />

                <Button
                    type="submit"
                    className="w-full"
                    isLoading={isLoading}
                >
                    Send Reset Link
                </Button>

                <div className="text-center text-sm text-slate-600">
                    Remember your password?{' '}
                    <Link to="/login" className="font-medium text-cyan-600 hover:text-cyan-500">
                        Sign in
                    </Link>
                </div>
            </form>
        </AuthLayout>
    );
};
