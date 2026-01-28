-- =============================================
-- RetailConnect - Stored Procedures for Campaign Module
-- Database: RetailConnect
-- Table: RetailConnect.T_Campaign
-- Created: 2026-01-28
-- =============================================

USE RetailConnect;
GO

PRINT '========================================';
PRINT 'Creating Campaign Stored Procedures...';
PRINT '========================================';
GO

-- =============================================
-- Drop existing procedures if they exist
-- =============================================
IF OBJECT_ID('dbo.usp_CreateCampaign', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_CreateCampaign;
IF OBJECT_ID('dbo.usp_GetCampaignById', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetCampaignById;
IF OBJECT_ID('dbo.usp_GetAllCampaigns', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetAllCampaigns;
IF OBJECT_ID('dbo.usp_UpdateCampaign', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_UpdateCampaign;
IF OBJECT_ID('dbo.usp_CheckCampaignExists', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_CheckCampaignExists;
GO

-- =============================================
-- 1. usp_CreateCampaign
-- =============================================
CREATE PROCEDURE dbo.usp_CreateCampaign
    @Name NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,
    @Type NVARCHAR(50) = NULL,
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO RetailConnect.T_Campaign (
        CampaignName, 
        CampaignDescription, 
        CampaignType, 
        StartDate, 
        EndDate, 
        Status,
        IsActive, 
        CreatedDate
    )
    VALUES (
        @Name, 
        @Description, 
        @Type, 
        @StartDate, 
        @EndDate, 
        'Draft',
        @IsActive, 
        GETUTCDATE()
    );
    
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO
PRINT '✓ Created: usp_CreateCampaign';
GO

-- =============================================
-- 2. usp_GetCampaignById
-- =============================================
CREATE PROCEDURE dbo.usp_GetCampaignById
    @CampaignId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        CampaignID AS Id,
        CampaignName AS Name,
        CampaignDescription AS Description,
        CampaignType AS Type,
        Status,
        StartDate,
        EndDate,
        IsActive,
        CreatedDate,
        ModifiedDate
    FROM RetailConnect.T_Campaign
    WHERE CampaignID = @CampaignId;
END
GO
PRINT '✓ Created: usp_GetCampaignById';
GO

-- =============================================
-- 3. usp_GetAllCampaigns
-- =============================================
CREATE PROCEDURE dbo.usp_GetAllCampaigns
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        CampaignID AS Id,
        CampaignName AS Name,
        CampaignType AS Type,
        Status,
        IsActive
    FROM RetailConnect.T_Campaign
    ORDER BY CampaignName;
END
GO
PRINT '✓ Created: usp_GetAllCampaigns';
GO

-- =============================================
-- 4. usp_UpdateCampaign
-- =============================================
CREATE PROCEDURE dbo.usp_UpdateCampaign
    @CampaignId INT,
    @Name NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,
    @Type NVARCHAR(50) = NULL,
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE RetailConnect.T_Campaign
    SET 
        CampaignName = @Name,
        CampaignDescription = @Description,
        CampaignType = @Type,
        StartDate = @StartDate,
        EndDate = @EndDate,
        IsActive = @IsActive,
        ModifiedDate = GETUTCDATE()
    WHERE CampaignID = @CampaignId;
    
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
PRINT '✓ Created: usp_UpdateCampaign';
GO

-- =============================================
-- 5. usp_CheckCampaignExists
-- =============================================
CREATE PROCEDURE dbo.usp_CheckCampaignExists
    @Name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(1) AS [Count]
    FROM RetailConnect.T_Campaign
    WHERE CampaignName = @Name AND IsActive = 1;
END
GO
PRINT '✓ Created: usp_CheckCampaignExists';
GO

PRINT '========================================';
PRINT 'Total: 5 Campaign procedures created!';
PRINT '========================================';
GO
