# RetailConnect - SQL Schema & Stored Procedure Reference

## Overview
This document provides guidelines and examples for SQL Server database schema and stored procedures used in the RetailConnect project.

---

## Database Schema Rules

### Schema Organization
- All tables are under the `RetailConnect` schema (or as specified)
- Database schema **already exists** and is the **source of truth**
- API does NOT design or modify tables
- API only **calls stored procedures**

### Table Naming Conventions
- Use descriptive, singular or plural names (e.g., `Segments`, `Campaigns`)
- Avoid prefixes like `T_`, `tbl_`, etc. in API references
- Use PascalCase for table names

---

## Stored Procedure Naming Conventions

### Standard Naming Pattern
```
usp_<Action><Entity>[<Details>]
```

### Examples

| Operation | Stored Procedure Name | Purpose |
|-----------|----------------------|---------|
| Create | `usp_CreateSegment` | Insert a new segment |
| Get Single | `usp_GetSegmentById` | Retrieve segment by ID |
| Get List | `usp_GetAllSegments` | Retrieve all segments |
| Get Filtered | `usp_GetSegmentsByStatus` | Retrieve segments by status |
| Update | `usp_UpdateSegment` | Update existing segment |
| Delete | `usp_DeleteSegment` | Delete segment (soft or hard) |
| Check Exists | `usp_CheckSegmentExists` | Check if segment exists |
| Custom Action | `usp_ActivateCampaign` | Activate a campaign |

---

## Example Table Structure

### Segments Table
```sql
CREATE TABLE RetailConnect.Segments
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    Criteria NVARCHAR(MAX) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100) NULL,
    ModifiedDate DATETIME NULL,
    ModifiedBy NVARCHAR(100) NULL,
    
    CONSTRAINT UK_Segments_Name UNIQUE (Name)
);
```

### Campaigns Table
```sql
CREATE TABLE RetailConnect.Campaigns
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000) NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NULL,
    Status NVARCHAR(50) NOT NULL, -- Draft, Active, Paused, Completed
    Budget DECIMAL(18,2) NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(100) NULL,
    ModifiedDate DATETIME NULL,
    ModifiedBy NVARCHAR(100) NULL,
    
    CONSTRAINT UK_Campaigns_Name UNIQUE (Name)
);
```

### CampaignSegments (Junction Table)
```sql
CREATE TABLE RetailConnect.CampaignSegments
(
    CampaignId INT NOT NULL,
    SegmentId INT NOT NULL,
    AssignedDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT PK_CampaignSegments PRIMARY KEY (CampaignId, SegmentId),
    CONSTRAINT FK_CampaignSegments_Campaign FOREIGN KEY (CampaignId) 
        REFERENCES RetailConnect.Campaigns(Id),
    CONSTRAINT FK_CampaignSegments_Segment FOREIGN KEY (SegmentId) 
        REFERENCES RetailConnect.Segments(Id)
);
```

---

## Stored Procedure Examples

### 1. Create Operation

```sql
CREATE PROCEDURE usp_CreateSegment
    @Name NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,
    @Criteria NVARCHAR(MAX) = NULL,
    @IsActive BIT = 1,
    @CreatedBy NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Insert the segment
    INSERT INTO RetailConnect.Segments (Name, Description, Criteria, IsActive, CreatedBy)
    VALUES (@Name, @Description, @Criteria, @IsActive, @CreatedBy);
    
    -- Return the new segment ID
    SELECT SCOPE_IDENTITY() AS Id;
END
```

**ADO.NET Call**:
```csharp
command.Parameters.AddWithValue("@Name", request.Name);
command.Parameters.AddWithValue("@Description", request.Description ?? (object)DBNull.Value);
command.Parameters.AddWithValue("@Criteria", request.Criteria ?? (object)DBNull.Value);
command.Parameters.AddWithValue("@IsActive", request.IsActive);
command.Parameters.AddWithValue("@CreatedBy", "SystemUser");

var segmentId = (int)await command.ExecuteScalarAsync();
```

---

### 2. Get Single Record

```sql
CREATE PROCEDURE usp_GetSegmentById
    @SegmentId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        Description,
        Criteria,
        IsActive,
        CreatedDate,
        CreatedBy,
        ModifiedDate,
        ModifiedBy
    FROM RetailConnect.Segments
    WHERE Id = @SegmentId;
END
```

**ADO.NET Call**:
```csharp
command.Parameters.AddWithValue("@SegmentId", id);

using var reader = await command.ExecuteReaderAsync();
if (await reader.ReadAsync())
{
    return new SegmentResponse
    {
        Id = reader.GetInt32(reader.GetOrdinal("Id")),
        Name = reader.GetString(reader.GetOrdinal("Name")),
        Description = reader.IsDBNull(reader.GetOrdinal("Description")) 
            ? null 
            : reader.GetString(reader.GetOrdinal("Description")),
        // ... map other fields
    };
}
```

---

### 3. Get List with Optional Filtering

```sql
CREATE PROCEDURE usp_GetAllSegments
    @IsActive BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        s.Id,
        s.Name,
        s.IsActive,
        COUNT(cs.SegmentId) AS MemberCount
    FROM RetailConnect.Segments s
    LEFT JOIN RetailConnect.CampaignSegments cs ON s.Id = cs.SegmentId
    WHERE (@IsActive IS NULL OR s.IsActive = @IsActive)
    GROUP BY s.Id, s.Name, s.IsActive
    ORDER BY s.Name;
END
```

**ADO.NET Call**:
```csharp
command.Parameters.AddWithValue("@IsActive", isActive ?? (object)DBNull.Value);

var segments = new List<SegmentListItem>();
using var reader = await command.ExecuteReaderAsync();
while (await reader.ReadAsync())
{
    segments.Add(new SegmentListItem
    {
        Id = reader.GetInt32(reader.GetOrdinal("Id")),
        Name = reader.GetString(reader.GetOrdinal("Name")),
        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
        MemberCount = reader.GetInt32(reader.GetOrdinal("MemberCount"))
    });
}
```

---

### 4. Update Operation

```sql
CREATE PROCEDURE usp_UpdateSegment
    @SegmentId INT,
    @Name NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,
    @Criteria NVARCHAR(MAX) = NULL,
    @IsActive BIT,
    @ModifiedBy NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE RetailConnect.Segments
    SET 
        Name = @Name,
        Description = @Description,
        Criteria = @Criteria,
        IsActive = @IsActive,
        ModifiedDate = GETUTCDATE(),
        ModifiedBy = @ModifiedBy
    WHERE Id = @SegmentId;
    
    -- Return number of rows affected
    SELECT @@ROWCOUNT AS RowsAffected;
END
```

**ADO.NET Call**:
```csharp
command.Parameters.AddWithValue("@SegmentId", id);
command.Parameters.AddWithValue("@Name", request.Name);
command.Parameters.AddWithValue("@Description", request.Description ?? (object)DBNull.Value);
command.Parameters.AddWithValue("@Criteria", request.Criteria ?? (object)DBNull.Value);
command.Parameters.AddWithValue("@IsActive", request.IsActive);
command.Parameters.AddWithValue("@ModifiedBy", "SystemUser");

var rowsAffected = await command.ExecuteNonQueryAsync();
return rowsAffected > 0;
```

---

### 5. Delete Operation (Soft Delete)

```sql
CREATE PROCEDURE usp_DeleteSegment
    @SegmentId INT,
    @DeletedBy NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Soft delete by setting IsActive = 0
    UPDATE RetailConnect.Segments
    SET 
        IsActive = 0,
        ModifiedDate = GETUTCDATE(),
        ModifiedBy = @DeletedBy
    WHERE Id = @SegmentId;
    
    SELECT @@ROWCOUNT AS RowsAffected;
END
```

**ADO.NET Call**:
```csharp
command.Parameters.AddWithValue("@SegmentId", id);
command.Parameters.AddWithValue("@DeletedBy", "SystemUser");

var rowsAffected = await command.ExecuteNonQueryAsync();
return rowsAffected > 0;
```

---

### 6. Check Exists

```sql
CREATE PROCEDURE usp_CheckSegmentExists
    @Name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(1)
    FROM RetailConnect.Segments
    WHERE Name = @Name AND IsActive = 1;
END
```

**ADO.NET Call**:
```csharp
command.Parameters.AddWithValue("@Name", name);

var count = (int)await command.ExecuteScalarAsync();
return count > 0;
```

---

### 7. Custom Business Action

```sql
CREATE PROCEDURE usp_ActivateCampaign
    @CampaignId INT,
    @ActivatedBy NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Update campaign status
        UPDATE RetailConnect.Campaigns
        SET 
            Status = 'Active',
            ModifiedDate = GETUTCDATE(),
            ModifiedBy = @ActivatedBy
        WHERE Id = @CampaignId;
        
        -- Additional business logic here
        -- e.g., create audit log, send notifications, etc.
        
        COMMIT TRANSACTION;
        SELECT 1 AS Success;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
```

---

### 8. Parent-Child Relationship

```sql
CREATE PROCEDURE usp_GetCampaignSegments
    @CampaignId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        s.Id,
        s.Name,
        s.IsActive,
        cs.AssignedDate
    FROM RetailConnect.CampaignSegments cs
    INNER JOIN RetailConnect.Segments s ON cs.SegmentId = s.Id
    WHERE cs.CampaignId = @CampaignId
    ORDER BY cs.AssignedDate DESC;
END
```

---

## Parameter Handling Guidelines

### Nullable Parameters

**SQL**:
```sql
@Description NVARCHAR(500) = NULL
```

**C# (ADO.NET)**:
```csharp
command.Parameters.AddWithValue("@Description", request.Description ?? (object)DBNull.Value);
```

### Reading Nullable Columns

**C# (ADO.NET)**:
```csharp
Description = reader.IsDBNull(reader.GetOrdinal("Description")) 
    ? null 
    : reader.GetString(reader.GetOrdinal("Description"))
```

### Date/Time Parameters

**SQL**:
```sql
@StartDate DATETIME
```

**C# (ADO.NET)**:
```csharp
command.Parameters.AddWithValue("@StartDate", request.StartDate);
```

### Decimal Parameters

**SQL**:
```sql
@Budget DECIMAL(18,2)
```

**C# (ADO.NET)**:
```csharp
command.Parameters.AddWithValue("@Budget", request.Budget);
```

---

## Common Table Patterns

### Audit Columns
```sql
CreatedDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
CreatedBy NVARCHAR(100) NULL,
ModifiedDate DATETIME NULL,
ModifiedBy NVARCHAR(100) NULL
```

### Soft Delete
```sql
IsActive BIT NOT NULL DEFAULT 1,
IsDeleted BIT NOT NULL DEFAULT 0,
DeletedDate DATETIME NULL,
DeletedBy NVARCHAR(100) NULL
```

### Status Tracking
```sql
Status NVARCHAR(50) NOT NULL DEFAULT 'Draft',
StatusChangedDate DATETIME NULL,
StatusChangedBy NVARCHAR(100) NULL
```

---

## Error Handling in Stored Procedures

### Pattern 1: Simple Error Handling
```sql
BEGIN TRY
    -- Your logic here
    INSERT INTO ...
    SELECT SCOPE_IDENTITY();
END TRY
BEGIN CATCH
    -- Re-throw the error to the caller
    THROW;
END CATCH
```

### Pattern 2: Custom Error Messages
```sql
IF NOT EXISTS (SELECT 1 FROM Segments WHERE Id = @SegmentId)
BEGIN
    THROW 50001, 'Segment not found', 1;
END
```

---

## Transaction Management

### Simple Transaction
```sql
BEGIN TRANSACTION;

BEGIN TRY
    -- Multiple operations
    UPDATE ...
    INSERT ...
    
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    THROW;
END CATCH
```

---

## Best Practices

### ✅ DO:
- Use `SET NOCOUNT ON;` at the start of every stored procedure
- Use parameterized queries (already enforced by using SPs)
- Return meaningful data (IDs for inserts, row counts for updates)
- Use transactions for multi-step operations
- Handle errors appropriately
- Use consistent naming conventions
- Add comments for complex business logic

### ❌ DON'T:
- Use dynamic SQL (unless absolutely necessary and properly sanitized)
- Return multiple result sets (keep it simple)
- Perform complex business logic in SQL (put it in Service layer)
- Use cursors (use set-based operations)
- Over-optimize prematurely

---

## Stored Procedure Template

```sql
-- =============================================
-- Author:      [Your Name]
-- Create date: [Date]
-- Description: [Description]
-- =============================================
CREATE PROCEDURE usp_[ActionName]
    @Parameter1 INT,
    @Parameter2 NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Your logic here
        
        -- Return result
        SELECT ...
    END TRY
    BEGIN CATCH
        -- Error handling
        THROW;
    END CATCH
END
```

---

## Summary

This document provides:
- ✅ Table structure examples
- ✅ Stored procedure naming conventions
- ✅ Complete SP examples for CRUD operations
- ✅ ADO.NET mapping examples
- ✅ Parameter handling guidelines
- ✅ Best practices

**Remember**: 
- Database schema is the source of truth
- API only calls stored procedures
- No inline SQL in the application
- All database logic lives in stored procedures
