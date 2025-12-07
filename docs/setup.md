# Setup

This guide will help you set up the Spotless project locally.

## Requirements

Ensure you have the following installed:

- **.NET SDK**: Version 8.0 or later.
- **Node.js**: Version 20.x or later (Development uses v24.x).
- **SQL Server**: LocalDB or a running instance.
- **Docker** (Optional): For containerized setup.

## Backend (ASP.NET Core)

1.  **Navigate to the API directory:**
    ```bash
    cd src/Spotless.API
    ```

2.  **Restore dependencies:**
    ```bash
    dotnet restore
    ```

3.  **Database Configuration:**
    - Update the **ConnectionStrings** in `appsettings.json` or `appsettings.Development.json` to point to your local SQL Server instance.
    - Example:
      ```json
      "ConnectionStrings": {
        "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SpotlessDb;Trusted_Connection=True;MultipleActiveResultSets=true"
      }
      ```

4.  **Apply Migrations:**
    ```bash
    dotnet ef database update --project ../Spotless.Infrastructure --startup-project .
    ```

5.  **Run the API:**
    ```bash
    dotnet run
    ```
    The API will be available at `https://localhost:7062` (or similar port).

## Frontend (React + Vite)

1.  **Navigate to the web directory:**
    ```bash
    cd src/Spotless.Web
    ```

2.  **Install dependencies:**
    ```bash
    npm install
    ```

3.  **Environment Setup:**
    - Create a `.env` file in the root of `Spotless.Web` if it doesn't exist.
    - Add the API base URL:
      ```env
      VITE_API_BASE_URL=https://localhost:7062/
      ```

4.  **Run the development server:**
    ```bash
    npm run dev
    ```
    The app will basically run at `http://localhost:5173`.

## Environment Variables

### Backend (`appsettings.json`)
Key configurations include:
- `ConnectionStrings:DefaultConnection`: Main database connection.
- `JwtSettings`: Secret and expiry for tokens.
- `EmailSettings`: SMTP configuration.
- `PaymentGateway`: Keys for payment integration.
- `Gemini:ApiKey`: Key for AI features.
- `SecuritySettings:Cors`: Allowed origins.

### Frontend (`.env`)
- `VITE_API_BASE_URL`: The URL of the backend API.
