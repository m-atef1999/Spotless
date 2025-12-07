# Security

## Secrets
- **Development**: Use .NET User Secrets to store sensitive data like connection strings and API keys to avoid committing them to source control.
  - Command: `dotnet user-secrets set "Key" "Value"`
- **Production**: Use Environment Variables or a secure vault (e.g., Azure Key Vault).

## Authentication
- **Mechanism**: JWT (JSON Web Tokens).
- **Process**:
  1.  User logs in with credentials.
  2.  Server validates and issues a signed JWT.
  3.  Client sends JWT in `Authorization` header for subsequent requests.
- **Policies**: Tokens have an expiration time. Refresh tokens are used to obtain new access tokens without re-login.

## CORS (Cross-Origin Resource Sharing)
- The API is configured to allow specific origins to prevent unauthorized browser requests.
- **Configuration**: Defined in `appsettings.json` under `SecuritySettings:Cors`.
- **Allowed Origins**: Must be explicitly listed (e.g., `https://spotless-project.vercel.app`).
- **Allowed Methods**: Standard HTTP methods (GET, POST, PUT, DELETE, etc.).

## Data Protection
- **Encryption**: Sensitive data at rest (if applicable) can be encrypted.
- **HTTPS**: Enforced for all communications.
- **Audit**: Critical actions are logged in the `AuditLogs` table/storage.
