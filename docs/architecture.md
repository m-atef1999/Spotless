# Architecture

## Overview
Spotless follows the **Clean Architecture** principles to ensure separation of concerns, testability, and maintainability. The core business logic is independent of frameworks, databases, and external interfaces.

## Layers

The solution is divided into the following projects/layers:

### 1. Presentation (`Spotless.API`)
- **Role**: Entry point for the application.
- **Responsibilities**:
  - Exposing RESTful API endpoints via Controllers.
  - Authentication and Authorization (JWT).
  - Exception Handling and Middleware.
  - IoC Container Configuration.
  - Swagger/OpenAPI documentation.

### 2. Application (`Spotless.Application`)
- **Role**: The "orchestrator" of the system.
- **Responsibilities**:
  - Implements Use Cases using **CQRS** (Command Query Responsibility Segregation) pattern with **MediatR**.
  - Defines DTOs (Data Transfer Objects).
  - Validation logic (FluentValidation).
  - Interfaces for Infrastructure services (abstractions).

### 3. Domain (`Spotless.Domain`)
- **Role**: The heart of the business logic.
- **Responsibilities**:
  - **Entities**: Core business objects (e.g., `Order`, `Service`, `User`).
  - **Value Objects**: Immutable objects.
  - **Domain Events**: Events triggered by state changes.
  - **Enums** and Constants.
  - **No dependencies** on other layers.

### 4. Infrastructure (`Spotless.Infrastructure`)
- **Role**: Implementation of external concerns.
- **Responsibilities**:
  - **Database Access**: Entity Framework Core implementation, DbContext, Migrations.
  - **Repositories**: Implementation of domain repository interfaces.
  - **External Services**: Email sending, Payment gateways, File storage.

### 5. Frontend (`Spotless.Web`)
- **Role**: Client-side application.
- **Stack**: React, Vite, TailwindCSS.
- **Responsibilities**: User Interface and interactions.

## Diagrams

### High-Level Architecture
```mermaid
graph TD
    UI[Frontend (React)] -->|HTTP/REST| API[Presentation (API)]
    API --> Application
    Application --> Domain
    Infrastructure --> Application
    Infrastructure --> Domain
    Infrastructure --> DB[(SQL Server)]
```
