/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { Spotless_Application_Dtos_Authentication_AuthResult } from '../models/Spotless_Application_Dtos_Authentication_AuthResult';
import type { Spotless_Application_Dtos_Authentication_ExternalAuthRequest } from '../models/Spotless_Application_Dtos_Authentication_ExternalAuthRequest';
import type { Spotless_Application_Features_Authentication_Commands_ChangePassword_ChangePasswordCommand } from '../models/Spotless_Application_Features_Authentication_Commands_ChangePassword_ChangePasswordCommand';
import type { Spotless_Application_Features_Authentication_Commands_ForgotPassword_ForgotPasswordCommand } from '../models/Spotless_Application_Features_Authentication_Commands_ForgotPassword_ForgotPasswordCommand';
import type { Spotless_Application_Features_Authentication_Commands_LoginCommand_LoginCommand } from '../models/Spotless_Application_Features_Authentication_Commands_LoginCommand_LoginCommand';
import type { Spotless_Application_Features_Authentication_Commands_RefreshToken_RefreshTokenCommand } from '../models/Spotless_Application_Features_Authentication_Commands_RefreshToken_RefreshTokenCommand';
import type { Spotless_Application_Features_Authentication_Commands_ResetPassword_ResetPasswordCommand } from '../models/Spotless_Application_Features_Authentication_Commands_ResetPassword_ResetPasswordCommand';
import type { Spotless_Application_Features_Authentication_Commands_SendOtp_SendOtpCommand } from '../models/Spotless_Application_Features_Authentication_Commands_SendOtp_SendOtpCommand';
import type { Spotless_Application_Features_Authentication_Commands_SendVerificationEmail_SendVerificationEmailCommand } from '../models/Spotless_Application_Features_Authentication_Commands_SendVerificationEmail_SendVerificationEmailCommand';
import type { Spotless_Application_Features_Authentication_Commands_VerifyOtp_VerifyOtpCommand } from '../models/Spotless_Application_Features_Authentication_Commands_VerifyOtp_VerifyOtpCommand';
import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';
export class AuthService {
    /**
     * Authenticates user and returns JWT token
     * @returns Spotless_Application_Dtos_Authentication_AuthResult Success
     * @throws ApiError
     */
    public static postApiAuthLogin({
        requestBody,
    }: {
        requestBody?: Spotless_Application_Features_Authentication_Commands_LoginCommand_LoginCommand,
    }): CancelablePromise<Spotless_Application_Dtos_Authentication_AuthResult> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/Auth/login',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                401: `Unauthorized`,
            },
        });
    }
    /**
     * Refreshes expired JWT token using refresh token
     * @returns Spotless_Application_Dtos_Authentication_AuthResult Success
     * @throws ApiError
     */
    public static postApiAuthRefresh({
        requestBody,
    }: {
        requestBody?: Spotless_Application_Features_Authentication_Commands_RefreshToken_RefreshTokenCommand,
    }): CancelablePromise<Spotless_Application_Dtos_Authentication_AuthResult> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/Auth/refresh',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                401: `Unauthorized`,
            },
        });
    }
    /**
     * Changes authenticated user's password
     * @returns any Success
     * @throws ApiError
     */
    public static postApiAuthChangePassword({
        requestBody,
    }: {
        requestBody?: Spotless_Application_Features_Authentication_Commands_ChangePassword_ChangePasswordCommand,
    }): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/Auth/change-password',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
            },
        });
    }
    /**
     * Initiates password reset process via email
     * @returns any Success
     * @throws ApiError
     */
    public static postApiAuthForgotPassword({
        requestBody,
    }: {
        requestBody?: Spotless_Application_Features_Authentication_Commands_ForgotPassword_ForgotPasswordCommand,
    }): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/Auth/forgot-password',
            body: requestBody,
            mediaType: 'application/json',
        });
    }
    /**
     * Resets user password using reset token
     * @returns any Success
     * @throws ApiError
     */
    public static postApiAuthResetPassword({
        requestBody,
    }: {
        requestBody?: Spotless_Application_Features_Authentication_Commands_ResetPassword_ResetPasswordCommand,
    }): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/Auth/reset-password',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
            },
        });
    }
    /**
     * Sends email verification link to user
     * @returns any Success
     * @throws ApiError
     */
    public static postApiAuthVerifyEmailSend({
        requestBody,
    }: {
        requestBody?: Spotless_Application_Features_Authentication_Commands_SendVerificationEmail_SendVerificationEmailCommand,
    }): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/Auth/verify-email/send',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
            },
        });
    }
    /**
     * Confirms user email address using verification token
     * @returns any Success
     * @throws ApiError
     */
    public static getApiAuthVerifyEmailConfirm({
        userId,
        token,
    }: {
        userId?: string,
        token?: string,
    }): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/api/Auth/verify-email/confirm',
            query: {
                'UserId': userId,
                'Token': token,
            },
            errors: {
                400: `Bad Request`,
            },
        });
    }
    /**
     * Sends OTP code to phone for verification
     * @returns any Success
     * @throws ApiError
     */
    public static postApiAuthVerifyPhoneSendOtp({
        requestBody,
    }: {
        requestBody?: Spotless_Application_Features_Authentication_Commands_SendOtp_SendOtpCommand,
    }): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/Auth/verify-phone/send-otp',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
            },
        });
    }
    /**
     * Verifies phone number using OTP code
     * @returns any Success
     * @throws ApiError
     */
    public static postApiAuthVerifyPhoneConfirmOtp({
        requestBody,
    }: {
        requestBody?: Spotless_Application_Features_Authentication_Commands_VerifyOtp_VerifyOtpCommand,
    }): CancelablePromise<any> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/Auth/verify-phone/confirm-otp',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
            },
        });
    }
    /**
     * Authenticates user via Google OAuth
     * @returns Spotless_Application_Dtos_Authentication_AuthResult Success
     * @throws ApiError
     */
    public static postApiAuthExternalGoogle({
        requestBody,
    }: {
        requestBody?: Spotless_Application_Dtos_Authentication_ExternalAuthRequest,
    }): CancelablePromise<Spotless_Application_Dtos_Authentication_AuthResult> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/Auth/external/google',
            body: requestBody,
            mediaType: 'application/json',
            errors: {
                400: `Bad Request`,
            },
        });
    }
}
