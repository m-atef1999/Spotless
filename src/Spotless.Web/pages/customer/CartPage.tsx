import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { CartsService, type CartCheckoutRequest, type PaymentMethodEnum } from '../../lib/api';
import { useToast } from '../../components/ui/Toast';
import { Trash2, ShoppingCart, CreditCard, Calendar, MapPin, Banknote } from 'lucide-react';

interface CartItem {
    serviceId: string;
    serviceName: string;
    quantity: number;
    unitPrice: number;
    totalPrice: number;
}

interface Cart {
    customerId: string;
    items: CartItem[];
    totalAmount: number;
}

export const CartPage: React.FC = () => {
    const [cart, setCart] = useState<Cart | null>(null);
    const [loading, setLoading] = useState(true);
    const [checkingOut, setCheckingOut] = useState(false);
    const { addToast } = useToast();
    const navigate = useNavigate();

    // Checkout form state
    const [scheduledDate, setScheduledDate] = useState('');
    const [timeSlotId, setTimeSlotId] = useState<string>('1'); // Default to first slot
    const [paymentMethod, setPaymentMethod] = useState<PaymentMethodEnum>('CashOnDelivery');
    const [address, setAddress] = useState('');

    useEffect(() => {
        fetchCart();
    }, []);

    const fetchCart = async () => {
        try {
            setLoading(true);
            const data = await CartsService.getApiCustomersCart();
            setCart(data);
        } catch (error) {
            console.error('Failed to fetch cart:', error);
            // Don't show error toast on 404 (empty cart)
        } finally {
            setLoading(false);
        }
    };

    const handleRemoveItem = async (serviceId: string) => {
        try {
            await CartsService.deleteApiCustomersCartItems({ serviceId });
            addToast('Item removed from cart', 'success');
            fetchCart();
        } catch {
            addToast('Failed to remove item', 'error');
        }
    };

    const handleClearCart = async () => {
        if (!confirm('Are you sure you want to clear your cart?')) return;

        try {
            await CartsService.deleteApiCustomersCart();
            setCart(null);
            addToast('Cart cleared', 'success');
        } catch {
            addToast('Failed to clear cart', 'error');
        }
    };

    const handleCheckout = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!cart || cart.items.length === 0) return;

        try {
            setCheckingOut(true);
            const checkoutRequest: CartCheckoutRequest = {
                scheduledDate: new Date(scheduledDate).toISOString(),
                timeSlotId,
                paymentMethod,
                pickupAddress: address,
                deliveryAddress: address, // Using same address for simplicity for now
                pickupLatitude: 0, // Mock coordinates
                pickupLongitude: 0,
                deliveryLatitude: 0,
                deliveryLongitude: 0
            };

            const response = await CartsService.postApiCustomersCartCheckout({
                requestBody: checkoutRequest
            });

            addToast('Order placed successfully!', 'success');
            navigate(`/customer/orders/${response.orderId}`);
        } catch (error) {
            console.error('Checkout failed:', error);
            addToast('Checkout failed. Please try again.', 'error');
        } finally {
            setCheckingOut(false);
        }
    };

    if (loading) {
        return (
            <div className="flex justify-center items-center min-h-screen">
                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
            </div>
        );
    }

    if (!cart || cart.items.length === 0) {
        return (
            <div className="max-w-4xl mx-auto p-6 text-center">
                <div className="bg-white rounded-xl shadow-sm p-12">
                    <div className="w-24 h-24 bg-blue-50 rounded-full flex items-center justify-center mx-auto mb-6">
                        <ShoppingCart className="w-12 h-12 text-blue-500" />
                    </div>
                    <h2 className="text-2xl font-bold text-gray-900 mb-2">Your cart is empty</h2>
                    <p className="text-gray-600 mb-8">Looks like you haven't added any services yet.</p>
                    <button
                        onClick={() => navigate('/customer/services')}
                        className="bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 transition-colors font-medium"
                    >
                        Browse Services
                    </button>
                </div>
            </div>
        );
    }

    return (
        <div className="max-w-6xl mx-auto p-6">
            <h1 className="text-2xl font-bold text-gray-900 mb-8 flex items-center gap-3">
                <ShoppingCart />
                Shopping Cart
            </h1>

            <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
                {/* Cart Items */}
                <div className="lg:col-span-2 space-y-4">
                    <div className="bg-white rounded-xl shadow-sm overflow-hidden">
                        <div className="p-6 border-b border-gray-100 flex justify-between items-center">
                            <h2 className="font-semibold text-gray-900">Items ({cart.items.length})</h2>
                            <button
                                onClick={handleClearCart}
                                className="text-red-600 hover:text-red-700 text-sm font-medium flex items-center gap-2"
                            >
                                <Trash2 size={16} />
                                Clear Cart
                            </button>
                        </div>
                        <div className="divide-y divide-gray-100">
                            {cart.items.map((item) => (
                                <div key={item.serviceId} className="p-6 flex items-center justify-between hover:bg-gray-50 transition-colors">
                                    <div className="flex-1">
                                        <h3 className="font-medium text-gray-900">{item.serviceName}</h3>
                                        <p className="text-sm text-gray-500 mt-1">
                                            {item.quantity} x {item.unitPrice} EGP
                                        </p>
                                    </div>
                                    <div className="flex items-center gap-6">
                                        <span className="font-semibold text-gray-900">{item.totalPrice} EGP</span>
                                        <button
                                            onClick={() => handleRemoveItem(item.serviceId)}
                                            className="p-2 text-gray-400 hover:text-red-600 transition-colors rounded-full hover:bg-red-50"
                                            title="Remove item"
                                        >
                                            <Trash2 size={18} />
                                        </button>
                                    </div>
                                </div>
                            ))}
                        </div>
                        <div className="p-6 bg-gray-50 border-t border-gray-100 flex justify-between items-center">
                            <span className="font-medium text-gray-900">Total Amount</span>
                            <span className="text-2xl font-bold text-blue-600">{cart.totalAmount} EGP</span>
                        </div>
                    </div>
                </div>

                {/* Checkout Form */}
                <div className="lg:col-span-1">
                    <div className="bg-white rounded-xl shadow-sm p-6 sticky top-6">
                        <h2 className="text-lg font-semibold text-gray-900 mb-6">Checkout Details</h2>
                        <form onSubmit={handleCheckout} className="space-y-4">
                            <div>
                                <label className="block text-sm font-medium text-gray-700 mb-1">
                                    Pickup & Delivery Address
                                </label>
                                <div className="relative">
                                    <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                                        <MapPin className="text-gray-400" size={18} />
                                    </div>
                                    <input
                                        type="text"
                                        required
                                        value={address}
                                        onChange={(e) => setAddress(e.target.value)}
                                        className="pl-10 w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                                        placeholder="Enter your address"
                                    />
                                </div>
                            </div>

                            <div>
                                <label className="block text-sm font-medium text-gray-700 mb-1">
                                    Scheduled Date
                                </label>
                                <div className="relative">
                                    <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                                        <Calendar className="text-gray-400" size={18} />
                                    </div>
                                    <input
                                        type="date"
                                        required
                                        value={scheduledDate}
                                        onChange={(e) => setScheduledDate(e.target.value)}
                                        min={new Date().toISOString().split('T')[0]}
                                        className="pl-10 w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                                    />
                                </div>
                            </div>

                            <div>
                                <label className="block text-sm font-medium text-gray-700 mb-1">
                                    Time Slot
                                </label>
                                <select
                                    value={timeSlotId}
                                    onChange={(e) => setTimeSlotId(e.target.value)}
                                    className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                                >
                                    <option value="1">Morning (9 AM - 12 PM)</option>
                                    <option value="2">Afternoon (12 PM - 4 PM)</option>
                                    <option value="3">Evening (4 PM - 8 PM)</option>
                                </select>
                            </div>

                            <div>
                                <label className="block text-sm font-medium text-gray-700 mb-1">
                                    Payment Method
                                </label>
                                <div className="grid grid-cols-2 gap-3">
                                    <button
                                        type="button"
                                        onClick={() => setPaymentMethod('CashOnDelivery')}
                                        className={`p-3 border rounded-lg flex flex-col items-center gap-2 transition-colors ${paymentMethod === 'CashOnDelivery'
                                                ? 'border-blue-600 bg-blue-50 text-blue-700'
                                                : 'border-gray-200 hover:border-blue-300'
                                            }`}
                                    >
                                        <Banknote size={24} />
                                        <span className="font-medium">Cash</span>
                                    </button>
                                    <button
                                        type="button"
                                        onClick={() => setPaymentMethod('CreditCard')}
                                        className={`p-3 border rounded-lg flex flex-col items-center gap-2 transition-colors ${paymentMethod === 'CreditCard'
                                                ? 'border-blue-600 bg-blue-50 text-blue-700'
                                                : 'border-gray-200 hover:border-blue-300'
                                            }`}
                                    >
                                        <CreditCard size={24} />
                                        <span className="font-medium">Card</span>
                                    </button>
                                </div>
                            </div>

                            <button
                                type="submit"
                                disabled={checkingOut}
                                className="w-full bg-blue-600 text-white py-3 rounded-lg hover:bg-blue-700 transition-colors font-medium disabled:opacity-50 disabled:cursor-not-allowed mt-6"
                            >
                                {checkingOut ? 'Processing...' : `Checkout • ${cart.totalAmount} EGP`}
                            </button>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );
};
