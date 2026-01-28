-- =============================================
-- RetailConnect - Stored Procedures for Segment Module
-- Database: RetailConnect
-- Table: RetailConnect.T_Segments
-- Created: 2026-01-28
-- Updated to match actual column names
-- =============================================

USE RetailConnect;
GO

PRINT '========================================';
PRINT 'Creating Stored Procedures...';
PRINT '========================================';
GO

-- =============================================
-- Drop existing procedures if they exist
-- =============================================
IF OBJECT_ID('dbo.usp_CreateSegment', 'P') IS NOT NULL 
    DROP PROCEDURE dbo.usp_CreateSegment;
GO

IF OBJECT_ID('dbo.usp_GetSegmentById', 'P') IS NOT NULL 
    DROP PROCEDURE dbo.usp_GetSegmentById;
GO

IF OBJECT_ID('dbo.usp_GetAllSegments', 'P') IS NOT NULL 
    DROP PROCEDURE dbo.usp_GetAllSegments;
GO

IF OBJECT_ID('dbo.usp_UpdateSegment', 'P') IS NOT NULL 
    DROP PROCEDURE dbo.usp_UpdateSegment;
GO

IF OBJECT_ID('dbo.usp_CheckSegmentExists', 'P') IS NOT NULL 
    DROP PROCEDURE dbo.usp_CheckSegmentExists;
GO

PRINT 'Dropped existing procedures (if any)';
GO

-- =============================================
-- 1. usp_CreateSegment
-- Description: Creates a new segment and returns the ID
-- Parameters:
--   @Name - Segment name (maps to SegmentName)
--   @Description - Segment description (maps to SegmentDefinition)
--   @Criteria - Segment criteria (maps to SegmentCriteria)
--   @IsActive - Active status (default: 1)
-- Returns: New segment ID
-- =============================================
CREATE PROCEDURE dbo.usp_CreateSegment
    @Name NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,
    @Criteria NVARCHAR(MAX) = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Insert the segment
        INSERT INTO RetailConnect.T_Segments (
            SegmentName,
            SegmentDefinition,
            SegmentCriteria,
            IsActive,
            CreatedDate
        )
        VALUES (
            @Name,
            @Description,
            @Criteria,
            @IsActive,
            GETUTCDATE()
        );
        
        -- Return the new segment ID
        SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
        
    END TRY
    BEGIN CATCH
        -- Return error information
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO

PRINT '✓ Created: usp_CreateSegment';
GO

-- =============================================
-- 2. usp_GetSegmentById
-- Description: Retrieves a single segment by ID
-- Parameters:
--   @SegmentId - The segment ID to retrieve
-- Returns: Segment details (mapped to API model names)
-- =============================================
CREATE PROCEDURE dbo.usp_GetSegmentById
    @SegmentId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        SegmentID AS Id,
        SegmentName AS Name,
        SegmentDefinition AS Description,
        SegmentCriteria AS Criteria,
        IsActive,
        CreatedDate,
        ModifiedDate
    FROM RetailConnect.T_Segments
    WHERE SegmentID = @SegmentId;
END
GO

PRINT '✓ Created: usp_GetSegmentById';
GO

-- =============================================
-- 3. usp_GetAllSegments
-- Description: Retrieves all segments
-- Returns: List of all segments with basic info
-- =============================================
CREATE PROCEDURE dbo.usp_GetAllSegments
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        SegmentID AS Id,
        SegmentName AS Name,
        IsActive,
        0 AS MemberCount  -- Placeholder - can be updated with actual count later
    FROM RetailConnect.T_Segments
    ORDER BY SegmentName;
END
GO

PRINT '✓ Created: usp_GetAllSegments';
GO

-- =============================================
-- 4. usp_UpdateSegment
-- Description: Updates an existing segment
-- Parameters:
--   @SegmentId - The segment ID to update
--   @Name - New segment name (maps to SegmentName)
--   @Description - New description (maps to SegmentDefinition)
--   @Criteria - New criteria (maps to SegmentCriteria)
--   @IsActive - New active status
-- Returns: Number of rows affected
-- =============================================
CREATE PROCEDURE dbo.usp_UpdateSegment
    @SegmentId INT,
    @Name NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,
    @Criteria NVARCHAR(MAX) = NULL,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        UPDATE RetailConnect.T_Segments
        SET 
            SegmentName = @Name,
            SegmentDefinition = @Description,
            SegmentCriteria = @Criteria,
            IsActive = @IsActive,
            ModifiedDate = GETUTCDATE()
        WHERE SegmentID = @SegmentId;
        
        -- Return number of rows affected
        SELECT @@ROWCOUNT AS RowsAffected;
        
    END TRY
    BEGIN CATCH
        -- Return error information
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO

PRINT '✓ Created: usp_UpdateSegment';
GO

-- =============================================
-- 5. usp_CheckSegmentExists
-- Description: Checks if a segment with the given name exists
-- Parameters:
--   @Name - Segment name to check (maps to SegmentName)
-- Returns: Count (0 = doesn't exist, >0 = exists)
-- =============================================
CREATE PROCEDURE dbo.usp_CheckSegmentExists
    @Name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(1) AS [Count]
    FROM RetailConnect.T_Segments
    WHERE SegmentName = @Name AND IsActive = 1;
END
GO

PRINT '✓ Created: usp_CheckSegmentExists';
GO

-- =============================================
-- Verification: List all created procedures
-- =============================================
PRINT '';
PRINT '========================================';
PRINT 'Verification: Stored Procedures Created';
PRINT '========================================';

SELECT 
    SCHEMA_NAME(schema_id) + '.' + name AS [Stored Procedure],
    create_date AS [Created],
    modify_date AS [Last Modified]
FROM sys.procedures
WHERE name LIKE 'usp_%Segment%'
ORDER BY name;

PRINT '';
PRINT '========================================';
PRINT 'Summary:';
PRINT '========================================';
PRINT '✓ usp_CreateSegment      - Create new segment';
PRINT '✓ usp_GetSegmentById     - Get segment by ID';
PRINT '✓ usp_GetAllSegments     - Get all segments';
PRINT '✓ usp_UpdateSegment      - Update segment';
PRINT '✓ usp_CheckSegmentExists - Check if name exists';
PRINT '';
PRINT 'Column Mapping:';
PRINT '  API: Name        → DB: SegmentName';
PRINT '  API: Description → DB: SegmentDefinition';
PRINT '  API: Criteria    → DB: SegmentCriteria';
PRINT '  API: Id          → DB: SegmentID';
PRINT '';
PRINT 'Total: 5 stored procedures created successfully!';
PRINT '';
PRINT '========================================';
PRINT 'Ready to test the API!';
PRINT 'Run: dotnet run';
PRINT 'Open: http://localhost:5005/swagger';
PRINT '========================================';
GO

-- =============================================
-- Test Scripts (Optional - Uncomment to test)
-- =============================================
/*
-- Test 1: Create a segment
EXEC usp_CreateSegment 
    @Name = 'Test Segment',
    @Description = 'This is a test segment',
    @Criteria = 'Age > 25',
    @IsActive = 1;

-- Test 2: Get all segments
EXEC usp_GetAllSegments;

-- Test 3: Get segment by ID
EXEC usp_GetSegmentById @SegmentId = 1;

-- Test 4: Check if segment exists
EXEC usp_CheckSegmentExists @Name = 'Test Segment';

-- Test 5: Update segment
EXEC usp_UpdateSegment 
    @SegmentId = 1,
    @Name = 'Updated Test Segment',
    @Description = 'Updated description',
    @Criteria = 'Age > 30',
    @IsActive = 1;

-- Verify the data
SELECT * FROM RetailConnect.T_Segments;
*/
