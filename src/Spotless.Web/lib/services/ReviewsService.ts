/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Spotless_Application_Dtos_Responses_PagedResponse_1 } from '../models/Spotless_Application_Dtos_Responses_PagedResponse_1';
import type { Spotless_Application_Dtos_Review_CreateReviewDto } from '../models/Spotless_Application_Dtos_Review_CreateReviewDto';
import type { Spotless_Application_Dtos_Review_ReviewDto } from '../models/Spotless_Application_Dtos_Review_ReviewDto';

import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class ReviewsService {
    /**
     * Creates a new review for a service
     * @returns string Created
     * @throws ApiError
     */
    public static postApiReviews({
        requestBody,
    }: {
        requestBody?: Spotless_Application_Dtos_Review_CreateReviewDto,
    }): CancelablePromise<string> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/Reviews',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * Retrieves all reviews for a specific driver
     * @returns Spotless_Application_Dtos_Review_ReviewDto Success
     * @throws ApiError
     */
    public static getApiReviewsDriver({
        driverId,
    }: {
        driverId: string,
    }): CancelablePromise<Array<Spotless_Application_Dtos_Review_ReviewDto>> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/Reviews/driver/{driverId}',
            path: {
                'driverId': driverId,
            },
        });
    }
    /**
     * Retrieves all reviews by authenticated customer
     * @returns Spotless_Application_Dtos_Review_ReviewDto Success
     * @throws ApiError
     */
    public static getApiReviewsCustomer(): CancelablePromise<Array<Spotless_Application_Dtos_Review_ReviewDto>> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/Reviews/customer',
        });
    }
    /**
     * Lists all reviews (admin only)
     * @returns Spotless_Application_Dtos_Responses_PagedResponse_1<Spotless_Application_Dtos_Review_ReviewDto_Spotless_Application_Version_1_0_0_0_Culture_neutral_PublicKeyToken_null_> Success
     * @throws ApiError
     */
    public static getApiReviewsAdminAll({
        pageNumber = 1,
        pageSize = 25,
    }: {
        pageNumber?: number,
        pageSize?: number,
    }): CancelablePromise<Spotless_Application_Dtos_Responses_PagedResponse_1> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/Reviews/admin/all',
            query: {
                'pageNumber': pageNumber,
                'pageSize': pageSize,
            },
        });
    }
}
