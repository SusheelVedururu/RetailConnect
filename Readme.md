# RetailConnect API

## Overview

RetailConnect API is a backend system built using **ASP.NET Core Web API** that follows a **strict layered architecture** as defined by the project guidelines.

The API acts as the **only bridge** between the application and the database and is designed to be clean, scalable, and easy to maintain.

---

## Architecture Summary

The project follows a **strict top-to-bottom layered architecture**:

1. API Controllers
2. Services (Business Logic)
3. Data Access (ADO.NET)
4. Stored Procedures
5. SQL Server Tables

Each layer communicates **only with the layer directly below it**.

---

## Technology Stack

* ASP.NET Core Web API (.NET 6+)
* SQL Server
* ADO.NET
* Stored Procedures
* Dependency Injection (built-in)

---

## Project Structure

```
RetailConnect.API
│
├── Controllers        # HTTP endpoints (thin controllers)
├── Services
│   ├── Interfaces     # Service contracts
│   └── Implementations# Business logic
├── Models             # DTOs only
└── Data               # ADO.NET + Stored Procedure calls
```

---

## Key Rules

* No Entity Framework
* No inline SQL
* No repository or unit-of-work patterns
* No unnecessary folders or abstractions
* Business logic must exist only in Services

---

## Development Guidelines

* Build **one module at a time**
* Follow the order:

  1. DTOs
  2. Data Access
  3. Service Interface
  4. Service Implementation
  5. Controller
* Do not proceed to the next module without approval

---

## Database Access

* Database schema already exists
* All access is done via **Stored Procedures only**
* API never accesses tables directly

---

## Status

🚧 Project under active development

---

## Note

This project strictly follows internal architectural standards. Any deviation from the defined rules should be reviewed and approved before implementation.
