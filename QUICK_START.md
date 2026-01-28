# 🚀 QUICK START - Run This Now!

## ✅ What You Need to Do (3 Simple Steps)

### Step 1: Run the SQL Script in SSMS

1. **Open** the file: `Database/CreateStoredProcedures.sql`
2. **Copy** all the content (Ctrl+A, Ctrl+C)
3. **Open** SQL Server Management Studio (SSMS)
4. **Connect** to: `192.168.1.18` (with login: RetailConnect / Prodigy@12#34)
5. **Click** "New Query"
6. **Paste** the SQL script
7. **Press** F5 to execute

**You should see:**
```
========================================
Creating Stored Procedures...
========================================
Dropped existing procedures (if any)
✓ Created: usp_CreateSegment
✓ Created: usp_GetSegmentById
✓ Created: usp_GetAllSegments
✓ Created: usp_UpdateSegment
✓ Created: usp_CheckSegmentExists

========================================
Total: 5 stored procedures created successfully!
========================================
```

---

### Step 2: Verify Stored Procedures

In SSMS, expand:
```
RetailConnect
  └── Programmability
       └── Stored Procedures
```

You should see:
- ✅ dbo.usp_CreateSegment
- ✅ dbo.usp_GetSegmentById
- ✅ dbo.usp_GetAllSegments
- ✅ dbo.usp_UpdateSegment
- ✅ dbo.usp_CheckSegmentExists

---

### Step 3: Test the API

1. **Open browser**: `http://localhost:5005/swagger`
2. **Click** on green `POST /api/segments`
3. **Click** "Try it out"
4. **Paste** this JSON:
```json
{
  "name": "Premium Customers",
  "description": "High-value customers with purchases over 10000",
  "criteria": "TotalPurchases > 10000",
  "isActive": true
}
```
5. **Click** "Execute"

**Expected Result:**
```json
{
  "id": 1,
  "name": "Premium Customers",
  "description": "High-value customers with purchases over 10000",
  "criteria": "TotalPurchases > 10000",
  "isActive": true,
  "createdDate": "2026-01-28T12:17:00",
  "modifiedDate": null
}
```

**Status Code:** `201 Created` ✅

---

## 🧪 Quick PowerShell Test (Optional)

Run this in PowerShell to test all endpoints:

```powershell
# Test 1: Create a segment
$body = @{
    name = "VIP Customers"
    description = "Very important customers"
    criteria = "TotalPurchases > 50000"
    isActive = $true
} | ConvertTo-Json

$created = Invoke-RestMethod -Uri "http://localhost:5005/api/segments" `
    -Method POST -ContentType "application/json" -Body $body

Write-Host "✅ Created segment ID: $($created.id)" -ForegroundColor Green

# Test 2: Get all segments
$all = Invoke-RestMethod -Uri "http://localhost:5005/api/segments" -Method GET
Write-Host "✅ Total segments: $($all.Count)" -ForegroundColor Green
$all | Format-Table

# Test 3: Get specific segment
$segment = Invoke-RestMethod -Uri "http://localhost:5005/api/segments/$($created.id)" -Method GET
Write-Host "✅ Retrieved segment: $($segment.name)" -ForegroundColor Green

# Test 4: Update segment
$updateBody = @{
    name = "VIP Customers - Updated"
    description = "Updated description"
    criteria = "TotalPurchases > 75000"
    isActive = $true
} | ConvertTo-Json

$updated = Invoke-RestMethod -Uri "http://localhost:5005/api/segments/$($created.id)" `
    -Method PUT -ContentType "application/json" -Body $updateBody

Write-Host "✅ Updated segment: $($updated.name)" -ForegroundColor Green
```

---

## 📊 Verify in Database

After creating segments via API, verify in SSMS:

```sql
-- See all segments
SELECT * FROM RetailConnect.T_Segments;

-- Count segments
SELECT COUNT(*) AS TotalSegments FROM RetailConnect.T_Segments;
```

---

## ✅ Success Checklist

- [ ] SQL script executed successfully
- [ ] 5 stored procedures visible in SSMS
- [ ] API is running (http://localhost:5005)
- [ ] Swagger UI accessible
- [ ] Created first segment via Swagger (201 Created)
- [ ] Retrieved all segments (200 OK)
- [ ] Data visible in database

---

## 🎯 That's It!

Your RetailConnect API is now **fully functional**! 🎉

**Next Steps:**
- Test all 4 endpoints in Swagger
- Add more segments
- Build additional modules (Campaigns, Customers, etc.)

---

**File Locations:**
- SQL Script: `Database/CreateStoredProcedures.sql`
- API Testing Guide: `API_TESTING_GUIDE.md`
- Architecture Docs: `.agent/Architecture.md`
