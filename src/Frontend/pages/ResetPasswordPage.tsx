import React, { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';
import { AuthLayout } from '../layouts/AuthLayout';
import { Button } from '../components/ui/Button';
import { Input } from '../components/ui/Input';
import { ResetPasswordCommand } from '../lib/apiClient';
import { apiClient } from '../lib/api';

const resetPasswordSchema = z.object({
    newPassword: z.string().min(6, 'Password must be at least 6 characters'),
    confirmPassword: z.string(),
}).refine((data) => data.newPassword === data.confirmPassword, {
    message: "Passwords don't match",
    path: ["confirmPassword"],
});

type ResetPasswordFormValues = z.infer<typeof resetPasswordSchema>;

export const ResetPasswordPage: React.FC = () => {
    const navigate = useNavigate();
    const [searchParams] = useSearchParams();
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState(false);

    const token = searchParams.get('token');
    const userId = searchParams.get('userId'); // Assuming userId is passed in query params

    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm<ResetPasswordFormValues>({
        resolver: zodResolver(resetPasswordSchema),
    });

    const onSubmit = async (data: ResetPasswordFormValues) => {
        if (!token || !userId) {
            setError('Invalid reset link. Please request a new one.');
            return;
        }

        setIsLoading(true);
        setError(null);

        try {
            const command = new ResetPasswordCommand({
                userId,
                token,
                newPassword: data.newPassword,
            });

            await apiClient.resetPassword(command);
            setSuccess(true);
            setTimeout(() => navigate('/login'), 3000);
        } catch (err) {
            console.error('Reset password failed', err);
            setError((err as Error).message || 'Failed to reset password. Please try again.');
        } finally {
            setIsLoading(false);
        }
    };

    if (success) {
        return (
            <AuthLayout
                title="Password Reset"
                subtitle="Your password has been successfully updated."
            >
                <div className="text-center space-y-6">
                    <div className="p-4 bg-green-50 text-green-700 rounded-lg border border-green-100">
                        Redirecting you to login...
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
            title="Reset Password"
            subtitle="Enter your new password"
        >
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
                {error && (
                    <div className="p-3 text-sm text-red-500 bg-red-50 rounded-lg border border-red-100">
                        {error}
                    </div>
                )}

                <Input
                    label="New Password"
                    type="password"
                    placeholder="••••••••"
                    error={errors.newPassword?.message}
                    {...register('newPassword')}
                />

                <Input
                    label="Confirm Password"
                    type="password"
                    placeholder="••••••••"
                    error={errors.confirmPassword?.message}
                    {...register('confirmPassword')}
                />

                <Button
                    type="submit"
                    className="w-full"
                    isLoading={isLoading}
                >
                    Reset Password
                </Button>
            </form>
        </AuthLayout>
    );
};
