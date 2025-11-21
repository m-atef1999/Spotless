import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';
import { Check, Calendar, MapPin, CreditCard, ShoppingBag, Loader2 } from 'lucide-react';
import { DashboardLayout } from '../../layouts/DashboardLayout';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import {
    ServicesService,
    OrdersService,
    type ServiceDto,
    type PaymentMethodEnum,
    type CreateOrderDto,
    type CreateOrderItemDto
} from '../../lib/api';
import { MapContainer, TileLayer, Marker, useMapEvents } from 'react-leaflet';
import type { LeafletMouseEvent } from 'leaflet';
import 'leaflet/dist/leaflet.css';
import { getServiceImage } from '../../lib/imageUtils';
import { usePayment } from '../../hooks/usePayment';
import { useToast } from '../../components/ui/Toast';

const STEPS = [
    { id: 1, title: 'Select Services', icon: ShoppingBag },
    { id: 2, title: 'Schedule & Location', icon: Calendar },
    { id: 3, title: 'Review & Pay', icon: CreditCard },
];

// Local constant for payment methods since the API exports a type, not a value
const PaymentMethods = {
    CreditCard: 'CreditCard' as PaymentMethodEnum,
    DebitCard: 'DebitCard' as PaymentMethodEnum,
    Fawry: 'Fawry' as PaymentMethodEnum,
    PayPal: 'PayPal' as PaymentMethodEnum,
    Wallet: 'Wallet' as PaymentMethodEnum,
    CashOnDelivery: 'CashOnDelivery' as PaymentMethodEnum,
    Paymob: 'Paymob' as PaymentMethodEnum,
    Instapay: 'Instapay' as PaymentMethodEnum,
};

export const NewOrderPage: React.FC = () => {
    const navigate = useNavigate();
    const { initiatePayment, isProcessing: isPaymentProcessing } = usePayment();
    const { addToast } = useToast();
    const [currentStep, setCurrentStep] = useState(1);
    const [isLoading, setIsLoading] = useState(false);
    const [services, setServices] = useState<ServiceDto[]>([]);
    const [selectedServices, setSelectedServices] = useState<{ service: ServiceDto; quantity: number }[]>([]);

    // Form State
    const [scheduledDate, setScheduledDate] = useState('');
    const [address, setAddress] = useState('');
    const [paymentMethod, setPaymentMethod] = useState<PaymentMethodEnum>(PaymentMethods.CreditCard);
    const [position, setPosition] = useState<[number, number] | null>(null);

    useEffect(() => {
        const fetchServices = async () => {
            try {
                const response = await ServicesService.getApiServices({ pageNumber: 1, pageSize: 50 });
                setServices(response.data || []);
            } catch (error) {
                console.error('Failed to fetch services', error);
            }
        };
        fetchServices();
    }, []);

    // Fix for Leaflet icon issue
    useEffect(() => {
        if (typeof window !== 'undefined') {
            import('leaflet').then(L => {
                // eslint-disable-next-line @typescript-eslint/no-explicit-any
                delete (L.Icon.Default.prototype as any)._getIconUrl;
                L.Icon.Default.mergeOptions({
                    iconRetinaUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-icon-2x.png',
                    iconUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-icon.png',
                    shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-shadow.png',
                });
            });
        }
    }, []);

    const handleServiceToggle = (service: ServiceDto, quantity: number) => {
        setSelectedServices(prev => {
            const existing = prev.find(s => s.service.id === service.id);
            if (existing) {
                if (quantity <= 0) {
                    return prev.filter(s => s.service.id !== service.id);
                }
                return prev.map(s => s.service.id === service.id ? { ...s, quantity } : s);
            }
            return [...prev, { service, quantity }];
        });
    };

    const calculateTotal = () => {
        return selectedServices.reduce((acc, item) => acc + (item.service.basePrice || 0) * item.quantity, 0);
    };

    const handleSubmit = async () => {
        setIsLoading(true);
        try {
            const orderItems: CreateOrderItemDto[] = selectedServices.map(s => ({
                serviceId: s.service.id,
                quantity: s.quantity
            }));

            const order: CreateOrderDto = {
                scheduledDate: new Date(scheduledDate).toISOString(),
                paymentMethod: paymentMethod,
                // Mock coordinates for now since we don't have a map picker yet
                pickupLatitude: position ? position[0] : 0,
                pickupLongitude: position ? position[1] : 0,
                deliveryLatitude: 0,
                deliveryLongitude: 0,
                items: orderItems,
            };

            const response = await OrdersService.postApiOrders({ requestBody: order });

            if (response.id) {
                const paymentResult = await initiatePayment(response.id, paymentMethod);

                if (paymentResult.success) {
                    if (!paymentResult.isRedirecting) {
                        addToast('Order placed successfully!', 'success');
                        navigate('/customer/dashboard');
                    }
                    // If redirecting, initiatePayment handles the window.location change
                }
            }
        } catch (error) {
            console.error('Failed to create order', error);
            addToast('Failed to create order. Please try again.', 'error');
        } finally {
            setIsLoading(false);
        }
    };

    const MapEvents = () => {
        useMapEvents({
            click(e: LeafletMouseEvent) {
                setPosition([e.latlng.lat, e.latlng.lng]);
                setAddress(`Lat: ${e.latlng.lat.toFixed(4)}, Lng: ${e.latlng.lng.toFixed(4)} (Mock Address)`);
            },
        });
        return null;
    };

    const renderStep1 = () => (
        <div className="space-y-6">
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                {services.map((service) => {
                    const selected = selectedServices.find(s => s.service.id === service.id);
                    const quantity = selected?.quantity || 0;

                    return (
                        <div
                            key={service.id}
                            className={`group relative overflow-hidden rounded-xl border transition-all cursor-pointer ${quantity > 0
                                ? 'border-cyan-500 ring-1 ring-cyan-500'
                                : 'border-slate-200 dark:border-slate-700 hover:border-cyan-300 dark:hover:border-cyan-700'
                                }`}
                            onClick={() => !quantity && handleServiceToggle(service, 1)}
                        >
                            <div className="aspect-video w-full overflow-hidden">
                                <img
                                    src={getServiceImage(service.name || '', service.categoryId)}
                                    alt={service.name || ''}
                                    className="h-full w-full object-cover transition-transform duration-500 group-hover:scale-110"
                                />
                                <div className="absolute inset-0 bg-gradient-to-t from-black/80 via-black/40 to-transparent" />
                            </div>

                            <div className="absolute bottom-0 left-0 right-0 p-4 text-white">
                                <div className="flex justify-between items-end mb-1">
                                    <h3 className="font-bold text-lg leading-tight">{service.name}</h3>
                                    <span className="font-bold text-cyan-400 bg-black/30 backdrop-blur-sm px-2 py-1 rounded-lg">
                                        ${service.basePrice?.toFixed(2)}
                                    </span>
                                </div>
                                <p className="text-sm text-slate-200 line-clamp-2 opacity-90">
                                    {service.description}
                                </p>
                            </div>

                            {quantity > 0 && (
                                <div className="absolute top-3 right-3 flex items-center gap-2 bg-white/90 dark:bg-slate-900/90 backdrop-blur-sm p-1.5 rounded-lg shadow-lg" onClick={e => e.stopPropagation()}>
                                    <button
                                        className="w-7 h-7 rounded-md bg-slate-100 dark:bg-slate-800 text-slate-600 dark:text-slate-300 flex items-center justify-center hover:bg-slate-200 dark:hover:bg-slate-700 transition-colors"
                                        onClick={() => handleServiceToggle(service, quantity - 1)}
                                    >
                                        -
                                    </button>
                                    <span className="font-bold w-6 text-center text-slate-900 dark:text-white">{quantity}</span>
                                    <button
                                        className="w-7 h-7 rounded-md bg-cyan-500 text-white flex items-center justify-center hover:bg-cyan-600 transition-colors"
                                        onClick={() => handleServiceToggle(service, quantity + 1)}
                                    >
                                        +
                                    </button>
                                </div>
                            )}
                        </div>
                    );
                })}
            </div>
        </div>
    );

    const renderStep2 = () => (
        <div className="space-y-6 max-w-md mx-auto">
            <div className="space-y-4">
                <label className="block text-sm font-medium text-slate-700 dark:text-slate-300">
                    When should we pick up?
                </label>
                <Input
                    type="datetime-local"
                    value={scheduledDate}
                    onChange={(e) => setScheduledDate(e.target.value)}
                    className="w-full"
                />
            </div>

            <div className="space-y-4">
                <label className="block text-sm font-medium text-slate-700 dark:text-slate-300">
                    Where should we pick up?
                </label>
                <div className="h-64 w-full rounded-xl overflow-hidden border border-slate-200 dark:border-slate-700 relative z-0">
                    <MapContainer
                        center={[40.7128, -74.0060]}
                        zoom={13}
                        style={{ height: '100%', width: '100%' }}
                    >
                        <TileLayer
                            attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                            url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                        />
                        <MapEvents />
                        {position && <Marker position={position} />}
                    </MapContainer>
                </div>
                <div className="relative">
                    <MapPin className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 w-5 h-5" />
                    <Input
                        type="text"
                        placeholder="Click on map to select location"
                        value={address}
                        onChange={(e) => setAddress(e.target.value)}
                        className="pl-10 w-full"
                        readOnly
                    />
                </div>
                <p className="text-xs text-slate-500">
                    Click on the map to pin your exact location.
                </p>
            </div>
        </div>
    );

    const renderStep3 = () => (
        <div className="space-y-8 max-w-md mx-auto">
            <div className="bg-slate-50 dark:bg-slate-900/50 p-6 rounded-xl space-y-4">
                <h3 className="font-semibold text-slate-900 dark:text-white border-b border-slate-200 dark:border-slate-700 pb-2">
                    Order Summary
                </h3>
                <div className="space-y-2">
                    {selectedServices.map(({ service, quantity }) => (
                        <div key={service.id} className="flex justify-between text-sm">
                            <span className="text-slate-600 dark:text-slate-400">
                                {quantity}x {service.name}
                            </span>
                            <span className="font-medium text-slate-900 dark:text-white">
                                ${((service.basePrice || 0) * quantity).toFixed(2)}
                            </span>
                        </div>
                    ))}
                </div>
                <div className="border-t border-slate-200 dark:border-slate-700 pt-2 flex justify-between font-bold text-lg">
                    <span>Total</span>
                    <span className="text-cyan-600 dark:text-cyan-400">${calculateTotal().toFixed(2)}</span>
                </div>
            </div>

            <div className="space-y-4">
                <label className="block text-sm font-medium text-slate-700 dark:text-slate-300">
                    Payment Method
                </label>
                <div className="grid grid-cols-2 gap-4">
                    <button
                        className={`p-4 rounded-xl border flex flex-col items-center gap-2 transition-all ${paymentMethod === PaymentMethods.CreditCard
                            ? 'border-cyan-500 bg-cyan-50 dark:bg-cyan-900/20 text-cyan-700 dark:text-cyan-300'
                            : 'border-slate-200 dark:border-slate-700 hover:border-slate-300'
                            }`}
                        onClick={() => setPaymentMethod(PaymentMethods.CreditCard)}
                    >
                        <CreditCard className="w-6 h-6" />
                        <span className="text-sm font-medium">Credit Card</span>
                    </button>
                    <button
                        className={`p-4 rounded-xl border flex flex-col items-center gap-2 transition-all ${paymentMethod === PaymentMethods.CashOnDelivery
                            ? 'border-cyan-500 bg-cyan-50 dark:bg-cyan-900/20 text-cyan-700 dark:text-cyan-300'
                            : 'border-slate-200 dark:border-slate-700 hover:border-slate-300'
                            }`}
                        onClick={() => setPaymentMethod(PaymentMethods.CashOnDelivery)}
                    >
                        <CreditCard className="w-6 h-6" />
                        <span className="text-sm font-medium">Cash</span>
                    </button>
                </div>
            </div>
        </div>
    );

    return (
        <DashboardLayout role="Customer">
            <div className="max-w-4xl mx-auto">
                <h1 className="text-2xl font-bold text-slate-900 dark:text-white mb-8">
                    Create New Order
                </h1>

                {/* Progress Steps */}
                <div className="mb-12 relative">
                    <div className="absolute left-0 top-1/2 w-full h-0.5 bg-slate-200 dark:bg-slate-800 -z-10" />
                    <div className="flex justify-between">
                        {STEPS.map((step) => {
                            const isCompleted = currentStep > step.id;
                            const isCurrent = currentStep === step.id;
                            const Icon = step.icon;

                            return (
                                <div key={step.id} className="flex flex-col items-center bg-white dark:bg-slate-950 px-2">
                                    <div
                                        className={`w-10 h-10 rounded-full flex items-center justify-center transition-colors ${isCompleted || isCurrent
                                            ? 'bg-cyan-500 text-white'
                                            : 'bg-slate-100 dark:bg-slate-800 text-slate-400'
                                            }`}
                                    >
                                        {isCompleted ? <Check className="w-6 h-6" /> : <Icon className="w-5 h-5" />}
                                    </div>
                                    <span
                                        className={`mt-2 text-sm font-medium ${isCompleted || isCurrent
                                            ? 'text-cyan-600 dark:text-cyan-400'
                                            : 'text-slate-500'
                                            }`}
                                    >
                                        {step.title}
                                    </span>
                                </div>
                            );
                        })}
                    </div>
                </div>

                {/* Step Content */}
                <motion.div
                    key={currentStep}
                    initial={{ opacity: 0, y: 20 }}
                    animate={{ opacity: 1, y: 0 }}
                    exit={{ opacity: 0, y: -20 }}
                    transition={{ duration: 0.3 }}
                >
                    {currentStep === 1 && renderStep1()}
                    {currentStep === 2 && renderStep2()}
                    {currentStep === 3 && renderStep3()}
                </motion.div>

                {/* Navigation Buttons */}
                <div className="mt-8 flex justify-between">
                    <Button
                        variant="outline"
                        onClick={() => setCurrentStep((prev) => Math.max(1, prev - 1))}
                        disabled={currentStep === 1}
                    >
                        Previous
                    </Button>
                    {currentStep < 3 ? (
                        <Button
                            onClick={() => setCurrentStep((prev) => Math.min(3, prev + 1))}
                            disabled={
                                (currentStep === 1 && selectedServices.length === 0) ||
                                (currentStep === 2 && (!scheduledDate || !position))
                            }
                        >
                            Next Step
                        </Button>
                    ) : (
                        <Button
                            onClick={handleSubmit}
                            isLoading={isLoading || isPaymentProcessing}
                            disabled={isLoading || isPaymentProcessing}
                        >
                            {isLoading || isPaymentProcessing ? (
                                <>
                                    <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                                    Processing...
                                </>
                            ) : (
                                'Place Order'
                            )}
                        </Button>
                    )}
                </div>
            </div>
        </DashboardLayout>
    );
};
