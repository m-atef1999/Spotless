# API Reference

## Base URL
- **Development**: `https://localhost:7062` (typical default)
- **Production**: `https://spotless.runasp.net/`

## Auth
- **Method**: JWT (JSON Web Tokens) Bearer Authentication.
- **Header**: `Authorization: Bearer <token>`
- **Flow**: Login/Register endpoints return a token which must be included in subsequent authorized requests.

## Endpoints

Below are some of the key controllers and resources available in the system:

### Authentication
- `POST /api/auth/login`
- `POST /api/auth/register`
- `POST /api/auth/refresh-token`

### Users
- `Admins`: Admin management endpoints.
- `Customers`: Customer profile and data.
- `Drivers`: Driver application, status, and profile management.

### Core Business
- `Orders`: Create, update, and track orders.
- `Services`: Manage cleaning services catalog.
- `Categories`: Service categories.
- `Reviews`: Customer reviews for services/drivers.
- `TimeSlots`: Appointment scheduling.

### Finance
- `Payments`: Process payments and handle callbacks.

### System
- `Notifications`: Manage user notifications.
- `AuditLogs`: View system activity logs.
- `Analytics`: System usage and performance stats.

## Swagger
Interactive API documentation is available at:
- **Path**: `/swagger`
- **Example**: `https://localhost:7062/swagger/index.html`
