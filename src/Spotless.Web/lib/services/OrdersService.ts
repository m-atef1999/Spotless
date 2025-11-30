/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Spotless_Application_Dtos_Order_CreateOrderDto } from '../models/Spotless_Application_Dtos_Order_CreateOrderDto';
import type { Spotless_Application_Dtos_Order_OrderDto } from '../models/Spotless_Application_Dtos_Order_OrderDto';

import type { Spotless_Application_Dtos_Responses_PagedResponse_1 } from '../models/Spotless_Application_Dtos_Responses_PagedResponse_1';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class OrdersService {
    /**
     * Creates a new laundry order
     * @returns Spotless_Application_Dtos_Order_OrderDto Created
     * @throws ApiError
     */
    public static postApiOrders({
        requestBody,
    }: {
        requestBody?: Spotless_Application_Dtos_Order_CreateOrderDto,
    }): CancelablePromise<Spotless_Application_Dtos_Order_OrderDto> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/orders',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
            },
        });
    }
    /**
     * Retrieves order details by ID
     * @returns Spotless_Application_Dtos_Order_OrderDto Success
     * @throws ApiError
     */
    public static getApiOrders({
        id,
    }: {
        id: string,
    }): CancelablePromise<Spotless_Application_Dtos_Order_OrderDto> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/orders/{id}',
            path: {
                'id': id,
            },
            errors: {
                404: `Not Found`,
            },
        });
    }
    /**
     * Lists all orders for authenticated customer
     * @returns Spotless_Application_Dtos_Responses_PagedResponse_1<Spotless_Application_Dtos_Order_OrderDto_Spotless_Application_Version_1_0_0_0_Culture_neutral_PublicKeyToken_null_> Success
     * @throws ApiError
     */
    public static getApiOrdersCustomer({
        pageNumber,
        pageSize,
    }: {
        pageNumber?: number,
        pageSize?: number,
    }): CancelablePromise<Spotless_Application_Dtos_Responses_PagedResponse_1> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/orders/customer',
            query: {
                'pageNumber': pageNumber,
                'pageSize': pageSize,
            },
        });
    }
    /**
     * Cancels an existing order
     * @returns Spotless_Application_Dtos_Order_OrderDto Success
     * @throws ApiError
     */
    public static postApiOrdersCancel({
        id,
    }: {
        id: string,
    }): CancelablePromise<Spotless_Application_Dtos_Order_OrderDto> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/orders/{id}/cancel',
            path: {
                'id': id,
            },
            errors: {
                403: `Forbidden`,
                404: `Not Found`,
                409: `Conflict`,
            },
        });
    }
    /**
     * Updates the status of an order (Admin only)
     * @returns Spotless_Application_Dtos_Order_OrderDto Success
     * @throws ApiError
     */
    public static putApiOrdersStatus({
        id,
        status,
    }: {
        id: string,
        status: number,
    }): CancelablePromise<Spotless_Application_Dtos_Order_OrderDto> {
        return __request(OpenAPI, {
            method: 'PUT',
            url: '/api/orders/{id}/status',
            path: {
                'id': id,
            },
            body: status,
            mediaType: 'application/json',
            errors: {
                404: `Not Found`,
            },
        });
    }

    /**
     * Lists all orders for admin with filtering
     * @returns Spotless_Application_Dtos_Responses_PagedResponse_1<Spotless_Application_Dtos_Order_OrderDto> Success
     * @throws ApiError
     */
    public static getApiOrdersAdmin({
        pageNumber,
        pageSize,
        status,
        searchTerm,
    }: {
        pageNumber?: number,
        pageSize?: number,
        status?: number,
        searchTerm?: string,
    }): CancelablePromise<Spotless_Application_Dtos_Responses_PagedResponse_1> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/orders/admin',
            query: {
                'pageNumber': pageNumber,
                'pageSize': pageSize,
                'status': status,
                'searchTerm': searchTerm,
            },
            errors: {
                401: `Unauthorized`,
                403: `Forbidden`,
            },
        });
    }
}
