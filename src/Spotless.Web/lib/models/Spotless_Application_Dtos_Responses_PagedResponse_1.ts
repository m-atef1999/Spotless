/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Spotless_Application_Dtos_Service_ServiceDto } from './Spotless_Application_Dtos_Service_ServiceDto';
export type Spotless_Application_Dtos_Responses_PagedResponse_1 = {
    pageNumber?: number;
    pageSize?: number;
    readonly totalPages?: number;
    totalRecords?: number;
    readonly hasPreviousPage?: boolean;
    readonly hasNextPage?: boolean;
    data?: Array<Spotless_Application_Dtos_Service_ServiceDto> | null;
};

