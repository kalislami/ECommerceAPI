# 🛒 ECommerce API

ECommerce API is a RESTful backend for a simple e-commerce application built using .NET 8 and SQL Server. This API provides features such as product and category management, user authentication with JWT, a shopping cart and checkout system, product image uploads to Firebase Storage, and payment integration using Midtrans.

---

## 🚀 Technologies Used

* [.NET 8](https://dotnet.microsoft.com/)
* Entity Framework Core (EF Core)
* SQL Server (via Docker)
* Firebase Admin SDK (for uploading images to Firebase Storage)
* JWT Authentication & Authorization
* Midtrans Payment Gateway (Snap API)
* Docker & Docker Compose
* Swagger / OpenAPI
* BCrypt (for password hashing)

---

## 📦 Running the Project

### ✅ Prerequisites

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* [Docker](https://www.docker.com/)
* A Firebase account with access to Firebase Storage (madatory file: firebase-adminsdk.json)
* A Midtrans Sandbox account ([https://dashboard.midtrans.com/](https://dashboard.midtrans.com/))

### 🚀 Run with Docker

```bash
docker compose up --build
```
