-- =============================================
-- RetailConnect - Update Read Queries for Delete Support
-- Version: 1.0
-- Created: 2026-01-29
-- Description: Updates all GetAll* stored procedures to filter DeleteStatus = 0
-- =============================================

USE RetailConnect;
GO

PRINT '========================================'
PRINT 'Updating Read Queries for Delete Support'
PRINT '========================================'
GO

-- =============================================
-- 1. Update usp_GetAllSegments
-- =============================================
IF OBJECT_ID('dbo.usp_GetAllSegments', 'P') IS NOT NULL 
    DROP PROCEDURE dbo.usp_GetAllSegments;
GO

CREATE PROCEDURE dbo.usp_GetAllSegments
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        SegmentID AS Id,
        Name,
        IsActive,
        0 AS MemberCount  -- Placeholder for actual member count
    FROM RetailConnect.T_Segments
    WHERE DeleteStatus = 0  -- Only active (non-deleted) records
    ORDER BY Name;
END
GO
PRINT '✓ Updated: usp_GetAllSegments';
GO

-- =============================================
-- 2. Update usp_GetSegmentById
-- =============================================
IF OBJECT_ID('dbo.usp_GetSegmentById', 'P') IS NOT NULL 
    DROP PROCEDURE dbo.usp_GetSegmentById;
GO

CREATE PROCEDURE dbo.usp_GetSegmentById
    @SegmentId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        SegmentID AS Id,
        Name,
        Description,
        Criteria,
        IsActive,
        CreatedDate,
        ModifiedDate
    FROM RetailConnect.T_Segments
    WHERE SegmentID = @SegmentId AND DeleteStatus = 0;
END
GO
PRINT '✓ Updated: usp_GetSegmentById';
GO

-- =============================================
-- 3. Update usp_GetAllCampaigns
-- =============================================
IF OBJECT_ID('dbo.usp_GetAllCampaigns', 'P') IS NOT NULL 
    DROP PROCEDURE dbo.usp_GetAllCampaigns;
GO

CREATE PROCEDURE dbo.usp_GetAllCampaigns
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        CampaignID AS Id,
        CampaignName AS Name,
        IsActive
    FROM RetailConnect.T_Campaign
    WHERE DeleteStatus = 0  -- Only active (non-deleted) records
    ORDER BY CampaignName;
END
GO
PRINT '✓ Updated: usp_GetAllCampaigns';
GO

-- =============================================
-- 4. Update usp_GetCampaignById
-- =============================================
IF OBJECT_ID('dbo.usp_GetCampaignById', 'P') IS NOT NULL 
    DROP PROCEDURE dbo.usp_GetCampaignById;
GO

CREATE PROCEDURE dbo.usp_GetCampaignById
    @CampaignId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        CampaignID AS Id,
        CampaignName AS Name,
        StartDate,
        EndDate,
        IsActive,
        CreatedDate,
        ModifiedDate
    FROM RetailConnect.T_Campaign
    WHERE CampaignID = @CampaignId AND DeleteStatus = 0;
END
GO
PRINT '✓ Updated: usp_GetCampaignById';
GO

-- =============================================
-- 5. Update usp_GetAllTouchpoints
-- =============================================
IF OBJECT_ID('dbo.usp_GetAllTouchpoints', 'P') IS NOT NULL 
    DROP PROCEDURE dbo.usp_GetAllTouchpoints;
GO

CREATE PROCEDURE dbo.usp_GetAllTouchpoints
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        TouchpointID AS Id,
        TouchPoint AS Name,
        TouchPointType AS Type,
        Configuration,
        IsActive,
        CreatedDate
    FROM RetailConnect.T_Touchpoints
    WHERE DeleteStatus = 0  -- Only active (non-deleted) records
    ORDER BY TouchPoint;
END
GO
PRINT '✓ Updated: usp_GetAllTouchpoints';
GO

-- =============================================
-- 6. Update usp_GetTouchpointById
-- =============================================
IF OBJECT_ID('dbo.usp_GetTouchpointById', 'P') IS NOT NULL 
    DROP PROCEDURE dbo.usp_GetTouchpointById;
GO

CREATE PROCEDURE dbo.usp_GetTouchpointById
    @TouchpointId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        TouchpointID AS Id,
        TouchPoint AS Name,
        TouchPointType AS Type,
        Configuration,
        IsActive,
        CreatedDate,
        ModifiedDate
    FROM RetailConnect.T_Touchpoints
    WHERE TouchpointID = @TouchpointId AND DeleteStatus = 0;
END
GO
PRINT '✓ Updated: usp_GetTouchpointById';
GO

-- =============================================
-- 7. Update usp_GetAllTemplates
-- =============================================
IF OBJECT_ID('dbo.usp_GetAllTemplates', 'P') IS NOT NULL 
    DROP PROCEDURE dbo.usp_GetAllTemplates;
GO

CREATE PROCEDURE dbo.usp_GetAllTemplates
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        TemplateVersionID AS Id,
        TemplateName AS Name,
        TemplateType AS Type,
        IsActive
    FROM RetailConnect.T_TemplateVersions
    WHERE DeleteStatus = 0  -- Only active (non-deleted) records
    ORDER BY TemplateName;
END
GO
PRINT '✓ Updated: usp_GetAllTemplates';
GO

-- =============================================
-- 8. Update usp_GetTemplateById
-- =============================================
IF OBJECT_ID('dbo.usp_GetTemplateById', 'P') IS NOT NULL 
    DROP PROCEDURE dbo.usp_GetTemplateById;
GO

CREATE PROCEDURE dbo.usp_GetTemplateById
    @TemplateId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        TemplateVersionID AS Id,
        TemplateName AS Name,
        Content,
        Subject,
        TemplateType AS Type,
        IsActive,
        CreatedDate,
        ModifiedDate
    FROM RetailConnect.T_TemplateVersions
    WHERE TemplateVersionID = @TemplateId AND DeleteStatus = 0;
END
GO
PRINT '✓ Updated: usp_GetTemplateById';
GO

-- =============================================
-- 9. Update usp_CheckSegmentExists (for uniqueness check)
-- =============================================
IF OBJECT_ID('dbo.usp_CheckSegmentExists', 'P') IS NOT NULL 
    DROP PROCEDURE dbo.usp_CheckSegmentExists;
GO

CREATE PROCEDURE dbo.usp_CheckSegmentExists
    @Name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Only check against non-deleted segments
    SELECT COUNT(1) AS [Count]
    FROM RetailConnect.T_Segments
    WHERE Name = @Name AND DeleteStatus = 0;
END
GO
PRINT '✓ Updated: usp_CheckSegmentExists';
GO

-- =============================================
-- ADMIN PROCEDURES (Include deleted records)
-- =============================================

-- Admin: Get All Segments (including deleted)
IF OBJECT_ID('dbo.usp_Admin_GetAllSegments', 'P') IS NOT NULL 
    DROP PROCEDURE dbo.usp_Admin_GetAllSegments;
GO

CREATE PROCEDURE dbo.usp_Admin_GetAllSegments
    @IncludeDeleted BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        SegmentID AS Id,
        Name,
        IsActive,
        DeleteStatus,
        DeletedOn,
        DeletedBy,
        DeleteReason,
        0 AS MemberCount
    FROM RetailConnect.T_Segments
    WHERE @IncludeDeleted = 1 OR DeleteStatus = 0
    ORDER BY DeleteStatus, Name;
END
GO
PRINT '✓ Created: usp_Admin_GetAllSegments';
GO

-- Admin: Get All Campaigns (including deleted)
IF OBJECT_ID('dbo.usp_Admin_GetAllCampaigns', 'P') IS NOT NULL 
    DROP PROCEDURE dbo.usp_Admin_GetAllCampaigns;
GO

CREATE PROCEDURE dbo.usp_Admin_GetAllCampaigns
    @IncludeDeleted BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        CampaignID AS Id,
        CampaignName AS Name,
        IsActive,
        DeleteStatus,
        DeletedOn,
        DeletedBy,
        DeleteReason
    FROM RetailConnect.T_Campaign
    WHERE @IncludeDeleted = 1 OR DeleteStatus = 0
    ORDER BY DeleteStatus, CampaignName;
END
GO
PRINT '✓ Created: usp_Admin_GetAllCampaigns';
GO

-- =============================================
-- VERIFICATION
-- =============================================
PRINT '';
PRINT '========================================'
PRINT 'Verification: Updated Stored Procedures'
PRINT '========================================'

SELECT 
    name AS ProcedureName,
    modify_date AS LastModified
FROM sys.procedures
WHERE name LIKE 'usp_Get%' OR name LIKE 'usp_Admin%' OR name LIKE 'usp_Check%'
ORDER BY name;

PRINT '';
PRINT 'Read queries updated to filter DeleteStatus = 0';
PRINT 'Admin procedures available for viewing deleted records';
PRINT '========================================'
GO
