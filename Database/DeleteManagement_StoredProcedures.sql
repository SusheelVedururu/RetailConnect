-- =============================================
-- RetailConnect - Delete Management Stored Procedures
-- Version: 1.0
-- Created: 2026-01-29
-- Description: Stored procedures for dependency checks and delete operations
-- =============================================

USE RetailConnect;
GO

PRINT '========================================'
PRINT 'Delete Management - Stored Procedures'
PRINT '========================================'
GO

-- =============================================
-- DEPENDENCY CHECK PROCEDURES
-- =============================================

-- Drop existing procedures if they exist
IF OBJECT_ID('dbo.usp_GetSegmentDependencies', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetSegmentDependencies;
IF OBJECT_ID('dbo.usp_GetTouchpointDependencies', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetTouchpointDependencies;
IF OBJECT_ID('dbo.usp_GetTemplateDependencies', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetTemplateDependencies;
IF OBJECT_ID('dbo.usp_GetCampaignDependencies', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetCampaignDependencies;
IF OBJECT_ID('dbo.usp_SoftDeleteEntity', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_SoftDeleteEntity;
IF OBJECT_ID('dbo.usp_HardDeleteEntity', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_HardDeleteEntity;
IF OBJECT_ID('dbo.usp_RestoreEntity', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_RestoreEntity;
IF OBJECT_ID('dbo.usp_LogDeleteAction', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_LogDeleteAction;
GO

-- =============================================
-- 1. Get Segment Dependencies
-- Returns campaigns that reference this segment
-- =============================================
CREATE PROCEDURE dbo.usp_GetSegmentDependencies
    @SegmentId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if segment exists and get current status
    DECLARE @CurrentStatus TINYINT, @SegmentName NVARCHAR(200);
    SELECT @CurrentStatus = DeleteStatus, @SegmentName = Name
    FROM RetailConnect.T_Segments 
    WHERE SegmentID = @SegmentId;
    
    IF @CurrentStatus IS NULL
    BEGIN
        SELECT 
            0 AS EntityExists,
            NULL AS EntityName,
            NULL AS CurrentStatus,
            0 AS TotalDependencies;
        RETURN;
    END
    
    -- Get dependency counts
    SELECT 
        1 AS EntityExists,
        @SegmentName AS EntityName,
        @CurrentStatus AS CurrentStatus,
        (SELECT COUNT(*) FROM RetailConnect.T_Campaign 
         WHERE SegmentID = @SegmentId AND DeleteStatus = 0) AS CampaignCount;
         
    -- Get sample campaign IDs (max 10)
    SELECT TOP 10 CampaignID AS DependentId, CampaignName AS DependentName, 'Campaign' AS DependentType
    FROM RetailConnect.T_Campaign 
    WHERE SegmentID = @SegmentId AND DeleteStatus = 0
    ORDER BY CampaignID;
END
GO
PRINT '✓ Created usp_GetSegmentDependencies';
GO

-- =============================================
-- 2. Get Touchpoint Dependencies
-- Returns campaign-touchpoint mappings that reference this touchpoint
-- =============================================
CREATE PROCEDURE dbo.usp_GetTouchpointDependencies
    @TouchpointId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @CurrentStatus TINYINT, @TouchpointName NVARCHAR(200);
    SELECT @CurrentStatus = DeleteStatus, @TouchpointName = TouchPoint
    FROM RetailConnect.T_Touchpoints 
    WHERE TouchpointID = @TouchpointId;
    
    IF @CurrentStatus IS NULL
    BEGIN
        SELECT 0 AS EntityExists, NULL AS EntityName, NULL AS CurrentStatus;
        RETURN;
    END
    
    SELECT 
        1 AS EntityExists,
        @TouchpointName AS EntityName,
        @CurrentStatus AS CurrentStatus,
        (SELECT COUNT(*) FROM RetailConnect.T_CampaignTouchpoints ct
         INNER JOIN RetailConnect.T_Campaign c ON ct.CampaignID = c.CampaignID
         WHERE ct.TouchpointID = @TouchpointId AND c.DeleteStatus = 0) AS CampaignTouchpointCount;
         
    -- Get sample campaigns using this touchpoint
    SELECT TOP 10 c.CampaignID AS DependentId, c.CampaignName AS DependentName, 'Campaign' AS DependentType
    FROM RetailConnect.T_CampaignTouchpoints ct
    INNER JOIN RetailConnect.T_Campaign c ON ct.CampaignID = c.CampaignID
    WHERE ct.TouchpointID = @TouchpointId AND c.DeleteStatus = 0
    ORDER BY c.CampaignID;
END
GO
PRINT '✓ Created usp_GetTouchpointDependencies';
GO

-- =============================================
-- 3. Get Template Dependencies
-- Returns campaign-template mappings that reference this template
-- =============================================
CREATE PROCEDURE dbo.usp_GetTemplateDependencies
    @TemplateId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @CurrentStatus TINYINT, @TemplateName NVARCHAR(200);
    SELECT @CurrentStatus = DeleteStatus, @TemplateName = TemplateName
    FROM RetailConnect.T_TemplateVersions 
    WHERE TemplateVersionID = @TemplateId;
    
    IF @CurrentStatus IS NULL
    BEGIN
        SELECT 0 AS EntityExists, NULL AS EntityName, NULL AS CurrentStatus;
        RETURN;
    END
    
    SELECT 
        1 AS EntityExists,
        @TemplateName AS EntityName,
        @CurrentStatus AS CurrentStatus,
        (SELECT COUNT(*) FROM RetailConnect.T_CampaignTemplates ct
         INNER JOIN RetailConnect.T_Campaign c ON ct.CampaignID = c.CampaignID
         WHERE ct.TemplateVersionID = @TemplateId AND c.DeleteStatus = 0) AS CampaignTemplateCount;
         
    SELECT TOP 10 c.CampaignID AS DependentId, c.CampaignName AS DependentName, 'Campaign' AS DependentType
    FROM RetailConnect.T_CampaignTemplates ct
    INNER JOIN RetailConnect.T_Campaign c ON ct.CampaignID = c.CampaignID
    WHERE ct.TemplateVersionID = @TemplateId AND c.DeleteStatus = 0
    ORDER BY c.CampaignID;
END
GO
PRINT '✓ Created usp_GetTemplateDependencies';
GO

-- =============================================
-- 4. Get Campaign Dependencies
-- Returns child records (touchpoints, templates, logs)
-- =============================================
CREATE PROCEDURE dbo.usp_GetCampaignDependencies
    @CampaignId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @CurrentStatus TINYINT, @CampaignName NVARCHAR(200);
    SELECT @CurrentStatus = DeleteStatus, @CampaignName = CampaignName
    FROM RetailConnect.T_Campaign 
    WHERE CampaignID = @CampaignId;
    
    IF @CurrentStatus IS NULL
    BEGIN
        SELECT 0 AS EntityExists, NULL AS EntityName, NULL AS CurrentStatus;
        RETURN;
    END
    
    -- Campaigns can always be deleted (children cascade)
    SELECT 
        1 AS EntityExists,
        @CampaignName AS EntityName,
        @CurrentStatus AS CurrentStatus,
        (SELECT COUNT(*) FROM RetailConnect.T_CampaignTouchpoints WHERE CampaignID = @CampaignId) AS TouchpointMappingCount,
        (SELECT COUNT(*) FROM RetailConnect.T_CampaignTemplates WHERE CampaignID = @CampaignId) AS TemplateMappingCount,
        (SELECT COUNT(*) FROM RetailConnect.T_CampaignLog WHERE CampaignID = @CampaignId) AS LogCount;
END
GO
PRINT '✓ Created usp_GetCampaignDependencies';
GO

-- =============================================
-- DELETE OPERATION PROCEDURES
-- =============================================

-- =============================================
-- 5. Soft Delete Entity (Generic)
-- =============================================
CREATE PROCEDURE dbo.usp_SoftDeleteEntity
    @EntityType NVARCHAR(50),
    @EntityId INT,
    @DeletedBy NVARCHAR(100),
    @DeleteReason NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @RowsAffected INT = 0;
        DECLARE @EntityName NVARCHAR(200);
        DECLARE @PreviousStatus TINYINT;
        
        IF @EntityType = 'Segment'
        BEGIN
            SELECT @PreviousStatus = DeleteStatus, @EntityName = Name 
            FROM RetailConnect.T_Segments WHERE SegmentID = @EntityId;
            
            IF @PreviousStatus IS NULL
                THROW 50001, 'Segment not found', 1;
            IF @PreviousStatus != 0
                THROW 50002, 'Segment is already deleted', 1;
            
            UPDATE RetailConnect.T_Segments
            SET DeleteStatus = 1,
                DeletedOn = GETUTCDATE(),
                DeletedBy = @DeletedBy,
                DeleteReason = @DeleteReason,
                ModifiedDate = GETUTCDATE()
            WHERE SegmentID = @EntityId AND DeleteStatus = 0;
            SET @RowsAffected = @@ROWCOUNT;
        END
        ELSE IF @EntityType = 'Campaign'
        BEGIN
            SELECT @PreviousStatus = DeleteStatus, @EntityName = CampaignName 
            FROM RetailConnect.T_Campaign WHERE CampaignID = @EntityId;
            
            IF @PreviousStatus IS NULL
                THROW 50001, 'Campaign not found', 1;
            IF @PreviousStatus != 0
                THROW 50002, 'Campaign is already deleted', 1;
            
            UPDATE RetailConnect.T_Campaign
            SET DeleteStatus = 1,
                DeletedOn = GETUTCDATE(),
                DeletedBy = @DeletedBy,
                DeleteReason = @DeleteReason,
                ModifiedDate = GETUTCDATE()
            WHERE CampaignID = @EntityId AND DeleteStatus = 0;
            SET @RowsAffected = @@ROWCOUNT;
        END
        ELSE IF @EntityType = 'Touchpoint'
        BEGIN
            SELECT @PreviousStatus = DeleteStatus, @EntityName = TouchPoint 
            FROM RetailConnect.T_Touchpoints WHERE TouchpointID = @EntityId;
            
            IF @PreviousStatus IS NULL
                THROW 50001, 'Touchpoint not found', 1;
            IF @PreviousStatus != 0
                THROW 50002, 'Touchpoint is already deleted', 1;
            
            UPDATE RetailConnect.T_Touchpoints
            SET DeleteStatus = 1,
                DeletedOn = GETUTCDATE(),
                DeletedBy = @DeletedBy,
                DeleteReason = @DeleteReason,
                ModifiedDate = GETUTCDATE()
            WHERE TouchpointID = @EntityId AND DeleteStatus = 0;
            SET @RowsAffected = @@ROWCOUNT;
        END
        ELSE IF @EntityType = 'Template'
        BEGIN
            SELECT @PreviousStatus = DeleteStatus, @EntityName = TemplateName 
            FROM RetailConnect.T_TemplateVersions WHERE TemplateVersionID = @EntityId;
            
            IF @PreviousStatus IS NULL
                THROW 50001, 'Template not found', 1;
            IF @PreviousStatus != 0
                THROW 50002, 'Template is already deleted', 1;
            
            UPDATE RetailConnect.T_TemplateVersions
            SET DeleteStatus = 1,
                DeletedOn = GETUTCDATE(),
                DeletedBy = @DeletedBy,
                DeleteReason = @DeleteReason,
                ModifiedDate = GETUTCDATE()
            WHERE TemplateVersionID = @EntityId AND DeleteStatus = 0;
            SET @RowsAffected = @@ROWCOUNT;
        END
        ELSE
            THROW 50003, 'Invalid entity type', 1;
        
        -- Log the action
        INSERT INTO RetailConnect.T_DeleteAuditLog 
            (EntityType, EntityID, EntityName, DeleteAction, PreviousStatus, NewStatus, DeletedBy, DeleteReason)
        VALUES 
            (@EntityType, @EntityId, @EntityName, 'SoftDelete', 0, 1, @DeletedBy, @DeleteReason);
        
        COMMIT TRANSACTION;
        
        SELECT @RowsAffected AS RowsAffected, 1 AS NewStatus, 'Soft delete successful' AS Message;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
PRINT '✓ Created usp_SoftDeleteEntity';
GO

-- =============================================
-- 6. Hard Delete Entity (Marks for Purge)
-- =============================================
CREATE PROCEDURE dbo.usp_HardDeleteEntity
    @EntityType NVARCHAR(50),
    @EntityId INT,
    @DeletedBy NVARCHAR(100),
    @DeleteReason NVARCHAR(500) = NULL,
    @ForceDelete BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @RowsAffected INT = 0;
        DECLARE @EntityName NVARCHAR(200);
        DECLARE @PreviousStatus TINYINT;
        DECLARE @DependencyCount INT = 0;
        
        -- Check dependencies first (except for Campaign which cascades)
        IF @EntityType = 'Segment'
        BEGIN
            SELECT @DependencyCount = COUNT(*) 
            FROM RetailConnect.T_Campaign 
            WHERE SegmentID = @EntityId AND DeleteStatus = 0;
            
            IF @DependencyCount > 0 AND @ForceDelete = 0
                THROW 50004, 'Cannot hard delete: Active campaigns reference this segment', 1;
        END
        ELSE IF @EntityType = 'Touchpoint'
        BEGIN
            SELECT @DependencyCount = COUNT(*) 
            FROM RetailConnect.T_CampaignTouchpoints ct
            INNER JOIN RetailConnect.T_Campaign c ON ct.CampaignID = c.CampaignID
            WHERE ct.TouchpointID = @EntityId AND c.DeleteStatus = 0;
            
            IF @DependencyCount > 0 AND @ForceDelete = 0
                THROW 50004, 'Cannot hard delete: Active campaigns use this touchpoint', 1;
        END
        ELSE IF @EntityType = 'Template'
        BEGIN
            SELECT @DependencyCount = COUNT(*) 
            FROM RetailConnect.T_CampaignTemplates ct
            INNER JOIN RetailConnect.T_Campaign c ON ct.CampaignID = c.CampaignID
            WHERE ct.TemplateVersionID = @EntityId AND c.DeleteStatus = 0;
            
            IF @DependencyCount > 0 AND @ForceDelete = 0
                THROW 50004, 'Cannot hard delete: Active campaigns use this template', 1;
        END
        
        -- Perform the hard delete (set status to 2)
        IF @EntityType = 'Segment'
        BEGIN
            SELECT @PreviousStatus = DeleteStatus, @EntityName = Name 
            FROM RetailConnect.T_Segments WHERE SegmentID = @EntityId;
            
            IF @PreviousStatus IS NULL
                THROW 50001, 'Segment not found', 1;
            IF @PreviousStatus = 2
                THROW 50002, 'Segment is already marked for purge', 1;
            
            UPDATE RetailConnect.T_Segments
            SET DeleteStatus = 2,
                DeletedOn = COALESCE(DeletedOn, GETUTCDATE()),
                DeletedBy = COALESCE(DeletedBy, @DeletedBy),
                DeleteReason = COALESCE(DeleteReason, @DeleteReason),
                ModifiedDate = GETUTCDATE()
            WHERE SegmentID = @EntityId;
            SET @RowsAffected = @@ROWCOUNT;
        END
        ELSE IF @EntityType = 'Campaign'
        BEGIN
            SELECT @PreviousStatus = DeleteStatus, @EntityName = CampaignName 
            FROM RetailConnect.T_Campaign WHERE CampaignID = @EntityId;
            
            IF @PreviousStatus IS NULL
                THROW 50001, 'Campaign not found', 1;
            IF @PreviousStatus = 2
                THROW 50002, 'Campaign is already marked for purge', 1;
            
            UPDATE RetailConnect.T_Campaign
            SET DeleteStatus = 2,
                DeletedOn = COALESCE(DeletedOn, GETUTCDATE()),
                DeletedBy = COALESCE(DeletedBy, @DeletedBy),
                DeleteReason = COALESCE(DeleteReason, @DeleteReason),
                ModifiedDate = GETUTCDATE()
            WHERE CampaignID = @EntityId;
            SET @RowsAffected = @@ROWCOUNT;
        END
        ELSE IF @EntityType = 'Touchpoint'
        BEGIN
            SELECT @PreviousStatus = DeleteStatus, @EntityName = TouchPoint 
            FROM RetailConnect.T_Touchpoints WHERE TouchpointID = @EntityId;
            
            IF @PreviousStatus IS NULL
                THROW 50001, 'Touchpoint not found', 1;
            IF @PreviousStatus = 2
                THROW 50002, 'Touchpoint is already marked for purge', 1;
            
            UPDATE RetailConnect.T_Touchpoints
            SET DeleteStatus = 2,
                DeletedOn = COALESCE(DeletedOn, GETUTCDATE()),
                DeletedBy = COALESCE(DeletedBy, @DeletedBy),
                DeleteReason = COALESCE(DeleteReason, @DeleteReason),
                ModifiedDate = GETUTCDATE()
            WHERE TouchpointID = @EntityId;
            SET @RowsAffected = @@ROWCOUNT;
        END
        ELSE IF @EntityType = 'Template'
        BEGIN
            SELECT @PreviousStatus = DeleteStatus, @EntityName = TemplateName 
            FROM RetailConnect.T_TemplateVersions WHERE TemplateVersionID = @EntityId;
            
            IF @PreviousStatus IS NULL
                THROW 50001, 'Template not found', 1;
            IF @PreviousStatus = 2
                THROW 50002, 'Template is already marked for purge', 1;
            
            UPDATE RetailConnect.T_TemplateVersions
            SET DeleteStatus = 2,
                DeletedOn = COALESCE(DeletedOn, GETUTCDATE()),
                DeletedBy = COALESCE(DeletedBy, @DeletedBy),
                DeleteReason = COALESCE(DeleteReason, @DeleteReason),
                ModifiedDate = GETUTCDATE()
            WHERE TemplateVersionID = @EntityId;
            SET @RowsAffected = @@ROWCOUNT;
        END
        ELSE
            THROW 50003, 'Invalid entity type', 1;
        
        -- Log the action
        INSERT INTO RetailConnect.T_DeleteAuditLog 
            (EntityType, EntityID, EntityName, DeleteAction, PreviousStatus, NewStatus, DeletedBy, DeleteReason)
        VALUES 
            (@EntityType, @EntityId, @EntityName, 'HardDelete', @PreviousStatus, 2, @DeletedBy, @DeleteReason);
        
        COMMIT TRANSACTION;
        
        SELECT @RowsAffected AS RowsAffected, 2 AS NewStatus, 'Hard delete successful - marked for purge' AS Message;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
PRINT '✓ Created usp_HardDeleteEntity';
GO

-- =============================================
-- 7. Restore Entity (Soft Delete -> Active)
-- =============================================
CREATE PROCEDURE dbo.usp_RestoreEntity
    @EntityType NVARCHAR(50),
    @EntityId INT,
    @RestoredBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @RowsAffected INT = 0;
        DECLARE @EntityName NVARCHAR(200);
        DECLARE @PreviousStatus TINYINT;
        
        IF @EntityType = 'Segment'
        BEGIN
            SELECT @PreviousStatus = DeleteStatus, @EntityName = Name 
            FROM RetailConnect.T_Segments WHERE SegmentID = @EntityId;
            
            IF @PreviousStatus IS NULL
                THROW 50001, 'Segment not found', 1;
            IF @PreviousStatus = 0
                THROW 50005, 'Segment is already active', 1;
            IF @PreviousStatus = 2
                THROW 50006, 'Cannot restore: Segment is marked for permanent deletion', 1;
            
            UPDATE RetailConnect.T_Segments
            SET DeleteStatus = 0,
                DeletedOn = NULL,
                DeletedBy = NULL,
                DeleteReason = NULL,
                ModifiedDate = GETUTCDATE()
            WHERE SegmentID = @EntityId;
            SET @RowsAffected = @@ROWCOUNT;
        END
        ELSE IF @EntityType = 'Campaign'
        BEGIN
            SELECT @PreviousStatus = DeleteStatus, @EntityName = CampaignName 
            FROM RetailConnect.T_Campaign WHERE CampaignID = @EntityId;
            
            IF @PreviousStatus IS NULL
                THROW 50001, 'Campaign not found', 1;
            IF @PreviousStatus = 0
                THROW 50005, 'Campaign is already active', 1;
            IF @PreviousStatus = 2
                THROW 50006, 'Cannot restore: Campaign is marked for permanent deletion', 1;
            
            UPDATE RetailConnect.T_Campaign
            SET DeleteStatus = 0,
                DeletedOn = NULL,
                DeletedBy = NULL,
                DeleteReason = NULL,
                ModifiedDate = GETUTCDATE()
            WHERE CampaignID = @EntityId;
            SET @RowsAffected = @@ROWCOUNT;
        END
        ELSE IF @EntityType = 'Touchpoint'
        BEGIN
            SELECT @PreviousStatus = DeleteStatus, @EntityName = TouchPoint 
            FROM RetailConnect.T_Touchpoints WHERE TouchpointID = @EntityId;
            
            IF @PreviousStatus IS NULL
                THROW 50001, 'Touchpoint not found', 1;
            IF @PreviousStatus = 0
                THROW 50005, 'Touchpoint is already active', 1;
            IF @PreviousStatus = 2
                THROW 50006, 'Cannot restore: Touchpoint is marked for permanent deletion', 1;
            
            UPDATE RetailConnect.T_Touchpoints
            SET DeleteStatus = 0,
                DeletedOn = NULL,
                DeletedBy = NULL,
                DeleteReason = NULL,
                ModifiedDate = GETUTCDATE()
            WHERE TouchpointID = @EntityId;
            SET @RowsAffected = @@ROWCOUNT;
        END
        ELSE IF @EntityType = 'Template'
        BEGIN
            SELECT @PreviousStatus = DeleteStatus, @EntityName = TemplateName 
            FROM RetailConnect.T_TemplateVersions WHERE TemplateVersionID = @EntityId;
            
            IF @PreviousStatus IS NULL
                THROW 50001, 'Template not found', 1;
            IF @PreviousStatus = 0
                THROW 50005, 'Template is already active', 1;
            IF @PreviousStatus = 2
                THROW 50006, 'Cannot restore: Template is marked for permanent deletion', 1;
            
            UPDATE RetailConnect.T_TemplateVersions
            SET DeleteStatus = 0,
                DeletedOn = NULL,
                DeletedBy = NULL,
                DeleteReason = NULL,
                ModifiedDate = GETUTCDATE()
            WHERE TemplateVersionID = @EntityId;
            SET @RowsAffected = @@ROWCOUNT;
        END
        ELSE
            THROW 50003, 'Invalid entity type', 1;
        
        -- Log the action
        INSERT INTO RetailConnect.T_DeleteAuditLog 
            (EntityType, EntityID, EntityName, DeleteAction, PreviousStatus, NewStatus, DeletedBy, DeleteReason)
        VALUES 
            (@EntityType, @EntityId, @EntityName, 'Restore', @PreviousStatus, 0, @RestoredBy, 'Restored from soft delete');
        
        COMMIT TRANSACTION;
        
        SELECT @RowsAffected AS RowsAffected, 0 AS NewStatus, 'Restore successful' AS Message;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
PRINT '✓ Created usp_RestoreEntity';
GO

-- =============================================
-- VERIFICATION
-- =============================================

PRINT '';
PRINT '========================================'
PRINT 'Stored Procedures Created:'
PRINT '========================================'

SELECT 
    name AS ProcedureName,
    create_date AS CreatedDate
FROM sys.procedures
WHERE name LIKE 'usp_%Delete%' OR name LIKE 'usp_%Dependencies%' OR name LIKE 'usp_%Restore%'
ORDER BY name;

PRINT '';
PRINT 'Delete Management Stored Procedures Complete!';
PRINT '========================================'
GO
