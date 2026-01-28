-- =============================================
-- RetailConnect - Stored Procedures for Template Module
-- Database: RetailConnect
-- Table: RetailConnect.T_CampaignTemplate
-- Created: 2026-01-28
-- =============================================

USE RetailConnect;
GO

PRINT '========================================';
PRINT 'Creating Template Stored Procedures...';
PRINT '========================================';
GO

IF OBJECT_ID('dbo.usp_CreateTemplate', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_CreateTemplate;
IF OBJECT_ID('dbo.usp_GetTemplateById', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetTemplateById;
IF OBJECT_ID('dbo.usp_GetAllTemplates', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetAllTemplates;
IF OBJECT_ID('dbo.usp_UpdateTemplate', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_UpdateTemplate;
GO

-- 1. Create Template
CREATE PROCEDURE dbo.usp_CreateTemplate
    @Name NVARCHAR(100),
    @Content NVARCHAR(MAX) = NULL,
    @Subject NVARCHAR(200) = NULL,
    @Type NVARCHAR(50),
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO RetailConnect.T_CampaignTemplate (TemplateName, TemplateContent, TemplateSubject, TemplateType, IsActive, CreatedDate)
    VALUES (@Name, @Content, @Subject, @Type, @IsActive, GETUTCDATE());
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO
PRINT '✓ Created: usp_CreateTemplate';
GO

-- 2. Get Template By Id
CREATE PROCEDURE dbo.usp_GetTemplateById
    @TemplateId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        TemplateID AS Id,
        TemplateName AS Name,
        TemplateContent AS Content,
        TemplateSubject AS Subject,
        TemplateType AS Type,
        IsActive,
        CreatedDate,
        ModifiedDate
    FROM RetailConnect.T_CampaignTemplate
    WHERE TemplateID = @TemplateId;
END
GO
PRINT '✓ Created: usp_GetTemplateById';
GO

-- 3. Get All Templates
CREATE PROCEDURE dbo.usp_GetAllTemplates
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        TemplateID AS Id,
        TemplateName AS Name,
        TemplateType AS Type,
        IsActive
    FROM RetailConnect.T_CampaignTemplate
    ORDER BY TemplateName;
END
GO
PRINT '✓ Created: usp_GetAllTemplates';
GO

-- 4. Update Template
CREATE PROCEDURE dbo.usp_UpdateTemplate
    @TemplateId INT,
    @Name NVARCHAR(100),
    @Content NVARCHAR(MAX) = NULL,
    @Subject NVARCHAR(200) = NULL,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE RetailConnect.T_CampaignTemplate
    SET 
        TemplateName = @Name,
        TemplateContent = @Content,
        TemplateSubject = @Subject,
        IsActive = @IsActive,
        ModifiedDate = GETUTCDATE()
    WHERE TemplateID = @TemplateId;
END
GO
PRINT '✓ Created: usp_UpdateTemplate';
GO

PRINT '========================================';
PRINT 'All Template Procedures Created!';
PRINT '========================================';
GO
