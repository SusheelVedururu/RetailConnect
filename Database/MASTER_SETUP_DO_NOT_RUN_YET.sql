-- =============================================
-- MASTER SETUP SCRIPT - RETAILCONNECT
-- Runs all module procedures (Segments, Campaigns, Templates, Touchpoints)
-- =============================================

USE RetailConnect;
GO

PRINT '>>> STARTING DATABASE SETUP <<<';
PRINT '';

-- 1. SEGMENTS
:r $(path)\StoredProcedures.sql
PRINT 'Segments... OK';

-- 2. CAMPAIGNS
:r $(path)\CampaignStoredProcedures.sql
PRINT 'Campaigns... OK';

-- 3. TEMPLATES
:r $(path)\TemplateStoredProcedures.sql
PRINT 'Templates... OK';

-- 4. TOUCHPOINTS
:r $(path)\TouchpointStoredProcedures.sql
PRINT 'Touchpoints... OK';

PRINT '';
PRINT '>>> DATABASE SETUP COMPLETE <<<';
GO
