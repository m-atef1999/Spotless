/* Enum Constants for easier usage across the app */
/* Based on backend C# enum definitions */

// OrderStatus enum values
// Backend: PaymentFailed=0, Requested=1, Confirmed=2, DriverAssigned=3, 
// PickedUp=4, InCleaning=5, OutForDelivery=6, Delivered=7, Cancelled=8
export const OrderStatus = {
    PaymentFailed: 0,
    Requested: 1,
    Confirmed: 2,
    DriverAssigned: 3,
    PickedUp: 4,
    InCleaning: 5,
    OutForDelivery: 6,
    Delivered: 7,
    Cancelled: 8,
} as const;

// PaymentStatus enum values
// Backend: Pending=0, Completed=1, Failed=2, Refunded=3
export const PaymentStatus = {
    Pending: 0,
    Completed: 1,
    Failed: 2,
    Refunded: 3,
} as const;

// PaymentMethod enum values  
// Backend: CreditCard=0, DebitCard=1, Fawry=2, PayPal=3, Wallet=4, CashOnDelivery=5, Paymob=6, Instapay=7
export const PaymentMethod = {
    CreditCard: 0,
    DebitCard: 1,
    Fawry: 2,
    PayPal: 3,
    Wallet: 4,
    CashOnDelivery: 5,
    Paymob: 6,
    Instapay: 7,
} as const;

// CustomerType enum values
// Based on backend: Regular = 0, Premium = 1
export const CustomerType = {
    Regular: 0,
    Premium: 1,
} as const;

// NotificationType enum values
// Based on backend: OrderUpdate = 0, PaymentUpdate = 1, DriverAssignment = 2, General = 3
export const NotificationType = {
    OrderUpdate: 0,
    PaymentUpdate: 1,
    DriverAssignment: 2,
    General: 3,
} as const;

// Helper functions for status display
export const getOrderStatusLabel = (status: number): string => {
    switch (status) {
        case OrderStatus.PaymentFailed: return 'Payment Failed';
        case OrderStatus.Requested: return 'Requested';
        case OrderStatus.Confirmed: return 'Confirmed';
        case OrderStatus.DriverAssigned: return 'Driver Assigned';
        case OrderStatus.PickedUp: return 'Picked Up';
        case OrderStatus.InCleaning: return 'In Cleaning';
        case OrderStatus.OutForDelivery: return 'Out for Delivery';
        case OrderStatus.Delivered: return 'Delivered';
        case OrderStatus.Cancelled: return 'Cancelled';
        default: return 'Unknown';
    }
};

export const getOrderStatusColor = (status: number): string => {
    switch (status) {
        case OrderStatus.PaymentFailed: return 'bg-red-100 text-red-800';
        case OrderStatus.Requested: return 'bg-blue-100 text-blue-800';
        case OrderStatus.Confirmed: return 'bg-green-100 text-green-800';
        case OrderStatus.DriverAssigned: return 'bg-purple-100 text-purple-800';
        case OrderStatus.PickedUp: return 'bg-indigo-100 text-indigo-800';
        case OrderStatus.InCleaning: return 'bg-yellow-100 text-yellow-800';
        case OrderStatus.OutForDelivery: return 'bg-cyan-100 text-cyan-800';
        case OrderStatus.Delivered: return 'bg-green-100 text-green-800';
        case OrderStatus.Cancelled: return 'bg-red-100 text-red-800';
        default: return 'bg-gray-100 text-gray-800';
    }
};

export const getPaymentStatusLabel = (status: number): string => {
    switch (status) {
        case PaymentStatus.Pending: return 'Pending';
        case PaymentStatus.Completed: return 'Completed';
        case PaymentStatus.Failed: return 'Failed';
        case PaymentStatus.Refunded: return 'Refunded';
        default: return 'Unknown';
    }
};

export const getPaymentMethodLabel = (method: number | string): string => {
    if (typeof method === 'string') {
        // Insert space before capital letters and trim
        return method.replace(/([A-Z])/g, ' $1').trim();
    }
    switch (method) {
        case PaymentMethod.CreditCard: return 'Credit Card';
        case PaymentMethod.DebitCard: return 'Debit Card';
        case PaymentMethod.Fawry: return 'Fawry';
        case PaymentMethod.PayPal: return 'PayPal';
        case PaymentMethod.Wallet: return 'Wallet';
        case PaymentMethod.CashOnDelivery: return 'Cash on Delivery';
        case PaymentMethod.Paymob: return 'Paymob';
        case PaymentMethod.Instapay: return 'Instapay';
        default: return 'Unknown';
    }
};
