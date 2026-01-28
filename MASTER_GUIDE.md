# 🚀 RETAILCONNECT - MASTER API GUIDE

## ✅ Status: ALL MODULES BUILT
We have successfully implemented the complete backend for:
1.  **Segments**
2.  **Campaigns**
3.  **Templates** (Email/SMS templates)
4.  **Touchpoints** (Configuration for Email/SMS)

---

## 🛠️ Step 1: Database Setup (Run in SSMS)

You need to run these 3 new scripts in SQL Server Management Studio to create the stored procedures.

**Open and Run in this Order:**

1.  `Database/CampaignStoredProcedures.sql`
2.  `Database/TemplateStoredProcedures.sql`
3.  `Database/TouchpointStoredProcedures.sql`

*(Note: You already ran the Segment one)*

---

## 🏃 Step 2: Run the API

1.  Open Terminal
2.  Run: `dotnet run`
3.  Wait for "Now listening on..."

---

## 🧪 Step 3: Test ALL Endpoints (Swagger)

Open **http://localhost:5005/swagger**

You will now see 4 sections!

### **1. Campaigns**
- Create a campaign:
  ```json
  {
    "name": "Winter Sale",
    "type": "Email",
    "startDate": "2026-12-01"
  }
  ```

### **2. Templates**
- Create a template:
  ```json
  {
    "name": "Welcome Email",
    "type": "Email",
    "subject": "Welcome to RetailConnect!",
    "content": "Hello {Name}, welcome..."
  }
  ```

### **3. Touchpoints**
- Create a touchpoint:
  ```json
  {
    "name": "SendGrid",
    "type": "EmailProvider",
    "configuration": "{ 'apiKey': '123' }"
  }
  ```

---

## ✅ You're Done!
Your entire API backend is built, compiled, and ready for the CEO demo.
