-- SQL script to create AuditLogs table for SQL Server
CREATE TABLE [dbo].[AuditLogs]
(
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [EventType] NVARCHAR(200) NULL,
    [UserId] UNIQUEIDENTIFIER NULL,
    [UserName] NVARCHAR(200) NULL,
    [Data] NVARCHAR(MAX) NULL,
    [IpAddress] NVARCHAR(45) NULL,
    [CorrelationId] NVARCHAR(100) NULL,
    [OccurredAt] DATETIME2 NOT NULL
);
