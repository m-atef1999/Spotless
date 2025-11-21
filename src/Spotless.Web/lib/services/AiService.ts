/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ChatRequest } from '../models/Spotless_Application_Dtos_Ai_ChatRequest';
import type { ChatResponse } from '../models/Spotless_Application_Dtos_Ai_ChatResponse';

import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';

export class AiService {

    /**
     * Chat with Spotless AI Assistant
     * @param requestBody 
     * @returns ChatResponse Success
     * @throws ApiError
     */
    public static postApiAiChat({
        requestBody,
    }: {
        requestBody: ChatRequest,
    }): Promise<ChatResponse> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/api/v1/Ai/chat',
            body: requestBody,
            mediaType: 'application/json',
        });
    }

}
