<div align="center">

# 🍔 FoodRush

### Enterprise Food Delivery Backend built with .NET 10

A production-oriented backend built using **Clean Architecture**, **Domain-Driven Design (DDD)**, **CQRS**, **Vertical Slice Architecture**, and **Event-Driven Architecture**.

Designed to demonstrate modern backend engineering practices for scalable, maintainable, and reliable distributed systems.

![.NET](https://img.shields.io/badge/.NET-10-512BD4?style=for-the-badge)
![Clean Architecture](https://img.shields.io/badge/Clean-Architecture-blue?style=for-the-badge)
![DDD](https://img.shields.io/badge/DDD-Domain--Driven--Design-success?style=for-the-badge)
![CQRS](https://img.shields.io/badge/CQRS-MediatR-orange?style=for-the-badge)
![MassTransit](https://img.shields.io/badge/MassTransit-RabbitMQ-FF6600?style=for-the-badge)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=for-the-badge)

</div>

---

# 📑 Table of Contents

- [Overview](#-overview)
- [Why FoodRush?](#-why-foodrush)
- [Highlights](#-highlights)
- [Architecture](#-architecture)
- [Technology Stack](#-technology-stack)
- [Solution Structure](#-solution-structure)
- [Getting Started](#-getting-started)
- [Documentation](#-documentation)
- [Project Status](#-project-status)
- [Roadmap](#-roadmap)
- [Design Principles](#-design-principles)
- [License](#-license)

---

# 📖 Overview

FoodRush is an enterprise-inspired backend for a food delivery platform built with modern .NET technologies and software architecture principles.

Rather than focusing on simple CRUD operations, the project explores real-world backend challenges including authentication, authorization, asynchronous messaging, transactional consistency, background processing, resilience, and cloud integrations.

The primary goal is to build a scalable and maintainable backend while applying production-ready engineering practices.

---

# 🎯 Why FoodRush?

FoodRush was created as a learning project that goes beyond basic application development.

It focuses on understanding how modern enterprise applications are designed by combining architectural patterns, messaging, and clean separation of concerns.

The project emphasizes writing code that is easy to maintain, extend, and evolve over time.

---

# ✨ Highlights

- ✅ Clean Architecture
- ✅ Domain-Driven Design (DDD)
- ✅ CQRS
- ✅ Vertical Slice Architecture
- ✅ Domain Events
- ✅ Integration Events
- ✅ Transactional Outbox
- ✅ Transactional Inbox
- ✅ RabbitMQ
- ✅ MassTransit
- ✅ Entity Framework Core
- ✅ Dapper
- ✅ ASP.NET Core Identity
- ✅ JWT Authentication
- ✅ Refresh Tokens
- ✅ Permission-Based Authorization
- ✅ Hangfire
- ✅ Polly
- ✅ SendGrid
- ✅ Scriban
- ✅ Cloudflare R2
- ✅ Cloudinary
- ✅ Serilog
- ✅ Seq
- ✅ Docker

---

# 🏛 Architecture

The solution follows **Clean Architecture**, keeping business rules completely isolated from infrastructure concerns.

```text
                Client
                   │
                   ▼
             FoodRush.API
                   │
                   ▼
        FoodRush.Application
                   │
                   ▼
           FoodRush.Domain

FoodRush.Infrastructure
implements Application abstractions
```

For a detailed explanation of the architecture, see:

**📘 [Architecture Documentation](docs/architecture.md)**

---

# 🛠 Technology Stack

## Backend

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- Dapper

## Database

- SQL Server
- Redis

## Messaging

- RabbitMQ
- MassTransit

## Background Processing

- Hangfire

## Storage

- Cloudflare R2
- Cloudinary

## Notifications

- SendGrid
- Scriban

## Resilience

- Polly

## Logging

- Serilog
- Seq

## Containerization

- Docker
- Docker Compose

---

# 📂 Solution Structure

```text
FoodRush
│
├── src
│   ├── FoodRush.API
│   ├── FoodRush.Application
│   ├── FoodRush.Domain
│   ├── FoodRush.Infrastructure
│   └── FoodRush.Hubs (Planned)
│
├── tests
│   ├── FoodRush.Tests.Unit (Planned)
│   └── FoodRush.Tests.Integration (Planned)
│
└── docs
    ├── architecture.md
    ├── api.md
    ├── application.md
    ├── domain.md
    └── infrastructure.md
```

---

# 🚀 Getting Started

## Prerequisites

Install the following before running the project:

- .NET 10 SDK
- Docker Desktop
- Git

---

## Clone the Repository

```bash
git clone https://github.com/saeed-owais/FoodRush.git

cd FoodRush
```

---

## Run the Application

Start all required services using Docker Compose:

```bash
docker compose up --build
```

This will start:

- SQL Server
- Redis
- RabbitMQ
- Seq
- FoodRush API

---

## Available Services

| Service | URL |
|----------|-----|
| API | http://localhost:8080 |
| RabbitMQ Management | http://localhost:15672 |
| Seq | http://localhost:5341 |
| Health Check | http://localhost:8080/health |

Default RabbitMQ credentials:

```text
Username: guest
Password: guest
```

---

## Entity Framework Commands

### Create Migration

```bash
dotnet ef migrations add MigrationName \
-p src/FoodRush.Infrastructure \
-s src/FoodRush.API
```

### Update Database

```bash
dotnet ef database update \
-p src/FoodRush.Infrastructure \
-s src/FoodRush.API
```

---

## Configuration

The application uses:

- appsettings.json
- appsettings.Development.json
- .NET User Secrets

Example:

```bash
dotnet user-secrets set "JwtSettings:SecretKey" "your-secret-key"
```

---

# 📚 Documentation

Detailed documentation is available in the **docs** directory.

| Document | Description |
|----------|-------------|
| [Architecture](docs/architecture.md) | Overall architecture and design decisions |
| [Domain](docs/domain.md) | Business model and domain layer |
| [Application](docs/application.md) | Use cases, CQRS, and orchestration |
| [Infrastructure](docs/infrastructure.md) | External integrations and technical implementations |
| [API](docs/api.md) | HTTP layer and application entry point |

---

# 📈 Project Status

| Module | Status |
|----------|--------|
| Authentication | ✅ |
| Authorization | ✅ |
| User Management | ✅ |
| Role Management | ✅ |
| Permission Management | ✅ |
| Restaurant Management | ✅ |
| RabbitMQ Integration | ✅ |
| Transactional Outbox | ✅ |
| Transactional Inbox | ✅ |
| Email Notifications | ✅ |
| Background Jobs | ✅ |
| Cloud Storage | ✅ |
| Health Checks | ✅ |
| SignalR | 🚧 Planned |
| Unit Tests | 🚧 Planned |
| Integration Tests | 🚧 Planned |

---

# 🗺 Roadmap

- SignalR Integration
- Unit Testing
- Integration Testing
- OpenTelemetry
- Distributed Caching
- API Versioning
- Rate Limiting
- CI/CD Pipeline

---

# 🎯 Design Principles

FoodRush follows modern software engineering practices including:

- Clean Architecture
- Domain-Driven Design
- CQRS
- Vertical Slice Architecture
- SOLID Principles
- Dependency Injection
- Separation of Concerns
- Transactional Outbox
- Asynchronous Messaging

---

# 🤝 Contributing

Contributions, suggestions, and feedback are always welcome.

If you'd like to improve the project, feel free to open an issue or submit a pull request.

---

# 📄 License

This project is licensed under the **MIT License**.