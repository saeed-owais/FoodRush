# Architecture

## Overview

FoodRush is designed using modern software architecture principles to achieve scalability, maintainability, testability, and clear separation of concerns.

The project combines several architectural patterns, including:

- Clean Architecture
- Domain-Driven Design (DDD)
- CQRS
- Vertical Slice Architecture
- Domain Events
- Integration Events
- Transactional Outbox Pattern
- Asynchronous Messaging

These patterns work together to keep the business logic independent from infrastructure concerns while allowing the system to evolve over time.

---

# High-Level Architecture

```text
                        Client
                           │
                           ▼
                    ASP.NET Core API
                           │
                           ▼
                  Application Layer
                           │
                           ▼
                     Domain Layer
                           ▲
                           │
                 Infrastructure Layer
```

### Responsibilities

| Layer | Responsibility |
|--------|---------------|
| API | HTTP endpoints, middleware, authentication, request handling |
| Application | Use cases, orchestration, validation, transactions |
| Domain | Business rules, aggregates, value objects, domain events |
| Infrastructure | Database, messaging, storage, email, authentication implementation |

---

# Dependency Flow

The solution follows the Dependency Inversion Principle.

```text
                API
                 │
                 ▼
          Application
                 │
                 ▼
             Domain

Infrastructure
      ▲
      │
implements
Application abstractions
```

Only the Infrastructure layer depends on external technologies.

The Domain layer has no knowledge of databases, messaging systems, or frameworks.

---

# Clean Architecture

The project separates responsibilities into independent layers.

Business rules remain isolated inside the Domain layer.

The Application layer coordinates business workflows.

The Infrastructure layer provides implementations for external services.

The API layer exposes the system through HTTP.

---

# Domain-Driven Design

Business logic is modeled using Domain-Driven Design concepts.

The project currently includes:

- Aggregates
- Entities
- Value Objects
- Domain Events
- Repository Contracts
- Domain Errors
- Result Pattern

Business behavior lives inside aggregates rather than services.

---

# CQRS

The application separates write operations from read operations.

```text
Command
    │
    ▼
Command Handler
    │
    ▼
Aggregate
    │
    ▼
Database
```

```text
Query
    │
    ▼
Query Handler
    │
    ▼
Read Model
```

Benefits include:

- Better separation of responsibilities
- Simpler handlers
- Independent optimization of reads and writes
- Easier maintenance

---

# Vertical Slice Architecture

Instead of organizing code by technical layers, features are grouped by business capabilities.

Example:

```text
RegisterRestaurant
│
├── RegisterRestaurantCommand
├── RegisterRestaurantCommandHandler
├── RegisterRestaurantValidator
└── RegisterRestaurantResponse
```

Everything required for a single use case is located in one place.

---

# Domain Events

Aggregates raise Domain Events whenever important business actions occur.

Example:

```text
Restaurant.Approve()
        │
        ▼
RestaurantApprovedDomainEvent
```

Domain Events are used only inside the application's boundary.

Examples include:

- Restaurant Registered
- Restaurant Approved
- Restaurant Suspended
- Document Uploaded
- Document Approved

---

# Integration Events

Some business events must notify external systems.

Application handlers transform Domain Events into Integration Events.

```text
Domain Event
      │
      ▼
Application Handler
      │
      ▼
Integration Event
      │
      ▼
Event Bus
```

This keeps the Domain isolated from messaging concerns.

---

# Messaging Architecture

FoodRush uses RabbitMQ together with MassTransit.

```text
Application
      │
      ▼
IEventBus
      │
      ▼
MassTransit
      │
      ▼
RabbitMQ
      │
      ▼
Consumers
```

Messaging is asynchronous and loosely coupled.

---

# Transactional Outbox

To guarantee reliable event publishing, the project implements the Transactional Outbox Pattern.

```text
Application
      │
      ▼
Save Aggregate
      │
      ▼
Store Integration Event
      │
      ▼
Commit Transaction
      │
      ▼
Outbox Processor
      │
      ▼
RabbitMQ
```

Benefits:

- No lost messages
- Database consistency
- Reliable event publishing

---

# Inbox Pattern

Incoming messages are processed through an Inbox to prevent duplicate execution.

Benefits include:

- Idempotent message processing
- Safe retries
- Better fault tolerance

---

# Background Processing

Long-running operations are executed outside HTTP requests.

Current implementation:

- Hangfire scheduled jobs
- MassTransit consumers

This keeps API requests fast and responsive.

---

# Persistence Strategy

The project uses both Entity Framework Core and Dapper.

### Entity Framework Core

Used for:

- Commands
- Aggregate persistence
- Transactions
- Change tracking

### Dapper

Used for:

- Read models
- Optimized queries
- CQRS query side

This combination provides both productivity and performance.

---

# Authentication Flow

```text
User Login
     │
     ▼
Validate Credentials
     │
     ▼
Generate JWT
     │
     ▼
Generate Refresh Token
     │
     ▼
Return Tokens
```

The authentication system also supports:

- Email Verification
- Password Reset
- Refresh Token Rotation
- Session Management

---

# Notification Flow

Email notifications are processed asynchronously.

```text
Business Action
      │
      ▼
Domain Event
      │
      ▼
Integration Event
      │
      ▼
RabbitMQ
      │
      ▼
Consumer
      │
      ▼
SendGrid
```

This avoids blocking HTTP requests.

---

# Cross-Cutting Concerns

The project centralizes common concerns including:

- Validation
- Transactions
- Logging
- Authorization
- Exception Handling
- Resilience
- Background Jobs

Each concern is implemented independently without polluting business logic.

---

# Design Principles

FoodRush follows several software engineering principles:

- Clean Architecture
- Domain-Driven Design
- SOLID
- CQRS
- Vertical Slice Architecture
- Dependency Inversion
- Separation of Concerns
- Tell, Don't Ask
- Persistence Ignorance
- Asynchronous Messaging

---

# Future Improvements

The following architectural enhancements are planned:

- SignalR Hubs
- Unit Testing
- Integration Testing
- OpenTelemetry
- Distributed Tracing
- CI/CD Pipeline
- API Versioning
- Rate Limiting
- Distributed Caching

---

# Conclusion

The architecture is designed to keep business logic independent from infrastructure while enabling reliable messaging, scalable application growth, and maintainable code.

As new modules are introduced, they can be added with minimal impact on the existing system thanks to the project's layered and feature-oriented design.