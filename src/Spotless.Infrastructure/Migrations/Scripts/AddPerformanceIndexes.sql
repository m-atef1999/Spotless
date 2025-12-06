-- ============================================================
-- Spotless Database - Performance Indexes Script
-- ============================================================
-- Run this script manually on your database to add indexes
-- that improve query performance.
-- 
-- These indexes are in ADDITION to the ones already created
-- by EF Core migrations.
-- ============================================================

-- ======================
-- SERVICES TABLE
-- (Already has: IX_Service_CategoryId)
-- ======================

-- Index for filtering active services
CREATE INDEX IX_Services_IsActive ON Services(IsActive);

-- Index for featured services query
CREATE INDEX IX_Services_IsFeatured ON Services(IsFeatured);


-- ======================
-- DRIVERS TABLE
-- (Already has: IX_Driver_AdminId, IX_Driver_Email_Unique)
-- ======================

-- Index for filtering by driver status
CREATE INDEX IX_Drivers_Status ON Drivers(Status);


-- ======================
-- NOTIFICATIONS TABLE
-- ======================

-- Index for fetching user's notifications
CREATE INDEX IX_Notifications_UserId ON Notifications(UserId);

-- Index for filtering unread notifications
CREATE INDEX IX_Notifications_IsRead ON Notifications(IsRead);

-- Index for ordering by created date
CREATE INDEX IX_Notifications_CreatedAt ON Notifications(CreatedAt);


-- ======================
-- REVIEWS TABLE
-- (Already has: IX on CustomerId, OrderId)
-- ======================

-- Index for looking up driver's reviews
CREATE INDEX IX_Reviews_DriverId ON Reviews(DriverId);


-- ======================
-- AUDIT LOGS TABLE
-- ======================

-- Index for filtering by user
CREATE INDEX IX_AuditLogs_UserId ON AuditLogs(UserId);

-- Index for filtering by event type (nvarchar(200) - indexable)
CREATE INDEX IX_AuditLogs_EventType ON AuditLogs(EventType);

-- Index for filtering/sorting by occurrence date
CREATE INDEX IX_AuditLogs_OccurredAt ON AuditLogs(OccurredAt);


-- ======================
-- ORDERS TABLE
-- (Already has: IX_Order_CustomerId, IX_Order_DriverId, IX_Order_Status, IX_Order_Scheduled)
-- ======================

-- Index for order date queries (reporting)
CREATE INDEX IX_Orders_OrderDate ON Orders(OrderDate);


-- ======================
-- PAYMENTS TABLE
-- (Already has: IX_Payment_AdminId, IX_Payment_CustomerId, IX_Payment_TransactionId)
-- ======================

-- Index for payment date queries
CREATE INDEX IX_Payments_PaymentDate ON Payments(PaymentDate);

-- Index for payment status filtering
CREATE INDEX IX_Payments_Status ON Payments(Status);


-- ============================================================
-- VERIFICATION (run after to confirm indexes were created)
-- ============================================================
-- SELECT name, type_desc FROM sys.indexes WHERE name LIKE 'IX_%' ORDER BY name;
