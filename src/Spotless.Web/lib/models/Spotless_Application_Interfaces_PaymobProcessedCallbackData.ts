/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
export type Spotless_Application_Interfaces_PaymobProcessedCallbackData = {
    amountCents?: number;
    createdAt?: string | null;
    currency?: string | null;
    errorOccured?: boolean;
    hasParentTransaction?: boolean;
    id?: number;
    integrationId?: number;
    is3dSecure?: boolean;
    isAuth?: boolean;
    isCapture?: boolean;
    isRefunded?: boolean;
    isStandalonePayment?: boolean;
    isVoided?: boolean;
    orderId?: number;
    owner?: number;
    pending?: boolean;
    sourceDataPan?: string | null;
    sourceDataSubType?: string | null;
    sourceDataType?: string | null;
    success?: boolean;
};

