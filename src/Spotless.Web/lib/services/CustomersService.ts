/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Spotless_Application_Dtos_Authentication_AuthResult } from '../models/Spotless_Application_Dtos_Authentication_AuthResult';
import type { Spotless_Application_Dtos_Customer_CustomerDashboardDto } from '../models/Spotless_Application_Dtos_Customer_CustomerDashboardDto';
import type { Spotless_Application_Dtos_Customer_CustomerDto } from '../models/Spotless_Application_Dtos_Customer_CustomerDto';

import type { Spotless_Application_Dtos_Customer_CustomerUpdateRequest } from '../models/Spotless_Application_Dtos_Customer_CustomerUpdateRequest';
import type { Spotless_Application_Dtos_Customer_WalletTopUpRequest } from '../models/Spotless_Application_Dtos_Customer_WalletTopUpRequest';
import type { Spotless_Application_Dtos_PaymentMethods_PaymentMethodDto } from '../models/Spotless_Application_Dtos_PaymentMethods_PaymentMethodDto';
import type { Spotless_Application_Dtos_Responses_PagedResponse_1 } from '../models/Spotless_Application_Dtos_Responses_PagedResponse_1';
import type { Spotless_Application_Features_Customers_Commands_RegisterCustomer_RegisterCustomerCommand } from '../models/Spotless_Application_Features_Customers_Commands_RegisterCustomer_RegisterCustomerCommand';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class CustomersService {
    /**
     * Lists all customers (Admin only)
     * @returns Spotless_Application_Dtos_Responses_PagedResponse_1<Spotless_Application_Dtos_Customer_CustomerDto_Spotless_Application_Version_1_0_0_0_Culture_neutral_PublicKeyToken_null_> Success
     * @throws ApiError
     */
    public static getApiCustomers({
        nameFilter,
        emailFilter,
        pageNumber,
        pageSize,
    }: {
        nameFilter?: string,
        emailFilter?: string,
        pageNumber?: number,
        pageSize?: number,
    }): CancelablePromise<Spotless_Application_Dtos_Responses_PagedResponse_1> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/customers',
            query: {
                'nameFilter': nameFilter,
                'emailFilter': emailFilter,
                'pageNumber': pageNumber,
                'pageSize': pageSize,
            },
            errors: {
                401: `Unauthorized`,
            },
        });
    }
    /**
     * Registers a new customer account
     * @returns Spotless_Application_Dtos_Authentication_AuthResult Success
     * @throws ApiError
     */
    public static postApiCustomersRegister({
        requestBody,
    }: {
        requestBody?: Spotless_Application_Features_Customers_Commands_RegisterCustomer_RegisterCustomerCommand,
    }): CancelablePromise<Spotless_Application_Dtos_Authentication_AuthResult> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/customers/register',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
            },
        });
    }
    /**
     * Retrieves customer dashboard with orders and wallet info
     * @returns Spotless_Application_Dtos_Customer_CustomerDashboardDto Success
     * @throws ApiError
     */
    public static getApiCustomersDashboard({
        pageNumber,
        pageSize,
    }: {
        pageNumber?: number,
        pageSize?: number,
    }): CancelablePromise<Spotless_Application_Dtos_Customer_CustomerDashboardDto> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/customers/dashboard',
            query: {
                'pageNumber': pageNumber,
                'pageSize': pageSize,
            },
            errors: {
                401: `Unauthorized`,
                404: `Not Found`,
            },
        });
    }
    /**
     * Retrieves authenticated customer's profile
     * @returns Spotless_Application_Dtos_Customer_CustomerDto Success
     * @throws ApiError
     */
    public static getApiCustomersMe(): CancelablePromise<Spotless_Application_Dtos_Customer_CustomerDto> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/customers/me',
            errors: {
                401: `Unauthorized`,
                404: `Not Found`,
            },
        });
    }
    /**
     * Updates authenticated customer's profile
     * @returns void
     * @throws ApiError
     */
    public static putApiCustomersMe({
        requestBody,
    }: {
        requestBody?: Spotless_Application_Dtos_Customer_CustomerUpdateRequest,
    }): CancelablePromise<void> {
        return __request(OpenAPI, {
            method: 'PUT',
            url: '/api/customers/me',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
                401: `Unauthorized`,
            },
        });
    }
    /**
     * Tops up customer's wallet balance
     * @returns any Success
     * @throws ApiError
     */
    public static postApiCustomersTopup({
        requestBody,
    }: {
        requestBody?: Spotless_Application_Dtos_Customer_WalletTopUpRequest,
    }): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/customers/topup',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
                401: `Unauthorized`,
            },
        });
    }

    /**
     * Retrieves authenticated customer's saved payment methods
     * @returns Array<Spotless_Application_Dtos_PaymentMethods_PaymentMethodDto> Success
     * @throws ApiError
     */
    public static getApiCustomersPaymentMethods(): CancelablePromise<Array<Spotless_Application_Dtos_PaymentMethods_PaymentMethodDto>> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/customers/payment-methods',
            errors: {
                401: `Unauthorized`,
                404: `Not Found`,
            },
        });
    }
}
