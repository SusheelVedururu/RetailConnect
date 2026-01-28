-- =============================================
-- RETAILCONNECT - FIXED STORED PROCEDURES (ALL MODULES)
-- Matched to actual database schema
-- Created: 2026-01-28
-- =============================================

USE RetailConnect;
GO

PRINT '>>> STARTING FIXED SETUP <<<';

-- =============================================
-- CAMPAIGNS (T_Campaign)
-- =============================================
IF OBJECT_ID('dbo.usp_CreateCampaign', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_CreateCampaign;
IF OBJECT_ID('dbo.usp_GetCampaignById', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetCampaignById;
IF OBJECT_ID('dbo.usp_GetAllCampaigns', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetAllCampaigns;
IF OBJECT_ID('dbo.usp_UpdateCampaign', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_UpdateCampaign;
GO

CREATE PROCEDURE dbo.usp_CreateCampaign
    @Name NVARCHAR(100),
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO RetailConnect.T_Campaign (
        CampaignName, 
        Campaign, -- Required column? Defaulting to Name
        StartDate, 
        EndDate, 
        IsActive, 
        CreatedDate
    )
    VALUES (
        @Name, 
        @Name, -- Defaulting 'Campaign' code to Name
        @StartDate, 
        @EndDate, 
        @IsActive, 
        GETUTCDATE()
    );
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
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
        CreatedDate
    FROM RetailConnect.T_Campaign
    WHERE CampaignID = @CampaignId;
END
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
    ORDER BY CampaignName;
END
GO

CREATE PROCEDURE dbo.usp_UpdateCampaign
    @CampaignId INT,
    @Name NVARCHAR(100),
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE RetailConnect.T_Campaign
    SET 
        CampaignName = @Name,
        Campaign = @Name,
        StartDate = @StartDate,
        EndDate = @EndDate,
        IsActive = @IsActive,
        ModifiedDate = GETUTCDATE()
    WHERE CampaignID = @CampaignId;
END
GO
PRINT 'Campaign Procedures... FIXED';

-- =============================================
-- TEMPLATES (T_TemplateVersions)
-- =============================================
IF OBJECT_ID('dbo.usp_CreateTemplate', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_CreateTemplate;
IF OBJECT_ID('dbo.usp_GetTemplateById', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetTemplateById;
IF OBJECT_ID('dbo.usp_GetAllTemplates', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetAllTemplates;
IF OBJECT_ID('dbo.usp_UpdateTemplate', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_UpdateTemplate;
GO

CREATE PROCEDURE dbo.usp_CreateTemplate
    @Name NVARCHAR(100),
    @Content NVARCHAR(MAX) = NULL,
    @Subject NVARCHAR(200) = NULL,
    @Type NVARCHAR(50) = 'Email',
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO RetailConnect.T_TemplateVersions (
        TemplateName, 
        TemplateContent, 
        Subject, 
        TemplateType, 
        TemplateVersion, 
        IsActive, 
        CreatedDate
    )
    VALUES (
        @Name, 
        @Content, 
        @Subject, 
        @Type, 
        'v1', -- Default Version
        @IsActive, 
        GETUTCDATE()
    );
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO

CREATE PROCEDURE dbo.usp_GetTemplateById
    @TemplateId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        TemplateVersionID AS Id,
        TemplateName AS Name,
        TemplateContent AS Content,
        Subject,
        TemplateType AS Type,
        IsActive,
        CreatedDate
    FROM RetailConnect.T_TemplateVersions
    WHERE TemplateVersionID = @TemplateId;
END
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
    ORDER BY TemplateName;
END
GO

CREATE PROCEDURE dbo.usp_UpdateTemplate
    @TemplateId INT,
    @Name NVARCHAR(100),
    @Content NVARCHAR(MAX) = NULL,
    @Subject NVARCHAR(200) = NULL,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE RetailConnect.T_TemplateVersions
    SET 
        TemplateName = @Name,
        TemplateContent = @Content,
        Subject = @Subject,
        IsActive = @IsActive,
        ModifiedDate = GETUTCDATE()
    WHERE TemplateVersionID = @TemplateId;
END
GO
PRINT 'Template Procedures... FIXED';

-- =============================================
-- TOUCHPOINTS (T_Touchpoints)
-- =============================================
IF OBJECT_ID('dbo.usp_CreateTouchpoint', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_CreateTouchpoint;
IF OBJECT_ID('dbo.usp_GetTouchpointById', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetTouchpointById;
IF OBJECT_ID('dbo.usp_GetAllTouchpoints', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetAllTouchpoints;
IF OBJECT_ID('dbo.usp_UpdateTouchpoint', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_UpdateTouchpoint;
GO

CREATE PROCEDURE dbo.usp_CreateTouchpoint
    @Name NVARCHAR(100),
    @Type NVARCHAR(50),
    @Configuration NVARCHAR(MAX) = NULL, -- Mapping to TouchCriteria
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO RetailConnect.T_Touchpoints (
        TouchPoint, 
        TouchType, 
        TouchCriteria, 
        IsActive, 
        CreatedDate
    )
    VALUES (
        @Name, 
        @Type, 
        @Configuration, -- Mapping Config -> Criteria
        @IsActive, 
        GETUTCDATE()
    );
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO

CREATE PROCEDURE dbo.usp_GetTouchpointById
    @TouchpointId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        TouchpointID AS Id,
        TouchPoint AS Name,
        TouchType AS Type,
        TouchCriteria AS Configuration,
        IsActive,
        CreatedDate
    FROM RetailConnect.T_Touchpoints
    WHERE TouchpointID = @TouchpointId;
END
GO

CREATE PROCEDURE dbo.usp_GetAllTouchpoints
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        TouchpointID AS Id,
        TouchPoint AS Name,
        TouchType AS Type,
        IsActive
    FROM RetailConnect.T_Touchpoints
    ORDER BY TouchPoint;
END
GO

CREATE PROCEDURE dbo.usp_UpdateTouchpoint
    @TouchpointId INT,
    @Name NVARCHAR(100),
    @Configuration NVARCHAR(MAX) = NULL,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE RetailConnect.T_Touchpoints
    SET 
        TouchPoint = @Name,
        TouchCriteria = @Configuration,
        IsActive = @IsActive,
        ModifiedDate = GETUTCDATE()
    WHERE TouchpointID = @TouchpointId;
END
GO
PRINT 'Touchpoint Procedures... FIXED';

PRINT '>>> ALL PROCEDURES FIXED & CREATED <<<';
GO
