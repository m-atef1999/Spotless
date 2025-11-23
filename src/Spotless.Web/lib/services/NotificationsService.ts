/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Spotless_Application_Dtos_Notification_NotificationDto } from '../models/Spotless_Application_Dtos_Notification_NotificationDto';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class NotificationsService {
    /**
     * Retrieves notifications for the authenticated user
     * @returns Spotless_Application_Dtos_Notification_NotificationDto Success
     * @throws ApiError
     */
    public static getApiNotifications({
        unreadOnly,
        page,
        pageSize,
    }: {
        unreadOnly?: boolean,
        page?: number,
        pageSize?: number,
    }): CancelablePromise<Array<Spotless_Application_Dtos_Notification_NotificationDto>> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/notifications',
            query: {
                'unreadOnly': unreadOnly,
                'page': page,
                'pageSize': pageSize,
            },
        });
    }
    /**
     * Marks a notification as read
     * @returns void
     * @throws ApiError
     */
    public static putApiNotificationsRead({
        id,
    }: {
        id: string,
    }): CancelablePromise<void> {
        return __request(OpenAPI, {
            method: 'PUT',
            url: '/api/notifications/{id}/read',
            path: {
                'id': id,
            },
            errors: {
                403: `Forbidden`,
                404: `Not Found`,
            },
        });
    }
    /**
     * Deletes a notification
     * @returns void
     * @throws ApiError
     */
    public static deleteApiNotifications({
        id,
    }: {
        id: string,
    }): CancelablePromise<void> {
        return __request(OpenAPI, {
            method: 'DELETE',
            url: '/api/notifications/{id}',
            path: {
                'id': id,
            },
            errors: {
                403: `Forbidden`,
                404: `Not Found`,
            },
        });
    }
}
