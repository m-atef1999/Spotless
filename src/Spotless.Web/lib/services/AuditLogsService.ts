/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */

import type { Spotless_Application_Dtos_Responses_PagedResponse_1 } from '../models/Spotless_Application_Dtos_Responses_PagedResponse_1';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class AuditLogsService {
    /**
     * Retrieves audit logs with optional filtering and pagination (Admin only)
     * @returns Spotless_Application_Dtos_Responses_PagedResponse_1<Spotless_Application_Dtos_AuditLog_AuditLogDto_Spotless_Application_Version_1_0_0_0_Culture_neutral_PublicKeyToken_null_> Success
     * @throws ApiError
     */
    public static getApiAuditLogs({
        userId,
        eventType,
        startDate,
        endDate,
        pageNumber = 1,
        pageSize = 20,
    }: {
        userId?: string,
        eventType?: string,
        startDate?: string,
        endDate?: string,
        pageNumber?: number,
        pageSize?: number,
    }): CancelablePromise<Spotless_Application_Dtos_Responses_PagedResponse_1> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/audit-logs',
            query: {
                'userId': userId,
                'eventType': eventType,
                'startDate': startDate,
                'endDate': endDate,
                'pageNumber': pageNumber,
                'pageSize': pageSize,
            },
        });
    }
}
