import React, { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Mail, ArrowLeft } from 'lucide-react';
import { Link } from 'react-router-dom';
import { AuthLayout } from '../layouts/AuthLayout';
import { Input } from '../components/ui/Input';
import { Button } from '../components/ui/Button';

const forgotPasswordSchema = z.object({
    email: z.string().email('Please enter a valid email address'),
});

type ForgotPasswordFormData = z.infer<typeof forgotPasswordSchema>;

export const ForgotPasswordPage: React.FC = () => {
    const [isSubmitted, setIsSubmitted] = useState(false);
    const [isLoading, setIsLoading] = useState(false);

    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm<ForgotPasswordFormData>({
        resolver: zodResolver(forgotPasswordSchema),
    });

    const onSubmit = async (data: ForgotPasswordFormData) => {
        setIsLoading(true);
        try {
            // Simulate API call
            await new Promise(resolve => setTimeout(resolve, 1500));
            console.log('Reset password for:', data.email);
            setIsSubmitted(true);
        } catch (error) {
            console.error('Failed to send reset email', error);
        } finally {
            setIsLoading(false);
        }
    };

    if (isSubmitted) {
        return (
            <AuthLayout
                title="Check your email"
                subtitle="We've sent password reset instructions to your email."
            >
                <div className="text-center space-y-6">
                    <div className="bg-green-50 dark:bg-green-900/20 p-4 rounded-full w-16 h-16 mx-auto flex items-center justify-center">
                        <Mail className="w-8 h-8 text-green-600 dark:text-green-400" />
                    </div>
                    <p className="text-slate-600 dark:text-slate-400">
                        If an account exists for that email, you will receive instructions shortly.
                    </p>
                    <Link to="/login">
                        <Button variant="outline" className="w-full">
                            Back to Login
                        </Button>
                    </Link>
                </div>
            </AuthLayout>
        );
    }

    return (
        <AuthLayout
            title="Reset Password"
            subtitle="Enter your email to receive reset instructions"
        >
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
                <Input
                    label="Email Address"
                    type="email"
                    placeholder="you@example.com"
                    icon={<Mail className="w-5 h-5" />}
                    error={errors.email?.message}
                    {...register('email')}
                />

                <Button type="submit" className="w-full" size="lg" isLoading={isLoading}>
                    Send Reset Link
                </Button>

                <div className="text-center">
                    <Link
                        to="/login"
                        className="inline-flex items-center text-sm font-medium text-slate-600 hover:text-slate-900 dark:text-slate-400 dark:hover:text-slate-200 transition-colors"
                    >
                        <ArrowLeft className="w-4 h-4 mr-2" />
                        Back to Login
                    </Link>
                </div>
            </form>
        </AuthLayout>
    );
};
