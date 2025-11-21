/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Spotless_Application_Dtos_Analytics_AdminDashboardDto } from '../models/Spotless_Application_Dtos_Analytics_AdminDashboardDto';
import type { Spotless_Application_Dtos_Analytics_RevenueReportDto } from '../models/Spotless_Application_Dtos_Analytics_RevenueReportDto';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class AnalyticsService {
    /**
     * Retrieves admin dashboard metrics (Admin only)
     * @returns Spotless_Application_Dtos_Analytics_AdminDashboardDto Success
     * @throws ApiError
     */
    public static getApiAnalyticsDashboard(): CancelablePromise<Spotless_Application_Dtos_Analytics_AdminDashboardDto> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/analytics/dashboard',
        });
    }
    /**
     * Retrieves revenue report for a date range (Admin only)
     * @returns Spotless_Application_Dtos_Analytics_RevenueReportDto Success
     * @throws ApiError
     */
    public static getApiAnalyticsRevenue({
        startDate,
        endDate,
    }: {
        startDate?: string,
        endDate?: string,
    }): CancelablePromise<Spotless_Application_Dtos_Analytics_RevenueReportDto> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/analytics/revenue',
            query: {
                'startDate': startDate,
                'endDate': endDate,
            },
        });
    }
}
