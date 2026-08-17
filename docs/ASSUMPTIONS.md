# Assumptions & Architecture Decisions — Milestone 1

## 1. Domain & Data Modeling

- **Soft Delete Behavior**:
  - `Warehouse.IsDeleted`: Warehouses marked as soft-deleted are hidden from standard warehouse listings and stock item queries.
  - `Product.IsDeleted`: Products marked as soft-deleted are hidden from product listings and block new stock adjustments (`POST /api/stock-items/adjust`). However, historical `StockAdjustment` records preserve the product name and metadata with a `productIsDeleted: true` flag in the API response.
  - `Category.IsDeleted`: Categories marked as soft-deleted hide the category from active listings without deleting linked products.

- **Stock Adjustment Logic**:
  - Unified stock adjustment endpoint (`POST /api/stock-items/adjust`) supports:
    - Relative stock changes via `delta` (e.g. `delta: -5` or `delta: +10`).
    - Target stock level setting via `newQuantity` (e.g. `newQuantity: 50`).
  - All adjustments update stock items and log a `StockAdjustment` audit record within a single Entity Framework Core database transaction to guarantee ACID consistency.
  - Adjustments that result in negative stock (`Quantity < 0`) are rejected with `400 Bad Request`.

## 2. Authentication & Authorization (RBAC)

- **Roles**:
  - `Admin`: Full access to manage Users, Categories, Products, Warehouses, Stock Items, Adjustments, and Summary Reports.
  - `Manager`: Read-only access to all stock, products, warehouses, history, and aggregate `StockSummary` reports. Cannot perform stock adjustments.
  - `Operator`: Can view stock and audit history in their **assigned warehouses** only. Can perform stock adjustments only in their assigned warehouses.

- **Security**:
  - Passwords are encrypted using **BCrypt** hashing.
  - API authentication relies on **JWT Bearer Tokens** (1 hour default expiration).

## 3. Pre-configured Seed Accounts

| Role | Username | Email | Password | Assigned Warehouses |
|------|----------|-------|----------|---------------------|
| Admin | `admin` | `admin@test.com` | `Admin@123` | All |
| Manager | `manager` | `manager@test.com` | `Manager@123` | All |
| Operator 1 | `operator1` | `operator1@test.com` | `Operator@123` | Main Warehouse, East Warehouse |
| Operator 2 | `operator2` | `operator2@test.com` | `Operator@123` | West Warehouse |
