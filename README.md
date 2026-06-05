# TaskManagerPro.Blazor

![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![Blazor Server](https://img.shields.io/badge/Blazor-Server-7B2FBE?logo=blazor)
![License: MIT](https://img.shields.io/badge/License-MIT-green)

A professional task management system built with **Blazor Server** and **Clean Architecture**.

---

## Features

- **Task Management** — Create, update, and delete tasks with subtasks and milestones
- **Task Assignment** — Assign tasks to team members with real-time badge notifications
- **Notification System** — Unread badge counter that updates automatically on assignment and status changes
- **JWT Authentication** — Secure login and registration backed by ASP.NET Core Identity
- **User Profiles** — Update personal information and change password from a dedicated profile page
- **Priority & Status Tracking** — Four priority levels and full lifecycle transitions (Pending → In Progress → Completed / Cancelled)
- **Overdue Detection** — Dashboard highlights tasks that have passed their due date

---

## Tech Stack

| Layer | Technology |
|---|---|
| **Frontend** | Blazor Server, MudBlazor |
| **Backend** | .NET 10, ASP.NET Core, MediatR, FluentValidation |
| **Database** | SQL Server, EF Core 10 |
| **Auth** | ASP.NET Core Identity, JWT Bearer |
| **Testing** | xUnit, NSubstitute, FluentAssertions |
| **Infrastructure** | Docker |

---

## Architecture

The solution follows strict **Clean Architecture** with a unidirectional dependency rule:

```
┌─────────────────────────────────────────────┐
│               Web (Blazor Server)            │  ← UI, Components, Pages
│         Razor Pages · MudBlazor · Services   │
├─────────────────────────────────────────────┤
│                Infrastructure                │  ← EF Core, Identity, JWT
│     Repositories · DbContext · Services      │
├─────────────────────────────────────────────┤
│                 Application                  │  ← Use Cases (CQRS)
│    Commands · Queries · Handlers · DTOs      │
├─────────────────────────────────────────────┤
│                   Domain                     │  ← Business Rules (no deps)
│       Entities · Enums · Interfaces          │
└─────────────────────────────────────────────┘
         Dependencies point inward only
```

**Layer responsibilities:**

- **Domain** — Core entities (`TaskItem`, `AppUser`, `Milestone`, `Notification`) with private setters and explicit state-transition methods. Zero external dependencies.
- **Application** — CQRS use cases via MediatR. Commands mutate state; queries return DTOs. FluentValidation pipeline behavior runs before every handler.
- **Infrastructure** — EF Core `ApplicationDbContext`, generic `Repository<T>`, ASP.NET Core Identity, JWT generation, and service implementations.
- **Web** — Blazor Server components and pages. Auth state is stored in `localStorage` via `CustomAuthStateProvider` and read through JS interop.

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- Visual Studio 2022+ or VS Code (optional)

### 1. Start SQL Server

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Admin1234!" \
  -p 1433:1433 --name sqlserver-taskmanager \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

### 2. Clone and Build

```bash
git clone https://github.com/nmino1984/TaskManagerPro.Blazor.git
cd TaskManagerPro.Blazor
dotnet build
```

### 3. Apply Migrations

```bash
cd src/TaskManagerPro.Blazor.Infrastructure
dotnet ef database update --startup-project ../TaskManagerPro.Blazor.Web
```

### 4. Run the App

```bash
dotnet run --project src/TaskManagerPro.Blazor.Web
```

Open **http://localhost:5026** in your browser.

### Demo Credentials

| Field | Value |
|---|---|
| Email | `demo@taskmanager.com` |
| Password | `Demo1234!` |

> Register a new account if no demo data has been seeded.

---

## Running Tests

```bash
dotnet test
```

28 tests across Domain and Application layers (xUnit + NSubstitute + FluentAssertions).

---

## Project Structure

```
TaskManagerPro.Blazor/
├── src/
│   ├── TaskManagerPro.Blazor.Domain/
│   │   ├── Common/           # BaseEntity
│   │   ├── Entities/         # TaskItem, AppUser, Milestone, Notification, SubTask
│   │   ├── Enums/            # WorkTaskStatus, TaskPriority, MilestoneStatus
│   │   └── Interfaces/       # IRepository<T>, IUnitOfWork
│   │
│   ├── TaskManagerPro.Blazor.Application/
│   │   ├── Common/           # Behaviors, Exceptions, Interfaces
│   │   └── Features/
│   │       ├── Auth/         # Login, Register, UpdateProfile, ChangePassword
│   │       ├── Tasks/        # CRUD + status commands, GetAllTasks query
│   │       ├── Milestones/   # Create, Update, Delete, GetByTask
│   │       ├── Notifications/# GetNotifications, MarkAsRead
│   │       └── Users/        # GetAllUsers
│   │
│   ├── TaskManagerPro.Blazor.Infrastructure/
│   │   ├── Extensions/       # InfrastructureServiceExtensions (DI wiring)
│   │   ├── Identity/         # ApplicationUser
│   │   ├── Persistence/
│   │   │   ├── Context/      # ApplicationDbContext
│   │   │   ├── Configurations/
│   │   │   ├── Migrations/
│   │   │   └── Repositories/ # Repository<T>, UnitOfWork
│   │   └── Services/         # JWT, PasswordHasher, UserRegistration, IdentityUser
│   │
│   └── TaskManagerPro.Blazor.Web/
│       ├── Components/
│       │   ├── Dialogs/      # TaskDialog
│       │   ├── Layout/       # MainLayout, NavMenu
│       │   └── Pages/        # Dashboard, Tasks, TaskDetail, Milestones,
│       │                     # Notifications, Profile, Login, Register
│       ├── Pages/            # AuthenticatedPageBase
│       └── Services/         # AuthService, CustomAuthStateProvider,
│                             # NotificationCountService
│
└── tests/
    └── TaskManagerPro.Blazor.Tests/
        ├── Domain/           # TaskItemTests, MilestoneTests
        └── Application/      # Handler tests (Create/Update Task, Login, Register)
```

---

## Future Improvements

- **Email verification** — Account confirmation flow via SendGrid
- **Avatar upload** — Profile picture storage with Azure Blob Storage
- **Overdue notifications** — Automated background jobs with Hangfire
- **Dashboard analytics** — Charts and burndown graphs with a charting library

---

## License

This project is licensed under the [MIT License](LICENSE).
