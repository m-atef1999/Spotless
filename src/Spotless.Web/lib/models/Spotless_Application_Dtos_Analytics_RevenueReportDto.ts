/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Spotless_Application_Dtos_Analytics_DailyRevenueDto } from './Spotless_Application_Dtos_Analytics_DailyRevenueDto';
export type Spotless_Application_Dtos_Analytics_RevenueReportDto = {
    startDate?: string;
    endDate?: string;
    totalRevenue?: number;
    totalOrders?: number;
    averageOrderValue?: number;
    dailyBreakdown?: Array<Spotless_Application_Dtos_Analytics_DailyRevenueDto> | null;
};

