USE RetailConnect;
GO

DROP PROCEDURE IF EXISTS dbo.usp_CreateCampaign;
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
        StartDate, 
        EndDate, 
        IsActive, 
        CreatedDate
    )
    VALUES (
        @Name, 
        @Name,
        NULL,  -- Allow NULL for SegmentID
        @StartDate, 
        @EndDate, 
        @IsActive, 
        GETUTCDATE()
    );
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO
