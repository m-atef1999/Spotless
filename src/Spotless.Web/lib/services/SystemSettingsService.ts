/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Spotless_Application_Dtos_Settings_SystemSettingDto } from '../models/Spotless_Application_Dtos_Settings_SystemSettingDto';
import type { Spotless_Application_Dtos_Settings_UpdateSystemSettingDto } from '../models/Spotless_Application_Dtos_Settings_UpdateSystemSettingDto';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class SystemSettingsService {
    /**
     * Retrieves system settings with optional category filter (Admin only)
     * @returns Spotless_Application_Dtos_Settings_SystemSettingDto Success
     * @throws ApiError
     */
    public static getApiSettings({
        category,
    }: {
        category?: string,
    }): CancelablePromise<Array<Spotless_Application_Dtos_Settings_SystemSettingDto>> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/settings',
            query: {
                'category': category,
            },
        });
    }
    /**
     * Updates a system setting value (Admin only)
     * @returns void
     * @throws ApiError
     */
    public static putApiSettings({
        id,
        requestBody,
    }: {
        id: string,
        requestBody?: Spotless_Application_Dtos_Settings_UpdateSystemSettingDto,
    }): CancelablePromise<void> {
        return __request(OpenAPI, {
            method: 'PUT',
            url: '/api/settings/{id}',
            path: {
                'id': id,
            },
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                404: `Not Found`,
            },
        });
    }
}
