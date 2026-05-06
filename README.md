# FoodRush Delivery Platform

## Overview

FoodRush is a scalable food delivery platform built using ASP.NET Core and Clean Architecture principles.
The project is designed to support enterprise-level features such as authentication, restaurant onboarding, ordering, payments, delivery tracking, notifications, and multi-language support.

This repository currently contains the Sprint 0 foundation setup, including:

* Clean Architecture solution structure
* Dockerized development environment
* SQL Server and Redis integration
* Entity Framework Core setup and migrations
* Structured logging with Serilog
* Health checks
* Global exception handling
* Strongly typed configuration system
* Middleware pipeline foundation

---

# Architecture

The solution follows Clean Architecture with clear separation of concerns.

## Solution Structure

```text
FoodRush/
 ├── src/
 │    ├── FoodRush.Domain
 │    ├── FoodRush.Application
 │    ├── FoodRush.Infrastructure
 │    ├── FoodRush.API
 │    ├── FoodRush.Hubs
 │
 ├── tests/
 │    ├── FoodRush.Tests.Unit
 │    ├── FoodRush.Tests.Integration
```

## Layer Responsibilities

### FoodRush.Domain

Contains:

- Core entities
- Base models
- Domain contracts and interfaces

### FoodRush.Application

Contains:

* Use cases
* DTOs
* Result pattern
* Validation
* Application contracts

### FoodRush.Infrastructure

Contains:

* EF Core
* Database access
* External services
* Repository implementations
* Infrastructure integrations

### FoodRush.API

Contains:

* Controllers
* Middleware
* Swagger
* Health checks
* Dependency injection setup

### FoodRush.Hubs

Contains:

* SignalR hubs
* Real-time communication features

---

# Tech Stack

## Backend

* ASP.NET Core
* Entity Framework Core
* SQL Server
* Redis
* Serilog
* Docker

## Architecture & Patterns

* Clean Architecture
* Result Pattern
* Middleware Pipeline
* Options Pattern
* Global Exception Handling

---

# Features Implemented in Sprint 0

## Foundation Setup

* Clean Architecture solution scaffold
* Docker Compose environment
* SQL Server container
* Redis container
* API container

## Database

* EF Core setup
* AppDbContext
* Initial migration
* Automatic migrations in development

## Logging

* Structured logging with Serilog
* Correlation ID support
* Request logging

## Error Handling

* Global exception handler
* Custom result pattern
* Validation errors
* ProblemDetails responses

## Health Monitoring

* SQL Server health checks
* Redis health checks
* `/health` endpoint

## Configuration

* Strongly typed settings
* Configuration validation
* Environment-based configuration

---

# Prerequisites

Before running the project, ensure the following are installed:

* .NET SDK 8+
* Docker Desktop
* Git

---

# Running the Project

## Clone Repository

```bash
git clone <repository-url>
cd FoodRush
```

---

# Run Using Docker

## Build and Run Containers

```bash
docker compose up --build
```

Containers:

* SQL Server
* Redis
* FoodRush API

---

# API Endpoints

## Swagger

```text
http://localhost:5000/swagger
```

## Health Check

```text
http://localhost:5000/health
```

---

# Entity Framework Commands

## Add Migration

```bash
dotnet ef migrations add MigrationName -p src/FoodRush.Infrastructure -s src/FoodRush.API
```

## Update Database

```bash
dotnet ef database update -p src/FoodRush.Infrastructure -s src/FoodRush.API
```

---

# Configuration

## Environment Files

```text
appsettings.json
appsettings.Development.json
appsettings.Production.json
```

## User Secrets

Sensitive information should not be committed to source control.

Use .NET User Secrets during development:

```bash
dotnet user-secrets init
```

Example:

```bash
dotnet user-secrets set "JwtSettings:SecretKey" "your-secret-key"
```

---

# Logging

The application uses Serilog for structured logging.

Configured features:

* Console logging
* Correlation IDs
* Request logging
* Environment-based log levels

---

# Health Checks

The project includes infrastructure health monitoring for:

* SQL Server
* Redis

Health endpoint:

```text
/health
```

---

# Docker Services

## SQL Server

* Port: 1433

## Redis

* Port: 6379

## API

* Port: 5000

---

# Git Workflow

Recommended branch strategy:

* `main` → production-ready code
* `develop` → active development
* `feature/*` → feature branches

Example:

```text
feature/authentication
feature/order-system
feature/payment-integration
```

---

# Current Project Status

## Completed

* Sprint 0 Foundation Setup

## Upcoming

* Authentication & Authorization
* JWT & Refresh Tokens
* Role-Based Access Control
* Restaurant Management
* Ordering System
* Payments
* Delivery Tracking

---

# Notes

* The project currently targets development infrastructure setup.
* Production deployment configuration will be added in later sprints.
* CI/CD pipelines and automated tests are planned for upcoming tasks.
