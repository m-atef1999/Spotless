/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Spotless_Application_Dtos_Admin_MostUsedServiceDto } from './Spotless_Application_Dtos_Admin_MostUsedServiceDto';
export type Spotless_Application_Dtos_Admin_AdminDashboardDto = {
    totalOrdersToday?: number;
    revenueToday?: number;
    revenueCurrency?: string | null;
    mostUsedServices?: Array<Spotless_Application_Dtos_Admin_MostUsedServiceDto> | null;
    numberOfActiveCleaners?: number;
    newRegistrationsToday?: number;
    pendingBookings?: number;
};

