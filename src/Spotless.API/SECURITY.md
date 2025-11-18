# Security & Encryption Implementation

This document outlines the security features implemented in the Spotless application.

## 1. Encryption Service

### Implementation
- **Data Protection API** (Primary): Uses ASP.NET Core Data Protection API (`DataProtectionService`)
  - Recommended approach for .NET applications
  - Handles key management automatically
  - Configured via `builder.Services.AddDataProtection()`

- **AES-256 Encryption** (Alternative): `AesEncryptionService` available for explicit AES encryption
  - Uses AES-256-CBC mode with PKCS7 padding
  - Requires encryption key in configuration

### Usage
```csharp
// Inject IEncryptionService
private readonly IEncryptionService _encryptionService;

// Encrypt sensitive data
var encrypted = _encryptionService.Encrypt("sensitive-value");

// Decrypt
var decrypted = _encryptionService.Decrypt(encrypted);
```

## 2. Encrypted Sensitive Fields

### API Keys
- **PaymentGatewaySettings**: `ApiKey` and `WebhookSecret` can be encrypted
- **EmailSettings**: `SmtpPassword` should be encrypted
- **JwtSettings**: `Secret` should be encrypted (or use User Secrets/Key Vault)

### Wallet Balance
- Currently stored as `Money` value object (decimal + currency)
- If additional encryption needed at rest, can be implemented via value converters in EF Core

### Payment Card Details
- **Note**: Payment card details should NOT be stored in the database (PCI DSS compliance)
- Payment gateway handles card tokenization
- Only payment references/transaction IDs are stored

## 3. Password Hashing

### Implementation
- **ASP.NET Core Identity** automatically handles password hashing
- Uses **PBKDF2** with **HMAC-SHA256**
- Configured in `ServiceCollectionExtensions.AddIdentityAndAuthentication()`
- Password requirements:
  - Requires digit
  - Requires lowercase
  - Unique email required

### Verification
- Passwords are hashed when created via `UserManager.CreateAsync()`
- Verification done via `UserManager.CheckPasswordAsync()`
- No plain text passwords are ever stored

## 4. HTTPS Enforcement

### Implementation
- **Middleware**: `HttpsEnforcementMiddleware`
- Enforces HTTPS in production (disabled in development)
- Blocks HTTP requests with 400 Bad Request
- Configured via `SecuritySettings.EnforceHttps`

### Configuration
```json
{
  "SecuritySettings": {
    "EnforceHttps": true  // true in production, false in development
  }
}
```

## 5. CORS Policy

### Implementation
- Configured in `Program.cs` with `DefaultCorsPolicy`
- Supports configurable:
  - Allowed origins
  - Allowed methods (GET, POST, PUT, DELETE, PATCH, OPTIONS)
  - Allowed headers (Content-Type, Authorization)
  - Credentials support

### Configuration
```json
{
  "SecuritySettings": {
    "Cors": {
      "EnableCors": true,
      "AllowedOrigins": ["https://yourdomain.com"],
      "AllowedMethods": ["GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS"],
      "AllowedHeaders": ["Content-Type", "Authorization"],
      "AllowCredentials": true
    }
  }
}
```

## 6. Rate Limiting

### Implementation
- **Middleware**: `RateLimitingMiddleware`
- IP-based rate limiting
- Configurable max requests and time window
- Returns 429 Too Many Requests when exceeded

### Configuration
```json
{
  "SecuritySettings": {
    "RateLimit": {
      "EnableRateLimit": true,
      "MaxRequests": 100,  // requests per time window
      "TimeWindowMinutes": 1
    }
  }
}
```

### Production Recommendation
For production, consider using `AspNetCoreRateLimit` NuGet package for more advanced features:
- Per-endpoint rate limits
- Distributed rate limiting (Redis)
- More sophisticated policies

## Security Best Practices

### Configuration
1. **Never commit sensitive values** to source control
2. Use **User Secrets** for local development
3. Use **Azure Key Vault** or **AWS Secrets Manager** for production
4. Encrypt API keys in configuration files

### Data Protection
1. Data Protection keys should be persisted and shared across app instances
2. Configure key storage location in production
3. Use separate keys per environment

### HTTPS
1. Always use HTTPS in production
2. Configure proper SSL/TLS certificates
3. Use HSTS (HTTP Strict Transport Security) headers

### CORS
1. Whitelist only necessary origins
2. Avoid `AllowAnyOrigin()` in production
3. Review allowed methods and headers regularly

### Rate Limiting
1. Adjust limits based on application load
2. Consider different limits for authenticated vs anonymous users
3. Monitor rate limit hits for potential attacks

## Notes

- **Password Hashing**: Automatically handled by ASP.NET Core Identity - no manual implementation needed
- **Payment Cards**: Never stored - only use payment gateway tokens (PCI DSS compliant)
- **API Keys**: Can be encrypted in configuration or better yet, use environment variables/secret managers
- **Wallet Balance**: Currently stored as plain decimal - can be encrypted at rest if required via EF Core value converters

