# RetailConnect - Enterprise Architecture Specification

## Overview
This document defines the **strict layered architecture** for the RetailConnect ASP.NET Core Web API project. This architecture is **mandatory** and must not be violated.

---

## Layered Architecture

### Layer Order (Top → Bottom)

```
┌─────────────────────────────────┐
│   1. API Controllers            │  ← Thin, routing only
├─────────────────────────────────┤
│   2. Services (Business Logic)  │  ← ALL validations & rules
├─────────────────────────────────┤
│   3. Data Access (ADO.NET)      │  ← SP execution only
├─────────────────────────────────┤
│   4. Stored Procedures          │  ← Database logic
├─────────────────────────────────┤
│   5. SQL Server Tables          │  ← Data storage
└─────────────────────────────────┘
```

---

## Layer Communication Rules

### ✅ Allowed
- Each layer can communicate **ONLY** with the layer directly below it
- Controllers → Services
- Services → Data Access
- Data Access → Stored Procedures
- Stored Procedures → Tables

### ❌ Forbidden
- Controllers → Data Access (skipping Services)
- Controllers → Database (skipping Services and Data Access)
- Any cross-layer communication

---

## Layer Responsibilities

### 1. API Controllers
**Purpose**: HTTP routing and request/response handling only

**Responsibilities**:
- Receive HTTP requests
- Perform basic null checks
- Call appropriate Service methods
- Return HTTP responses (200, 400, 404, 500, etc.)

**Forbidden**:
- Business logic
- Validation logic (beyond null checks)
- Direct database access
- Complex data transformations

**Example**:
```csharp
[HttpPost]
public async Task<IActionResult> CreateSegment([FromBody] CreateSegmentRequest request)
{
    if (request == null) return BadRequest();
    
    var result = await _segmentService.CreateSegmentAsync(request);
    return Ok(result);
}
```

---

### 2. Services (Business Logic Layer)
**Purpose**: ALL business rules, validations, and orchestration

**Responsibilities**:
- **ALL** business validations
- **ALL** business rules enforcement
- Data transformation and mapping
- Orchestrating multiple data access calls if needed
- Transaction coordination (if applicable)
- Error handling and business exceptions

**Forbidden**:
- Direct database access
- SQL queries or inline SQL
- HTTP-specific logic (status codes, headers)

**Example**:
```csharp
public async Task<SegmentResponse> CreateSegmentAsync(CreateSegmentRequest request)
{
    // Validation
    if (string.IsNullOrWhiteSpace(request.Name))
        throw new ValidationException("Segment name is required");
    
    // Business rule
    if (await _dataAccess.SegmentExistsAsync(request.Name))
        throw new BusinessException("Segment already exists");
    
    // Call data layer
    var segmentId = await _dataAccess.CreateSegmentAsync(request);
    
    return new SegmentResponse { Id = segmentId, Name = request.Name };
}
```

---

### 3. Data Access Layer (ADO.NET)
**Purpose**: Execute stored procedures and map results to objects

**Responsibilities**:
- Open/close database connections
- Execute stored procedures using ADO.NET
- Map SqlDataReader results to DTOs/models
- Handle database-specific exceptions
- Pass parameters to stored procedures

**Forbidden**:
- Business logic
- Inline SQL or dynamic queries
- Validation logic
- Direct table access

**Example**:
```csharp
public async Task<int> CreateSegmentAsync(CreateSegmentRequest request)
{
    using var connection = new SqlConnection(_connectionString);
    using var command = new SqlCommand("usp_CreateSegment", connection);
    command.CommandType = CommandType.StoredProcedure;
    
    command.Parameters.AddWithValue("@Name", request.Name);
    command.Parameters.AddWithValue("@Description", request.Description);
    
    await connection.OpenAsync();
    return (int)await command.ExecuteScalarAsync();
}
```

---

### 4. Stored Procedures
**Purpose**: Database logic and data manipulation

**Responsibilities**:
- CRUD operations
- Complex queries and joins
- Database-level validations
- Data integrity enforcement
- Audit trail updates (if required)

---

### 5. SQL Server Tables
**Purpose**: Data persistence

**Responsibilities**:
- Store data
- Enforce constraints (PK, FK, unique, check)
- Maintain indexes

---

## Project Structure (Minimal & Strict)

```
RetailConnect.API/
│
├── Controllers/              ← API endpoints only
│   ├── SegmentController.cs
│   └── CampaignController.cs
│
├── Services/                 ← Business logic
│   ├── Interfaces/
│   │   ├── ISegmentService.cs
│   │   └── ICampaignService.cs
│   └── Implementations/
│       ├── SegmentService.cs
│       └── CampaignService.cs
│
├── Models/                   ← DTOs ONLY (Request/Response)
│   ├── SegmentModels.cs
│   └── CampaignModels.cs
│
├── Data/                     ← ADO.NET + SP calls
│   ├── SegmentDataAccess.cs
│   └── CampaignDataAccess.cs
│
├── Program.cs
└── appsettings.json
```

### ❌ DO NOT CREATE:
- `Repositories/` folder
- `Domain/`, `Core/`, `Infrastructure/` folders
- `Common/`, `Shared/`, `Helpers/`, `Utils/` folders
- Generic base classes or abstractions
- Any unused folders or files

---

## Technology Stack (Mandatory)

### ✅ Required
- **ASP.NET Core Web API** (.NET 6 or later)
- **SQL Server** (database)
- **ADO.NET** (data access)
- **Stored Procedures** (all database operations)
- **DTOs** (request/response models)
- **Built-in Dependency Injection**

### ❌ Absolutely Forbidden
- Entity Framework (EF / EF Core)
- Repository pattern
- UnitOfWork pattern
- CQRS, MediatR
- Clean Architecture layers
- Generic base controllers
- Dapper, Micro-ORMs
- Dynamic SQL anywhere

---

## Dependency Injection Setup

**Program.cs**:
```csharp
// Register services
builder.Services.AddScoped<ISegmentService, SegmentService>();
builder.Services.AddScoped<ICampaignService, CampaignService>();

// Register data access
builder.Services.AddScoped<SegmentDataAccess>();
builder.Services.AddScoped<CampaignDataAccess>();
```

---

## Key Principles

1. **Separation of Concerns**: Each layer has ONE clear responsibility
2. **Single Direction Flow**: Data flows top-down only
3. **No Layer Skipping**: Must go through each layer in order
4. **Minimal Abstraction**: Only create what's explicitly needed
5. **Stored Procedures Only**: No inline SQL anywhere
6. **Business Logic in Services**: Controllers are thin, Services are smart

---

## Architecture Validation Checklist

Before considering any module complete, verify:

- [ ] Controllers contain NO business logic
- [ ] Controllers contain NO validation (except null checks)
- [ ] ALL validations are in Service layer
- [ ] Data Access layer ONLY calls stored procedures
- [ ] NO inline SQL exists anywhere
- [ ] NO forbidden patterns are used (Repository, UoW, etc.)
- [ ] Project structure matches the defined minimal structure
- [ ] No extra folders or abstractions exist
- [ ] Dependency injection is properly configured

---

## Summary

This architecture prioritizes:
- **Clarity** over cleverness
- **Simplicity** over flexibility
- **Explicitness** over abstraction
- **Architectural correctness** over speed

**If ANY requirement is unclear, ASK before proceeding.**
