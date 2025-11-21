/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Spotless_Application_Dtos_Payment_InitiatePaymentDto } from '../models/Spotless_Application_Dtos_Payment_InitiatePaymentDto';
import type { Spotless_Application_Dtos_Payment_InitiatePaymentResponseDto } from '../models/Spotless_Application_Dtos_Payment_InitiatePaymentResponseDto';
import type { Spotless_Application_Interfaces_PaymobProcessedCallbackData } from '../models/Spotless_Application_Interfaces_PaymobProcessedCallbackData';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class PaymentsService {
    /**
     * Initiates a payment for an order
     * @returns Spotless_Application_Dtos_Payment_InitiatePaymentResponseDto Success
     * @throws ApiError
     */
    public static postApiPaymentsInitiate({
        requestBody,
    }: {
        requestBody?: Spotless_Application_Dtos_Payment_InitiatePaymentDto,
    }): CancelablePromise<Spotless_Application_Dtos_Payment_InitiatePaymentResponseDto> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/Payments/initiate',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
                404: `Not Found`,
            },
        });
    }
    /**
     * Processes Paymob payment webhook notifications
     * @returns any Success
     * @throws ApiError
     */
    public static postApiPaymentsWebhook({
        hmacSha512,
        requestBody,
    }: {
        hmacSha512?: string,
        requestBody?: Spotless_Application_Interfaces_PaymobProcessedCallbackData,
    }): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/Payments/webhook',
            headers: {
                'Hmac-SHA512': hmacSha512,
            },
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
                401: `Unauthorized`,
            },
        });
    }
    /**
     * Health check endpoint for payment service
     * @returns any Success
     * @throws ApiError
     */
    public static getApiPaymentsHealth(): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/Payments/health',
        });
    }
}
