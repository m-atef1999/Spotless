/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Spotless_Application_Dtos_Cart_AddToCartDto } from '../models/Spotless_Application_Dtos_Cart_AddToCartDto';
import type { Spotless_Application_Dtos_Cart_BuyNowRequest } from '../models/Spotless_Application_Dtos_Cart_BuyNowRequest';
import type { Spotless_Application_Dtos_Cart_CartCheckoutRequest } from '../models/Spotless_Application_Dtos_Cart_CartCheckoutRequest';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class CartsService {
    /**
     * Retrieves authenticated customer's shopping cart
     * @returns any Success
     * @throws ApiError
     */
    public static getApiCustomersCart(): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/customers/cart',
        });
    }
    /**
     * Clears all items from customer's cart
     * @returns any Success
     * @throws ApiError
     */
    public static deleteApiCustomersCart(): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'DELETE',
            url: '/api/customers/cart',
        });
    }
    /**
     * Adds a service to customer's cart
     * @returns any Success
     * @throws ApiError
     */
    public static postApiCustomersCartItems({
        requestBody,
    }: {
        requestBody?: Spotless_Application_Dtos_Cart_AddToCartDto,
    }): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/customers/cart/items',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * Removes a service from customer's cart
     * @returns any Success
     * @throws ApiError
     */
    public static deleteApiCustomersCartItems({
        serviceId,
    }: {
        serviceId: string,
    }): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'DELETE',
            url: '/api/customers/cart/items/{serviceId}',
            path: {
                'serviceId': serviceId,
            },
        });
    }
    /**
     * Checks out cart and creates an order
     * @returns any Success
     * @throws ApiError
     */
    public static postApiCustomersCartCheckout({
        requestBody,
    }: {
        requestBody?: Spotless_Application_Dtos_Cart_CartCheckoutRequest,
    }): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/customers/cart/checkout',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * Creates immediate order without cart
     * @returns any Success
     * @throws ApiError
     */
    public static postApiCustomersCartBuyNow({
        requestBody,
    }: {
        requestBody?: Spotless_Application_Dtos_Cart_BuyNowRequest,
    }): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/customers/cart/buy-now',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
}
