USE RetailConnect;
GO

PRINT '========================================';
PRINT 'Creating Remaining Module Procedures';
PRINT '========================================';
GO

-- =============================================
-- CAMPAIGN-TOUCHPOINTS MODULE
-- =============================================
IF OBJECT_ID('dbo.usp_AddCampaignTouchpoint', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_AddCampaignTouchpoint;
IF OBJECT_ID('dbo.usp_GetCampaignTouchpoints', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetCampaignTouchpoints;
IF OBJECT_ID('dbo.usp_RemoveCampaignTouchpoint', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_RemoveCampaignTouchpoint;
GO

CREATE PROCEDURE dbo.usp_AddCampaignTouchpoint
    @CampaignId INT,
    @TouchpointId INT,
    @SequenceOrder INT = 1,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO RetailConnect.T_Campaign_Touchpoints (CampaignID, TouchpointID, SequenceOrder, IsActive, CreatedDate)
    VALUES (@CampaignId, @TouchpointId, @SequenceOrder, @IsActive, GETUTCDATE());
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO

CREATE PROCEDURE dbo.usp_GetCampaignTouchpoints
    @CampaignId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        CampaignTouchpointID AS Id,
        CampaignID AS CampaignId,
        TouchpointID AS TouchpointId,
        SequenceOrder,
        IsActive,
        CreatedDate
    FROM RetailConnect.T_Campaign_Touchpoints
    WHERE CampaignID = @CampaignId
    ORDER BY SequenceOrder;
END
GO

CREATE PROCEDURE dbo.usp_RemoveCampaignTouchpoint
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM RetailConnect.T_Campaign_Touchpoints WHERE CampaignTouchpointID = @Id;
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

PRINT '✓ Campaign-Touchpoints procedures created';
GO

-- =============================================
-- CAMPAIGN-TEMPLATE MODULE
-- =============================================
IF OBJECT_ID('dbo.usp_AddCampaignTemplate', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_AddCampaignTemplate;
IF OBJECT_ID('dbo.usp_GetCampaignTemplates', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetCampaignTemplates;
IF OBJECT_ID('dbo.usp_RemoveCampaignTemplate', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_RemoveCampaignTemplate;
GO

CREATE PROCEDURE dbo.usp_AddCampaignTemplate
    @CampaignId INT,
    @TemplateVersionId INT,
    @AllocationPercent INT = 100,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO RetailConnect.T_CampaignTemplate (CampaignID, TemplateVersionID, AllocationPercent, IsActive, CreatedDate)
    VALUES (@CampaignId, @TemplateVersionId, @AllocationPercent, @IsActive, GETUTCDATE());
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO

CREATE PROCEDURE dbo.usp_GetCampaignTemplates
    @CampaignId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        CampaignTemplateID AS Id,
        CampaignID AS CampaignId,
        TemplateVersionID AS TemplateVersionId,
        AllocationPercent,
        IsActive,
        CreatedDate
    FROM RetailConnect.T_CampaignTemplate
    WHERE CampaignID = @CampaignId;
END
GO

CREATE PROCEDURE dbo.usp_RemoveCampaignTemplate
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM RetailConnect.T_CampaignTemplate WHERE CampaignTemplateID = @Id;
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

PRINT '✓ Campaign-Template procedures created';
GO

-- =============================================
-- CAMPAIGN LOG MODULE (Read-only)
-- =============================================
IF OBJECT_ID('dbo.usp_GetCampaignLogs', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetCampaignLogs;
IF OBJECT_ID('dbo.usp_GetCampaignLogById', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetCampaignLogById;
GO

CREATE PROCEDURE dbo.usp_GetCampaignLogs
    @CampaignId INT = NULL,
    @ContactId INT = NULL,
    @ExecutionStatus NVARCHAR(50) = NULL,
    @FromDate DATETIME = NULL,
    @ToDate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        CampaignLogID AS Id,
        CampaignID AS CampaignId,
        ContactID AS ContactId,
        TouchpointID AS TouchpointId,
        TouchCounter,
        TemplateVersionID AS TemplateVersionId,
        SuccessValue,
        ExecutionStatus,
        ErrorMessage,
        SentDate,
        DeliveredDate,
        OpenedDate,
        ClickedDate,
        LastUpdated,
        CreatedDate
    FROM RetailConnect.T_CampaignLog
    WHERE (@CampaignId IS NULL OR CampaignID = @CampaignId)
      AND (@ContactId IS NULL OR ContactID = @ContactId)
      AND (@ExecutionStatus IS NULL OR ExecutionStatus = @ExecutionStatus)
      AND (@FromDate IS NULL OR CreatedDate >= @FromDate)
      AND (@ToDate IS NULL OR CreatedDate <= @ToDate)
    ORDER BY CreatedDate DESC;
END
GO

CREATE PROCEDURE dbo.usp_GetCampaignLogById
    @Id BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        CampaignLogID AS Id,
        CampaignID AS CampaignId,
        ContactID AS ContactId,
        TouchpointID AS TouchpointId,
        TouchCounter,
        TemplateVersionID AS TemplateVersionId,
        SuccessValue,
        ExecutionStatus,
        ErrorMessage,
        SentDate,
        DeliveredDate,
        OpenedDate,
        ClickedDate,
        LastUpdated,
        CreatedDate
    FROM RetailConnect.T_CampaignLog
    WHERE CampaignLogID = @Id;
END
GO

PRINT '✓ Campaign-Log procedures created';
GO

PRINT '========================================';
PRINT 'All 3 Remaining Modules Created!';
PRINT '========================================';
GO
