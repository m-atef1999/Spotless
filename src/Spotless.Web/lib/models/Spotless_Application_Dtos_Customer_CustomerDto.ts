/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Spotless_Domain_Enums_CustomerType } from './Spotless_Domain_Enums_CustomerType';
export type Spotless_Application_Dtos_Customer_CustomerDto = {
    id?: string;
    name?: string | null;
    phone?: string | null;
    email?: string | null;
    street?: string | null;
    city?: string | null;
    country?: string | null;
    zipCode?: string | null;
    walletBalance?: number;
    walletCurrency?: string | null;
    type?: Spotless_Domain_Enums_CustomerType;
};

