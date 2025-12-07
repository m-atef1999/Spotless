# Deployment

## Build

### Backend
To publish the API for deployment:

```bash
cd src/Spotless.API
dotnet publish -c Release -o ./publish
```
The `./publish` folder will contain the DLLs and runtime files.

### Frontend
To build the React application for production:

```bash
cd src/Spotless.Web
npm install
npm run build
```
The output will be in the `dist` folder.

## Server Steps

### 1. Upload
- Transfer the backend `./publish` contents to your server (e.g., IIS folder, Linux directory).
- Transfer the frontend `./dist` contents to your static host or web server root.

### 2. Configure Environment Variables
- Ensure the production server has the correct `appsettings.Production.json` or Environment Variables set (Connection Strings, Jwt Secrets, etc.).
- For Frontend, ensure the build was run with the correct `VITE_API_BASE_URL` or serve it with appropriate proxying.

### 3. Restart Service
- **IIS**: Restart the App Pool.
- **Linux (systemd)**: `sudo systemctl restart spotless-api`.
- **Docker**: `docker-compose up -d --build`.

## Docker
The project can also be deployed using Docker. Refer to the `Dockerfile` (if available) or create one for standard .NET and Node environments.
