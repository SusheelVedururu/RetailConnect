# RetailConnect - Required Stored Procedures

## Overview
This file contains the SQL stored procedures that need to be created in your SQL Server database for the Segment module to work.

---

## Database Setup

### 1. Create Database (if not exists)
```sql
CREATE DATABASE RetailConnect;
GO

USE RetailConnect;
GO
```

### 2. Create Segments Table
```sql
CREATE TABLE Segments
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    Criteria NVARCHAR(MAX) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
    ModifiedDate DATETIME NULL,
    
    CONSTRAINT UK_Segments_Name UNIQUE (Name)
);
GO
```

---

## Stored Procedures

### 1. usp_CreateSegment
```sql
CREATE PROCEDURE usp_CreateSegment
    @Name NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,
    @Criteria NVARCHAR(MAX) = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Insert the segment
    INSERT INTO Segments (Name, Description, Criteria, IsActive)
    VALUES (@Name, @Description, @Criteria, @IsActive);
    
    -- Return the new segment ID
    SELECT SCOPE_IDENTITY() AS Id;
END
GO
```

### 2. usp_GetSegmentById
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
        ModifiedDate
    FROM Segments
    WHERE Id = @SegmentId;
END
GO
```

### 3. usp_GetAllSegments
```sql
CREATE PROCEDURE usp_GetAllSegments
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        Name,
        IsActive,
        0 AS MemberCount  -- Placeholder, update based on your business logic
    FROM Segments
    ORDER BY Name;
END
GO
```

### 4. usp_UpdateSegment
```sql
CREATE PROCEDURE usp_UpdateSegment
    @SegmentId INT,
    @Name NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,
    @Criteria NVARCHAR(MAX) = NULL,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Segments
    SET 
        Name = @Name,
        Description = @Description,
        Criteria = @Criteria,
        IsActive = @IsActive,
        ModifiedDate = GETUTCDATE()
    WHERE Id = @SegmentId;
    
    -- Return number of rows affected
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
```

### 5. usp_CheckSegmentExists
```sql
CREATE PROCEDURE usp_CheckSegmentExists
    @Name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(1)
    FROM Segments
    WHERE Name = @Name AND IsActive = 1;
END
GO
```

---

## How to Run These Scripts

1. Open SQL Server Management Studio (SSMS)
2. Connect to your SQL Server instance
3. Copy and paste each script above
4. Execute them in order
5. Verify the stored procedures are created by checking the Programmability > Stored Procedures folder in SSMS

---

## Update Connection String

After creating the database, update your connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=RetailConnect;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Replace `YOUR_SERVER_NAME` with:
- `localhost` or `(localdb)\\mssqllocaldb` for local development
- Your actual SQL Server instance name for production

---

## Testing the Stored Procedures

### Test usp_CreateSegment
```sql
EXEC usp_CreateSegment 
    @Name = 'Test Segment',
    @Description = 'This is a test segment',
    @Criteria = 'Age > 25',
    @IsActive = 1;
```

### Test usp_GetAllSegments
```sql
EXEC usp_GetAllSegments;
```

### Test usp_GetSegmentById
```sql
EXEC usp_GetSegmentById @SegmentId = 1;
```

### Test usp_UpdateSegment
```sql
EXEC usp_UpdateSegment 
    @SegmentId = 1,
    @Name = 'Updated Segment',
    @Description = 'Updated description',
    @Criteria = 'Age > 30',
    @IsActive = 1;
```

### Test usp_CheckSegmentExists
```sql
EXEC usp_CheckSegmentExists @Name = 'Test Segment';
```
