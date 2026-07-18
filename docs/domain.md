# Domain

The **Domain** layer represents the heart of the application.

It contains the core business model, business rules, and domain behavior without any dependency on external frameworks, databases, messaging systems, or infrastructure concerns.

Everything in this layer is focused on expressing the business in code.

---

# Responsibilities

The Domain layer is responsible for:

- Business entities and aggregates.
- Business rules and invariants.
- Value Objects.
- Domain Events.
- Domain Errors.
- Result Pattern.
- Repository Contracts.
- Domain Interfaces.
- Strongly Typed IDs.

The Domain layer **never communicates** with databases, APIs, message brokers, email providers, or any external system.

---

# Folder Structure

```text
Domain
│
├── Common
│   ├── Errors
│   ├── Result
│   ├── AggregateRoot
│   ├── DomainEvent
│   └── BaseEntity
│
├── Entities
│   └── Identity
│
├── Restaurants
│   ├── DomainEvents
│   ├── Entities
│   ├── Enums
│   ├── ValueObjects
│   ├── IRestaurantRepository
│   └── Restaurant
│
├── Interfaces
└── Constants
```

---

# Design Goals

The Domain layer was designed to achieve:

- High cohesion
- Low coupling
- Rich domain model
- Encapsulation of business rules
- Framework independence
- Persistence ignorance
- Testability

---

# Aggregate Roots

Business consistency is enforced through Aggregate Roots.

Aggregates protect their internal state by exposing behaviors instead of allowing arbitrary modifications.

Current Aggregate Roots include:

- Restaurant
- User

Each aggregate is responsible for maintaining its own invariants and raising Domain Events whenever important business actions occur.

---

# Entities

Entities represent business concepts that have identity throughout their lifecycle.

Examples include:

- User
- Role
- Permission
- Refresh Token
- OTP Request
- Restaurant Document

Entity identity is preserved regardless of state changes.

---

# Value Objects

Value Objects model immutable concepts without identity.

Current examples include:

- Name
- Latitude
- Longitude
- Delivery Radius
- Average Rating
- File URL
- Public ID
- Document Type
- Document Status
- Rejection Reason

Value Objects validate themselves during construction, ensuring invalid states cannot exist inside the domain.

---

# Strongly Typed IDs

Instead of primitive identifiers such as `Guid` or `int`, the domain uses Strongly Typed IDs.

Examples:

- UserId
- RestaurantId
- DocumentId

Benefits include:

- Better type safety
- Improved readability
- Preventing accidental identifier mix-ups
- Stronger compile-time validation

---

# Domain Events

Domain Events capture important business occurrences inside aggregates.

Examples include:

- Restaurant Registered
- Restaurant Submitted For Review
- Restaurant Approved
- Restaurant Rejected
- Restaurant Suspended
- Restaurant Reactivated
- Restaurant Document Uploaded
- Restaurant Document Approved
- Restaurant Document Rejected
- Restaurant Document Replaced

Domain Events allow the application to react to business changes without tightly coupling different parts of the system.

---

# Result Pattern

The domain uses the Result Pattern instead of exceptions for expected business failures.

Benefits include:

- Explicit success/failure handling
- Predictable control flow
- Better API responses
- Easier testing

Unexpected failures are still represented by exceptions.

---

# Domain Errors

Business failures are represented using strongly typed error objects.

Current error groups include:

- Authentication
- Users
- Roles
- Permissions
- Restaurants
- Validation

Keeping domain errors centralized ensures consistency across the application.

---

# Repository Contracts

The Domain defines repository contracts while leaving implementations to the Infrastructure layer.

Example:

```csharp
IRestaurantRepository
```

This follows the Dependency Inversion Principle.

---

# Domain Interfaces

The domain defines several reusable contracts for cross-cutting concerns.

Examples include:

- IAuditable
- ISoftDeletable
- IConcurrencyAware
- IHasDomainEvents

Implementations are handled outside the domain.

---

# Business Rules

Business rules live exclusively inside the domain.

Examples include:

- Restaurant lifecycle transitions
- Document approval workflow
- Status validation
- Aggregate invariants
- Value Object validation

This guarantees that business behavior remains consistent regardless of how the application is accessed.

---

# Dependency Rule

```
Infrastructure
        ▲
Application
        ▲
      Domain
```

The Domain layer depends on **nothing** except the .NET Base Class Library and lightweight contracts.

No dependency exists on:

- Entity Framework Core
- ASP.NET Core
- SQL Server
- RabbitMQ
- MassTransit
- Hangfire
- SendGrid
- Cloud Storage
- Logging Frameworks

This makes the business model completely independent of infrastructure concerns.

---

# Technologies

| Technology | Purpose |
|------------|---------|
| .NET 10 | Runtime |
| MediatR.Contracts | Domain Event contracts |

---

# Design Principles

The Domain layer follows several architectural principles:

- Domain-Driven Design (DDD)
- Rich Domain Model
- Clean Architecture
- SOLID Principles
- Dependency Inversion Principle
- Encapsulation
- Immutability
- Persistence Ignorance
- Tell, Don't Ask

---

# Summary

The Domain layer contains the application's business knowledge and represents the single source of truth for business behavior.

It defines **what the system does**, while every other layer is responsible only for **how those behaviors are executed**.