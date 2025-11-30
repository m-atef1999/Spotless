import React, { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { Wallet, CreditCard, Banknote, Plus, History, Loader2 } from 'lucide-react';
import { DashboardLayout } from '../../layouts/DashboardLayout';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';

import { CustomersService, type WalletTopUpRequest } from '../../lib/api';

const topUpSchema = z.object({
    amount: z.string().refine((val) => !isNaN(Number(val)) && Number(val) > 0, {
        message: 'Amount must be a positive number',
    }),
});

type TopUpFormValues = z.infer<typeof topUpSchema>;

export const WalletPage: React.FC = () => {
    const [balance, setBalance] = useState<number | null>(null);
    const [currency, setCurrency] = useState<string>('EGP');
    const [isLoading, setIsLoading] = useState(true);
    const [isProcessing, setIsProcessing] = useState(false);
    const [successMessage, setSuccessMessage] = useState<string | null>(null);
    const [error, setError] = useState<string | null>(null);

    const {
        register,
        handleSubmit,
        reset,
        formState: { errors },
    } = useForm<TopUpFormValues>({
        resolver: zodResolver(topUpSchema),
    });

    const fetchBalance = async () => {
        try {
            const profile = await CustomersService.getApiCustomersMe();
            setBalance(profile.walletBalance || 0);
            setCurrency(profile.walletCurrency || 'EGP');
        } catch (err) {
            console.error('Failed to fetch wallet balance', err);
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        fetchBalance();
    }, []);

    const onTopUp = async (data: TopUpFormValues) => {
        setIsProcessing(true);
        setError(null);
        setSuccessMessage(null);

        try {
            const request: WalletTopUpRequest = {
                amountValue: Number(data.amount),
                paymentMethod: 'CreditCard', // Default to Credit Card for now
            };

            const response = await CustomersService.postApiCustomersTopup({ requestBody: request });

            if (response && response.paymentUrl) {

                setSuccessMessage('Redirecting to payment gateway...');
                window.location.href = response.paymentUrl;
            } else {
                console.warn('No payment URL received', response);
                // Fallback for direct deposit (if backend logic changes back or for testing)
                setSuccessMessage(`Successfully added ${data.amount} EGP to your wallet.`);
                reset();
                fetchBalance(); // Refresh balance
            }
        } catch (err) {
            console.error('Top-up failed', err);
            let msg = 'Failed to process top-up. Please try again.';

            if (err) {
                if (typeof err === 'string') {
                    msg = err;
                } else if (typeof err === 'object') {
                    const errorObj = err as any;
                    if (errorObj.body && typeof errorObj.body === 'string') {
                        msg = errorObj.body;
                    } else if (errorObj.body && errorObj.body.message) {
                        msg = errorObj.body.message;
                    } else if (errorObj.message) {
                        msg = errorObj.message;
                    } else {
                        try {
                            msg += ` Server: ${JSON.stringify(err)}`;
                        } catch {
                            msg += ' (Unknown error format)';
                        }
                    }
                }
            }
            setError(msg);
        } finally {
            setIsProcessing(false);
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
                <h1 className="text-2xl font-bold text-slate-900 dark:text-white flex items-center gap-3">
                    <Wallet className="w-8 h-8 text-cyan-500" />
                    My Wallet
                </h1>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                    {/* Balance Card */}
                    <div className="bg-gradient-to-br from-cyan-500 to-blue-600 rounded-2xl p-8 text-white shadow-lg relative overflow-hidden">
                        <div className="absolute top-0 right-0 w-32 h-32 bg-white/10 rounded-full -mr-16 -mt-16 blur-2xl"></div>
                        <div className="absolute bottom-0 left-0 w-24 h-24 bg-black/10 rounded-full -ml-12 -mb-12 blur-xl"></div>

                        <div className="relative z-10">
                            <p className="text-cyan-100 font-medium mb-2">Current Balance</p>
                            <div className="text-4xl font-bold mb-6">
                                {balance?.toFixed(2)} <span className="text-lg font-normal opacity-80">{currency}</span>
                            </div>
                            <div className="flex items-center gap-2 text-sm bg-white/20 w-fit px-3 py-1 rounded-full backdrop-blur-sm">
                                <div className="w-2 h-2 bg-green-400 rounded-full animate-pulse"></div>
                                Active
                            </div>
                        </div>
                    </div>

                    {/* Top Up Form */}
                    <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 p-6">
                        <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-6 flex items-center gap-2">
                            <Plus className="w-5 h-5 text-cyan-500" />
                            Top Up Wallet
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

                        <form onSubmit={handleSubmit(onTopUp)} className="space-y-6">
                            <div>
                                <Input
                                    label="Amount"
                                    placeholder="0.00"
                                    type="number"
                                    step="0.01"
                                    min="0"
                                    icon={<Banknote className="w-5 h-5" />}
                                    error={errors.amount?.message}
                                    {...register('amount')}
                                />
                            </div>

                            <div className="p-4 bg-slate-50 dark:bg-slate-800 rounded-lg border border-slate-100 dark:border-slate-700">
                                <div className="flex items-center gap-3 mb-2">
                                    <CreditCard className="w-5 h-5 text-slate-400" />
                                    <span className="font-medium text-slate-700 dark:text-slate-300">Payment Method</span>
                                </div>
                                <p className="text-sm text-slate-500 dark:text-slate-400 ml-8">
                                    Using default payment method (Credit Card)
                                </p>
                            </div>

                            <Button
                                type="submit"
                                isLoading={isProcessing}
                                className="w-full"
                            >
                                Add Funds
                            </Button>
                        </form>
                    </div>
                </div>

                {/* Recent Transactions */}
                <div className="bg-white dark:bg-slate-900 rounded-xl border border-slate-200 dark:border-slate-800 p-6">
                    <h3 className="text-lg font-semibold text-slate-900 dark:text-white mb-6 flex items-center gap-2">
                        <History className="w-5 h-5 text-cyan-500" />
                        Recent Transactions
                    </h3>

                    <div className="space-y-4">
                        {[
                            { id: 1, date: '2023-10-25', description: 'Top Up - Credit Card', amount: 50.00, type: 'credit' },
                            { id: 2, date: '2023-10-20', description: 'Service Payment - Order #1234', amount: -85.00, type: 'debit' },
                            { id: 3, date: '2023-10-15', description: 'Top Up - Credit Card', amount: 100.00, type: 'credit' },
                        ].map((tx) => (
                            <div key={tx.id} className="flex items-center justify-between p-4 rounded-lg bg-slate-50 dark:bg-slate-800/50">
                                <div className="flex items-center gap-4">
                                    <div className={`p-2 rounded-full ${tx.type === 'credit' ? 'bg-green-100 text-green-600' : 'bg-red-100 text-red-600'}`}>
                                        {tx.type === 'credit' ? <Plus className="w-4 h-4" /> : <Banknote className="w-4 h-4" />}
                                    </div>
                                    <div>
                                        <p className="font-medium text-slate-900 dark:text-white">{tx.description}</p>
                                        <p className="text-sm text-slate-500 dark:text-slate-400">{tx.date}</p>
                                    </div>
                                </div>
                                <div className={`font-bold ${tx.type === 'credit' ? 'text-green-600' : 'text-slate-900 dark:text-white'}`}>
                                    {tx.type === 'credit' ? '+' : ''}{Math.abs(tx.amount).toFixed(2)} EGP
                                </div>
                            </div>
                        ))}
                    </div>
                </div>
            </div>
        </DashboardLayout>
    );
};
