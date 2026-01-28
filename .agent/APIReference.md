# RetailConnect - API Reference & Implementation Guide

## Overview
This document provides reference examples and implementation patterns for building APIs in the RetailConnect project following the strict layered architecture.

---

## API Naming Conventions

### ✅ RESTful Naming Standards

| HTTP Method | Pattern | Example | Purpose |
|-------------|---------|---------|---------|
| `GET` | `/api/{resource}` | `GET /api/segments` | Get all resources |
| `GET` | `/api/{resource}/{id}` | `GET /api/segments/123` | Get single resource |
| `POST` | `/api/{resource}` | `POST /api/segments` | Create new resource |
| `PUT` | `/api/{resource}/{id}` | `PUT /api/segments/123` | Update resource |
| `DELETE` | `/api/{resource}/{id}` | `DELETE /api/segments/123` | Delete resource |
| `POST` | `/api/{resource}/{id}/{action}` | `POST /api/campaigns/123/activate` | Custom action |
| `GET` | `/api/{parent}/{id}/{child}` | `GET /api/campaigns/123/segments` | Get related resources |

### ❌ Anti-Patterns to Avoid

```
❌ /api/GetSegments              → ✅ GET /api/segments
❌ /api/CreateSegment            → ✅ POST /api/segments
❌ /api/UpdateSegment/123        → ✅ PUT /api/segments/123
❌ /api/DeleteSegment/123        → ✅ DELETE /api/segments/123
❌ /api/T_Segment                → ✅ /api/segments
❌ /api/InsertSegmentRecord      → ✅ POST /api/segments
❌ /api/segment                  → ✅ /api/segments (use plural)
```

---

## Complete Implementation Example

### Example Module: Segment Management

#### 1. DTOs (Models/SegmentModels.cs)

```csharp
namespace RetailConnect.API.Models
{
    /// <summary>
    /// Request model for creating a new segment
    /// </summary>
    public class CreateSegmentRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Criteria { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Request model for updating an existing segment
    /// </summary>
    public class UpdateSegmentRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Criteria { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Response model for segment operations
    /// </summary>
    public class SegmentResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Criteria { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    /// <summary>
    /// Lightweight segment list item
    /// </summary>
    public class SegmentListItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int MemberCount { get; set; }
    }
}
```

**Why this file exists**: Defines the contract between API consumers and the API. Separates external API models from internal database structures.

---

#### 2. Data Access (Data/SegmentDataAccess.cs)

```csharp
using System.Data;
using System.Data.SqlClient;
using RetailConnect.API.Models;

namespace RetailConnect.API.Data
{
    /// <summary>
    /// Data access layer for Segment operations
    /// Executes stored procedures only - NO inline SQL
    /// </summary>
    public class SegmentDataAccess
    {
        private readonly string _connectionString;

        public SegmentDataAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Creates a new segment by calling usp_CreateSegment
        /// </summary>
        public async Task<int> CreateSegmentAsync(CreateSegmentRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_CreateSegment", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Name", request.Name);
            command.Parameters.AddWithValue("@Description", request.Description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Criteria", request.Criteria ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", request.IsActive);

            await connection.OpenAsync();
            var segmentId = (int)await command.ExecuteScalarAsync();

            return segmentId;
        }

        /// <summary>
        /// Gets a segment by ID by calling usp_GetSegmentById
        /// </summary>
        public async Task<SegmentResponse> GetSegmentByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_GetSegmentById", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@SegmentId", id);

            await connection.OpenAsync();
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
                    Criteria = reader.IsDBNull(reader.GetOrdinal("Criteria")) 
                        ? null 
                        : reader.GetString(reader.GetOrdinal("Criteria")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                    ModifiedDate = reader.IsDBNull(reader.GetOrdinal("ModifiedDate")) 
                        ? null 
                        : reader.GetDateTime(reader.GetOrdinal("ModifiedDate"))
                };
            }

            return null;
        }

        /// <summary>
        /// Gets all segments by calling usp_GetAllSegments
        /// </summary>
        public async Task<List<SegmentListItem>> GetAllSegmentsAsync()
        {
            var segments = new List<SegmentListItem>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_GetAllSegments", connection);
            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();
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

            return segments;
        }

        /// <summary>
        /// Updates a segment by calling usp_UpdateSegment
        /// </summary>
        public async Task<bool> UpdateSegmentAsync(int id, UpdateSegmentRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_UpdateSegment", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@SegmentId", id);
            command.Parameters.AddWithValue("@Name", request.Name);
            command.Parameters.AddWithValue("@Description", request.Description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Criteria", request.Criteria ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", request.IsActive);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }

        /// <summary>
        /// Checks if a segment with the given name exists
        /// </summary>
        public async Task<bool> SegmentExistsAsync(string name)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("usp_CheckSegmentExists", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Name", name);

            await connection.OpenAsync();
            var exists = (int)await command.ExecuteScalarAsync();

            return exists > 0;
        }
    }
}
```

**Why this file exists**: Encapsulates all database interactions for Segments. Only executes stored procedures using ADO.NET.

---

#### 3. Service Interface (Services/Interfaces/ISegmentService.cs)

```csharp
using RetailConnect.API.Models;

namespace RetailConnect.API.Services.Interfaces
{
    /// <summary>
    /// Service contract for Segment business operations
    /// </summary>
    public interface ISegmentService
    {
        Task<SegmentResponse> CreateSegmentAsync(CreateSegmentRequest request);
        Task<SegmentResponse> GetSegmentByIdAsync(int id);
        Task<List<SegmentListItem>> GetAllSegmentsAsync();
        Task<SegmentResponse> UpdateSegmentAsync(int id, UpdateSegmentRequest request);
    }
}
```

**Why this file exists**: Defines the contract for segment business operations. Enables dependency injection and testability.

---

#### 4. Service Implementation (Services/Implementations/SegmentService.cs)

```csharp
using RetailConnect.API.Data;
using RetailConnect.API.Models;
using RetailConnect.API.Services.Interfaces;

namespace RetailConnect.API.Services.Implementations
{
    /// <summary>
    /// Business logic implementation for Segment operations
    /// Contains ALL validations and business rules
    /// </summary>
    public class SegmentService : ISegmentService
    {
        private readonly SegmentDataAccess _dataAccess;

        public SegmentService(SegmentDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<SegmentResponse> CreateSegmentAsync(CreateSegmentRequest request)
        {
            // Validation: Name is required
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Segment name is required");

            // Validation: Name length
            if (request.Name.Length > 100)
                throw new ArgumentException("Segment name cannot exceed 100 characters");

            // Business Rule: Name must be unique
            var exists = await _dataAccess.SegmentExistsAsync(request.Name);
            if (exists)
                throw new InvalidOperationException($"Segment with name '{request.Name}' already exists");

            // Validation: Criteria format (example business rule)
            if (!string.IsNullOrWhiteSpace(request.Criteria) && !IsValidCriteria(request.Criteria))
                throw new ArgumentException("Invalid segment criteria format");

            // Call data layer
            var segmentId = await _dataAccess.CreateSegmentAsync(request);

            // Retrieve and return the created segment
            var segment = await _dataAccess.GetSegmentByIdAsync(segmentId);
            return segment;
        }

        public async Task<SegmentResponse> GetSegmentByIdAsync(int id)
        {
            // Validation: ID must be positive
            if (id <= 0)
                throw new ArgumentException("Segment ID must be greater than zero");

            var segment = await _dataAccess.GetSegmentByIdAsync(id);

            // Business Rule: Segment must exist
            if (segment == null)
                throw new KeyNotFoundException($"Segment with ID {id} not found");

            return segment;
        }

        public async Task<List<SegmentListItem>> GetAllSegmentsAsync()
        {
            return await _dataAccess.GetAllSegmentsAsync();
        }

        public async Task<SegmentResponse> UpdateSegmentAsync(int id, UpdateSegmentRequest request)
        {
            // Validation: ID must be positive
            if (id <= 0)
                throw new ArgumentException("Segment ID must be greater than zero");

            // Validation: Name is required
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Segment name is required");

            // Validation: Name length
            if (request.Name.Length > 100)
                throw new ArgumentException("Segment name cannot exceed 100 characters");

            // Business Rule: Segment must exist
            var existingSegment = await _dataAccess.GetSegmentByIdAsync(id);
            if (existingSegment == null)
                throw new KeyNotFoundException($"Segment with ID {id} not found");

            // Business Rule: Name must be unique (if changed)
            if (existingSegment.Name != request.Name)
            {
                var nameExists = await _dataAccess.SegmentExistsAsync(request.Name);
                if (nameExists)
                    throw new InvalidOperationException($"Segment with name '{request.Name}' already exists");
            }

            // Call data layer
            var updated = await _dataAccess.UpdateSegmentAsync(id, request);

            if (!updated)
                throw new InvalidOperationException("Failed to update segment");

            // Retrieve and return the updated segment
            var segment = await _dataAccess.GetSegmentByIdAsync(id);
            return segment;
        }

        /// <summary>
        /// Example business rule validation
        /// </summary>
        private bool IsValidCriteria(string criteria)
        {
            // Implement your criteria validation logic here
            // This is just an example
            return !string.IsNullOrWhiteSpace(criteria);
        }
    }
}
```

**Why this file exists**: Contains ALL business logic, validations, and business rules for Segment operations. This is where the "smart" logic lives.

---

#### 5. API Controller (Controllers/SegmentController.cs)

```csharp
using Microsoft.AspNetCore.Mvc;
using RetailConnect.API.Models;
using RetailConnect.API.Services.Interfaces;

namespace RetailConnect.API.Controllers
{
    /// <summary>
    /// API endpoints for Segment management
    /// THIN controller - routing only, NO business logic
    /// </summary>
    [ApiController]
    [Route("api/segments")]
    public class SegmentController : ControllerBase
    {
        private readonly ISegmentService _segmentService;

        public SegmentController(ISegmentService segmentService)
        {
            _segmentService = segmentService;
        }

        /// <summary>
        /// Creates a new segment
        /// </summary>
        /// <param name="request">Segment creation details</param>
        /// <returns>Created segment</returns>
        [HttpPost]
        [ProducesResponseType(typeof(SegmentResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSegment([FromBody] CreateSegmentRequest request)
        {
            if (request == null)
                return BadRequest("Request body cannot be null");

            try
            {
                var result = await _segmentService.CreateSegmentAsync(request);
                return CreatedAtAction(nameof(GetSegment), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        /// <summary>
        /// Gets a segment by ID
        /// </summary>
        /// <param name="id">Segment ID</param>
        /// <returns>Segment details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SegmentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSegment(int id)
        {
            try
            {
                var result = await _segmentService.GetSegmentByIdAsync(id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Gets all segments
        /// </summary>
        /// <returns>List of segments</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<SegmentListItem>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllSegments()
        {
            var result = await _segmentService.GetAllSegmentsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing segment
        /// </summary>
        /// <param name="id">Segment ID</param>
        /// <param name="request">Updated segment details</param>
        /// <returns>Updated segment</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SegmentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSegment(int id, [FromBody] UpdateSegmentRequest request)
        {
            if (request == null)
                return BadRequest("Request body cannot be null");

            try
            {
                var result = await _segmentService.UpdateSegmentAsync(id, request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
```

**Why this file exists**: Exposes HTTP endpoints for Segment operations. Thin layer that only handles routing and HTTP concerns.

---

#### 6. Dependency Injection (Program.cs)

```csharp
// Add services to the container
builder.Services.AddScoped<ISegmentService, SegmentService>();
builder.Services.AddScoped<SegmentDataAccess>();

// Add connection string
builder.Configuration.GetConnectionString("DefaultConnection");
```

---

## HTTP Status Code Guidelines

| Status Code | When to Use | Example |
|-------------|-------------|---------|
| `200 OK` | Successful GET, PUT | Resource retrieved/updated |
| `201 Created` | Successful POST | Resource created |
| `204 No Content` | Successful DELETE | Resource deleted |
| `400 Bad Request` | Validation error | Invalid input |
| `404 Not Found` | Resource doesn't exist | Segment ID not found |
| `409 Conflict` | Business rule violation | Duplicate name |
| `500 Internal Server Error` | Unexpected error | Database connection failed |

---

## Common Patterns

### Pattern 1: List with Filtering

```csharp
// Controller
[HttpGet]
public async Task<IActionResult> GetSegments([FromQuery] bool? isActive)
{
    var result = await _segmentService.GetSegmentsAsync(isActive);
    return Ok(result);
}

// Service
public async Task<List<SegmentListItem>> GetSegmentsAsync(bool? isActive)
{
    return await _dataAccess.GetSegmentsAsync(isActive);
}

// Data Access
public async Task<List<SegmentListItem>> GetSegmentsAsync(bool? isActive)
{
    using var command = new SqlCommand("usp_GetSegments", connection);
    command.Parameters.AddWithValue("@IsActive", isActive ?? (object)DBNull.Value);
    // ... execute and map
}
```

### Pattern 2: Parent-Child Relationship

```csharp
// Get segments for a campaign
[HttpGet("campaigns/{campaignId}/segments")]
public async Task<IActionResult> GetCampaignSegments(int campaignId)
{
    var result = await _campaignService.GetCampaignSegmentsAsync(campaignId);
    return Ok(result);
}
```

### Pattern 3: Custom Action

```csharp
// Activate a campaign
[HttpPost("{id}/activate")]
public async Task<IActionResult> ActivateCampaign(int id)
{
    await _campaignService.ActivateCampaignAsync(id);
    return Ok();
}
```

---

## Summary

This reference guide provides:
- ✅ Complete working examples
- ✅ Proper layer separation
- ✅ RESTful API design
- ✅ Error handling patterns
- ✅ Dependency injection setup
- ✅ Naming conventions

**Use these patterns as templates for all modules in RetailConnect.**
