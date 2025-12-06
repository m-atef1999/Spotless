# Database

The project uses **Microsoft SQL Server** with **Entity Framework Core** as the ORM.

## ERD
*(Diagram would be placed here. Use tools like SSMS or DBeaver to generate a visual diagram from the actual schema.)*

## Entities
The core entities in the domain include:

- **Users** (`ApplicationUser`): Extends IdentityUser. Includes Admins, Customers, and Drivers.
- **Service**: Represents a cleaning service offered (e.g., "Deep Clean").
- **Category**: Grouping for services (e.g., "Residential", "Commercial").
- **Order**: A booking made by a customer.
- **OrderDetail**: Line items within an order.
- **Driver**: Specific profile/data for service providers.
- **Review**: Ratings and comments left by customers.
- **Payment**: Transaction records.
- **Notification**: User alerts.
- **AuditLog**: Records of important system actions for security and tracking.

## Migrations

Migrations are managed via Entity Framework Core tools.

### Add a Migration
To add a new migration after modifying entities:

```bash
dotnet ef migrations add <MigrationName> --project src/Spotless.Infrastructure --startup-project src/Spotless.API
```

### Update Database
To apply pending migrations to the database:

```bash
dotnet ef database update --project src/Spotless.Infrastructure --startup-project src/Spotless.API
```

### Remove Last Migration
To remove the last created migration (if not applied):

```bash
dotnet ef migrations remove --project src/Spotless.Infrastructure --startup-project src/Spotless.API
```
