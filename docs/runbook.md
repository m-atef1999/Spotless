# Runbook

## Common Issues

### API Won't Start
- **Cause**: Invalid configuration or missing database connection.
- **Fix**: Check `appsettings.json` syntax and verify ConnectionStrings. Check the logs for specific exception messages.

### DB Connection Error
- **Symptoms**: Timeout, "Network-related instance-specific error".
- **Fix**:
  - Verify SQL Server is running.
  - Check firewall rules (port 1433).
  - Confirm the connection string user/password are correct.

### Swagger Not Loading
- **Cause**: Typically only enabled in Development environment.
- **Fix**: Check `Program.cs`. ensuring `app.UseSwagger()` is not restricted inside `if (app.Environment.IsDevelopment())` if you need it in other environments (not recommended for Prod).

## Logs
- **Location**: The application uses Serilog and writes to files in the `Logs/` directory relative to the executable.
- **Format**: `log-YYYYMMDD.txt`
- **Console**: Logs are also output to the console (stdout) which can be viewed in Docker logs or IIS stdout logs.
