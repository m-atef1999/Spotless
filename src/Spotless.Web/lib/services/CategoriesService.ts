/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Spotless_Application_Dtos_Category_CategoryDto } from '../models/Spotless_Application_Dtos_Category_CategoryDto';

import type { Spotless_Application_Dtos_Category_CreateCategoryDto } from '../models/Spotless_Application_Dtos_Category_CreateCategoryDto';
import type { Spotless_Application_Dtos_Category_UpdateCategoryDto } from '../models/Spotless_Application_Dtos_Category_UpdateCategoryDto';
import type { Spotless_Application_Dtos_Responses_PagedResponse_1 } from '../models/Spotless_Application_Dtos_Responses_PagedResponse_1';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class CategoriesService {
    /**
     * Lists all service categories with pagination
     * @returns Spotless_Application_Dtos_Responses_PagedResponse_1<Spotless_Application_Dtos_Category_CategoryDto_Spotless_Application_Version_1_0_0_0_Culture_neutral_PublicKeyToken_null_> Success
     * @throws ApiError
     */
    public static getApiCategories({
        pageNumber,
        pageSize,
    }: {
        pageNumber?: number,
        pageSize?: number,
    }): CancelablePromise<Spotless_Application_Dtos_Responses_PagedResponse_1> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/categories',
            query: {
                'pageNumber': pageNumber,
                'pageSize': pageSize,
            },
        });
    }
    /**
     * Creates a new category
     * @returns Spotless_Application_Dtos_Category_CategoryDto Created
     * @throws ApiError
     */
    public static postApiCategories({
        requestBody,
    }: {
        requestBody?: Spotless_Application_Dtos_Category_CreateCategoryDto,
    }): CancelablePromise<Spotless_Application_Dtos_Category_CategoryDto> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/categories',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
            },
        });
    }
    /**
     * Updates an existing category
     * @returns Spotless_Application_Dtos_Category_CategoryDto Success
     * @throws ApiError
     */
    public static putApiCategories({
        id,
        requestBody,
    }: {
        id: string,
        requestBody?: Spotless_Application_Dtos_Category_UpdateCategoryDto,
    }): CancelablePromise<Spotless_Application_Dtos_Category_CategoryDto> {
        return __request(OpenAPI, {
            method: 'PUT',
            url: '/api/categories/{id}',
            path: {
                'id': id,
            },
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
                404: `Not Found`,
            },
        });
    }
    /**
     * Deletes a category
     * @returns void
     * @throws ApiError
     */
    public static deleteApiCategories({
        id,
    }: {
        id: string,
    }): CancelablePromise<void> {
        return __request(OpenAPI, {
            method: 'DELETE',
            url: '/api/categories/{id}',
            path: {
                'id': id,
            },
            errors: {
                400: `Bad Request`,
                404: `Not Found`,
            },
        });
    }
}
