import { useState, useEffect } from 'react';
import { useToast } from '../../components/ui/Toast';

import { CustomersService } from '../../lib/api';
import type { Spotless_Application_Dtos_PaymentMethods_PaymentMethodDto } from '../../lib/models/Spotless_Application_Dtos_PaymentMethods_PaymentMethodDto';

type PaymentMethodDto = Spotless_Application_Dtos_PaymentMethods_PaymentMethodDto;

export function PaymentMethodsPage() {
    const [paymentMethods, setPaymentMethods] = useState<PaymentMethodDto[]>([]);
    const [loading, setLoading] = useState(true);
    const { addToast } = useToast();

    useEffect(() => {
        fetchPaymentMethods();
    }, []);

    const fetchPaymentMethods = async () => {
        try {
            setLoading(true);
            const response = await CustomersService.getApiCustomersPaymentMethods();
            setPaymentMethods(response);
        } catch (error) {
            addToast('Failed to load payment methods', 'error');
            console.error('Error fetching payment methods:', error);
        } finally {
            setLoading(false);
        }
    };

    const handleSetDefault = async (id: string) => {
        try {
            // TODO: Replace with actual API call
            setPaymentMethods(paymentMethods.map(pm => ({
                ...pm,
                isDefault: pm.id === id
            })));
            addToast('Default payment method updated', 'success');
        } catch {
            addToast('Failed to update default payment method', 'error');
        }
    };

    const handleDelete = async (id: string) => {
        if (!confirm('Are you sure you want to remove this payment method?')) return;

        try {
            // TODO: Replace with actual API call
            setPaymentMethods(paymentMethods.filter(pm => pm.id !== id));
            addToast('Payment method removed', 'success');
        } catch {
            addToast('Failed to remove payment method', 'error');
        }
    };

    const getCardIcon = (type: string) => {
        switch (type.toLowerCase()) {
            case 'visa':
                return '💳';
            case 'mastercard':
                return '💳';
            case 'amex':
                return '💳';
            default:
                return '💳';
        }
    };

    return (
        <div className="min-h-screen bg-gray-50 p-6">
            <div className="max-w-4xl mx-auto">
                {/* Header */}
                <div className="mb-8 flex justify-between items-center">
                    <div>
                        <h1 className="text-3xl font-bold text-gray-900">Payment Methods</h1>
                        <p className="text-gray-600 mt-2">Manage your saved payment methods</p>
                    </div>
                    <button
                        onClick={() => addToast('Add payment method feature coming soon', 'info')}
                        className="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 transition-colors"
                    >
                        + Add Payment Method
                    </button>
                </div>

                {/* Payment Methods List */}
                <div className="space-y-4">
                    {loading ? (
                        <div className="flex items-center justify-center h-64">
                            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
                        </div>
                    ) : paymentMethods.length === 0 ? (
                        <div className="bg-white rounded-lg shadow-sm p-12 text-center">
                            <p className="text-gray-500 text-lg">No payment methods saved</p>
                            <button
                                onClick={() => addToast('Add payment method feature coming soon', 'info')}
                                className="mt-4 bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 transition-colors"
                            >
                                Add Your First Payment Method
                            </button>
                        </div>
                    ) : (
                        paymentMethods.map((method) => (
                            <div
                                key={method.id}
                                className={`bg-white rounded-lg shadow-sm p-6 ${method.isDefault ? 'border-2 border-blue-600' : 'border border-gray-200'
                                    }`}
                            >
                                <div className="flex items-center justify-between">
                                    <div className="flex items-center gap-4">
                                        <div className="text-4xl">{getCardIcon(method.type || '')}</div>
                                        <div>
                                            <div className="flex items-center gap-2">
                                                <h3 className="text-lg font-semibold text-gray-900">
                                                    {method.type} •••• {method.last4Digits}
                                                </h3>
                                                {method.isDefault && (
                                                    <span className="bg-blue-100 text-blue-700 text-xs px-2 py-1 rounded-full font-medium">
                                                        Default
                                                    </span>
                                                )}
                                            </div>
                                            {method.expiryDate && (
                                                <p className="text-gray-600 text-sm mt-1">
                                                    Expires {new Date(method.expiryDate).toLocaleDateString(undefined, { month: '2-digit', year: 'numeric' })}
                                                </p>
                                            )}
                                        </div>
                                    </div>
                                    <div className="flex gap-2">
                                        {!method.isDefault && (
                                            <button
                                                onClick={() => handleSetDefault(method.id!)}
                                                className="bg-blue-100 text-blue-700 px-4 py-2 rounded-lg hover:bg-blue-200 transition-colors"
                                            >
                                                Set as Default
                                            </button>
                                        )}
                                        <button
                                            onClick={() => handleDelete(method.id!)}
                                            className="bg-red-100 text-red-700 px-4 py-2 rounded-lg hover:bg-red-200 transition-colors"
                                        >
                                            Remove
                                        </button>
                                    </div>
                                </div>
                            </div>
                        ))
                    )}
                </div>

                {/* Info Box */}
                <div className="mt-8 bg-blue-50 border border-blue-200 rounded-lg p-6">
                    <h3 className="text-lg font-semibold text-blue-900 mb-2">🔒 Secure Payment</h3>
                    <p className="text-blue-700 text-sm">
                        Your payment information is encrypted and securely stored. We never store your full card number.
                    </p>
                </div>
            </div>
        </div>
    );
}

