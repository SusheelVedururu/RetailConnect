USE RetailConnect;
GO

DROP PROCEDURE dbo.usp_CreateTemplate;
GO

CREATE PROCEDURE dbo.usp_CreateTemplate
    @Name NVARCHAR(100),
    @Content NVARCHAR(MAX) = NULL,
    @Subject NVARCHAR(200) = NULL,
    @Type NVARCHAR(50) = 'Email',
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO RetailConnect.T_TemplateVersions (
        TemplateName, 
        TemplateContent, 
        Subject, 
        TemplateType, 
        TemplateVersion,
        TemplateFile,  -- ADDED REQUIRED FIELD
        IsActive, 
        CreatedDate
    )
    VALUES (
        @Name, 
        @Content, 
        @Subject, 
        @Type, 
        'v1',  -- Default Version
        'default.html',  -- ADDED DEFAULT FILE
        @IsActive, 
        GETUTCDATE()
    );
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO

PRINT 'Fixed: usp_CreateTemplate';
GO
