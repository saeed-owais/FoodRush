# Infrastructure

The **Infrastructure** layer contains all external integrations and technical implementations required by the application. It is responsible for implementing the abstractions defined in the Application layer while keeping business rules isolated inside the Domain.

This layer acts as the bridge between the application and external systems such as databases, message brokers, storage providers, email services, authentication mechanisms, background processing, and resilience policies.

---

# Responsibilities

The Infrastructure layer is responsible for:

- Implementing Application interfaces.
- Database access using Entity Framework Core and Dapper.
- Authentication and authorization.
- RabbitMQ messaging using MassTransit.
- Transactional Outbox & Inbox.
- Email delivery using SendGrid.
- Background processing using Hangfire.
- File storage using Cloudflare R2 and Cloudinary.
- Resilience and retry policies using Polly.
- Database migrations and seeders.
- Domain Event publishing.

Business rules **do not belong here**.

---

# Folder Structure

```text
Infrastructure
│
├── Authentication
├── Authorization
├── BackgroundJobs
├── MassTransit
├── Notifications
├── Persistence
├── Resilience
├── Storage
│
└── DependencyInjection.cs
```

---

# Authentication

Provides the complete authentication infrastructure including:

- JWT Access Tokens
- Refresh Token management
- Refresh Token hashing
- Token generation
- Email verification tokens
- Password reset tokens
- Email change tokens
- ASP.NET Core Identity Data Protection tokens
- Current user context
- Security stamp validation

Authentication logic is implemented here while the Application layer depends only on abstractions.

---

# Authorization

Implements permission-based authorization.

Includes:

- Custom Authorization Policy Provider
- Authorization Handler
- Permission Requirements
- Permission Discovery

This allows the application to authorize users based on fine-grained permissions instead of roles only.

---

# Persistence

Contains the complete persistence implementation.

## Entity Framework Core

Used for:

- Aggregate persistence
- Change Tracking
- Transactions
- Migrations
- Configurations
- Interceptors

## Dapper

Used for read-only queries that require maximum performance.

Following CQRS principles:

- EF Core → Commands
- Dapper → Queries

---

## Entity Configurations

All entity mappings are separated using the Fluent API.

Examples include:

- Users
- Roles
- Permissions
- Restaurants
- Restaurant Documents
- Refresh Tokens

---

## Interceptors

The persistence layer contains several EF Core interceptors:

### AuditInterceptor

Automatically updates audit fields.

### SoftDeleteInterceptor

Converts delete operations into soft deletes.

### PublishDomainEventsInterceptor

Collects Domain Events after SaveChanges and publishes them for further processing.

---

## Seeders

Responsible for initializing application data such as:

- Roles
- Permissions
- Super Admin
- Identity data

---

# Messaging

Messaging is implemented using **MassTransit** with **RabbitMQ**.

Current features include:

- Event Bus abstraction
- RabbitMQ transport
- Consumers
- Automatic endpoint configuration
- Retry policies
- Transactional Outbox
- Transactional Inbox

Using the Outbox pattern guarantees that database changes and integration events remain consistent.

---

# Notifications

Handles email notifications across the application.

Current implementation includes:

- SendGrid Email Provider
- Scriban Email Templates
- Template Rendering
- Email Models
- Fake Email Service for development/testing

Emails are published as Integration Events and processed asynchronously through MassTransit consumers.

This keeps user requests fast while improving reliability.

---

# Background Jobs

Background processing is implemented using Hangfire.

Current scheduled jobs:

- Refresh Token Cleanup

Additional jobs can be added without affecting the application layer.

---

# Storage

Responsible for file storage implementations.

Supported providers:

- Cloudflare R2
- Cloudinary

The Application layer communicates only through abstractions without knowing which provider is being used.

---

# Resilience

Infrastructure uses Polly to improve reliability when communicating with external services.

Current resilience pipelines include:

- Cloudflare R2 uploads
- SendGrid email delivery

Policies include:

- Retry
- Exponential Backoff
- Timeout

This helps the application gracefully recover from transient failures.

---

# Dependency Injection

Each infrastructure module exposes its own registration methods.

The root `DependencyInjection.cs` composes all modules and exposes a single registration point to the API layer.

---

# Technologies

| Technology | Purpose |
|------------|---------|
| Entity Framework Core | ORM |
| Dapper | High-performance queries |
| SQL Server | Relational Database |
| ASP.NET Core Identity | Authentication |
| JWT | Access Tokens |
| MassTransit | Messaging Framework |
| RabbitMQ | Message Broker |
| Transactional Outbox | Reliable Event Publishing |
| Transactional Inbox | Idempotent Message Processing |
| Hangfire | Background Jobs |
| Polly | Resilience |
| SendGrid | Email Delivery |
| Scriban | Email Templates |
| Cloudflare R2 | Object Storage |
| Cloudinary | Media Storage |

---

# Design Principles

This layer follows several architectural principles:

- Clean Architecture
- Dependency Inversion Principle
- CQRS
- Domain Events
- Integration Events
- Transactional Outbox
- Inbox Pattern
- Asynchronous Messaging
- Separation of Concerns

---

# Dependency Rule

```
API
    ↓
Application
    ↓
Domain

Infrastructure
    ↑
implements Application abstractions
```

The Domain layer has no dependency on Infrastructure.

The Application layer depends only on abstractions.

Infrastructure contains all implementation details.

---

# Summary

The Infrastructure layer isolates every external dependency from the core business logic.

This design allows technologies to evolve independently while keeping the Domain and Application layers clean, testable, and independent from implementation details.