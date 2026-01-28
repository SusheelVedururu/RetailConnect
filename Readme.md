# RetailConnect - ASP.NET Core Web API

## Overview
RetailConnect is an enterprise-grade ASP.NET Core Web API built with a **strict layered architecture**. This project follows enterprise best practices with clear separation of concerns and uses ADO.NET with stored procedures for all database operations.

---

## Architecture

This project implements a **5-layer strict architecture**:

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

### Key Principles
- ✅ Each layer communicates ONLY with the layer directly below it
- ✅ Controllers are THIN (no business logic)
- ✅ ALL business rules live in the Service layer
- ✅ Data Access layer ONLY executes stored procedures
- ✅ NO inline SQL anywhere in the project

---

## Project Structure

```
RetailConnect.API/
│
├── Controllers/              ← API endpoints (HTTP routing only)
│   └── SegmentController.cs
│
├── Services/                 ← Business logic layer
│   ├── Interfaces/
│   │   └── ISegmentService.cs
│   └── Implementations/
│       └── SegmentService.cs
│
├── Models/                   ← DTOs (Request/Response models)
│   └── SegmentModels.cs
│
├── Data/                     ← ADO.NET + Stored Procedure calls
│   └── SegmentDataAccess.cs
│
├── Database/                 ← SQL scripts
│   └── StoredProcedures.sql
│
├── Program.cs               ← Application startup & DI configuration
└── appsettings.json         ← Configuration (including connection string)
```

---

## Technology Stack

- **Framework**: ASP.NET Core 8.0 Web API
- **Database**: SQL Server
- **Data Access**: ADO.NET (System.Data.SqlClient)
- **Database Logic**: Stored Procedures ONLY
- **API Documentation**: Swagger/OpenAPI (Swashbuckle)
- **Dependency Injection**: Built-in ASP.NET Core DI

### What's NOT Used (By Design)
- ❌ Entity Framework (EF Core)
- ❌ Repository Pattern
- ❌ Unit of Work Pattern
- ❌ CQRS / MediatR
- ❌ Inline SQL or Dynamic Queries

---

## Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- SQL Server (Express, Developer, or Standard edition)
- SQL Server Management Studio (SSMS) or Azure Data Studio

### 1. Clone the Repository
```bash
git clone <repository-url>
cd RetailConnect
```

### 2. Setup Database

#### Option A: Using SSMS
1. Open SQL Server Management Studio
2. Connect to your SQL Server instance
3. Open the file `Database/StoredProcedures.sql`
4. Execute the entire script (this creates the database, tables, and stored procedures)

#### Option B: Using Command Line
```bash
sqlcmd -S YOUR_SERVER_NAME -i Database/StoredProcedures.sql
```

### 3. Configure Connection String

Update `appsettings.json` with your SQL Server connection details:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=RetailConnect;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**Common Server Names:**
- Local SQL Server: `localhost` or `.`
- LocalDB: `(localdb)\\mssqllocaldb`
- SQL Server Express: `.\\SQLEXPRESS`
- Remote Server: `your-server-name` or `IP_ADDRESS`

**For SQL Authentication (instead of Windows Authentication):**
```json
"DefaultConnection": "Server=YOUR_SERVER;Database=RetailConnect;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
```

### 4. Restore NuGet Packages
```bash
dotnet restore
```

### 5. Build the Project
```bash
dotnet build
```

### 6. Run the Application
```bash
dotnet run
```

The API will start at:
- HTTPS: `https://localhost:7xxx`
- HTTP: `http://localhost:5xxx`

(Exact ports will be shown in the console output)

### 7. Access Swagger UI
Open your browser and navigate to:
```
https://localhost:7xxx/swagger
```

---

## API Endpoints

### Segment Management

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/segments` | Get all segments |
| `GET` | `/api/segments/{id}` | Get segment by ID |
| `POST` | `/api/segments` | Create new segment |
| `PUT` | `/api/segments/{id}` | Update existing segment |

### Example Requests

#### Create Segment
```bash
POST /api/segments
Content-Type: application/json

{
  "name": "Premium Customers",
  "description": "Customers with high purchase value",
  "criteria": "TotalPurchases > 10000",
  "isActive": true
}
```

#### Get All Segments
```bash
GET /api/segments
```

#### Get Segment by ID
```bash
GET /api/segments/1
```

#### Update Segment
```bash
PUT /api/segments/1
Content-Type: application/json

{
  "name": "Premium Customers Updated",
  "description": "Updated description",
  "criteria": "TotalPurchases > 15000",
  "isActive": true
}
```

---

## Connection String Location

### ⭐ **The connection string is configured in `appsettings.json`**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=RetailConnect;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### How It's Used

1. **Configuration File**: Connection string is stored in `appsettings.json`
2. **Dependency Injection**: `IConfiguration` is injected into Data Access classes
3. **Data Access Layer**: Retrieves connection string via:
   ```csharp
   _connectionString = configuration.GetConnectionString("DefaultConnection");
   ```

### Example in Code (SegmentDataAccess.cs)
```csharp
public class SegmentDataAccess
{
    private readonly string _connectionString;

    public SegmentDataAccess(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }
    
    // ... rest of the code
}
```

---

## Development Guidelines

### Adding a New Module

When adding a new module (e.g., Campaigns), follow this order:

1. **Create DTOs** (`Models/CampaignModels.cs`)
   - Request models
   - Response models

2. **Create Data Access** (`Data/CampaignDataAccess.cs`)
   - Methods to call stored procedures
   - Use ADO.NET only

3. **Create Service Interface** (`Services/Interfaces/ICampaignService.cs`)
   - Define business operation contracts

4. **Create Service Implementation** (`Services/Implementations/CampaignService.cs`)
   - Implement ALL business logic
   - Implement ALL validations

5. **Create Controller** (`Controllers/CampaignController.cs`)
   - Thin controller (routing only)
   - No business logic

6. **Register in DI** (`Program.cs`)
   ```csharp
   builder.Services.AddScoped<ICampaignService, CampaignService>();
   builder.Services.AddScoped<CampaignDataAccess>();
   ```

7. **Create Stored Procedures** (SQL Server)
   - Add to `Database/StoredProcedures.sql`

---

## Architecture Documentation

For detailed architecture guidelines, see:
- `.agent/Architecture.md` - Complete architecture specification
- `.agent/Rules.md` - Development rules and constraints
- `.agent/APIReference.md` - API implementation examples
- `.agent/SQLSchema.md` - Database schema and SP examples

---

## Testing

### Using Swagger UI
1. Run the application
2. Navigate to `https://localhost:xxxx/swagger`
3. Test endpoints directly from the browser

### Using Postman or curl
```bash
# Get all segments
curl -X GET https://localhost:7xxx/api/segments

# Create segment
curl -X POST https://localhost:7xxx/api/segments \
  -H "Content-Type: application/json" \
  -d '{"name":"Test Segment","description":"Test","isActive":true}'
```

---

## Troubleshooting

### Issue: Cannot connect to database
**Solution**: 
- Verify SQL Server is running
- Check connection string in `appsettings.json`
- Test connection using SSMS

### Issue: Stored procedure not found
**Solution**: 
- Run `Database/StoredProcedures.sql` script
- Verify procedures exist in SSMS under Programmability > Stored Procedures

### Issue: Build errors
**Solution**: 
```bash
dotnet clean
dotnet restore
dotnet build
```

---

## Contributing

When contributing to this project:
1. Follow the strict layered architecture
2. Never skip layers
3. Keep controllers thin
4. Put ALL business logic in Services
5. Use stored procedures only (no inline SQL)
6. Follow the naming conventions

---

## License

[Your License Here]

---

## Contact

[Your Contact Information]
