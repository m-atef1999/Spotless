import { useState } from 'react';
import { PaymentsService, type PaymentMethodEnum } from '../lib/api';
import { useToast } from '../components/ui/Toast';

export const usePayment = () => {
    const [isProcessing, setIsProcessing] = useState(false);
    const { addToast } = useToast();

    const initiatePayment = async (orderId: string, paymentMethod: PaymentMethodEnum) => {
        setIsProcessing(true);
        try {
            // For cash payments, we don't need to initiate a payment flow
            if (paymentMethod === 'CashOnDelivery') {
                return { success: true, isCash: true };
            }

            const returnUrl = `${window.location.origin}/customer/orders`;

            const response = await PaymentsService.postApiPaymentsInitiate({
                requestBody: {
                    orderId,
                    paymentMethod,
                    returnUrl
                }
            });

            if (response.paymentUrl) {
                window.location.href = response.paymentUrl;
                return { success: true, isRedirecting: true };
            }

            return { success: true };
        } catch (error) {
            console.error('Payment initiation failed', error);
            addToast('Failed to initiate payment. Please try again.', 'error');
            return { success: false, error };
        } finally {
            setIsProcessing(false);
        }
    };

    return {
        initiatePayment,
        isProcessing
    };
};
