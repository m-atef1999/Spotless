-- SQL script to create AuditEvents table for SQL Server used by Audit.NET
CREATE TABLE [dbo].[AuditEvents]
(
    [EventId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [EventDate] DATETIME2 NOT NULL,
    [EventType] NVARCHAR(200) NULL,
    [Environment] NVARCHAR(100) NULL,
    [UserName] NVARCHAR(200) NULL,
    [EventData] NVARCHAR(MAX) NULL,
    [LastUpdatedDate] DATETIME2 NULL
);

-- Optional: index on EventDate
CREATE INDEX IX_AuditEvents_EventDate ON [dbo].[AuditEvents] ([EventDate]);
