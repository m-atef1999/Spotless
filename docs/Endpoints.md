# API Endpoints & Objects

This document provides a high-level overview of the available API resources and the Data Transfer Objects (DTOs) used for communication.

**Note:** For interactive documentation and testing, please run the API locally and visit the Swagger UI at `/swagger` (e.g., `https://localhost:7062/swagger/index.html`).

---

## 🚀 Key Controllers

### 🔐 Authentication (`/api/auth`)
Handles user identity, logins, and security tokens.
- **POST** `/login`: Authenticate with email/password to receive a JWT and Refresh Token.
- **POST** `/refresh`: Obtain a new access token using a valid refresh token.
- **POST** `/register`: (If applicable) Register a new user account.
- **POST** `/change-password`: Update the current user's password.
- **POST** `/forgot-password` & `/reset-password`: Recover lost access.
- **POST** `/external/google`: Login using Google OAuth.

### 📦 Orders (`/api/orders`)
Core business logic for placing and managing cleaning requests.
- **GET** `/`: Get all orders (Admin) or user's orders (Customer/Driver).
- **GET** `/{id}`: Get details of a specific order.
- **POST** `/`: Create a new order.
- **PUT** `/{id}/status`: Update order status (e.g., from `Pending` to `Confirmed`).
- **POST** `/{id}/assign`: Assign a driver to an order.

### 🧹 Services & Categories
- **Categories (`/api/categories`)**:
  - **GET** `/`: List all available service categories (e.g., "Home Cleaning", "Dry Cleaning").
- **Services (`/api/services`)**:
  - **GET** `/`: List all individual services.
  - **GET** `/category/{categoryId}`: Filter services by category.

### 👥 Users
- **Customers (`/api/customers`)**: Manage customer profiles and addresses.
- **Drivers (`/api/drivers`)**: Manage driver status (`Online`/`Offline`), location, and assigned jobs.
- **Admins (`/api/admins`)**: System administration endpoints.

### 📅 Scheduling
- **TimeSlots (`/api/timeslots`)**: Retrieve available booking windows (e.g., "9-11 AM").

---

## 📦 Data Transfer Objects (DTOs)

Below are the definitions of key objects used in API responses and requests.

### `OrderDto`
Represents a full order object including status, pricing, and locations.
- **Id** `(Guid)`: Unique identifier for the order.
- **CustomerId** `(Guid)`: ID of the customer who placed the order.
- **CustomerName** `(string)`: Display name of the customer.
- **DriverId** `(Guid?)`: ID of the assigned driver (nullable).
- **DriverName** `(string?)`: Name of the assigned driver.
- **Status** `(OrderStatus)`: Current state (e.g., `Pending`, `Confirmed`, `Completed`).
- **TotalPrice** `(decimal)`: Final calculated cost.
- **Currency** `(string)`: Currency code (e.g., "EGP").
- **ServiceName** `(string)`: Name of the primary service requested.
- **TimeSlotId** `(Guid)`: Selected time slot ID.
- **ScheduledDate** `(DateTime)`: Date of the service.
- **PickupAddress** `(string)`: Text address for pickup/service location.
- **DeliveryAddress** `(string)`: Text address for delivery (if applicable).

### `CustomerDto`
Customer profile information.
- **Id** `(Guid)`: Unique Customer ID.
- **Name** `(string)`: Full name.
- **Email** `(string)`: Contact email.
- **Phone** `(string)`: Contact phone number.
- **City, Country, Street** `(string)`: Address components.
- **WalletBalance** `(decimal)`: Current credit in the user's wallet.
- **Type** `(CustomerType)`: e.g., `Individual` or `Business`.

### `DriverDto`
Driver profile and live status.
- **Id** `(Guid)`: Unique Driver ID.
- **Name** `(string)`: Driver's full name.
- **Phone** `(string)`: Contact number.
- **VehicleInfo** `(string)`: Description of the car/vehicle (e.g., "Toyota Corolla 2020").
- **Status** `(string)`: Current availability (`Online`, `Offline`, `Busy`).
- **CurrentLocation** `(LocationDto)`: Last known GPS coordinates.

### `AuthResult`
Response from a successful login.
- **Token** `(string)`: JWT Access Token.
- **RefreshToken** `(string)`: Token to renew the session.
- **Expiry** `(DateTime)`: When the access token expires.
- **UserRole** `(string)`: Role of the authenticated user (e.g., `Admin`, `Customer`).

### `LocationDto`
Simple coordinate pair.
- **Latitude** `(double)`
- **Longitude** `(double)`
