/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Spotless_Application_Dtos_Responses_PagedResponse_1 } from '../models/Spotless_Application_Dtos_Responses_PagedResponse_1';
import type { Spotless_Application_Dtos_Service_ServiceDto } from '../models/Spotless_Application_Dtos_Service_ServiceDto';

import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class ServicesService {
    /**
     * Lists all services with optional search and pagination
     * @returns Spotless_Application_Dtos_Responses_PagedResponse_1<Spotless_Application_Dtos_Service_ServiceDto_Spotless_Application_Version_1_0_0_0_Culture_neutral_PublicKeyToken_null_> Success
     * @throws ApiError
     */
    public static getApiServices({
        nameSearchTerm,
        pageNumber,
        pageSize,
    }: {
        nameSearchTerm?: string,
        pageNumber?: number,
        pageSize?: number,
    }): CancelablePromise<Spotless_Application_Dtos_Responses_PagedResponse_1> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/Services',
            query: {
                'nameSearchTerm': nameSearchTerm,
                'pageNumber': pageNumber,
                'pageSize': pageSize,
            },
        });
    }
    /**
     * Retrieves a specific service by ID
     * @returns Spotless_Application_Dtos_Service_ServiceDto Success
     * @throws ApiError
     */
    public static getApiServices1({
        id,
    }: {
        id: string,
    }): CancelablePromise<Spotless_Application_Dtos_Service_ServiceDto> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/Services/{id}',
            path: {
                'id': id,
            },
            errors: {
                404: `Not Found`,
            },
        });
    }
    /**
     * Retrieves featured services for homepage
     * @returns Spotless_Application_Dtos_Service_ServiceDto Success
     * @throws ApiError
     */
    public static getApiServicesFeatured({
        count = 4,
    }: {
        count?: number,
    }): CancelablePromise<Array<Spotless_Application_Dtos_Service_ServiceDto>> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/Services/featured',
            query: {
                'count': count,
            },
        });
    }

    /**
     * Creates a new service (Admin only)
     * @returns string Created - Service ID
     * @throws ApiError
     */
    public static postApiServices({
        requestBody,
    }: {
        requestBody: {
            categoryId: string;
            name: string;
            description: string;
            pricePerUnitAmount: number;
            pricePerUnitCurrency: string;
            estimatedDurationHours: number;
            maxWeightKg?: number;
            imageUrl?: string | null;
        };
    }): CancelablePromise<string> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/Services',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
                401: `Unauthorized`,
                403: `Forbidden`,
            },
        });
    }

    /**
     * Updates an existing service (Admin only)
     * @returns void No Content
     * @throws ApiError
     */
    public static putApiServices({
        id,
        requestBody,
    }: {
        id: string;
        requestBody: {
            name?: string | null;
            description?: string | null;
            pricePerUnitValue?: number | null;
            pricePerUnitCurrency?: string | null;
            estimatedDurationHours?: number | null;
            maxWeightKg?: number | null;
            categoryId?: string | null;
            imageUrl?: string | null;
        };
    }): CancelablePromise<void> {
        return __request(OpenAPI, {
            method: 'PUT',
            url: '/api/Services/{id}',
            path: {
                'id': id,
            },
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
                401: `Unauthorized`,
                403: `Forbidden`,
                404: `Not Found`,
            },
        });
    }

    /**
     * Deletes a service (Admin only)
     * @returns void No Content
     * @throws ApiError
     */
    public static deleteApiServices({
        id,
    }: {
        id: string;
    }): CancelablePromise<void> {
        return __request(OpenAPI, {
            method: 'DELETE',
            url: '/api/Services/{id}',
            path: {
                'id': id,
            },
            errors: {
                401: `Unauthorized`,
                403: `Forbidden`,
                404: `Not Found`,
            },
        });
    }
}

