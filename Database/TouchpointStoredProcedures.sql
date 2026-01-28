-- =============================================
-- RetailConnect - Stored Procedures for Touchpoints Module
-- Database: RetailConnect
-- Table: RetailConnect.T_Touchpoints
-- Created: 2026-01-28
-- =============================================

USE RetailConnect;
GO

PRINT '========================================';
PRINT 'Creating Touchpoint Stored Procedures...';
PRINT '========================================';
GO

IF OBJECT_ID('dbo.usp_CreateTouchpoint', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_CreateTouchpoint;
IF OBJECT_ID('dbo.usp_GetTouchpointById', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetTouchpointById;
IF OBJECT_ID('dbo.usp_GetAllTouchpoints', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_GetAllTouchpoints;
IF OBJECT_ID('dbo.usp_UpdateTouchpoint', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_UpdateTouchpoint;
GO

-- 1. Create
CREATE PROCEDURE dbo.usp_CreateTouchpoint
    @Name NVARCHAR(100),
    @Type NVARCHAR(50),
    @Configuration NVARCHAR(MAX) = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO RetailConnect.T_Touchpoints (TouchpointName, TouchpointType, TouchpointConfig, IsActive, CreatedDate)
    VALUES (@Name, @Type, @Configuration, @IsActive, GETUTCDATE());
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO
PRINT '✓ Created: usp_CreateTouchpoint';
GO

-- 2. Get By Id
CREATE PROCEDURE dbo.usp_GetTouchpointById
    @TouchpointId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        TouchpointID AS Id,
        TouchpointName AS Name,
        TouchpointType AS Type,
        TouchpointConfig AS Configuration,
        IsActive,
        CreatedDate,
        ModifiedDate
    FROM RetailConnect.T_Touchpoints
    WHERE TouchpointID = @TouchpointId;
END
GO
PRINT '✓ Created: usp_GetTouchpointById';
GO

-- 3. Get All
CREATE PROCEDURE dbo.usp_GetAllTouchpoints
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        TouchpointID AS Id,
        TouchpointName AS Name,
        TouchpointType AS Type,
        IsActive
    FROM RetailConnect.T_Touchpoints
    ORDER BY TouchpointName;
END
GO
PRINT '✓ Created: usp_GetAllTouchpoints';
GO

-- 4. Update
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
        TouchpointName = @Name,
        TouchpointConfig = @Configuration,
        IsActive = @IsActive,
        ModifiedDate = GETUTCDATE()
    WHERE TouchpointID = @TouchpointId;
END
GO
PRINT '✓ Created: usp_UpdateTouchpoint';
GO

PRINT '========================================';
PRINT 'All Touchpoint Procedures Created!';
PRINT '========================================';
GO
