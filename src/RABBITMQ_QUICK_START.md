# Quick Start - RabbitMQ Configuration

## To run the application WITHOUT RabbitMQ (Development):

**Option A - Using Configuration:**

```json
// In appsettings.Development.json
"RabbitMQ": {
  "HostName": "localhost",
  "Port": 5672,
  "UserName": "guest",
  "Password": "guest",
  "VirtualHost": "/",
  "Enabled": false  // ← Add this to disable
}
```

**Option B - With RabbitMQ running but unreachable:**

- Application will start automatically
- Connection will be attempted lazily when needed
- Warnings logged but application continues

## To run the application WITH RabbitMQ:

**Step 1: Start RabbitMQ (Docker):**

```powershell
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

**Step 2: Update configuration:**

```json
// In appsettings.json or appsettings.Development.json
"RabbitMQ": {
  "HostName": "localhost",
  "Port": 5672,
  "UserName": "guest",
  "Password": "guest",
  "VirtualHost": "/",
  "Enabled": true  // ← Ensure this is true
}
```

**Step 3: Run the application:**

```powershell
cd c:\Users\egysc\Documents\Work\GitHub\Spotless\src
dotnet run --project Spotless.API/Spotless.API.csproj
```

## RabbitMQ Management UI:

After starting RabbitMQ, access the management dashboard:

- URL: http://localhost:15672
- Username: guest
- Password: guest

## Files Changed:

1. `RabbitMqMessageBroker.cs` - Lazy connection
2. `NoOpMessageBroker.cs` - Fallback implementation (new)
3. `ServiceCollectionExtensions.cs` - Conditional DI
4. `BackgroundJobProcessor.cs` - Graceful handling

## What the fix does:

✅ Application starts even if RabbitMQ is down
✅ Logs warnings instead of crashing
✅ Can be completely disabled via configuration
✅ Recovers when RabbitMQ becomes available
✅ No code changes needed in consuming services
