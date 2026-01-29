# 📚 RetailConnect API - Swagger Documentation

## 🌐 API Information

**Base URL:** `http://localhost:5005`  
**Swagger UI:** `http://localhost:5005/swagger`  
**Swagger JSON:** `http://localhost:5005/swagger/v1/swagger.json`  
**API Version:** v1  
**Framework:** ASP.NET Core 8.0

---

## 📦 Documentation Files

This package includes:

1. **`swagger.json`** - OpenAPI 3.0 specification (machine-readable)
2. **`SWAGGER_DOCUMENTATION.md`** - This file (human-readable guide)
3. **`SWAGGER_QUICK_GUIDE.md`** - Quick reference for testing
4. **`API_TEST_RESULTS.md`** - Detailed test results

---

## 🎯 Available API Endpoints

### **1. Segments API**

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/segments` | Get all segments |
| `GET` | `/api/segments/{id}` | Get segment by ID |
| `POST` | `/api/segments` | Create a new segment |
| `PUT` | `/api/segments/{id}` | Update a segment |

### **2. Campaigns API**

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/campaigns` | Get all campaigns |
| `GET` | `/api/campaigns/{id}` | Get campaign by ID |
| `POST` | `/api/campaigns` | Create a new campaign |
| `PUT` | `/api/campaigns/{id}` | Update a campaign |

### **3. Templates API**

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/templates` | Get all templates |
| `GET` | `/api/templates/{id}` | Get template by ID |
| `POST` | `/api/templates` | Create a new template |
| `PUT` | `/api/templates/{id}` | Update a template |

### **4. Touchpoints API**

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/touchpoints` | Get all touchpoints |
| `GET` | `/api/touchpoints/{id}` | Get touchpoint by ID |
| `POST` | `/api/touchpoints` | Create a new touchpoint |
| `PUT` | `/api/touchpoints/{id}` | Update a touchpoint |

---

## 📖 API Reference

### **Segments API**

#### **GET /api/segments**
Get all segments (summary view)

**Request:**
```http
GET /api/segments HTTP/1.1
Host: localhost:5005
```

**Response (200 OK):**
```json
[
  {
    "id": 1,
    "name": "Premium Customers",
    "isActive": true,
    "memberCount": 0
  }
]
```

---

#### **GET /api/segments/{id}**
Get detailed information about a specific segment

**Request:**
```http
GET /api/segments/1 HTTP/1.1
Host: localhost:5005
```

**Response (200 OK):**
```json
{
  "id": 1,
  "name": "Premium Customers",
  "description": "High-value customers",
  "criteria": "TotalPurchases > 10000",
  "isActive": true,
  "createdDate": "2026-01-28T12:30:00",
  "modifiedDate": null
}
```

**Response (404 Not Found):**
```json
"Segment with ID 999 not found"
```

---

#### **POST /api/segments**
Create a new segment

**Request:**
```http
POST /api/segments HTTP/1.1
Host: localhost:5005
Content-Type: application/json

{
  "name": "Premium Customers",
  "description": "High-value customers",
  "criteria": "TotalPurchases > 10000",
  "isActive": true
}
```

**Response (201 Created):**
```json
{
  "id": 1,
  "name": "Premium Customers",
  "description": "High-value customers",
  "criteria": "TotalPurchases > 10000",
  "isActive": true,
  "createdDate": "2026-01-28T12:30:00",
  "modifiedDate": null
}
```

**Response (400 Bad Request):**
```json
"Segment name is required"
```

**Response (409 Conflict):**
```json
"Segment with name 'Premium Customers' already exists"
```

---

#### **PUT /api/segments/{id}**
Update an existing segment

**Request:**
```http
PUT /api/segments/1 HTTP/1.1
Host: localhost:5005
Content-Type: application/json

{
  "name": "Premium Customers - VIP",
  "description": "Updated description",
  "criteria": "TotalPurchases > 15000",
  "isActive": true
}
```

**Response (200 OK):**
```json
{
  "id": 1,
  "name": "Premium Customers - VIP",
  "description": "Updated description",
  "criteria": "TotalPurchases > 15000",
  "isActive": true,
  "createdDate": "2026-01-28T12:30:00",
  "modifiedDate": "2026-01-28T14:30:00"
}
```

---

### **Campaigns API**

#### **POST /api/campaigns**
Create a new campaign

**Request:**
```http
POST /api/campaigns HTTP/1.1
Host: localhost:5005
Content-Type: application/json

{
  "name": "Summer Sale 2026",
  "description": "Summer promotional campaign",
  "segmentId": 1,
  "startDate": "2026-06-01T00:00:00",
  "endDate": "2026-08-31T23:59:59",
  "isActive": true
}
```

**Required Fields:**
- `name` (string, max 100 chars)
- `segmentId` (integer, must exist)
- `startDate` (ISO 8601 format)
- `endDate` (ISO 8601 format)
- `isActive` (boolean)

**Response (201 Created):**
```json
{
  "id": 1,
  "name": "Summer Sale 2026",
  "startDate": "2026-06-01T00:00:00",
  "endDate": "2026-08-31T23:59:59",
  "isActive": true,
  "createdDate": "2026-01-28T12:30:00"
}
```

---

### **Templates API**

#### **POST /api/templates**
Create a new template

**Request:**
```http
POST /api/templates HTTP/1.1
Host: localhost:5005
Content-Type: application/json

{
  "name": "Welcome Email",
  "description": "Welcome email for new customers",
  "content": "Hello {{CustomerName}}, welcome!",
  "subject": "Welcome to our store",
  "templateType": "Email",
  "isActive": true
}
```

**Template Types:**
- `Email`
- `SMS`
- `Push`

**Response (201 Created):**
```json
{
  "id": 1,
  "name": "Welcome Email",
  "content": "Hello {{CustomerName}}, welcome!",
  "subject": "Welcome to our store",
  "type": "Email",
  "isActive": true,
  "createdDate": "2026-01-28T12:30:00"
}
```

---

### **Touchpoints API**

#### **POST /api/touchpoints**
Create a new touchpoint

**Request:**
```http
POST /api/touchpoints HTTP/1.1
Host: localhost:5005
Content-Type: application/json

{
  "name": "Email Gateway",
  "type": "Email",
  "configuration": "smtp.gmail.com:587",
  "isActive": true
}
```

**⚠️ IMPORTANT:** Use `type` field, NOT `channelType`

**Response (201 Created):**
```json
{
  "id": 1,
  "name": "Email Gateway",
  "type": "Email",
  "configuration": "smtp.gmail.com:587",
  "isActive": true,
  "createdDate": "2026-01-28T12:30:00"
}
```

---

## 🔐 Authentication

Currently, the API does not require authentication. All endpoints are publicly accessible.

---

## 📊 Response Codes

| Code | Description | When It Occurs |
|------|-------------|----------------|
| `200` | OK | Successful GET or PUT request |
| `201` | Created | Successful POST request |
| `400` | Bad Request | Invalid data, missing required fields |
| `404` | Not Found | Resource with specified ID doesn't exist |
| `409` | Conflict | Duplicate name (resource already exists) |
| `500` | Server Error | Database or server issue |

---

## 🧪 Testing the API

### **Using Swagger UI (Recommended)**

1. Open: `http://localhost:5005/swagger`
2. Click on any endpoint to expand it
3. Click "Try it out"
4. Enter your test data
5. Click "Execute"
6. View the response below

### **Using PowerShell**

```powershell
# Get all segments
Invoke-RestMethod -Uri "http://localhost:5005/api/segments" -Method GET

# Create a segment
$body = @{
    name = "Test Segment"
    description = "Testing"
    isActive = $true
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5005/api/segments" `
    -Method POST `
    -ContentType "application/json" `
    -Body $body
```

### **Using cURL**

```bash
# Get all segments
curl -X GET "http://localhost:5005/api/segments"

# Create a segment
curl -X POST "http://localhost:5005/api/segments" \
  -H "Content-Type: application/json" \
  -d '{"name":"Test Segment","description":"Testing","isActive":true}'
```

### **Using Postman**

1. Import the `swagger.json` file into Postman
2. Postman will automatically create a collection with all endpoints
3. Set the base URL to `http://localhost:5005`
4. Test each endpoint

---

## 📋 Data Models

### **Segment**

```typescript
{
  id: number,              // Auto-generated
  name: string,            // Required, max 100 chars, unique
  description?: string,    // Optional
  criteria?: string,       // Optional, SQL-like condition
  isActive: boolean,       // Required
  createdDate: DateTime,   // Auto-generated
  modifiedDate?: DateTime  // Auto-updated
}
```

### **Campaign**

```typescript
{
  id: number,              // Auto-generated
  name: string,            // Required, max 100 chars
  description?: string,    // Optional
  segmentId: number,       // Required, must exist
  startDate: DateTime,     // Required
  endDate: DateTime,       // Required
  isActive: boolean,       // Required
  createdDate: DateTime    // Auto-generated
}
```

### **Template**

```typescript
{
  id: number,              // Auto-generated
  name: string,            // Required, max 100 chars
  description?: string,    // Optional
  content: string,         // Required
  subject?: string,        // Optional
  type: string,            // Required (Email, SMS, Push)
  isActive: boolean,       // Required
  createdDate: DateTime    // Auto-generated
}
```

### **Touchpoint**

```typescript
{
  id: number,              // Auto-generated
  name: string,            // Required, max 100 chars
  type: string,            // Required (Email, SMS, etc.)
  configuration?: string,  // Optional
  isActive: boolean,       // Required
  createdDate: DateTime    // Auto-generated
}
```

---

## ⚠️ Common Errors

### **409 Conflict - Duplicate Name**
```json
"Segment with name 'Test' already exists"
```
**Solution:** Use a unique name

### **400 Bad Request - Missing Field**
```json
"Segment name is required"
```
**Solution:** Include all required fields

### **404 Not Found**
```json
"Segment with ID 999 not found"
```
**Solution:** Use a valid ID

### **400 Bad Request - Wrong Field Name (Touchpoints)**
```json
"Type is required"
```
**Solution:** Use `type` instead of `channelType`

---

## 🚀 Quick Start

1. **Start the API:**
   ```powershell
   cd C:\Users\shiva\source\repos\RetailConnect
   dotnet run
   ```

2. **Open Swagger UI:**
   ```
   http://localhost:5005/swagger
   ```

3. **Test an endpoint:**
   - Click `GET /api/segments`
   - Click "Try it out"
   - Click "Execute"
   - View the results

---

## 📞 Support

For questions or issues, contact the development team.

**API Status:** ✅ All endpoints tested and working  
**Last Updated:** January 29, 2026  
**Version:** 1.0

---

## 📦 Files Included

- `swagger.json` - OpenAPI specification
- `SWAGGER_DOCUMENTATION.md` - This documentation
- `SWAGGER_QUICK_GUIDE.md` - Quick reference guide
- `API_TEST_RESULTS.md` - Test results and troubleshooting

---

**Happy Testing! 🎯**
