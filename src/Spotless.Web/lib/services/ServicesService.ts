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
}
