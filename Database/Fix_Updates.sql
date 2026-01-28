USE RetailConnect;
GO

-- Fix UPDATE Template
DROP PROCEDURE dbo.usp_UpdateTemplate;
GO

CREATE PROCEDURE dbo.usp_UpdateTemplate
    @TemplateId INT,
    @Name NVARCHAR(100),
    @Content NVARCHAR(MAX) = NULL,
    @Subject NVARCHAR(200) = NULL,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE RetailConnect.T_TemplateVersions
    SET 
        TemplateName = @Name,
        TemplateContent = @Content,
        Subject = @Subject,
        IsActive = @IsActive,
        ModifiedDate = GETUTCDATE()
    WHERE TemplateVersionID = @TemplateId;
END
GO

-- Fix UPDATE Touchpoint
DROP PROCEDURE dbo.usp_UpdateTouchpoint;
GO

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
        TouchPoint = @Name,
        TouchCriteria = @Configuration,
        IsActive = @IsActive,
        ModifiedDate = GETUTCDATE()
    WHERE TouchpointID = @TouchpointId;
END
GO

PRINT 'Fixed Update procedures for Templates and Touchpoints';
GO
