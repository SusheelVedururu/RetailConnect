USE RetailConnect;
GO

PRINT '========================================';
PRINT 'FIXING STORED PROCEDURES FOR COMPATIBILITY';
PRINT '========================================';
GO

-- 1. usp_UpdateSegment
ALTER PROCEDURE dbo.usp_UpdateSegment
    @SegmentId INT,
    @Name NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,
    @Criteria NVARCHAR(MAX) = NULL,
    @IsActive BIT
AS
BEGIN
    UPDATE RetailConnect.T_Segments
    SET SegmentName = @Name, SegmentDefinition = @Description, SegmentCriteria = @Criteria, IsActive = @IsActive, ModifiedDate = GETUTCDATE()
    WHERE SegmentID = @SegmentId;
END
GO

-- 2. usp_UpdateCampaign
ALTER PROCEDURE dbo.usp_UpdateCampaign
    @CampaignId INT,
    @Name NVARCHAR(100),
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL,
    @IsActive BIT
AS
BEGIN
    UPDATE RetailConnect.T_Campaign
    SET CampaignName = @Name, Campaign = @Name, StartDate = @StartDate, EndDate = @EndDate, IsActive = @IsActive, ModifiedDate = GETUTCDATE()
    WHERE CampaignID = @CampaignId;
END
GO

-- 3. usp_UpdateTemplate
ALTER PROCEDURE dbo.usp_UpdateTemplate
    @TemplateId INT,
    @Name NVARCHAR(100),
    @Content NVARCHAR(MAX) = NULL,
    @Subject NVARCHAR(200) = NULL,
    @IsActive BIT
AS
BEGIN
    UPDATE RetailConnect.T_TemplateVersions
    SET TemplateName = @Name, TemplateContent = @Content, Subject = @Subject, IsActive = @IsActive, ModifiedDate = GETUTCDATE()
    WHERE TemplateVersionID = @TemplateId;
END
GO

-- 4. usp_UpdateTouchpoint
ALTER PROCEDURE dbo.usp_UpdateTouchpoint
    @TouchpointId INT,
    @Name NVARCHAR(100),
    @Configuration NVARCHAR(MAX) = NULL,
    @IsActive BIT
AS
BEGIN
    UPDATE RetailConnect.T_Touchpoints
    SET TouchPoint = @Name, TouchCriteria = @Configuration, IsActive = @IsActive, ModifiedDate = GETUTCDATE()
    WHERE TouchpointID = @TouchpointId;
END
GO

-- 5. usp_RemoveCampaignTemplate
ALTER PROCEDURE dbo.usp_RemoveCampaignTemplate
    @Id INT
AS
BEGIN
    DELETE FROM RetailConnect.T_CampaignTemplate WHERE CampaignTemplateID = @Id;
END
GO

-- 6. usp_RemoveCampaignTouchpoint
ALTER PROCEDURE dbo.usp_RemoveCampaignTouchpoint
    @Id INT
AS
BEGIN
    DELETE FROM RetailConnect.T_Campaign_Touchpoints WHERE CampaignTouchpointID = @Id;
END
GO

PRINT '✓ Procedures fixed';
GO
