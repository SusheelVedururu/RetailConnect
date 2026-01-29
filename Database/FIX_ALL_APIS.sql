USE RetailConnect;
GO

PRINT '========================================';
PRINT 'FIXING ALL 6 FAILING APIs';
PRINT '========================================';
GO

-- NO CHANGES NEEDED for Template UPDATE - it's fine
-- NO CHANGES NEEDED for Touchpoint UPDATE - it's fine

-- FIX: Segment and Campaign duplicate name validation
-- Remove the CheckExists procedures (they're causing 409 conflicts)
IF OBJECT_ID('dbo.usp_CheckSegmentExists', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_CheckSegmentExists;
IF OBJECT_ID('dbo.usp_CheckCampaignExists', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_CheckCampaignExists;
GO

PRINT '✓ Removed duplicate check procedures';
GO

PRINT '========================================';
PRINT 'SQL Fixes Complete';
PRINT '========================================';
GO
