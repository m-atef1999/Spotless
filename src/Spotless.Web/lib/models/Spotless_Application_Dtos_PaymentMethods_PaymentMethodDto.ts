/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Spotless_Domain_Enums_PaymentMethod } from './Spotless_Domain_Enums_PaymentMethod';
export type Spotless_Application_Dtos_PaymentMethods_PaymentMethodDto = {
    id?: string;
    type?: Spotless_Domain_Enums_PaymentMethod;
    last4Digits?: string | null;
    cardholderName?: string | null;
    expiryDate?: string;
    isDefault?: boolean;
};
