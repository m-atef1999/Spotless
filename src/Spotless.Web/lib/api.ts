// ============================================================================
export { AdminsService } from './services/AdminsService';
export { AnalyticsService } from './services/AnalyticsService';
export { AuditLogsService } from './services/AuditLogsService';
export { AuthService } from './services/AuthService';
export { CartsService } from './services/CartsService';
export { CategoriesService } from './services/CategoriesService';
export { CustomersService } from './services/CustomersService';
export { DriversService } from './services/DriversService';
export { NotificationsService } from './services/NotificationsService';
export { OrdersService } from './services/OrdersService';
export { PaymentsService } from './services/PaymentsService';
export { ReviewsService } from './services/ReviewsService';
export { ServicesService } from './services/ServicesService';
export { SystemSettingsService } from './services/SystemSettingsService';

// ============================================================================
// CORE UTILITIES
// ============================================================================
export { ApiError } from './core/ApiError';
export { CancelablePromise, CancelError } from './core/CancelablePromise';
export { OpenAPI } from './core/OpenAPI';

// ============================================================================
// ENUM CONSTANTS & HELPERS
// ============================================================================
export * from './constants';

// ============================================================================
// MODEL TYPES - Import these directly in your components
// ============================================================================
export type {
  // Auth & Authentication
  Spotless_Application_Dtos_Authentication_AuthResult as AuthResult,
  Spotless_Application_Dtos_Authentication_ExternalAuthRequest as ExternalAuthRequest,
  Spotless_Application_Features_Authentication_Commands_LoginCommand_LoginCommand as LoginCommand,
  Spotless_Application_Features_Authentication_Commands_RefreshToken_RefreshTokenCommand as RefreshTokenCommand,
  Spotless_Application_Features_Authentication_Commands_ForgotPassword_ForgotPasswordCommand as ForgotPasswordCommand,
  Spotless_Application_Features_Authentication_Commands_ResetPassword_ResetPasswordCommand as ResetPasswordCommand,
  Spotless_Application_Features_Authentication_Commands_ChangePassword_ChangePasswordCommand as ChangePasswordCommand,
  Spotless_Application_Features_Authentication_Commands_SendOtp_SendOtpCommand as SendOtpCommand,
  Spotless_Application_Features_Authentication_Commands_VerifyOtp_VerifyOtpCommand as VerifyOtpCommand,
  Spotless_Application_Features_Authentication_Commands_SendVerificationEmail_SendVerificationEmailCommand as SendVerificationEmailCommand,

  // Customer
  Spotless_Application_Dtos_Customer_CustomerDto as CustomerDto,
  Spotless_Application_Dtos_Customer_CustomerDashboardDto as CustomerDashboardDto,
  Spotless_Application_Dtos_Customer_CustomerUpdateRequest as CustomerUpdateRequest,
  Spotless_Application_Dtos_Customer_WalletTopUpRequest as WalletTopUpRequest,
  Spotless_Application_Features_Customers_Commands_RegisterCustomer_RegisterCustomerCommand as RegisterCustomerCommand,

  // Driver
  Spotless_Application_Dtos_Driver_DriverDto as DriverDto,
  Spotless_Application_Dtos_Driver_DriverApplicationRequest as DriverApplicationRequest,
  Spotless_Application_Dtos_Driver_DriverStatusUpdateDto as DriverStatusUpdateDto,
  Spotless_Application_Dtos_Driver_DriverUpdateRequest as DriverUpdateRequest,
  Spotless_Application_Dtos_Driver_ApproveDriverRequest as ApproveDriverRequest,
  Spotless_Application_Features_Drivers_Commands_AssignDriver_AssignDriverCommand as AssignDriverCommand,

  // Order
  Spotless_Application_Dtos_Order_OrderDto as OrderDto,
  Spotless_Application_Dtos_Order_CreateOrderDto as CreateOrderDto,
  Spotless_Application_Dtos_Order_OrderItemDto as OrderItemDto,
  Spotless_Application_Dtos_Order_CreateOrderItemDto as CreateOrderItemDto,

  // Service
  Spotless_Application_Dtos_Service_ServiceDto as ServiceDto,

  // Category
  Spotless_Application_Dtos_Category_CategoryDto as CategoryDto,
  Spotless_Application_Dtos_Category_CreateCategoryDto as CreateCategoryDto,
  Spotless_Application_Dtos_Category_UpdateCategoryDto as UpdateCategoryDto,

  // Cart
  Spotless_Application_Dtos_Cart_AddToCartDto as AddToCartDto,
  Spotless_Application_Dtos_Cart_BuyNowRequest as BuyNowRequest,
  Spotless_Application_Dtos_Cart_CartCheckoutRequest as CartCheckoutRequest,

  // Review
  Spotless_Application_Dtos_Review_ReviewDto as ReviewDto,
  Spotless_Application_Dtos_Review_CreateReviewDto as CreateReviewDto,

  // Payment
  Spotless_Application_Dtos_Payment_PaymentDto as PaymentDto,
  Spotless_Application_Dtos_Payment_InitiatePaymentDto as InitiatePaymentDto,
  Spotless_Application_Dtos_Payment_InitiatePaymentResponseDto as InitiatePaymentResponseDto,
  Spotless_Application_Interfaces_PaymobProcessedCallbackData as PaymobCallbackData,

  // Notification
  Spotless_Application_Dtos_Notification_NotificationDto as NotificationDto,

  // Analytics
  Spotless_Application_Dtos_Analytics_AdminDashboardDto as AnalyticsDashboardDto,
  Spotless_Application_Dtos_Analytics_RevenueReportDto as RevenueReportDto,
  Spotless_Application_Dtos_Analytics_DailyRevenueDto as DailyRevenueDto,

  // Admin
  Spotless_Application_Dtos_Admin_AdminDto as AdminDto,
  Spotless_Application_Dtos_Admin_AdminDashboardDto as AdminDashboardDto,
  Spotless_Application_Dtos_Admin_MostUsedServiceDto as MostUsedServiceDto,

  // Audit
  Spotless_Application_Dtos_AuditLog_AuditLogDto as AuditLogDto,

  // Settings
  Spotless_Application_Dtos_Settings_SystemSettingDto as SystemSettingDto,
  Spotless_Application_Dtos_Settings_UpdateSystemSettingDto as UpdateSystemSettingDto,

  // Common
  Spotless_Application_Dtos_Responses_PagedResponse_1 as PagedResponse,
  Spotless_Application_Dtos_LocationDto as LocationDto,
  Microsoft_AspNetCore_Mvc_ProblemDetails as ProblemDetails,

  // Enums (also available from ./constants)
  Spotless_Domain_Enums_OrderStatus as OrderStatusEnum,
  Spotless_Domain_Enums_PaymentStatus as PaymentStatusEnum,
  Spotless_Domain_Enums_PaymentMethod as PaymentMethodEnum,
  Spotless_Domain_Enums_CustomerType as CustomerTypeEnum,
  Spotless_Domain_Enums_NotificationType as NotificationTypeEnum,
} from './index';
