# Fulfillment & Inventory Management Platform — Milestone 1

A Web API solution built with **.NET 10**, **Entity Framework Core**, **SQL Server / LocalDB**, **JWT Authentication**, and **Swagger OpenAPI** for maintaining products, categories, warehouses, multi-warehouse stock inventory, and full adjustment audit histories.

---

## 🚀 Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server LocalDB, Express, or Docker SQL Server container.

### Setup & Run Instructions

1. **Restore dependencies & build**:
   ```bash
   dotnet restore
   dotnet build
   ```

2. **Run the API server**:
   ```bash
   dotnet run
   ```
   *The application automatically creates the database and populates seed data on startup.*

3. **Access Interactive API Documentation**:
   Navigate to `https://localhost:7000/swagger` or `http://localhost:5000/swagger` in your web browser.

---

## 🔐 Seed User Accounts

The database comes pre-populated with test accounts:

| Role | Username | Email | Password | Scope / Permissions |
|------|----------|-------|----------|--------------------|
| **Admin** | `admin` | `admin@test.com` | `Admin@123` | Full access (CRUD, Users, RBAC, Adjustments, Reports) |
| **Manager** | `manager` | `manager@test.com` | `Manager@123` | Read-only across all warehouses + Manager Stock Summary Reports |
| **Operator 1** | `operator1` | `operator1@test.com` | `Operator@123` | Assigned to *Main Warehouse* & *East Warehouse* |
| **Operator 2** | `operator2` | `operator2@test.com` | `Operator@123` | Assigned to *West Warehouse* |

To test endpoints in Swagger:
1. Call `POST /api/auth/login` with credentials above.
2. Copy the returned `token`.
3. Click the **Authorize** button in Swagger and enter `Bearer <YOUR_TOKEN>`.

---

## 🧪 Automated Tests

Run the unit test suite covering stock calculations, transaction safety, negative stock protection, soft-delete restrictions, and RBAC authorization rules:

```bash
dotnet test
```

---

## 📄 Key Features & Endpoints

- **Auth & User Management**: `/api/auth/login`, `/api/auth/register`, `/api/users/{userId}/role`, `/api/users/{userId}/warehouses`
- **Catalog Management**: `/api/categories`, `/api/products` (with pagination, sorting, search filter, and SKU validation)
- **Warehouse Management**: `/api/warehouses`
- **Inventory Stock & Adjustments**: `/api/stock-items`, `/api/stock-items/adjust` (unified quantity delta/absolute update with DB transaction safety)
- **Audit History**: `/api/stock-adjustments` (filterable by product, warehouse, date range, user)
- **Reports**: `/api/reports/stock-summary` (aggregated inventory metrics for Managers/Admins)
