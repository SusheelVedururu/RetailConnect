USE RetailConnect;
GO

PRINT '========================================';
PRINT 'RESTORING IMPROVED VALIDATION PROCEDURES';
PRINT '========================================';
GO

-- Improved check for Segments (excludes current ID)
IF OBJECT_ID('dbo.usp_CheckSegmentExists', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_CheckSegmentExists;
GO
CREATE PROCEDURE dbo.usp_CheckSegmentExists
    @Name NVARCHAR(100),
    @ExcludeId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM RetailConnect.T_Segments WHERE SegmentName = @Name AND (@ExcludeId IS NULL OR SegmentID <> @ExcludeId))
        SELECT 1 AS [Exists];
    ELSE
        SELECT 0 AS [Exists];
END
GO

-- Improved check for Campaigns (excludes current ID)
IF OBJECT_ID('dbo.usp_CheckCampaignExists', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_CheckCampaignExists;
GO
CREATE PROCEDURE dbo.usp_CheckCampaignExists
    @Name NVARCHAR(100),
    @ExcludeId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM RetailConnect.T_Campaign WHERE CampaignName = @Name AND (@ExcludeId IS NULL OR CampaignID <> @ExcludeId))
        SELECT 1 AS [Exists];
    ELSE
        SELECT 0 AS [Exists];
END
GO

PRINT '✓ Restored improved validation procedures';
GO
