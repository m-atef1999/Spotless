/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Spotless_Domain_Enums_PaymentMethod } from './Spotless_Domain_Enums_PaymentMethod';
import type { Spotless_Domain_Enums_PaymentStatus } from './Spotless_Domain_Enums_PaymentStatus';
export type Spotless_Application_Dtos_Payment_PaymentDto = {
    id?: string;
    orderId?: string;
    amount?: number;
    currency?: string | null;
    paymentDate?: string;
    method?: Spotless_Domain_Enums_PaymentMethod;
    status?: Spotless_Domain_Enums_PaymentStatus;
};

