# Application

The **Application** layer orchestrates the execution of business use cases.

It coordinates interactions between the Domain and Infrastructure layers while remaining independent of implementation details.

This layer contains application-specific logic, use case orchestration, validation, authorization, messaging, and abstractions for external services.

It does **not** contain infrastructure implementations or business rules.

---

# Responsibilities

The Application layer is responsible for:

- Executing application use cases.
- Coordinating domain objects.
- Command and Query handling.
- Validation.
- Authorization.
- Transaction management.
- Domain Event handling.
- Integration Event publishing.
- Defining infrastructure abstractions.
- Cross-cutting behaviors.

Business rules remain inside the Domain layer.

Infrastructure implementations remain inside the Infrastructure layer.

---

# Folder Structure

```text
Application
│
├── Abstractions
├── Common
├── Features
│
├── DependencyInjection.cs
```

---

# Architecture

The Application layer follows the **CQRS (Command Query Responsibility Segregation)** pattern.

Every feature is organized into independent vertical slices.

Example:

```text
CreateRestaurant
│
├── CreateRestaurantCommand
├── CreateRestaurantCommandHandler
├── CreateRestaurantValidator
└── CreateRestaurantResponse
```

Each feature contains everything required for that specific use case.

This approach keeps related code together and minimizes coupling between different parts of the application.

---

# Features

Application use cases are grouped by business capabilities rather than technical concerns.

Current modules include:

- Authentication
- Restaurants
- Users
- Roles
- Permissions
- Administration

Each module contains its own:

- Commands
- Queries
- Validators
- Responses
- Handlers

This organization follows the Vertical Slice Architecture approach.

---

# Commands

Commands represent operations that change system state.

Examples include:

- Register User
- Login
- Register Restaurant
- Upload Restaurant Document
- Approve Restaurant
- Assign Role
- Ban User

Every command is handled by exactly one command handler.

---

# Queries

Queries retrieve information without modifying application state.

Examples include:

- Search Restaurants
- Get Restaurant Details
- Get Users
- Get Roles
- Get Sessions
- Get User Profile

Queries are optimized for read operations and may use dedicated query services for better performance.

---

# Handlers

Handlers coordinate the execution of application use cases.

Their responsibilities include:

- Loading aggregates.
- Calling domain behaviors.
- Persisting changes.
- Publishing integration events.
- Returning application responses.

Handlers should remain focused on orchestration rather than business rules.

---

# Validation

Validation is implemented using **FluentValidation**.

Each command or query owns its dedicated validator.

Examples include:

- LoginValidator
- RegisterValidator
- UploadDocumentValidator
- SuspendRestaurantValidator

This keeps validation logic close to the corresponding use case.

---

# Pipeline Behaviors

Cross-cutting concerns are implemented through MediatR Pipeline Behaviors.

Current behaviors include:

### ValidationBehavior

Validates requests before handlers execute.

---

### TransactionBehavior

Ensures write operations execute inside a database transaction.

---

### LoggingBehavior

Logs application requests for diagnostics and observability.

---

# Domain Event Handlers

The Application layer reacts to business events raised by aggregates.

Responsibilities include:

- Executing follow-up workflows.
- Coordinating additional application logic.
- Publishing Integration Events.

Domain Events remain internal to the application.

---

# Integration Events

Integration Events allow communication with external systems through the Event Bus.

Examples include:

- Restaurant Approved
- Restaurant Document Approved
- Restaurant Document Rejected

These events are consumed asynchronously by Infrastructure messaging components.

---

# Abstractions

The Application layer defines contracts for all external dependencies.

Examples include:

## Authentication

- Token Provider
- Password Hasher
- Refresh Token Service
- User Context

---

## Messaging

- Event Bus
- Commands
- Queries

---

## Notifications

- Email Service
- Email Template Renderer

---

## Storage

- File Storage
- Document Storage

---

## Background Jobs

- Background Job Service

---

## Persistence

- Application DbContext
- SQL Connection Factory
- Query Services

Infrastructure provides the implementations for these abstractions.

---

# Settings

Configuration models are defined inside the Application layer.

Current settings include:

- JWT
- RabbitMQ
- Redis
- SendGrid
- Frontend

This keeps configuration strongly typed throughout the application.

---

# Common Components

The Application layer also contains reusable components including:

- Authorization constants
- Pagination models
- Helper utilities
- Shared validators

These components are shared across multiple features.

---

# Dependency Rule

```
API
    │
    ▼
Application
    │
    ▼
Domain

Infrastructure
      ▲
implements Application abstractions
```

The Application layer depends only on the Domain layer.

It defines abstractions that are implemented by the Infrastructure layer.

---

# Technologies

| Technology | Purpose |
|------------|---------|
| MediatR | CQRS & Request Dispatching |
| FluentValidation | Request Validation |
| Entity Framework Core | Transaction Abstractions |
| .NET 10 | Runtime |

---

# Design Principles

The Application layer follows several architectural principles:

- Clean Architecture
- CQRS
- Vertical Slice Architecture
- Dependency Inversion Principle
- Separation of Concerns
- Single Responsibility Principle
- MediatR Pipeline Behaviors

---

# Summary

The Application layer acts as the application's orchestrator.

It coordinates business workflows, validates requests, manages transactions, reacts to domain events, and communicates with external systems exclusively through abstractions, while keeping business rules inside the Domain layer and implementation details inside the Infrastructure layer.