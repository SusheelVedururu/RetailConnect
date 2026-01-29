# 🔧 Campaign-Touchpoint Error - Solution Guide

## ❌ Error You're Seeing

```
500 Internal Server Error
Violation of UNIQUE KEY constraint 'UQ_Campaign_Touchpoint_Sequence'. 
Cannot insert duplicate key in object 'RetailConnect.T_Campaign_Touchpoints'. 
The duplicate key value is (1, 1).
```

---

## 🔍 What This Means

The database has a **UNIQUE constraint** that prevents duplicate `(CampaignId, SequenceOrder)` combinations.

**Translation:** Each campaign can only have **ONE touchpoint per sequence order number**.

For example:
- Campaign 1 can have touchpoint at sequence 1
- Campaign 1 can have touchpoint at sequence 2
- Campaign 1 can have touchpoint at sequence 3
- But Campaign 1 **CANNOT** have two different touchpoints both at sequence 1

---

## ✅ Solutions

### **Solution 1: Use a Different Sequence Order**

If Campaign 1 already has a touchpoint at sequence 1, use sequence 2, 3, 4, etc.

**❌ This will fail:**
```json
{
  "touchpointId": 1,
  "sequenceOrder": 1,  // Already exists!
  "isActive": true
}
```

**✅ This will work:**
```json
{
  "touchpointId": 1,
  "sequenceOrder": 3,  // Use next available sequence
  "isActive": true
}
```

---

### **Solution 2: Check Existing Touchpoints First**

Before adding, check what sequence orders are already used:

**Step 1: Get existing touchpoints for the campaign**
```http
GET /api/campaigns/1/touchpoints
```

**Response:**
```json
[
  {
    "touchpointId": 2,
    "sequenceOrder": 2,
    "isActive": true
  }
]
```

**Step 2: Use a sequence order that's NOT in the list**

Since sequence 2 is used, you can use 1, 3, 4, 5, etc.

---

### **Solution 3: Use a Different Campaign**

Test with a campaign that has no touchpoints yet:

```json
POST /api/campaigns/14/touchpoints

{
  "touchpointId": 1,
  "sequenceOrder": 1,
  "isActive": true
}
```

---

## 🧪 Working Example

### **Step 1: Find a campaign with no touchpoints**

```powershell
# Get all campaigns
$campaigns = Invoke-RestMethod -Uri "http://localhost:5005/api/campaigns" -Method GET
$campaigns | Format-Table Id, Name
```

### **Step 2: Add a touchpoint to that campaign**

```powershell
$body = @{
    touchpointId = 1
    sequenceOrder = 1
    isActive = $true
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5005/api/campaigns/14/touchpoints" `
    -Method POST `
    -ContentType "application/json" `
    -Body $body
```

---

## 📊 Understanding the Constraint

The constraint `UQ_Campaign_Touchpoint_Sequence` ensures:

| CampaignId | SequenceOrder | Status |
|------------|---------------|--------|
| 1 | 1 | ✅ Allowed (first entry) |
| 1 | 2 | ✅ Allowed (different sequence) |
| 1 | 1 | ❌ **NOT ALLOWED** (duplicate) |
| 2 | 1 | ✅ Allowed (different campaign) |

**Key Point:** The combination of `(CampaignId, SequenceOrder)` must be unique.

---

## 🎯 Best Practice

**Always check existing touchpoints before adding:**

1. **GET** `/api/campaigns/{id}/touchpoints` to see what's already there
2. Find the **highest sequence order** in the response
3. Use **next sequence number** (highest + 1)

**Example:**
```
Existing touchpoints: [sequence 1, sequence 2, sequence 5]
Next available: 3, 4, 6, 7, 8, etc.
```

---

## 🔧 Quick Fix for Your Current Error

Since Campaign 1 has issues, try with Campaign 14:

**In Swagger:**
1. Click `POST /api/campaigns/{campaignId}/touchpoints`
2. Enter `campaignId`: **14** (instead of 1)
3. Use this JSON:
```json
{
  "touchpointId": 1,
  "sequenceOrder": 1,
  "isActive": true
}
```
4. Click Execute

**Expected Result:** ✅ 200 OK

---

## 📝 Summary

**Problem:** Trying to add a touchpoint with a sequence order that already exists for that campaign

**Solution:** 
- Use a different sequence order, OR
- Use a different campaign, OR
- Check existing touchpoints first and use next available sequence

**API Port:** Your API is running on **port 5005** (HTTP) and **port 7211** (HTTPS)

---

## ✅ Test This Now

Try this in Swagger:

```
POST /api/campaigns/14/touchpoints

{
  "touchpointId": 1,
  "sequenceOrder": 1,
  "isActive": true
}
```

This should work! ✅
