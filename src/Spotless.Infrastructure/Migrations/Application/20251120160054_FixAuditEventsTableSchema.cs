using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spotless.Infrastructure.Migrations.Application
{
    
    
    
    
    /// <inheritdoc />
    public partial class FixAuditEventsTableSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            // Check if AuditEvents table exists, if not create it
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AuditEvents')
                BEGIN
                    CREATE TABLE [dbo].[AuditEvents] (
                        [EventId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
                        [EventDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                        [EventData] NVARCHAR(MAX) NULL,
                        [LastUpdatedDate] DATETIME2 NULL,
                        CONSTRAINT [PK_AuditEvents] PRIMARY KEY ([EventId])
                    );
                END
                ELSE
                BEGIN
                    -- If table exists, add DEFAULT constraints if they don't exist
                    IF NOT EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_AuditEvents_EventId')
                    BEGIN
                        ALTER TABLE [dbo].[AuditEvents] 
                        ADD CONSTRAINT [DF_AuditEvents_EventId] DEFAULT NEWID() FOR [EventId];
                    END

                    IF NOT EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_AuditEvents_EventDate')
                    BEGIN
                        ALTER TABLE [dbo].[AuditEvents] 
                        ADD CONSTRAINT [DF_AuditEvents_EventDate] DEFAULT GETUTCDATE() FOR [EventDate];
                    END

                    -- Ensure columns are NOT NULL
                    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.AuditEvents') AND name = 'EventId' AND is_nullable = 1)
                    BEGIN
                        -- First, update any existing NULL values
                        UPDATE [dbo].[AuditEvents] SET [EventId] = NEWID() WHERE [EventId] IS NULL;
                        ALTER TABLE [dbo].[AuditEvents] ALTER COLUMN [EventId] UNIQUEIDENTIFIER NOT NULL;
                    END

                    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.AuditEvents') AND name = 'EventDate' AND is_nullable = 1)
                    BEGIN
                        -- First, update any existing NULL values
                        UPDATE [dbo].[AuditEvents] SET [EventDate] = GETUTCDATE() WHERE [EventDate] IS NULL;
                        ALTER TABLE [dbo].[AuditEvents] ALTER COLUMN [EventDate] DATETIME2 NOT NULL;
                    END
                END
            ");
        }

        
        
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the DEFAULT constraints
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_AuditEvents_EventId')
                BEGIN
                    ALTER TABLE [dbo].[AuditEvents] DROP CONSTRAINT [DF_AuditEvents_EventId];
                END

                IF EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_AuditEvents_EventDate')
                BEGIN
                    ALTER TABLE [dbo].[AuditEvents] DROP CONSTRAINT [DF_AuditEvents_EventDate];
                END
            ");
        }
    }
}
