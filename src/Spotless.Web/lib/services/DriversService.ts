/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Spotless_Application_Dtos_Driver_ApproveDriverRequest } from '../models/Spotless_Application_Dtos_Driver_ApproveDriverRequest';
import type { Spotless_Application_Dtos_Driver_DriverApplicationRequest } from '../models/Spotless_Application_Dtos_Driver_DriverApplicationRequest';
import type { Spotless_Application_Dtos_Driver_DriverDto } from '../models/Spotless_Application_Dtos_Driver_DriverDto';
import type { Spotless_Application_Dtos_Driver_DriverStatusUpdateDto } from '../models/Spotless_Application_Dtos_Driver_DriverStatusUpdateDto';
import type { Spotless_Application_Dtos_Driver_DriverUpdateRequest } from '../models/Spotless_Application_Dtos_Driver_DriverUpdateRequest';
import type { Spotless_Application_Dtos_LocationDto } from '../models/Spotless_Application_Dtos_LocationDto';
import type { Spotless_Application_Dtos_Order_OrderDto } from '../models/Spotless_Application_Dtos_Order_OrderDto';
import type { Spotless_Application_Features_Drivers_Commands_AssignDriver_AssignDriverCommand } from '../models/Spotless_Application_Features_Drivers_Commands_AssignDriver_AssignDriverCommand';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class DriversService {
    /**
     * Submits driver application
     * @returns string Created
     * @throws ApiError
     */
    public static postApiDriversRegister({
        requestBody,
    }: {
        requestBody?: Spotless_Application_Dtos_Driver_DriverApplicationRequest,
    }): CancelablePromise<string> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/Drivers/register',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
                401: `Unauthorized`,
            },
        });
    }
    /**
     * Retrieves authenticated driver's profile
     * @returns Spotless_Application_Dtos_Driver_DriverDto Success
     * @throws ApiError
     */
    public static getApiDriversProfile(): CancelablePromise<Spotless_Application_Dtos_Driver_DriverDto> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/Drivers/profile',
            errors: {
                401: `Unauthorized`,
                404: `Not Found`,
            },
        });
    }
    /**
     * Updates authenticated driver's profile
     * @returns void
     * @throws ApiError
     */
    public static putApiDriversProfile({
        requestBody,
    }: {
        requestBody?: Spotless_Application_Dtos_Driver_DriverUpdateRequest,
    }): CancelablePromise<void> {
        return __request(OpenAPI, {
            method: 'PUT',
            url: '/api/Drivers/profile',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
            },
        });
    }
    /**
     * Retrieves orders assigned to authenticated driver
     * @returns Spotless_Application_Dtos_Order_OrderDto Success
     * @throws ApiError
     */
    public static getApiDriversOrders(): CancelablePromise<Array<Spotless_Application_Dtos_Order_OrderDto>> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/Drivers/orders',
        });
    }
    /**
     * Retrieves available orders for drivers to accept
     * @returns Spotless_Application_Dtos_Order_OrderDto Success
     * @throws ApiError
     */
    public static getApiDriversAvailable(): CancelablePromise<Array<Spotless_Application_Dtos_Order_OrderDto>> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/Drivers/available',
        });
    }
    /**
     * @returns void
     * @throws ApiError
     */
    public static putApiDriversStatus({
        requestBody,
    }: {
        requestBody?: Spotless_Application_Dtos_Driver_DriverStatusUpdateDto,
    }): CancelablePromise<void> {
        return __request(OpenAPI, {
            method: 'PUT',
            url: '/api/Drivers/status',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
            },
        });
    }
    /**
     * @returns void
     * @throws ApiError
     */
    public static putApiDriversLocation({
        requestBody,
    }: {
        requestBody?: Spotless_Application_Dtos_LocationDto,
    }): CancelablePromise<void> {
        return __request(OpenAPI, {
            method: 'PUT',
            url: '/api/Drivers/location',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @returns string Created
     * @throws ApiError
     */
    public static postApiDriversApply({
        orderId,
    }: {
        orderId: string,
    }): CancelablePromise<string> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/Drivers/apply/{orderId}',
            path: {
                'orderId': orderId,
            },
        });
    }
    /**
     * @returns string Success
     * @throws ApiError
     */
    public static postApiDriversAdminApplicationsApprove({
        applicationId,
        requestBody,
    }: {
        applicationId: string,
        requestBody?: Spotless_Application_Dtos_Driver_ApproveDriverRequest,
    }): CancelablePromise<string> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/Drivers/admin/applications/{applicationId}/approve',
            path: {
                'applicationId': applicationId,
            },
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * @returns any Success
     * @throws ApiError
     */
    public static getApiDriversAdminOrderApplications({
        orderId,
    }: {
        orderId: string,
    }): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/Drivers/admin/order-applications/{orderId}',
            path: {
                'orderId': orderId,
            },
        });
    }
    /**
     * @returns any Success
     * @throws ApiError
     */
    public static postApiDriversAdminApplicationsAccept({
        applicationId,
    }: {
        applicationId: string,
    }): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/Drivers/admin/applications/{applicationId}/accept',
            path: {
                'applicationId': applicationId,
            },
        });
    }
    /**
     * @returns any Success
     * @throws ApiError
     */
    public static postApiDriversAdminAssign({
        requestBody,
    }: {
        requestBody?: Spotless_Application_Features_Drivers_Commands_AssignDriver_AssignDriverCommand,
    }): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/Drivers/admin/assign',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
}
