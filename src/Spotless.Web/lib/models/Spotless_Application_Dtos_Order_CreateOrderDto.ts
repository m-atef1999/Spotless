/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Spotless_Application_Dtos_Order_CreateOrderItemDto } from './Spotless_Application_Dtos_Order_CreateOrderItemDto';
import type { Spotless_Domain_Enums_PaymentMethod } from './Spotless_Domain_Enums_PaymentMethod';
export type Spotless_Application_Dtos_Order_CreateOrderDto = {
    timeSlotId?: string;
    scheduledDate?: string;
    paymentMethod?: Spotless_Domain_Enums_PaymentMethod;
    pickupLatitude?: number;
    pickupLongitude?: number;
    pickupAddress?: string | null;
    deliveryLatitude?: number;
    deliveryLongitude?: number;
    deliveryAddress?: string | null;
    items?: Array<Spotless_Application_Dtos_Order_CreateOrderItemDto> | null;
};

