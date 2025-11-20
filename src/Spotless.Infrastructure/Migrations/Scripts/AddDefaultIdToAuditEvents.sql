-- SQL script to add DEFAULT NEWID() to existing AuditEvents table
ALTER TABLE [dbo].[AuditEvents] 
ADD CONSTRAINT [DF_AuditEvents_EventId] DEFAULT NEWID() FOR [EventId];
