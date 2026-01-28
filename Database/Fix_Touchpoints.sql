USE RetailConnect;
GO

DROP PROCEDURE dbo.usp_CreateTouchpoint;
GO

CREATE PROCEDURE dbo.usp_CreateTouchpoint
    @Name NVARCHAR(100),
    @Type NVARCHAR(50),
    @Configuration NVARCHAR(MAX) = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO RetailConnect.T_Touchpoints (
        TouchPoint, 
        TouchType, 
        TouchCriteria,
        TemplateName,  -- ADDED REQUIRED FIELD
        IsActive, 
        CreatedDate
    )
    VALUES (
        @Name, 
        @Type, 
        @Configuration,
        'DefaultTemplate',  -- ADDED DEFAULT TEMPLATE NAME
        @IsActive, 
        GETUTCDATE()
    );
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO

PRINT 'Fixed: usp_CreateTouchpoint';
GO
