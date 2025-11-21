import React from 'react';
import { Link } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Mail, Lock } from 'lucide-react';
import { AuthLayout } from '../layouts/AuthLayout';
import { Input } from '../components/ui/Input';
import { Button } from '../components/ui/Button';
import { useAuthStore } from '../store/authStore';

import { useGoogleLogin } from '@react-oauth/google';
import { AuthService } from '../lib/services/AuthService';
import { useNavigate } from 'react-router-dom';

const loginSchema = z.object({
    email: z.string().email('Please enter a valid email address'),
    password: z.string().min(6, 'Password must be at least 6 characters'),
});

type LoginFormData = z.infer<typeof loginSchema>;

export const LoginPage: React.FC = () => {
    const { login, isLoading, error } = useAuthStore();
    const navigate = useNavigate();

    const googleLogin = useGoogleLogin({
        onSuccess: async (tokenResponse) => {
            try {
                // Send the access token to the backend
                // Note: The backend might expect 'idToken' or 'accessToken' depending on implementation.
                // The generated client expects 'idToken' in the body.
                // Google's 'useGoogleLogin' (implicit flow) returns 'access_token'.
                // If backend expects ID Token, we might need 'flow: "auth-code"' or just use the access token as id token if backend supports it.
                // However, usually 'idToken' is a JWT. 'access_token' is opaque.
                // Let's assume for now we send the access_token as the idToken, or we might need to fetch user info.
                // BUT, standard OIDC flow: useGoogleLogin with flow: 'implicit' (default) gives access_token.
                // To get ID Token, we usually use the <GoogleLogin> component or flow: 'auth-code'.
                // Let's try sending the access_token. If backend fails, we might need to adjust.

                const result = await AuthService.postApiAuthExternalGoogle({
                    requestBody: {
                        provider: 'Google',
                        idToken: tokenResponse.access_token
                    }
                });

                // Manually update auth store (we might need to expose a method or just reload)
                // Ideally useAuthStore should have an 'externalLogin' method.
                // For now, let's just reload or navigate.
                localStorage.setItem('token', result.accessToken || '');
                localStorage.setItem('refreshToken', result.refreshToken || '');
                // We need to decode token to get user info, but for now let's just redirect.
                window.location.href = '/customer/dashboard';
            } catch (err) {
                console.error('Google login failed', err);
                alert('Google login failed. Please try again.');
            }
        },
        onError: () => {
            console.error('Google Login Failed');
            alert('Google Login Failed');
        }
    });

    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm<LoginFormData>({
        resolver: zodResolver(loginSchema),
    });

    const onSubmit = async (data: LoginFormData) => {
        try {
            await login({ email: data.email, password: data.password });
            // Login successful - store handles redirect usually, or we do it here
            // The store implementation of 'login' usually sets state.
            // We should check if we need to navigate.
            navigate('/customer/dashboard');
        } catch (err) {
            console.error('Login failed', err);
        }
    };

    return (
        <AuthLayout
            title="Welcome back"
            subtitle="Sign in to your account to continue"
        >
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
                {error && (
                    <div className="p-4 rounded-xl bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800">
                        <p className="text-sm font-medium text-red-600 dark:text-red-400">
                            {error}
                        </p>
                    </div>
                )}

                <Input
                    label="Email Address"
                    type="email"
                    placeholder="you@example.com"
                    icon={<Mail className="w-5 h-5" />}
                    error={errors.email?.message}
                    {...register('email')}
                />

                <Input
                    label="Password"
                    type="password"
                    placeholder="Enter your password"
                    icon={<Lock className="w-5 h-5" />}
                    error={errors.password?.message}
                    {...register('password')}
                />

                <div className="flex items-center justify-between">
                    <label className="flex items-center gap-2 cursor-pointer group">
                        <input
                            type="checkbox"
                            className="w-4 h-4 rounded border-2 border-slate-300 text-cyan-600 focus:ring-4 focus:ring-cyan-500/20 transition-all cursor-pointer"
                        />
                        <span className="text-sm font-medium text-slate-600 dark:text-slate-400 group-hover:text-slate-900 dark:group-hover:text-slate-200 transition-colors">
                            Remember me
                        </span>
                    </label>

                    <Link
                        to="/forgot-password"
                        className="text-sm font-semibold text-cyan-600 hover:text-cyan-700 dark:text-cyan-400 dark:hover:text-cyan-300 transition-colors"
                    >
                        Forgot password?
                    </Link>
                </div>

                <Button type="submit" className="w-full" size="lg" isLoading={isLoading}>
                    Sign in
                </Button>

                <div className="relative">
                    <div className="absolute inset-0 flex items-center">
                        <div className="w-full border-t border-slate-200 dark:border-slate-700"></div>
                    </div>
                    <div className="relative flex justify-center text-sm">
                        <span className="px-2 bg-white dark:bg-slate-900 text-slate-500">Or continue with</span>
                    </div>
                </div>

                <button
                    type="button"
                    onClick={() => googleLogin()}
                    className="w-full flex items-center justify-center gap-3 px-4 py-3 border border-slate-200 dark:border-slate-700 rounded-xl text-slate-700 dark:text-slate-200 font-medium hover:bg-slate-50 dark:hover:bg-slate-800 transition-colors focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-slate-200 dark:focus:ring-slate-700"
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
                            d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z"
                            fill="#FBBC05"
                        />
                        <path
                            d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"
                            fill="#EA4335"
                        />
                    </svg>
                    Sign in with Google
                </button>

                <p className="text-center text-sm text-slate-600 dark:text-slate-400 mt-6">
                    Don't have an account?{' '}
                    <Link to="/register" className="font-semibold text-cyan-600 hover:text-cyan-700 dark:text-cyan-400 dark:hover:text-cyan-300 transition-colors">
                        Sign up for free
                    </Link>
                </p>
            </form>
        </AuthLayout>
    );
};
