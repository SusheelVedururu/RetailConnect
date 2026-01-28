# RetailConnect API - Complete Testing Guide

## 🌐 API Base URLs

When you run the application with `dotnet run`, the API will be available at:

- **HTTPS**: `https://localhost:7211`
- **HTTP**: `http://localhost:5005`
- **Swagger UI**: `https://localhost:7211/swagger`

---

## 📋 Complete API Endpoints

### Base URL: `https://localhost:7211/api/segments`

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/segments` | Create a new segment |
| `GET` | `/api/segments` | Get all segments |
| `GET` | `/api/segments/{id}` | Get segment by ID |
| `PUT` | `/api/segments/{id}` | Update existing segment |

---

## 🧪 Testing Methods

### Method 1: Using Swagger UI (Easiest)

1. **Run the application**:
   ```bash
   dotnet run
   ```

2. **Open Swagger in browser**:
   ```
   https://localhost:7211/swagger
   ```

3. **Click on any endpoint** → Click "Try it out" → Fill in the request → Click "Execute"

---

### Method 2: Using PowerShell (Windows)

#### 1️⃣ Create a New Segment
```powershell
$body = @{
    name = "Premium Customers"
    description = "High-value customers with purchases over 10000"
    criteria = "TotalPurchases > 10000"
    isActive = $true
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:7211/api/segments" `
    -Method POST `
    -ContentType "application/json" `
    -Body $body `
    -SkipCertificateCheck
```

**Expected Response (201 Created)**:
```json
{
  "id": 1,
  "name": "Premium Customers",
  "description": "High-value customers with purchases over 10000",
  "criteria": "TotalPurchases > 10000",
  "isActive": true,
  "createdDate": "2026-01-28T12:00:00",
  "modifiedDate": null
}
```

---

#### 2️⃣ Get All Segments
```powershell
Invoke-RestMethod -Uri "https://localhost:7211/api/segments" `
    -Method GET `
    -SkipCertificateCheck
```

**Expected Response (200 OK)**:
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

#### 3️⃣ Get Segment by ID
```powershell
Invoke-RestMethod -Uri "https://localhost:7211/api/segments/1" `
    -Method GET `
    -SkipCertificateCheck
```

**Expected Response (200 OK)**:
```json
{
  "id": 1,
  "name": "Premium Customers",
  "description": "High-value customers with purchases over 10000",
  "criteria": "TotalPurchases > 10000",
  "isActive": true,
  "createdDate": "2026-01-28T12:00:00",
  "modifiedDate": null
}
```

---

#### 4️⃣ Update a Segment
```powershell
$body = @{
    name = "Premium Customers Updated"
    description = "Updated description for premium customers"
    criteria = "TotalPurchases > 15000"
    isActive = $true
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:7211/api/segments/1" `
    -Method PUT `
    -ContentType "application/json" `
    -Body $body `
    -SkipCertificateCheck
```

**Expected Response (200 OK)**:
```json
{
  "id": 1,
  "name": "Premium Customers Updated",
  "description": "Updated description for premium customers",
  "criteria": "TotalPurchases > 15000",
  "isActive": true,
  "createdDate": "2026-01-28T12:00:00",
  "modifiedDate": "2026-01-28T12:05:00"
}
```

---

### Method 3: Using curl (Cross-platform)

#### 1️⃣ Create a New Segment
```bash
curl -X POST "https://localhost:7211/api/segments" ^
  -H "Content-Type: application/json" ^
  -d "{\"name\":\"Premium Customers\",\"description\":\"High-value customers\",\"criteria\":\"TotalPurchases > 10000\",\"isActive\":true}" ^
  -k
```

#### 2️⃣ Get All Segments
```bash
curl -X GET "https://localhost:7211/api/segments" -k
```

#### 3️⃣ Get Segment by ID
```bash
curl -X GET "https://localhost:7211/api/segments/1" -k
```

#### 4️⃣ Update a Segment
```bash
curl -X PUT "https://localhost:7211/api/segments/1" ^
  -H "Content-Type: application/json" ^
  -d "{\"name\":\"Premium Customers Updated\",\"description\":\"Updated\",\"criteria\":\"TotalPurchases > 15000\",\"isActive\":true}" ^
  -k
```

---

### Method 4: Using Postman

#### Setup:
1. Download and install [Postman](https://www.postman.com/downloads/)
2. Create a new collection called "RetailConnect API"

#### 1️⃣ Create Segment
- **Method**: `POST`
- **URL**: `https://localhost:7211/api/segments`
- **Headers**: 
  - `Content-Type: application/json`
- **Body** (raw JSON):
```json
{
  "name": "Premium Customers",
  "description": "High-value customers with purchases over 10000",
  "criteria": "TotalPurchases > 10000",
  "isActive": true
}
```

#### 2️⃣ Get All Segments
- **Method**: `GET`
- **URL**: `https://localhost:7211/api/segments`

#### 3️⃣ Get Segment by ID
- **Method**: `GET`
- **URL**: `https://localhost:7211/api/segments/1`

#### 4️⃣ Update Segment
- **Method**: `PUT`
- **URL**: `https://localhost:7211/api/segments/1`
- **Headers**: 
  - `Content-Type: application/json`
- **Body** (raw JSON):
```json
{
  "name": "Premium Customers Updated",
  "description": "Updated description",
  "criteria": "TotalPurchases > 15000",
  "isActive": true
}
```

---

## 🧪 Complete Test Scenario

### Step-by-Step Testing Flow:

```powershell
# 1. Create first segment
$segment1 = @{
    name = "Premium Customers"
    description = "Customers with high purchase value"
    criteria = "TotalPurchases > 10000"
    isActive = $true
} | ConvertTo-Json

$result1 = Invoke-RestMethod -Uri "https://localhost:7211/api/segments" `
    -Method POST -ContentType "application/json" -Body $segment1 -SkipCertificateCheck

Write-Host "Created Segment ID: $($result1.id)"

# 2. Create second segment
$segment2 = @{
    name = "New Customers"
    description = "Customers registered in last 30 days"
    criteria = "RegistrationDate > DATEADD(day, -30, GETDATE())"
    isActive = $true
} | ConvertTo-Json

$result2 = Invoke-RestMethod -Uri "https://localhost:7211/api/segments" `
    -Method POST -ContentType "application/json" -Body $segment2 -SkipCertificateCheck

Write-Host "Created Segment ID: $($result2.id)"

# 3. Get all segments
$allSegments = Invoke-RestMethod -Uri "https://localhost:7211/api/segments" `
    -Method GET -SkipCertificateCheck

Write-Host "Total Segments: $($allSegments.Count)"
$allSegments | Format-Table

# 4. Get specific segment
$segment = Invoke-RestMethod -Uri "https://localhost:7211/api/segments/1" `
    -Method GET -SkipCertificateCheck

Write-Host "Retrieved Segment:"
$segment | Format-List

# 5. Update segment
$updateData = @{
    name = "Premium Customers - VIP"
    description = "Updated description for premium customers"
    criteria = "TotalPurchases > 15000"
    isActive = $true
} | ConvertTo-Json

$updated = Invoke-RestMethod -Uri "https://localhost:7211/api/segments/1" `
    -Method PUT -ContentType "application/json" -Body $updateData -SkipCertificateCheck

Write-Host "Updated Segment:"
$updated | Format-List
```

---

## ❌ Error Testing

### Test Validation Errors:

#### 1️⃣ Missing Name (400 Bad Request)
```powershell
$body = @{
    name = ""
    description = "Test"
    isActive = $true
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:7211/api/segments" `
    -Method POST -ContentType "application/json" -Body $body -SkipCertificateCheck
```
**Expected**: `400 Bad Request` - "Segment name is required"

---

#### 2️⃣ Duplicate Name (409 Conflict)
```powershell
# Create first segment
$body = @{
    name = "Test Segment"
    description = "First"
    isActive = $true
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:7211/api/segments" `
    -Method POST -ContentType "application/json" -Body $body -SkipCertificateCheck

# Try to create duplicate
Invoke-RestMethod -Uri "https://localhost:7211/api/segments" `
    -Method POST -ContentType "application/json" -Body $body -SkipCertificateCheck
```
**Expected**: `409 Conflict` - "Segment with name 'Test Segment' already exists"

---

#### 3️⃣ Non-existent ID (404 Not Found)
```powershell
Invoke-RestMethod -Uri "https://localhost:7211/api/segments/999" `
    -Method GET -SkipCertificateCheck
```
**Expected**: `404 Not Found` - "Segment with ID 999 not found"

---

## 📊 Expected HTTP Status Codes

| Status Code | Scenario | Example |
|-------------|----------|---------|
| `200 OK` | Successful GET or PUT | Get segment, Update segment |
| `201 Created` | Successful POST | Create new segment |
| `400 Bad Request` | Validation error | Empty name, name too long |
| `404 Not Found` | Resource not found | Get segment with invalid ID |
| `409 Conflict` | Business rule violation | Duplicate segment name |
| `500 Internal Server Error` | Database error | Connection failed, SP error |

---

## 🔍 Troubleshooting

### Issue: SSL Certificate Error
**Solution**: Add `-SkipCertificateCheck` (PowerShell) or `-k` (curl)

### Issue: Connection Refused
**Solution**: 
1. Make sure the API is running (`dotnet run`)
2. Check the correct port (7211 for HTTPS, 5005 for HTTP)

### Issue: 500 Internal Server Error
**Solution**: 
1. Check database connection string in `appsettings.json`
2. Verify database and stored procedures exist
3. Check API console for error details

---

## 🚀 Quick Start Commands

### Run the API:
```bash
cd c:\Users\shiva\source\repos\RetailConnect
dotnet run
```

### Open Swagger:
```
https://localhost:7211/swagger
```

### Quick Test (PowerShell):
```powershell
# Create a segment
$body = @{ name = "Test"; description = "Test Segment"; isActive = $true } | ConvertTo-Json
Invoke-RestMethod -Uri "https://localhost:7211/api/segments" -Method POST -ContentType "application/json" -Body $body -SkipCertificateCheck

# Get all segments
Invoke-RestMethod -Uri "https://localhost:7211/api/segments" -Method GET -SkipCertificateCheck
```

---

## 📝 Sample Request Bodies

### CreateSegmentRequest
```json
{
  "name": "Premium Customers",
  "description": "High-value customers",
  "criteria": "TotalPurchases > 10000",
  "isActive": true
}
```

### UpdateSegmentRequest
```json
{
  "name": "Premium Customers Updated",
  "description": "Updated description",
  "criteria": "TotalPurchases > 15000",
  "isActive": false
}
```

---

**Happy Testing! 🎉**
