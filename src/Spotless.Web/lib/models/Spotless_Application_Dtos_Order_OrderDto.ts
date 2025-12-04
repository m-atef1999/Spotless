/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Spotless_Application_Dtos_Order_OrderItemDto } from './Spotless_Application_Dtos_Order_OrderItemDto';
import type { Spotless_Domain_Enums_OrderStatus } from './Spotless_Domain_Enums_OrderStatus';
import type { Spotless_Domain_Enums_PaymentMethod } from './Spotless_Domain_Enums_PaymentMethod';
export type Spotless_Application_Dtos_Order_OrderDto = {
    id?: string;
    customerId?: string;
    driverId?: string | null;
    driverName?: string | null;
    timeSlotId?: string;
    startTime?: string | null;
    endTime?: string | null;
    scheduledDate?: string;
    pickupLatitude?: number;
    pickupLongitude?: number;
    pickupAddress?: string | null;
    deliveryLatitude?: number;
    deliveryLongitude?: number;
    deliveryAddress?: string | null;
    totalPrice?: number;
    currency?: string | null;
    status?: Spotless_Domain_Enums_OrderStatus;
    paymentMethod?: Spotless_Domain_Enums_PaymentMethod;
    serviceName?: string;
    createdAt?: string;
    orderDate?: string;
    estimatedDurationHours?: number;
    items?: Array<Spotless_Application_Dtos_Order_OrderItemDto>;
};
