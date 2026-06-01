# TaskManagerPro.Blazor

[![Status: Work in Progress](https://img.shields.io/badge/Status-Work%20in%20Progress-yellow)](https://github.com)

A modern task management application built with **Blazor Server**, following **Clean Architecture** principles. TaskManagerPro.Blazor provides a robust, scalable solution for managing tasks with a focus on maintainability and testability.

## 🎯 Overview

TaskManagerPro.Blazor is a comprehensive task management system designed for teams and individuals to organize, track, and collaborate on tasks efficiently. Built with modern .NET technologies and architectural best practices.

## 🛠️ Tech Stack

| Component | Technology |
|-----------|-----------|
| **Frontend** | Blazor Server |
| **Backend** | .NET 10 |
| **Database** | SQL Server |
| **ORM** | Entity Framework Core |
| **Architecture** | Clean Architecture |
| **CQRS** | MediatR |
| **Validation** | FluentValidation |
| **API Documentation** | Scalar |
| **Logging** | Serilog |

## 📁 Solution Structure

The project follows **Clean Architecture** with the following 4 layers:

```
src/
├── TaskManagerPro.Blazor.Domain/              # Core business logic
│   └── Entities, Value Objects, Interfaces
├── TaskManagerPro.Blazor.Application/         # Use cases & business rules
│   └── Commands, Queries, Validators, DTOs
├── TaskManagerPro.Blazor.Infrastructure/      # External integrations
│   └── DbContext, Repositories, Identity
└── TaskManagerPro.Blazor.Web/                 # Blazor Server UI
    └── Components, Pages, Services

tests/
└── TaskManagerPro.Blazor.Tests/               # Unit & Integration Tests
```

### Layer Responsibilities

- **Domain**: Contains business entities and core business rules
- **Application**: Implements use cases using CQRS pattern via MediatR
- **Infrastructure**: Data access, database context, repositories, external services
- **Web**: Blazor Server components, pages, and user interface

## 📋 Prerequisites

- **.NET 10 SDK** or later ([Download](https://dotnet.microsoft.com/download))
- **Docker** (for SQL Server) ([Download](https://www.docker.com/products/docker-desktop))
- **Visual Studio 2022** or **VS Code** (optional)

## 🚀 Getting Started

### 1. SQL Server Setup

Start a SQL Server 2022 container:

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourPassword@123" \
  -p 1433:1433 \
  --name sqlserver2022 \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

### 2. Build the Solution

```bash
dotnet build
```

### 3. Apply Database Migrations

```bash
dotnet ef database update --project src/TaskManagerPro.Blazor.Infrastructure
```

### 4. Run the Application

```bash
dotnet run --project src/TaskManagerPro.Blazor.Web
```

The application will be available at `https://localhost:7000`

## 🧪 Running Tests

```bash
dotnet test
```

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## ✨ Status

🟡 **Work in Progress** - Core architecture established, implementing features and database integration.
