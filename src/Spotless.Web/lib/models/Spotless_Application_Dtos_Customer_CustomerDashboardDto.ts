/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Spotless_Application_Dtos_Order_OrderDto } from './Spotless_Application_Dtos_Order_OrderDto';
import type { Spotless_Application_Dtos_Payment_PaymentDto } from './Spotless_Application_Dtos_Payment_PaymentDto';
export type Spotless_Application_Dtos_Customer_CustomerDashboardDto = {
    totalOrders?: number;
    upcomingBookedServices?: number;
    walletBalance?: number;
    walletCurrency?: string | null;
    upcomingOrders?: Array<Spotless_Application_Dtos_Order_OrderDto> | null;
    recentTransactions?: Array<Spotless_Application_Dtos_Payment_PaymentDto> | null;
    totalReviewsSent?: number;
};

