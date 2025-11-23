import { OpenAPI } from '../../lib/api';
import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';
import { Check, CreditCard, Loader2, MapPin } from 'lucide-react';
import { MapContainer, TileLayer, Marker, useMapEvents } from 'react-leaflet';
import type { LeafletMouseEvent } from 'leaflet';
import { DashboardLayout } from '../../layouts/DashboardLayout';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { useToast } from '../../components/ui/Toast';
import { useAuthStore } from '../../store/authStore';
import { usePayment } from '../../hooks/usePayment';
import { getServiceImage } from '../../lib/imageUtils';
import {
    ServicesService,
    CartsService,
    OrdersService,
    CustomersService,
    type ServiceDto,
    type CreateOrderDto,
    type CreateOrderItemDto,
    type CustomerDto,
    type PaymentMethodEnum,
} from '../../lib/api';

const PaymentMethods = {
    CreditCard: 'CreditCard',
    CashOnDelivery: 'CashOnDelivery',
} as const;

const STEPS = [
    { id: 1, title: 'Select Services', icon: Check },
    { id: 2, title: 'Schedule & Location', icon: MapPin },
    { id: 3, title: 'Payment', icon: CreditCard },
];

interface TimeSlot {
    id: string;
    name: string;
    startTime: string;
    endTime: string;
    maxCapacity: number;
    days?: string[];
    validDaysOfWeek?: string;
}

export const NewOrderPage: React.FC = () => {
    const navigate = useNavigate();
    const { isProcessing: isPaymentProcessing } = usePayment();
    const { addToast } = useToast();
    const { user } = useAuthStore();
    const [currentStep, setCurrentStep] = useState(1);
    const [isLoading, setIsLoading] = useState(false);
    const [services, setServices] = useState<ServiceDto[]>([]);
    const [selectedServices, setSelectedServices] = useState<{ service: ServiceDto; quantity: number }[]>([]);
    const [timeSlots, setTimeSlots] = useState<TimeSlot[]>([]);
    const [selectedTimeSlotId, setSelectedTimeSlotId] = useState<string | null>(null);

    // Form State
    const [scheduledDate, setScheduledDate] = useState('');
    const [address, setAddress] = useState('');
    const [paymentMethod, setPaymentMethod] = useState<PaymentMethodEnum>(PaymentMethods.CreditCard);
    const [position, setPosition] = useState<[number, number] | null>(null);

    useEffect(() => {
        const fetchServices = async () => {
            try {
                const response = await ServicesService.getApiServices({ pageNumber: 1, pageSize: 50 });
                const allServices = response.data || [];
                setServices(allServices);

                const params = new URLSearchParams(window.location.search);
                const serviceId = params.get('serviceId');

                if (serviceId) {
                    const serviceToSelect = allServices.find(s => s.id === serviceId);
                    if (serviceToSelect) {
                        setSelectedServices([{ service: serviceToSelect, quantity: 1 }]);
                        addToast(`Added ${serviceToSelect.name} to the cart`, 'success');
                    }
                }
            } catch (error) {
                console.error('Failed to fetch services', error);
            }
        };
        fetchServices();
    }, []);

    useEffect(() => {
        const fetchTimeSlots = async () => {
            try {
                let token = OpenAPI.TOKEN;
                if (typeof token === 'function') {
                    token = await (token as any)();
                }

                const response = await fetch(`${OpenAPI.BASE}/api/timeslots`, {
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json',
                        'Cache-Control': 'no-cache, no-store, must-revalidate',
                        'Pragma': 'no-cache',
                        'Expires': '0'
                    }
                });
                if (response.ok) {
                    const data = await response.json();
                    setTimeSlots(data);
                }
            } catch (error) {
                console.error('Failed to fetch time slots', error);
            }
        };
        fetchTimeSlots();
    }, []);



    const handleServiceToggle = async (service: ServiceDto, quantity: number) => {
        const currentService = selectedServices.find(s => s.service.id === service.id);
        const currentQty = currentService?.quantity || 0;
        const isAdding = quantity > currentQty;
        const diff = Math.abs(quantity - currentQty);

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

        if (isAdding) {
            addToast(`Added ${service.name} to the cart`, 'success');
            try {
                await CartsService.postApiCustomersCartItems({
                    requestBody: {
                        serviceId: service.id,
                        quantity: diff
                    }
                });
            } catch (error) {
                console.error('Failed to add to backend cart', error);
            }
        } else {
            addToast(`Removed ${service.name} from the cart`, 'info');
            if (quantity <= 0) {
                try {
                    await CartsService.deleteApiCustomersCartItems({ serviceId: service.id! });
                } catch (error) {
                    console.error('Failed to remove from backend cart', error);
                }
            }
        }
    };

    const calculateTotal = () => {
        const total = selectedServices.reduce((acc, item) => acc + (item.service.basePrice || 0) * item.quantity, 0);
        return total;
    };

    const calculateTotalWeight = () => {
        return selectedServices.reduce((acc, item) => acc + (item.service.maxWeightKg || 0) * item.quantity, 0);
    };

    const handleSubmit = async () => {
        if (!selectedTimeSlotId) {
            addToast('Please select a time slot.', 'error');
            return;
        }

        setIsLoading(true);
        try {
            if (paymentMethod === PaymentMethods.CreditCard) {
                const totalAmount = calculateTotal();
                const response = await CustomersService.postApiCustomersTopup({
                    requestBody: {
                        amountValue: totalAmount,
                        paymentMethod: 'CreditCard'
                    }
                });

                if (response && response.paymentUrl) {
                    window.location.href = response.paymentUrl;
                    return;
                } else {
                    throw new Error("Failed to get payment URL for wallet top-up.");
                }
            }

            const orderItems: CreateOrderItemDto[] = selectedServices.map(s => ({
                serviceId: s.service.id,
                itemName: s.service.name || 'Unknown Service',
                quantity: s.quantity
            }));

            const order: CreateOrderDto = {
                scheduledDate: new Date(scheduledDate).toISOString(),
                timeSlotId: selectedTimeSlotId,
                paymentMethod: paymentMethod,
                pickupLatitude: position ? position[0] : 0,
                pickupLongitude: position ? position[1] : 0,
                deliveryLatitude: 0,
                deliveryLongitude: 0,
                deliveryAddress: address,
                pickupAddress: address,
                items: orderItems,
            };

            const response = await OrdersService.postApiOrders({ requestBody: order });

            if (response.id) {
                addToast('Order placed successfully!', 'success');
                navigate('/customer/dashboard');
            }
        } catch (error: any) {
            console.error('Failed to create order/payment', error);
            if (error.body) {
                addToast(`Error: ${JSON.stringify(error.body)}`, 'error');
            } else {
                addToast('Failed to process request. Please try again.', 'error');
            }
        } finally {
            setIsLoading(false);
        }
    };

    const reverseGeocode = async (lat: number, lng: number) => {
        try {
            const response = await fetch(`https://nominatim.openstreetmap.org/reverse?format=json&lat=${lat}&lon=${lng}`);
            const data = await response.json();
            if (data && data.display_name) {
                setAddress(data.display_name);
            } else {
                setAddress(`Lat: ${lat.toFixed(4)}, Lng: ${lng.toFixed(4)}`);
            }
        } catch (error) {
            console.error("Geocoding failed", error);
            setAddress(`Lat: ${lat.toFixed(4)}, Lng: ${lng.toFixed(4)}`);
        }
    };

    const handleUseMyAddress = async () => {
        if (!user) {
            const { token, fetchProfile } = useAuthStore.getState();
            if (token) {
                try {
                    await fetchProfile();
                    // Re-check user after fetch
                    const updatedUser = useAuthStore.getState().user;
                    if (updatedUser) {
                        const customer = updatedUser as CustomerDto;
                        const parts = [customer.street, customer.city, customer.country].filter(Boolean);
                        if (parts.length > 0) {
                            const fullAddress = parts.join(', ');
                            setAddress(fullAddress);
                            addToast('Address loaded from profile', 'success');
                            return;
                        }
                    }
                } catch (error) {
                    console.error('Failed to fetch profile', error);
                }
            }
            addToast('User information not found. Please try logging in again.', 'error');
            return;
        }

        const customer = user as CustomerDto;
        const parts = [customer.street, customer.city, customer.country].filter(Boolean);
        if (parts.length > 0) {
            const fullAddress = parts.join(', ');
            setAddress(fullAddress);
            addToast('Address loaded from profile', 'success');
        } else {
            addToast('No address found in your profile.', 'error');
        }
    };

    const MapEvents = () => {
        useMapEvents({
            click(e: LeafletMouseEvent) {
                setPosition([e.latlng.lat, e.latlng.lng]);
                reverseGeocode(e.latlng.lat, e.latlng.lng);
            },
        });
        return null;
    };

    const renderStep1 = () => {
        return (
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
                                onClick={() => {
                                    if (!quantity) {
                                        handleServiceToggle(service, 1);
                                    }
                                }}
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
                                    </div>
                                    <div className="flex flex-col items-start gap-1">
                                        <span className="font-bold text-cyan-400 bg-black/30 backdrop-blur-sm px-2 py-1 rounded-lg">
                                            {service.basePrice?.toFixed(2)} EGP
                                        </span>
                                        {service.maxWeightKg !== undefined && (
                                            <span className="text-xs font-medium text-slate-300 bg-black/30 backdrop-blur-sm px-2 py-0.5 rounded">
                                                Max {service.maxWeightKg} KG
                                            </span>
                                        )}
                                    </div>
                                    <p className="text-sm text-slate-200 line-clamp-2 opacity-90 mt-2">
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
    };

    const renderStep2 = () => {
        const getFilteredTimeSlots = () => {
            if (!scheduledDate) return [];
            if (!Array.isArray(timeSlots)) return [];

            const date = new Date(scheduledDate);
            const dayName = date.toLocaleDateString('en-US', { weekday: 'long' });

            return timeSlots.filter(slot => {
                // Defensive check for slot object
                if (!slot) return false;

                // 1. Check 'days' array (if exists)
                if (Array.isArray(slot.days)) {
                    const match = slot.days.some(d => d?.toLowerCase() === dayName.toLowerCase());
                    if (match) return true;
                }

                // 2. Check 'validDaysOfWeek' string (comma separated)
                if (slot.validDaysOfWeek && typeof slot.validDaysOfWeek === 'string') {
                    const days = slot.validDaysOfWeek.split(',').map(d => d.trim().toLowerCase());
                    const match = days.includes(dayName.toLowerCase());
                    if (match) return true;
                }

                // 3. Fallback: If no restrictions, assume available
                if ((!slot.days || slot.days.length === 0) && !slot.validDaysOfWeek) {
                    return true;
                }

                return false;
            });
        };

        const filteredSlots = getFilteredTimeSlots();

        return (
            <div className="space-y-6 max-w-md mx-auto">
                <div className="space-y-4">
                    <label className="block text-sm font-medium text-slate-700 dark:text-slate-300">
                        When should we pick up?
                    </label>
                    <div className="grid grid-cols-3 gap-2 mb-4">
                        {['Today', 'Tomorrow', 'Next Day'].map((day, index) => {
                            const date = new Date();
                            date.setDate(date.getDate() + index);
                            const isSelected = scheduledDate && new Date(scheduledDate).toDateString() === date.toDateString();

                            return (
                                <button
                                    key={day}
                                    className={`p-3 rounded-xl border text-sm font-medium transition-all ${isSelected
                                        ? 'border-cyan-500 bg-cyan-50 dark:bg-cyan-900/20 text-cyan-700 dark:text-cyan-300'
                                        : 'border-slate-200 dark:border-slate-700 hover:border-cyan-300 dark:hover:border-cyan-700 text-slate-600 dark:text-slate-400'
                                        }`}
                                    onClick={() => {
                                        const newDate = new Date();
                                        newDate.setDate(newDate.getDate() + index);
                                        if (scheduledDate) {
                                            const existing = new Date(scheduledDate);
                                            newDate.setHours(existing.getHours(), existing.getMinutes());
                                        } else {
                                            newDate.setHours(9, 0, 0, 0);
                                        }
                                        setScheduledDate(newDate.toISOString());
                                        setSelectedTimeSlotId(null);
                                    }}
                                >
                                    {day}
                                    <span className="block text-xs opacity-70 font-normal">
                                        {date.toLocaleDateString('en-US', { weekday: 'short', day: 'numeric' })}
                                    </span>
                                </button>
                            );
                        })}
                    </div>

                    {scheduledDate && (
                        <div className="grid grid-cols-3 gap-2">
                            {filteredSlots.length > 0 ? (
                                filteredSlots.map((slot) => {
                                    const isSelected = selectedTimeSlotId === slot.id;
                                    return (
                                        <button
                                            key={slot.id}
                                            className={`p-2 rounded-lg border text-sm transition-all ${isSelected
                                                ? 'border-cyan-500 bg-cyan-500 text-white'
                                                : 'border-slate-200 dark:border-slate-700 hover:border-cyan-300 dark:hover:border-cyan-700 text-slate-600 dark:text-slate-400'
                                                }`}
                                            onClick={() => {
                                                setSelectedTimeSlotId(slot.id);
                                                if (slot.startTime && scheduledDate) {
                                                    try {
                                                        const parts = slot.startTime.split(':');
                                                        if (parts.length >= 2) {
                                                            const hours = parseInt(parts[0], 10);
                                                            const minutes = parseInt(parts[1], 10);
                                                            if (!isNaN(hours) && !isNaN(minutes)) {
                                                                const newDate = new Date(scheduledDate);
                                                                if (!isNaN(newDate.getTime())) {
                                                                    newDate.setHours(hours, minutes, 0, 0);
                                                                    setScheduledDate(newDate.toISOString());
                                                                }
                                                            }
                                                        }
                                                    } catch (e) {
                                                        console.error("Error setting time slot:", e);
                                                    }
                                                }
                                            }}
                                        >
                                            {slot.name}
                                        </button>
                                    );
                                })
                            ) : (
                                <div className="col-span-3 text-center text-sm text-slate-500">
                                    No time slots available for this day. Please select another date.
                                </div>
                            )}
                        </div>
                    )}
                </div>

                <div className="space-y-4">
                    <label className="block text-sm font-medium text-slate-700 dark:text-slate-300">
                        Where should we pick up?
                    </label>
                    <div className="h-64 w-full rounded-xl overflow-hidden border border-slate-200 dark:border-slate-700 relative z-0">
                        <MapContainer
                            center={[30.0444, 31.2357]}
                            zoom={12}
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
                    <div className="relative flex gap-2 z-[1000]">
                        <div className="relative flex-1">
                            <MapPin className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400 w-5 h-5" />
                            <Input
                                type="text"
                                placeholder="Click on map to select location"
                                value={address}
                                onChange={(e) => setAddress(e.target.value)}
                                className="pl-10 w-full"
                            />
                        </div>
                        <Button variant="outline" onClick={handleUseMyAddress} type="button">
                            Use Saved
                        </Button>
                    </div>
                    <p className="text-xs text-slate-500">
                        Click on the map to pin your exact location.
                    </p>
                </div>
            </div>
        );
    };

    const renderStep3 = () => {
        return (
            <div className="space-y-8 max-w-md mx-auto">
                <div className="bg-slate-50 dark:bg-slate-900/50 p-6 rounded-xl space-y-4">
                    <h3 className="font-semibold text-slate-900 dark:text-white border-b border-slate-200 dark:border-slate-700 pb-2">
                        Order Summary
                    </h3>
                    <div className="space-y-2">
                        {selectedServices.map(({ service, quantity }) => (
                            <div key={service.id} className="flex justify-between text-sm">
                                <span className="text-slate-700 dark:text-slate-300 font-medium">
                                    {quantity}x {service.name}
                                </span>
                                <span className="font-bold text-slate-900 dark:text-white">
                                    {((service.basePrice || 0) * quantity).toFixed(2)} EGP
                                </span>
                            </div>
                        ))}
                    </div>
                    <div className="border-t border-slate-200 dark:border-slate-700 pt-2 flex justify-between font-bold text-lg">
                        <span className="text-slate-900 dark:text-white">Total Price</span>
                        <span className="text-cyan-600 dark:text-cyan-400">{calculateTotal().toFixed(2)} EGP</span>
                    </div>
                    <div className="flex justify-between font-medium text-sm">
                        <span className="text-slate-700 dark:text-slate-300">Total Weight</span>
                        <span className="text-slate-900 dark:text-white">{calculateTotalWeight()} KG</span>
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
                                : 'border-slate-200 dark:border-slate-700 hover:border-slate-300 dark:hover:border-slate-600 text-slate-600 dark:text-slate-300'
                                }`}
                            onClick={() => setPaymentMethod(PaymentMethods.CreditCard)}
                        >
                            <CreditCard className="w-6 h-6" />
                            <span className="text-sm font-medium">Credit Card</span>
                        </button>
                        <button
                            className={`p-4 rounded-xl border flex flex-col items-center gap-2 transition-all ${paymentMethod === PaymentMethods.CashOnDelivery
                                ? 'border-cyan-500 bg-cyan-50 dark:bg-cyan-900/20 text-cyan-700 dark:text-cyan-300'
                                : 'border-slate-200 dark:border-slate-700 hover:border-slate-300 dark:hover:border-slate-600 text-slate-600 dark:text-slate-300'
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
    };

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

                {/* Top Navigation Buttons */}
                <div className="mb-6 flex justify-between">
                    <Button
                        variant="outline"
                        onClick={() => setCurrentStep((prev) => Math.max(1, prev - 1))}
                        disabled={currentStep === 1}
                        className="text-sm px-4 py-2 h-auto"
                    >
                        Previous
                    </Button>
                    {currentStep < 3 && (
                        <Button
                            onClick={() => setCurrentStep((prev) => Math.min(3, prev + 1))}
                            disabled={
                                (currentStep === 1 && selectedServices.length === 0) ||
                                (currentStep === 2 && (!scheduledDate || (!position && !address) || !selectedTimeSlotId))
                            }
                            className="text-sm px-4 py-2 h-auto"
                        >
                            Next Step
                        </Button>
                    )}
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
                                (currentStep === 2 && (!scheduledDate || (!position && !address) || !selectedTimeSlotId))
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
