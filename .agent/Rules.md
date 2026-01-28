# RetailConnect - Development Rules & Constraints

## 🚨 Critical Rules (MUST FOLLOW)

### Rule 1: Layer Communication
**Each layer can communicate ONLY with the layer directly below it**

✅ **Allowed**:
```
Controller → Service → Data Access → Stored Procedure → Table
```

❌ **Forbidden**:
```
Controller → Data Access (skipping Service)
Controller → Database (skipping Service and Data Access)
Service → Table (skipping Data Access and SP)
```

---

### Rule 2: Controller Constraints
**Controllers must be THIN - routing only**

✅ **Allowed in Controllers**:
- Receive HTTP requests
- Basic null checks (`if (request == null) return BadRequest()`)
- Call Service methods
- Return HTTP status codes

❌ **Forbidden in Controllers**:
- Business logic
- Validation logic (beyond null checks)
- Data transformations
- Database access
- Complex conditionals
- Try-catch blocks (unless for logging only)

**Example - Correct Controller**:
```csharp
[HttpPost]
public async Task<IActionResult> CreateSegment([FromBody] CreateSegmentRequest request)
{
    if (request == null) return BadRequest("Request cannot be null");
    
    var result = await _segmentService.CreateSegmentAsync(request);
    return Ok(result);
}
```

**Example - WRONG Controller**:
```csharp
[HttpPost]
public async Task<IActionResult> CreateSegment([FromBody] CreateSegmentRequest request)
{
    // ❌ WRONG: Validation in controller
    if (string.IsNullOrWhiteSpace(request.Name))
        return BadRequest("Name is required");
    
    // ❌ WRONG: Business logic in controller
    if (request.Name.Length > 100)
        return BadRequest("Name too long");
    
    // ❌ WRONG: Direct data access
    var exists = await _dataAccess.CheckExists(request.Name);
    
    return Ok();
}
```

---

### Rule 3: Service Layer Requirements
**ALL business rules and validations MUST live in the Service layer**

✅ **Required in Services**:
- ALL input validation
- ALL business rules
- ALL business logic
- Data transformation
- Orchestration of multiple data calls
- Exception handling for business errors

❌ **Forbidden in Services**:
- Direct database access (must use Data Access layer)
- SQL queries or inline SQL
- HTTP-specific logic (status codes, headers)

**Example - Correct Service**:
```csharp
public async Task<SegmentResponse> CreateSegmentAsync(CreateSegmentRequest request)
{
    // ✅ Validation in service
    if (string.IsNullOrWhiteSpace(request.Name))
        throw new ValidationException("Segment name is required");
    
    if (request.Name.Length > 100)
        throw new ValidationException("Segment name cannot exceed 100 characters");
    
    // ✅ Business rule in service
    var exists = await _dataAccess.SegmentExistsAsync(request.Name);
    if (exists)
        throw new BusinessException("Segment with this name already exists");
    
    // ✅ Call data layer
    var segmentId = await _dataAccess.CreateSegmentAsync(request);
    
    // ✅ Transform and return
    return new SegmentResponse 
    { 
        Id = segmentId, 
        Name = request.Name,
        CreatedDate = DateTime.UtcNow
    };
}
```

---

### Rule 4: Data Access Constraints
**Data Access layer must ONLY execute Stored Procedures**

✅ **Allowed in Data Access**:
- Open/close SQL connections
- Create SqlCommand with stored procedure name
- Add parameters to SqlCommand
- Execute stored procedures
- Map SqlDataReader to objects
- Handle SqlException

❌ **Forbidden in Data Access**:
- Inline SQL or dynamic queries
- Business logic
- Validation logic
- String concatenation for SQL
- Direct table access

**Example - Correct Data Access**:
```csharp
public async Task<int> CreateSegmentAsync(CreateSegmentRequest request)
{
    using var connection = new SqlConnection(_connectionString);
    using var command = new SqlCommand("usp_CreateSegment", connection);
    command.CommandType = CommandType.StoredProcedure;
    
    command.Parameters.AddWithValue("@Name", request.Name);
    command.Parameters.AddWithValue("@Description", request.Description ?? (object)DBNull.Value);
    
    await connection.OpenAsync();
    var segmentId = (int)await command.ExecuteScalarAsync();
    
    return segmentId;
}
```

**Example - WRONG Data Access**:
```csharp
public async Task<int> CreateSegmentAsync(CreateSegmentRequest request)
{
    // ❌ WRONG: Inline SQL
    var sql = $"INSERT INTO Segments (Name) VALUES ('{request.Name}')";
    
    // ❌ WRONG: Dynamic query
    var query = "INSERT INTO Segments (Name) VALUES (@Name)";
    
    // ❌ WRONG: Direct table access
    using var command = new SqlCommand(query, connection);
}
```

---

### Rule 5: Database Access Rules
**NO inline SQL anywhere in the project**

✅ **Allowed**:
- Stored procedure calls only
- Parameterized stored procedure execution

❌ **Forbidden**:
- Inline SQL statements
- Dynamic SQL generation
- String concatenation for queries
- Direct table INSERT/UPDATE/DELETE
- Ad-hoc queries

---

### Rule 6: API Must NEVER Access Tables Directly
**All database operations go through Stored Procedures**

✅ **Correct Flow**:
```
Controller → Service → Data Access → Stored Procedure → Table
```

❌ **Forbidden**:
```
Any layer → Direct table access
```

---

## 🚫 Absolutely Forbidden Patterns

### ❌ Entity Framework
```csharp
// FORBIDDEN
public class ApplicationDbContext : DbContext { }
builder.Services.AddDbContext<ApplicationDbContext>();
```

### ❌ Repository Pattern
```csharp
// FORBIDDEN
public interface IRepository<T> { }
public class GenericRepository<T> : IRepository<T> { }
```

### ❌ Unit of Work Pattern
```csharp
// FORBIDDEN
public interface IUnitOfWork { }
public class UnitOfWork : IUnitOfWork { }
```

### ❌ CQRS / MediatR
```csharp
// FORBIDDEN
public class CreateSegmentCommand : IRequest<int> { }
public class CreateSegmentHandler : IRequestHandler<CreateSegmentCommand, int> { }
```

### ❌ Generic Base Controllers
```csharp
// FORBIDDEN
public class BaseController<T> : ControllerBase { }
public class SegmentController : BaseController<Segment> { }
```

### ❌ Helpers / Utils / Extensions (unless explicitly requested)
```csharp
// FORBIDDEN (unless explicitly requested)
public static class StringHelper { }
public static class ValidationUtils { }
public static class Extensions { }
```

---

## 📁 Project Structure Rules

### ✅ Create ONLY These Folders (if needed)

```
RetailConnect.API/
├── Controllers/
├── Services/
│   ├── Interfaces/
│   └── Implementations/
├── Models/
└── Data/
```

### ❌ DO NOT Create

- `Repositories/`
- `Domain/`, `Core/`, `Infrastructure/`
- `Common/`, `Shared/`
- `Helpers/`, `Utils/`, `Extensions/`
- `Entities/`, `ViewModels/`
- Any extra layers or abstractions

---

## 🗄️ Database Rules

### Rule 7: Database Schema
- Database schema **already exists** and is the source of truth
- All tables are under a single schema (e.g., `RetailConnect`)
- Stored Procedures already exist or will be created separately
- Your job is **ONLY to CALL** stored procedures

### Rule 8: Table Design
❌ **DO NOT**:
- Design tables (unless explicitly asked)
- Modify table schemas
- Insert or update audit columns (unless required by the API)
- Create migrations or schema changes

✅ **DO**:
- Use existing tables
- Call existing stored procedures
- Pass required parameters to SPs

---

## 🌐 API Design Rules

### Rule 9: API Naming
**APIs must represent BUSINESS ACTIONS, not tables**

✅ **Correct Examples**:
```
POST   /api/segments
GET    /api/segments/{id}
PUT    /api/segments/{id}
GET    /api/campaigns/{id}/journey
POST   /api/campaigns/{id}/activate
```

❌ **Wrong Examples**:
```
POST   /api/InsertSegment
GET    /api/T_CampaignTemplate
POST   /api/CreateSegmentRecord
GET    /api/GetAllSegments
```

### Rule 10: RESTful Principles
- Use HTTP verbs correctly (GET, POST, PUT, DELETE)
- Use plural nouns for resources (`/segments`, not `/segment`)
- Use path parameters for IDs (`/segments/{id}`)
- Use query parameters for filtering (`/segments?status=active`)

### Rule 11: CRUD Operations
- **No blind CRUD generation**
- Only create endpoints that represent actual business needs
- **No DELETE APIs unless explicitly requested**

---

## 📝 Code Generation Rules

### Rule 12: Work on ONE Module at a Time
- Complete one module fully before moving to the next
- Do NOT generate multiple modules in parallel
- Wait for explicit confirmation before proceeding

### Rule 13: File Generation Order
Generate code **strictly in this order**:

1. **DTOs (Models)** - Request and Response models
2. **Data Access** - ADO.NET stored procedure calls
3. **Service Interface** - Business logic contract
4. **Service Implementation** - Business logic implementation
5. **API Controller** - HTTP endpoints

### Rule 14: File Documentation
For each file generated:
- Show exact file name and folder path
- Explain briefly WHY the file exists
- Show the complete file content

---

## 🚫 What NOT to Generate

### ❌ DO NOT Generate:
- UI or Web project code
- Swagger customization (unless explicitly requested)
- Authentication/Authorization (unless explicitly requested)
- Logging infrastructure (unless explicitly requested)
- Exception handling middleware (unless explicitly requested)
- Unit tests (unless explicitly requested)
- Docker files (unless explicitly requested)
- CI/CD pipelines (unless explicitly requested)

---

## ✅ Code Quality Rules

### Rule 15: Clean Code
- Use meaningful variable names
- Keep methods focused and small
- Avoid magic strings or numbers
- Use `async`/`await` for all database operations
- Properly dispose of database connections (`using` statements)

### Rule 16: Error Handling
- Services should throw meaningful exceptions
- Controllers should catch and return appropriate HTTP status codes
- Use custom exception types (e.g., `ValidationException`, `BusinessException`)

### Rule 17: Null Safety
- Check for null in controllers (basic check only)
- Handle `DBNull.Value` when reading from database
- Use `?? (object)DBNull.Value` when passing nullable parameters

---

## 🎯 Output Quality Rules

### Rule 18: Completeness
- Generate complete, production-ready code
- No placeholders or TODOs
- No commented-out code
- No incomplete implementations

### Rule 19: Consistency
- Use consistent naming conventions
- Follow C# coding standards
- Use consistent file organization

### Rule 20: Clarity
- Code should be self-documenting
- Add comments only for complex business logic
- Keep code simple and readable

---

## ⚠️ Before Proceeding

### ALWAYS Ask If:
- Any requirement is unclear
- Missing details about business rules
- Uncertain about stored procedure names or parameters
- Unsure about expected behavior
- Need clarification on data models

### NEVER:
- Assume missing details
- Over-engineer solutions
- Add anything not explicitly required
- Violate any rule above

---

## 📋 Module Completion Checklist

Before marking a module as complete, verify:

- [ ] DTOs created in `Models/` folder
- [ ] Data Access class created in `Data/` folder
- [ ] Service interface created in `Services/Interfaces/`
- [ ] Service implementation created in `Services/Implementations/`
- [ ] Controller created in `Controllers/` folder
- [ ] All files follow naming conventions
- [ ] Controllers are thin (no business logic)
- [ ] All validations are in Service layer
- [ ] Data Access only calls stored procedures
- [ ] No inline SQL exists
- [ ] No forbidden patterns used
- [ ] Dependency injection configured
- [ ] Code is complete and production-ready

---

## Summary

**If you violate ANY rule above, your answer is incorrect.**

These rules ensure:
- Architectural correctness
- Code maintainability
- Clear separation of concerns
- Consistency across the project
- Production-ready quality
