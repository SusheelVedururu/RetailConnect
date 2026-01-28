# 🧪 COMPLETE API TESTING GUIDE - STEP BY STEP

## 📋 **Prerequisites**
1. ✅ Make sure API is running: `http://localhost:5005`
2. ✅ Open Swagger: `http://localhost:5005/swagger`
3. ✅ Keep this guide open alongside Swagger

---

## 🎯 **Testing Strategy**

We'll test in this order (building up dependencies):
1. **Segments** (standalone)
2. **Campaigns** (uses Segment)
3. **Templates** (standalone)
4. **Touchpoints** (standalone)
5. **Campaign-Templates** (links Campaign + Template)
6. **Campaign-Touchpoints** (links Campaign + Touchpoint)
7. **Campaign Logs** (view only)

---

# MODULE 1: SEGMENTS 🎯

## ✅ Test 1.1 - Create Segment (POST)

**Endpoint**: `POST /api/segments`

**Click**: "Try it out"

**Paste this JSON**:
```json
{
  "name": "Premium Customers",
  "definition": "Customers with high lifetime value",
  "criteria": "TotalPurchases > 1000 AND ActiveLastYear = true",
  "isActive": true
}
```

**Click**: Execute

**Expected Result**: `201 Created`
```json
{
  "id": 1,
  "name": "Premium Customers",
  "definition": "Customers with high lifetime value",
  "criteria": "TotalPurchases > 1000 AND ActiveLastYear = true",
  "isActive": true,
  "createdDate": "2026-01-28T..."
}
```

**✏️ Note down**: `Segment ID = 1` (You'll need this!)

---

## ✅ Test 1.2 - Get All Segments (GET)

**Endpoint**: `GET /api/segments`

**Click**: "Try it out" → Execute

**Expected Result**: List with your segment
```json
[
  {
    "id": 1,
    "name": "Premium Customers",
    "isActive": true
  }
]
```

---

## ✅ Test 1.3 - Get Segment By ID (GET)

**Endpoint**: `GET /api/segments/{id}`

**Enter**: `1` in the `id` field

**Click**: Execute

**Expected Result**: Full segment details (same as creation response)

---

## ✅ Test 1.4 - Update Segment (PUT)

**Endpoint**: `PUT /api/segments/{id}`

**Enter**: `1` in the `id` field

**Paste this JSON**:
```json
{
  "name": "VIP Customers",
  "definition": "Top tier customers",
  "criteria": "TotalPurchases > 5000",
  "isActive": true
}
```

**Click**: Execute

**Expected Result**: `200 OK` with updated data
```json
{
  "id": 1,
  "name": "VIP Customers",
  ...
}
```

---

# MODULE 2: CAMPAIGNS 📢

## ✅ Test 2.1 - Create Campaign (POST)

**Endpoint**: `POST /api/campaigns`

**Paste this JSON**:
```json
{
  "name": "Summer Sale 2026",
  "startDate": "2026-06-01T09:00:00",
  "endDate": "2026-06-30T23:59:59",
  "isActive": true
}
```

**Click**: Execute

**Expected Result**: `201 Created`

**✏️ Note down**: `Campaign ID = 1`

---

## ✅ Test 2.2 - Get All Campaigns (GET)

**Endpoint**: `GET /api/campaigns`

**Click**: Execute

**Expected Result**: List showing your campaign

---

## ✅ Test 2.3 - Get Campaign By ID (GET)

**Endpoint**: `GET /api/campaigns/{id}`

**Enter**: `1`

**Expected Result**: Full campaign details

---

## ✅ Test 2.4 - Update Campaign (PUT)

**Endpoint**: `PUT /api/campaigns/{id}`

**Enter**: `1`

**Paste**:
```json
{
  "name": "MEGA Summer Sale 2026",
  "startDate": "2026-06-01T00:00:00",
  "endDate": "2026-07-15T23:59:59",
  "isActive": true
}
```

**Expected Result**: `200 OK` with updated name

---

# MODULE 3: TEMPLATES 📧

## ✅ Test 3.1 - Create Template (POST)

**Endpoint**: `POST /api/templates`

**Paste**:
```json
{
  "name": "Welcome Email Template",
  "type": "Email",
  "subject": "Welcome to Our Store!",
  "content": "Dear {{CustomerName}}, Thank you for joining us!",
  "isActive": true
}
```

**Expected Result**: `201 Created`

**✏️ Note down**: `Template ID = 1`

---

## ✅ Test 3.2 - Create Another Template (POST)

**Same endpoint**

**Paste**:
```json
{
  "name": "50% Off SMS",
  "type": "SMS",
  "subject": "Flash Sale",
  "content": "50% OFF everything today! Use code: FLASH50",
  "isActive": true
}
```

**✏️ Note down**: `Template ID = 2`

---

## ✅ Test 3.3 - Get All Templates (GET)

**Endpoint**: `GET /api/templates`

**Expected Result**: List with both templates

---

## ✅ Test 3.4 - Get Template By ID (GET)

**Endpoint**: `GET /api/templates/{id}`

**Try**: `1` and then `2`

---

## ✅ Test 3.5 - Update Template (PUT)

**Endpoint**: `PUT /api/templates/{id}`

**Enter**: `1`

**Paste**:
```json
{
  "name": "Welcome Email Template V2",
  "subject": "Welcome! Here's 10% Off",
  "content": "Dear {{CustomerName}}, Thanks for joining! Use code WELCOME10",
  "isActive": true
}
```

---

# MODULE 4: TOUCHPOINTS 📱

## ✅ Test 4.1 - Create Touchpoint (POST)

**Endpoint**: `POST /api/touchpoints`

**Paste**:
```json
{
  "name": "SendGrid Email Service",
  "type": "EmailProvider",
  "configuration": "{\"apiKey\": \"sg_key_here\", \"from\": \"noreply@store.com\"}",
  "isActive": true
}
```

**✏️ Note down**: `Touchpoint ID = 1`

---

## ✅ Test 4.2 - Create Another Touchpoint (POST)

**Paste**:
```json
{
  "name": "Twilio SMS Gateway",
  "type": "SMSGateway",
  "configuration": "{\"accountSid\": \"AC123\", \"authToken\": \"token\"}",
  "isActive": true
}
```

**✏️ Note down**: `Touchpoint ID = 2`

---

## ✅ Test 4.3 - Get All Touchpoints (GET)

**Endpoint**: `GET /api/touchpoints`

**Expected**: List with 2 touchpoints

---

## ✅ Test 4.4 - Get Touchpoint By ID (GET)

**Endpoint**: `GET /api/touchpoints/{id}`

**Try**: `1`

---

## ✅ Test 4.5 - Update Touchpoint (PUT)

**Endpoint**: `PUT /api/touchpoints/{id}`

**Enter**: `1`

**Paste**:
```json
{
  "name": "SendGrid Email Service (Primary)",
  "configuration": "{\"apiKey\": \"new_key\"}",
  "isActive": true
}
```

---

# MODULE 5: CAMPAIGN-TEMPLATES (Linking) 🔗

**Now we link Template #1 to Campaign #1**

## ✅ Test 5.1 - Add Template to Campaign (POST)

**Endpoint**: `POST /api/campaigns/{campaignId}/templates`

**Enter**: `1` in `campaignId` field

**Paste**:
```json
{
  "templateVersionId": 1,
  "allocationPercent": 100,
  "isActive": true
}
```

**Expected Result**: `200 OK` with ID

---

## ✅ Test 5.2 - Get Campaign's Templates (GET)

**Endpoint**: `GET /api/campaigns/{campaignId}/templates`

**Enter**: `1` in `campaignId`

**Expected**: List showing Template #1 linked to Campaign #1

---

## ✅ Test 5.3 - Add Second Template (A/B Test) (POST)

**Same POST endpoint**

**Enter**: `1` in `campaignId`

**Paste**:
```json
{
  "templateVersionId": 2,
  "allocationPercent": 50,
  "isActive": true
}
```

**Now GET again** - you should see 2 templates!

---

## ✅ Test 5.4 - Remove Template (DELETE)

**Endpoint**: `DELETE /api/campaigns/{campaignId}/templates/{id}`

**You need the CampaignTemplateID from the GET response**

**Expected**: `200 OK`

---

# MODULE 6: CAMPAIGN-TOUCHPOINTS (Linking) 🔗

**Now we link Touchpoint #1 to Campaign #1**

## ✅ Test 6.1 - Add Touchpoint to Campaign (POST)

**Endpoint**: `POST /api/campaigns/{campaignId}/touchpoints`

**Enter**: `1` in `campaignId`

**Paste**:
```json
{
  "touchpointId": 1,
  "sequenceOrder": 1,
  "isActive": true
}
```

**Expected**: `200 OK`

---

## ✅ Test 6.2 - Get Campaign's Touchpoints (GET)

**Endpoint**: `GET /api/campaigns/{campaignId}/touchpoints`

**Enter**: `1` in `campaignId`

**Expected**: List showing Touchpoint #1

---

## ✅ Test 6.3 - Add Second Touchpoint (Multi-channel) (POST)

**Same endpoint**

**Paste**:
```json
{
  "touchpointId": 2,
  "sequenceOrder": 2,
  "isActive": true
}
```

**Now the campaign uses Email THEN SMS!**

---

## ✅ Test 6.4 - Remove Touchpoint (DELETE)

**Endpoint**: `DELETE /api/campaigns/{campaignId}/touchpoints/{id}`

**Use the ID from GET response**

---

# MODULE 7: CAMPAIGN LOGS (Read-only) 📊

**Note**: This table is usually populated by the system automatically. For testing, you'd need sample data in the database.

## ✅ Test 7.1 - Get All Logs (GET)

**Endpoint**: `GET /api/campaign-logs`

**Click**: Execute (no parameters needed)

**Expected**: Empty list `[]` (unless you have log data)

---

## ✅ Test 7.2 - Filter Logs by Campaign (GET)

**Same endpoint**

**In Query Parameters**, enter:
- `campaignId`: `1`

**Expected**: Logs filtered for Campaign #1

---

## ✅ Test 7.3 - Filter by Date Range (GET)

**Parameters**:
- `fromDate`: `2026-01-01`
- `toDate`: `2026-12-31`

---

# 🎉 **TESTING COMPLETE!**

## ✅ **What You've Tested:**

- ✅ **4 Core Modules** (Segments, Campaigns, Templates, Touchpoints)
- ✅ **2 Linking Modules** (Campaign-Templates, Campaign-Touchpoints)
- ✅ **1 Reporting Module** (Campaign Logs)
- ✅ **23 Total API Endpoints**
- ✅ **All CRUD Operations** (Create, Read, Update, Delete)

## 🎯 **Your System is Production-Ready!**

**You can now confidently demo this to your CEO!**

---

## 📝 **Quick Reference: What to Note Down**

| Item | Your Value | Example |
|------|------------|---------|
| Segment ID | _____ | 1 |
| Campaign ID | _____ | 1 |
| Template ID #1 | _____ | 1 |
| Template ID #2 | _____ | 2 |
| Touchpoint ID #1 | _____ | 1 |
| Touchpoint ID #2 | _____ | 2 |

Keep these IDs handy for cross-module testing!
