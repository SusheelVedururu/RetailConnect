USE RetailConnect;
GO

DROP PROCEDURE dbo.usp_CreateCampaign;
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
        Campaign,
        SegmentID,
        SuccessName,
        SuccessCriteria,
        StartDate, 
        EndDate, 
        IsActive, 
        CreatedDate
    )
    VALUES (
        @Name, 
        @Name,
        1,  -- Default to SegmentID 1
        'Default Success',  -- Default success name
        'Default criteria',  -- Default success criteria
        @StartDate, 
        @EndDate, 
        @IsActive, 
        GETUTCDATE()
    );
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO

PRINT 'usp_CreateCampaign fixed with all required fields';
GO
