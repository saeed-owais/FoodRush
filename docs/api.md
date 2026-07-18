# API

The **API** layer serves as the application's entry point.

Its primary responsibility is exposing the application through HTTP endpoints while remaining as thin as possible.

This layer contains **no business logic**. Instead, it delegates all application behavior to the Application layer, ensuring a clear separation between presentation and business concerns.

---

# Responsibilities

The API layer is responsible for:

- Exposing RESTful endpoints.
- Request validation and model binding.
- Authentication and authorization.
- Exception handling.
- HTTP response formatting.
- Middleware configuration.
- Dependency injection bootstrapping.
- Application startup configuration.
- Health checks.

Business rules are handled entirely by the Application and Domain layers.

---

# Folder Structure

```text
API
│
├── Attributes
├── Controllers
│   ├── Admin
│   ├── AuthController
│   └── RestaurantsController
│
├── Extensions
├── Middleware
├── ViewModels
│
├── Program.cs
└── appsettings.json
```

---

# Controllers

Controllers expose the application's capabilities through RESTful APIs.

Current areas include:

- Authentication
- Restaurant Management
- User Management
- Role Management
- Permission Management
- Administrative Operations

Controllers act only as orchestration endpoints and immediately delegate execution to the Application layer.

---

# Request Models

The API defines request models dedicated to HTTP communication.

Examples include:

- UploadDocumentRequest
- UpdateRoleRequest
- UpdatePermissionRequest
- SuspendRestaurantRequest
- RejectRestaurantDocumentRequest
- BanUserRequest

Separating request models from application commands keeps the API contract independent from internal implementation.

---

# Authorization

Authorization is implemented using custom permission-based authorization.

Features include:

- Custom Permission Attribute
- Fine-grained permission checks
- Policy-based authorization

Permissions are evaluated before requests reach the application layer.

---

# Global Exception Handling

A centralized exception handler converts application and infrastructure exceptions into consistent HTTP responses.

Benefits include:

- Consistent error responses
- Cleaner controllers
- Centralized exception handling
- Better client experience

---

# Response Mapping

The API layer converts application results into appropriate HTTP responses.

Examples include:

- 200 OK
- 201 Created
- 204 No Content
- 400 Bad Request
- 401 Unauthorized
- 403 Forbidden
- 404 Not Found
- 409 Conflict

This mapping is implemented through reusable extension methods.

---

# Middleware

The API pipeline includes custom middleware for cross-cutting concerns.

Current middleware includes:

- Request Context Logging

Middleware executes before requests reach the controllers.

---

# Configuration

Application startup is organized using extension methods instead of placing all configuration inside `Program.cs`.

Examples include:

- Service registration
- Middleware registration
- Options binding
- Database migration
- Result mapping

This keeps the startup process modular and maintainable.

---

# Health Checks

The API exposes health checks for external dependencies.

Current checks include:

- SQL Server
- Redis

Health checks simplify monitoring and deployment validation.

---

# Logging

Logging is implemented using **Serilog**.

Configured sinks include:

- Console
- Seq

Structured logging enables easier debugging, monitoring, and observability.

---

# API Design Principles

The API follows several design principles:

- RESTful Architecture
- Thin Controllers
- Separation of Concerns
- Consistent Error Responses
- Permission-Based Authorization
- Dependency Injection
- Centralized Configuration

---

# Dependency Rule

```
Client
    │
    ▼
API
    │
    ▼
Application
    │
    ▼
Domain

Infrastructure
      ▲
```

The API depends only on the Application and Infrastructure layers.

Business rules never exist inside controllers.

---

# Technologies

| Technology | Purpose |
|------------|---------|
| ASP.NET Core | Web API Framework |
| JWT Bearer Authentication | Authentication |
| Serilog | Structured Logging |
| Seq | Log Aggregation |
| Health Checks | Dependency Monitoring |
| Postman | API Testing |

---

# Summary

The API layer provides a clean and lightweight HTTP interface over the application's use cases.

Its responsibility is limited to handling HTTP concerns, leaving all business logic to the Application and Domain layers while coordinating infrastructure through dependency injection.