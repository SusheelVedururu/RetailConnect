# 🚀 Phase 2: Campaign Module - Setup Guide

## ✅ What's Been Built
We have successfully implemented the **Campaign Module** with the following files:

1.  **Models** (`Models/CampaignModels.cs`)
2.  **Data Access** (`Data/CampaignDataAccess.cs`)
3.  **Service** (`Services/Implementations/CampaignService.cs`)
4.  **Controller** (`Controllers/CampaignController.cs`)
5.  **SQL Script** (`Database/CampaignStoredProcedures.sql`)

---

## 🛠️ Step 1: Run SQL Scripts (CRITICAL)

Before running the API, you MUST update the database schema for the Campaigns module.

1.  **Open** `Database/CampaignStoredProcedures.sql`
2.  **Copy** all content.
3.  **Run** in SSMS (F5).

> **Note:** This script assumes your table columns are named `CampaignName`, `CampaignDescription`, etc. If you get an error like "Invalid column name", create a `FIXED` version like we did for Segments.

---

## 🏃 Step 2: Run the API

Since we stopped it to build, start it again:

```bash
dotnet run
```

---

## 🧪 Step 3: Test Campaign Endpoints

Open **Swagger** (`http://localhost:5005/swagger`) and look for the new **Campaign** section!

### **1. Create Campaign**
```json
{
  "name": "Summer Sale 2026",
  "description": "Big summer discount event",
  "type": "Email",
  "startDate": "2026-06-01",
  "endDate": "2026-06-30",
  "isActive": true
}
```

### **2. Get All Campaigns**
Verify your new campaign appears in the list.

---

## 🔜 Next Steps: Remaining Modules

To complete the "Build All" request, we still need:
1.  **Campaign Templates**
2.  **Touchpoints**
3.  **Logs**

I can proceed with building these next!
