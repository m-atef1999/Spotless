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
import type { Spotless_Application_Dtos_Driver_DriverEarningsDto } from '../models/Spotless_Application_Dtos_Driver_DriverEarningsDto';
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
            url: '/api/Drivers/admin/registrations/{applicationId}/approve',
            path: {
                'applicationId': applicationId,
            },
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * Retrieves driver's earnings
     * @returns Spotless_Application_Dtos_Driver_DriverEarningsDto Success
     * @throws ApiError
     */
    public static getApiDriversEarnings(): CancelablePromise<Spotless_Application_Dtos_Driver_DriverEarningsDto> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/Drivers/earnings',
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
            url: '/api/Drivers/admin/order-applications/{applicationId}/accept',
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

    /**
     * Retrieves driver applications (Admin)
     * @returns any Success
     * @throws ApiError
     */
    public static getApiDriversAdminApplications({
        pageNumber,
        pageSize,
        status,
    }: {
        pageNumber?: number,
        pageSize?: number,
        status?: 'Submitted' | 'Approved' | 'Rejected',
    }): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/Drivers/admin/applications',
            query: {
                'pageNumber': pageNumber,
                'pageSize': pageSize,
                'status': status,
            },
        });
    }

    /**
     * Rejects driver application (Admin)
     * @returns void
     * @throws ApiError
     */
    public static postApiDriversAdminRegistrationsReject({
        applicationId,
        requestBody,
    }: {
        applicationId: string,
        requestBody: string,
    }): CancelablePromise<void> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/Drivers/admin/registrations/{applicationId}/reject',
            path: {
                'applicationId': applicationId,
            },
            body: requestBody,
            mediaType: 'application/json',
        });
    }

    /**
     * Lists all drivers with optional filtering (Admin)
     * @returns Spotless_Application_Dtos_Responses_PagedResponse_1<Spotless_Application_Dtos_Driver_DriverDto> Success
     * @throws ApiError
     */
    public static getApiDrivers({
        pageNumber,
        pageSize,
        status,
        searchTerm,
    }: {
        pageNumber?: number,
        pageSize?: number,
        status?: number,
        searchTerm?: string,
    }): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/Drivers',
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

    /**
     * Revokes driver access (Admin only)
     * @returns void Success
     * @throws ApiError
     */
    public static postApiDriversAdminRevoke({
        driverId,
    }: {
        driverId: string,
    }): CancelablePromise<void> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/Drivers/admin/{driverId}/revoke',
            path: {
                'driverId': driverId,
            },
            errors: {
                401: `Unauthorized`,
                403: `Forbidden`,
                404: `Not Found`,
            },
        });
    }
    /**
     * Accepts an order for the authenticated driver
     * @returns Spotless_Application_Dtos_Order_OrderDto Success
     * @throws ApiError
     */
    public static postApiDriversAccept({
        orderId,
    }: {
        orderId: string,
    }): CancelablePromise<Spotless_Application_Dtos_Order_OrderDto> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/Drivers/accept/{orderId}',
            path: {
                'orderId': orderId,
            },
            errors: {
                400: `Bad Request`,
                404: `Not Found`,
            },
        });
    }

    /**
     * Updates order status (Driver)
     * @returns void Success
     * @throws ApiError
     */
    public static putApiDriversOrdersStatus({
        orderId,
        requestBody,
    }: {
        orderId: string,
        requestBody: number,
    }): CancelablePromise<void> {
        return __request(OpenAPI, {
            method: 'PUT',
            url: '/api/Drivers/orders/{orderId}/status',
            path: {
                'orderId': orderId,
            },
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
                404: `Not Found`,
            },
        });
    }
}
