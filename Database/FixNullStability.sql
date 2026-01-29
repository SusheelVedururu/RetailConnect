USE RetailConnect;
GO

PRINT '========================================';
PRINT 'FIXING STORED PROCEDURES FOR NULLS';
PRINT '========================================';
GO

-- Fix Segment Create
ALTER PROCEDURE dbo.usp_CreateSegment
    @Name NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,
    @Criteria NVARCHAR(MAX) = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO RetailConnect.T_Segments (
        SegmentName,
        SegmentDefinition,
        SegmentCriteria,
        IsActive,
        CreatedDate
    )
    VALUES (
        @Name,
        ISNULL(@Description, ''),
        ISNULL(@Criteria, ''),
        @IsActive,
        GETUTCDATE()
    );
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO

-- Fix Touchpoint Create
ALTER PROCEDURE dbo.usp_CreateTouchpoint
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
        TemplateName,
        IsActive, 
        CreatedDate
    )
    VALUES (
        @Name, 
        @Type, 
        ISNULL(@Configuration, ''),
        'DefaultTemplate',
        @IsActive, 
        GETUTCDATE()
    );
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO

PRINT '✓ Stored procedures fixed for NULL stability';
GO
