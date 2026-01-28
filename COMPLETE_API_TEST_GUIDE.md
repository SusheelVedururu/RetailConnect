# 🧪 Complete API Testing Guide - Step by Step

## 🎯 All 4 API Endpoints

| # | Method | Endpoint | Purpose |
|---|--------|----------|---------|
| 1 | `POST` | `/api/segments` | Create a new segment |
| 2 | `GET` | `/api/segments` | Get all segments |
| 3 | `GET` | `/api/segments/{id}` | Get segment by ID |
| 4 | `PUT` | `/api/segments/{id}` | Update a segment |

---

## 🌐 Method 1: Using Swagger UI (Easiest - Recommended!)

### Step 1: Open Swagger
1. Make sure API is running (`dotnet run`)
2. Open browser: `http://localhost:5005/swagger`
3. You'll see 4 endpoints listed

---

### Test 1: POST - Create a Segment

1. **Click** on the green `POST /api/segments` button
2. **Click** "Try it out" button (top right)
3. **Replace** the example JSON with this:
   ```json
   {
     "name": "Premium Customers",
     "description": "High-value customers with purchases over 10000",
     "criteria": "TotalPurchases > 10000",
     "isActive": true
   }
   ```
4. **Click** "Execute" button
5. **Scroll down** to see the response

**Expected Response (201 Created):**
```json
{
  "id": 1,
  "name": "Premium Customers",
  "description": "High-value customers with purchases over 10000",
  "criteria": "TotalPurchases > 10000",
  "isActive": true,
  "createdDate": "2026-01-28T12:30:00",
  "modifiedDate": null
}
```

✅ **Success!** Note the `id` value (you'll need it for the next tests)

---

### Test 2: GET - Get All Segments

1. **Click** on the blue `GET /api/segments` button (the first one)
2. **Click** "Try it out"
3. **Click** "Execute"

**Expected Response (200 OK):**
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

✅ **Success!** You see a list of all segments

---

### Test 3: GET - Get Segment by ID

1. **Click** on the blue `GET /api/segments/{id}` button (the second one)
2. **Click** "Try it out"
3. **Enter** `1` in the `id` field
4. **Click** "Execute"

**Expected Response (200 OK):**
```json
{
  "id": 1,
  "name": "Premium Customers",
  "description": "High-value customers with purchases over 10000",
  "criteria": "TotalPurchases > 10000",
  "isActive": true,
  "createdDate": "2026-01-28T12:30:00",
  "modifiedDate": null
}
```

✅ **Success!** You see the full details of segment ID 1

---

### Test 4: PUT - Update a Segment

1. **Click** on the orange `PUT /api/segments/{id}` button
2. **Click** "Try it out"
3. **Enter** `1` in the `id` field
4. **Replace** the JSON with this:
   ```json
   {
     "name": "Premium Customers - VIP",
     "description": "Updated: Very high-value customers",
     "criteria": "TotalPurchases > 15000",
     "isActive": true
   }
   ```
5. **Click** "Execute"

**Expected Response (200 OK):**
```json
{
  "id": 1,
  "name": "Premium Customers - VIP",
  "description": "Updated: Very high-value customers",
  "criteria": "TotalPurchases > 15000",
  "isActive": true,
  "createdDate": "2026-01-28T12:30:00",
  "modifiedDate": "2026-01-28T12:35:00"
}
```

✅ **Success!** The segment is updated (note the `modifiedDate` is now set)

---

## 💻 Method 2: Using PowerShell (Complete Test Script)

Copy and paste this entire script into PowerShell:

```powershell
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Testing RetailConnect API - All Endpoints" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$baseUrl = "http://localhost:5005/api/segments"

# Test 1: Create a Segment
Write-Host "Test 1: POST - Create a Segment" -ForegroundColor Yellow
$createBody = @{
    name = "PowerShell Test Segment"
    description = "Created via PowerShell"
    criteria = "Age > 30"
    isActive = $true
} | ConvertTo-Json

try {
    $created = Invoke-RestMethod -Uri $baseUrl `
        -Method POST `
        -ContentType "application/json" `
        -Body $createBody
    
    Write-Host "✅ SUCCESS - Created segment with ID: $($created.id)" -ForegroundColor Green
    Write-Host "   Name: $($created.name)" -ForegroundColor Gray
    Write-Host "   Description: $($created.description)" -ForegroundColor Gray
    Write-Host ""
    
    $segmentId = $created.id
} catch {
    Write-Host "❌ FAILED - $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    exit
}

# Test 2: Get All Segments
Write-Host "Test 2: GET - Get All Segments" -ForegroundColor Yellow
try {
    $allSegments = Invoke-RestMethod -Uri $baseUrl -Method GET
    
    Write-Host "✅ SUCCESS - Found $($allSegments.Count) segment(s)" -ForegroundColor Green
    $allSegments | Format-Table Id, Name, IsActive, MemberCount
} catch {
    Write-Host "❌ FAILED - $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
}

# Test 3: Get Segment by ID
Write-Host "Test 3: GET - Get Segment by ID ($segmentId)" -ForegroundColor Yellow
try {
    $segment = Invoke-RestMethod -Uri "$baseUrl/$segmentId" -Method GET
    
    Write-Host "✅ SUCCESS - Retrieved segment" -ForegroundColor Green
    Write-Host "   ID: $($segment.id)" -ForegroundColor Gray
    Write-Host "   Name: $($segment.name)" -ForegroundColor Gray
    Write-Host "   Description: $($segment.description)" -ForegroundColor Gray
    Write-Host "   Criteria: $($segment.criteria)" -ForegroundColor Gray
    Write-Host "   IsActive: $($segment.isActive)" -ForegroundColor Gray
    Write-Host "   CreatedDate: $($segment.createdDate)" -ForegroundColor Gray
    Write-Host ""
} catch {
    Write-Host "❌ FAILED - $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
}

# Test 4: Update Segment
Write-Host "Test 4: PUT - Update Segment ($segmentId)" -ForegroundColor Yellow
$updateBody = @{
    name = "PowerShell Test Segment - UPDATED"
    description = "Updated via PowerShell"
    criteria = "Age > 35"
    isActive = $true
} | ConvertTo-Json

try {
    $updated = Invoke-RestMethod -Uri "$baseUrl/$segmentId" `
        -Method PUT `
        -ContentType "application/json" `
        -Body $updateBody
    
    Write-Host "✅ SUCCESS - Updated segment" -ForegroundColor Green
    Write-Host "   New Name: $($updated.name)" -ForegroundColor Gray
    Write-Host "   New Description: $($updated.description)" -ForegroundColor Gray
    Write-Host "   New Criteria: $($updated.criteria)" -ForegroundColor Gray
    Write-Host "   ModifiedDate: $($updated.modifiedDate)" -ForegroundColor Gray
    Write-Host ""
} catch {
    Write-Host "❌ FAILED - $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
}

# Final Summary
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "All Tests Complete!" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Final verification - All segments:" -ForegroundColor Yellow
$final = Invoke-RestMethod -Uri $baseUrl -Method GET
$final | Format-Table Id, Name, IsActive
```

**Expected Output:**
```
========================================
Testing RetailConnect API - All Endpoints
========================================

Test 1: POST - Create a Segment
✅ SUCCESS - Created segment with ID: 1
   Name: PowerShell Test Segment
   Description: Created via PowerShell

Test 2: GET - Get All Segments
✅ SUCCESS - Found 1 segment(s)

Id Name                      IsActive MemberCount
-- ----                      -------- -----------
 1 PowerShell Test Segment       True           0

Test 3: GET - Get Segment by ID (1)
✅ SUCCESS - Retrieved segment
   ID: 1
   Name: PowerShell Test Segment
   Description: Created via PowerShell
   Criteria: Age > 30
   IsActive: True
   CreatedDate: 2026-01-28T12:30:00

Test 4: PUT - Update Segment (1)
✅ SUCCESS - Updated segment
   New Name: PowerShell Test Segment - UPDATED
   New Description: Updated via PowerShell
   New Criteria: Age > 35
   ModifiedDate: 2026-01-28T12:35:00

========================================
All Tests Complete!
========================================
```

---

## 🔧 Method 3: Using curl (Command Line)

### Test 1: Create Segment
```bash
curl -X POST "http://localhost:5005/api/segments" ^
  -H "Content-Type: application/json" ^
  -d "{\"name\":\"Curl Test Segment\",\"description\":\"Created via curl\",\"criteria\":\"Age > 25\",\"isActive\":true}"
```

### Test 2: Get All Segments
```bash
curl -X GET "http://localhost:5005/api/segments"
```

### Test 3: Get Segment by ID
```bash
curl -X GET "http://localhost:5005/api/segments/1"
```

### Test 4: Update Segment
```bash
curl -X PUT "http://localhost:5005/api/segments/1" ^
  -H "Content-Type: application/json" ^
  -d "{\"name\":\"Curl Test - Updated\",\"description\":\"Updated via curl\",\"criteria\":\"Age > 30\",\"isActive\":true}"
```

---

## 🧪 Method 4: Error Testing (Validation)

### Test Invalid Data (Empty Name)
**In Swagger:**
```json
{
  "name": "",
  "description": "Test",
  "isActive": true
}
```
**Expected:** `400 Bad Request` - "Segment name is required"

---

### Test Duplicate Name
1. Create a segment with name "Test"
2. Try to create another segment with the same name "Test"

**Expected:** `409 Conflict` - "Segment with name 'Test' already exists"

---

### Test Non-existent ID
**In Swagger:** GET `/api/segments/999`

**Expected:** `404 Not Found` - "Segment with ID 999 not found"

---

### Test Name Too Long
```json
{
  "name": "This is a very long segment name that exceeds the maximum allowed length of 100 characters and should fail validation",
  "description": "Test",
  "isActive": true
}
```
**Expected:** `400 Bad Request` - "Segment name cannot exceed 100 characters"

---

## 📊 Verify in Database

After testing, verify the data in SQL Server:

```sql
-- See all segments
SELECT * FROM RetailConnect.T_Segments;

-- Count segments
SELECT COUNT(*) AS TotalSegments FROM RetailConnect.T_Segments;

-- See only active segments
SELECT * FROM RetailConnect.T_Segments WHERE IsActive = 1;

-- See recently modified
SELECT * FROM RetailConnect.T_Segments 
WHERE ModifiedDate IS NOT NULL 
ORDER BY ModifiedDate DESC;
```

---

## 🎯 Complete Test Scenario (Recommended)

Run this complete scenario in Swagger:

### 1. Create Multiple Segments
Create these 3 segments one by one:

**Segment 1:**
```json
{
  "name": "Premium Customers",
  "description": "High-value customers",
  "criteria": "TotalPurchases > 10000",
  "isActive": true
}
```

**Segment 2:**
```json
{
  "name": "New Customers",
  "description": "Recently registered",
  "criteria": "RegistrationDate > DATEADD(day, -30, GETDATE())",
  "isActive": true
}
```

**Segment 3:**
```json
{
  "name": "Inactive Customers",
  "description": "No purchases in 90 days",
  "criteria": "LastPurchaseDate < DATEADD(day, -90, GETDATE())",
  "isActive": false
}
```

### 2. Get All Segments
You should see all 3 segments

### 3. Get Each Segment by ID
Test with IDs 1, 2, and 3

### 4. Update One Segment
Update segment ID 1 to change the criteria

### 5. Get All Again
Verify the update worked

---

## ✅ Success Checklist

After testing, you should have:

- [ ] Created at least 1 segment successfully (201 Created)
- [ ] Retrieved all segments (200 OK)
- [ ] Retrieved a specific segment by ID (200 OK)
- [ ] Updated a segment successfully (200 OK)
- [ ] Tested error scenarios (400, 404, 409)
- [ ] Verified data in database using SQL queries

---

## 🚀 Quick Start (Copy & Paste)

**Fastest way to test all endpoints:**

1. **Open:** `http://localhost:5005/swagger`
2. **Run these tests in order:**
   - POST → Create segment
   - GET (all) → See your segment
   - GET (by ID) → Get details
   - PUT → Update it
3. **Done!** ✅

---

**All 4 endpoints tested! Your API is working perfectly! 🎉**
