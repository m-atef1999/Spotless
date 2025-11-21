/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Spotless_Application_Dtos_Admin_AdminDashboardDto } from '../models/Spotless_Application_Dtos_Admin_AdminDashboardDto';

import type { Spotless_Application_Dtos_Responses_PagedResponse_1 } from '../models/Spotless_Application_Dtos_Responses_PagedResponse_1';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class AdminsService {
    /**
     * Lists all administrators with pagination
     * @returns Spotless_Application_Dtos_Responses_PagedResponse_1<Spotless_Application_Dtos_Admin_AdminDto_Spotless_Application_Version_1_0_0_0_Culture_neutral_PublicKeyToken_null_> Success
     * @throws ApiError
     */
    public static getApiAdmins({
        searchTerm,
        pageNumber,
        pageSize,
    }: {
        searchTerm?: string,
        pageNumber?: number,
        pageSize?: number,
    }): CancelablePromise<Spotless_Application_Dtos_Responses_PagedResponse_1> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/admins',
            query: {
                'searchTerm': searchTerm,
                'pageNumber': pageNumber,
                'pageSize': pageSize,
            },
            errors: {
                401: `Unauthorized`,
            },
        });
    }
    /**
     * Retrieves admin dashboard with system statistics
     * @returns Spotless_Application_Dtos_Admin_AdminDashboardDto Success
     * @throws ApiError
     */
    public static getApiAdminsDashboard({
        pageNumber,
        pageSize,
    }: {
        pageNumber?: number,
        pageSize?: number,
    }): CancelablePromise<Spotless_Application_Dtos_Admin_AdminDashboardDto> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/admins/dashboard',
            query: {
                'pageNumber': pageNumber,
                'pageSize': pageSize,
            },
            errors: {
                401: `Unauthorized`,
                403: `Forbidden`,
            },
        });
    }
}
