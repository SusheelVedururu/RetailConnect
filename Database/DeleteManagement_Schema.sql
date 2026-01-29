-- =============================================
-- RetailConnect - Delete Management System Schema
-- Version: 1.0
-- Created: 2026-01-29
-- Description: Adds soft delete and hard delete support
-- =============================================

USE RetailConnect;
GO

PRINT '========================================'
PRINT 'Delete Management System - Schema Setup'
PRINT '========================================'
GO

-- =============================================
-- PHASE 1: Add Delete Columns to All Tables
-- =============================================

-- 1. T_Campaign
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('RetailConnect.T_Campaign') AND name = 'DeleteStatus')
BEGIN
    ALTER TABLE RetailConnect.T_Campaign ADD 
        DeleteStatus TINYINT NOT NULL CONSTRAINT DF_Campaign_DeleteStatus DEFAULT 0,
        DeletedOn DATETIME2 NULL,
        DeletedBy NVARCHAR(100) NULL,
        DeleteReason NVARCHAR(500) NULL;
    PRINT '✓ Added delete columns to T_Campaign';
END
ELSE
    PRINT '○ T_Campaign already has delete columns';
GO

-- 2. T_Segments
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('RetailConnect.T_Segments') AND name = 'DeleteStatus')
BEGIN
    ALTER TABLE RetailConnect.T_Segments ADD 
        DeleteStatus TINYINT NOT NULL CONSTRAINT DF_Segments_DeleteStatus DEFAULT 0,
        DeletedOn DATETIME2 NULL,
        DeletedBy NVARCHAR(100) NULL,
        DeleteReason NVARCHAR(500) NULL;
    PRINT '✓ Added delete columns to T_Segments';
END
ELSE
    PRINT '○ T_Segments already has delete columns';
GO

-- 3. T_Touchpoints
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('RetailConnect.T_Touchpoints') AND name = 'DeleteStatus')
BEGIN
    ALTER TABLE RetailConnect.T_Touchpoints ADD 
        DeleteStatus TINYINT NOT NULL CONSTRAINT DF_Touchpoints_DeleteStatus DEFAULT 0,
        DeletedOn DATETIME2 NULL,
        DeletedBy NVARCHAR(100) NULL,
        DeleteReason NVARCHAR(500) NULL;
    PRINT '✓ Added delete columns to T_Touchpoints';
END
ELSE
    PRINT '○ T_Touchpoints already has delete columns';
GO

-- 4. T_TemplateVersions
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('RetailConnect.T_TemplateVersions') AND name = 'DeleteStatus')
BEGIN
    ALTER TABLE RetailConnect.T_TemplateVersions ADD 
        DeleteStatus TINYINT NOT NULL CONSTRAINT DF_TemplateVersions_DeleteStatus DEFAULT 0,
        DeletedOn DATETIME2 NULL,
        DeletedBy NVARCHAR(100) NULL,
        DeleteReason NVARCHAR(500) NULL;
    PRINT '✓ Added delete columns to T_TemplateVersions';
END
ELSE
    PRINT '○ T_TemplateVersions already has delete columns';
GO

-- =============================================
-- PHASE 2: Create Filtered Indexes for Performance
-- =============================================

PRINT '';
PRINT 'Creating filtered indexes...';
GO

-- Index for Campaign (filter active records)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Campaign_DeleteStatus_Active' AND object_id = OBJECT_ID('RetailConnect.T_Campaign'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Campaign_DeleteStatus_Active 
        ON RetailConnect.T_Campaign (DeleteStatus) 
        INCLUDE (CampaignID, CampaignName, IsActive)
        WHERE DeleteStatus = 0;
    PRINT '✓ Created IX_Campaign_DeleteStatus_Active';
END
GO

-- Index for Segments (filter active records)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Segments_DeleteStatus_Active' AND object_id = OBJECT_ID('RetailConnect.T_Segments'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Segments_DeleteStatus_Active 
        ON RetailConnect.T_Segments (DeleteStatus) 
        INCLUDE (SegmentID, Name, IsActive)
        WHERE DeleteStatus = 0;
    PRINT '✓ Created IX_Segments_DeleteStatus_Active';
END
GO

-- Index for Touchpoints (filter active records)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Touchpoints_DeleteStatus_Active' AND object_id = OBJECT_ID('RetailConnect.T_Touchpoints'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Touchpoints_DeleteStatus_Active 
        ON RetailConnect.T_Touchpoints (DeleteStatus) 
        INCLUDE (TouchpointID, TouchPoint, IsActive)
        WHERE DeleteStatus = 0;
    PRINT '✓ Created IX_Touchpoints_DeleteStatus_Active';
END
GO

-- Index for TemplateVersions (filter active records)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_TemplateVersions_DeleteStatus_Active' AND object_id = OBJECT_ID('RetailConnect.T_TemplateVersions'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_TemplateVersions_DeleteStatus_Active 
        ON RetailConnect.T_TemplateVersions (DeleteStatus) 
        INCLUDE (TemplateVersionID, TemplateName, IsActive)
        WHERE DeleteStatus = 0;
    PRINT '✓ Created IX_TemplateVersions_DeleteStatus_Active';
END
GO

-- =============================================
-- PHASE 3: Create Delete Audit Log Table
-- =============================================

PRINT '';
PRINT 'Creating audit log table...';
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID('RetailConnect.T_DeleteAuditLog') AND type = 'U')
BEGIN
    CREATE TABLE RetailConnect.T_DeleteAuditLog (
        AuditLogID BIGINT IDENTITY(1,1) PRIMARY KEY,
        EntityType NVARCHAR(50) NOT NULL,       -- Campaign, Segment, Touchpoint, Template
        EntityID INT NOT NULL,
        EntityName NVARCHAR(200) NULL,
        DeleteAction NVARCHAR(20) NOT NULL,     -- SoftDelete, HardDelete, Restore, Purge
        PreviousStatus TINYINT NOT NULL,
        NewStatus TINYINT NOT NULL,
        DeletedBy NVARCHAR(100) NOT NULL,
        DeleteReason NVARCHAR(500) NULL,
        DependencySnapshot NVARCHAR(MAX) NULL,  -- JSON snapshot of dependencies at delete time
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        
        INDEX IX_DeleteAuditLog_Entity (EntityType, EntityID),
        INDEX IX_DeleteAuditLog_Date (CreatedDate DESC)
    );
    PRINT '✓ Created T_DeleteAuditLog table';
END
ELSE
    PRINT '○ T_DeleteAuditLog already exists';
GO

-- =============================================
-- VERIFICATION
-- =============================================

PRINT '';
PRINT '========================================'
PRINT 'Schema Verification'
PRINT '========================================'

SELECT 
    t.name AS TableName,
    c.name AS ColumnName,
    ty.name AS DataType,
    c.is_nullable AS IsNullable
FROM sys.tables t
INNER JOIN sys.columns c ON t.object_id = c.object_id
INNER JOIN sys.types ty ON c.user_type_id = ty.user_type_id
WHERE t.schema_id = SCHEMA_ID('RetailConnect')
    AND c.name IN ('DeleteStatus', 'DeletedOn', 'DeletedBy', 'DeleteReason')
ORDER BY t.name, c.name;

PRINT '';
PRINT '========================================'
PRINT 'Delete Management Schema Setup Complete!'
PRINT '========================================'
PRINT '';
PRINT 'Next Steps:';
PRINT '1. Run DeleteManagement_StoredProcedures.sql';
PRINT '2. Deploy C# code changes';
PRINT '3. Test delete endpoints';
GO
