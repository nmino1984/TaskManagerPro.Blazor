# TaskManagerPro.Blazor

![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![Blazor Server](https://img.shields.io/badge/Blazor-Server-7B2FBE?logo=blazor)
![Tests](https://img.shields.io/badge/tests-28%20passing-brightgreen)
![License: MIT](https://img.shields.io/badge/License-MIT-green)

A task management app built with Blazor Server and Clean Architecture. The goal was to practice building something real — not a tutorial CRUD, but a system with proper auth, external integrations, CQRS, and a layered codebase that doesn't turn into spaghetti as it grows.

---

## What it does

- Full task lifecycle: create, assign, track status (Pending → In Progress → Completed / Cancelled), subtasks, milestones
- Assign tasks to team members with in-app notifications and a live badge counter
- Email verification on registration via SendGrid — 5-minute token, resend link available from both Login and /verify-email
- Avatar upload through Cloudinary — instant preview on the profile page, header updates without a page reload
- Dashboard with three MudChart graphs: tasks by status (donut), by priority (bar), tasks created over 6 months (line). Toggle between your assigned tasks and all tasks
- Structured logging to a rotating daily file via Serilog
- User profile page: edit personal info, change password

---

## Stack

| Layer | What |
|---|---|
| Frontend | Blazor Server, MudBlazor |
| Backend | .NET 10, MediatR (CQRS), FluentValidation |
| Database | SQL Server (Docker), EF Core 10 |
| Auth | ASP.NET Core Identity, JWT, BCrypt |
| Email | SendGrid |
| Storage | Cloudinary (avatar images) |
| Logging | Serilog (file sink, daily rolling) |
| Testing | xUnit, NSubstitute, FluentAssertions |

---

## Architecture

Clean Architecture with a strict inward dependency rule — Domain knows nothing about EF Core, Blazor, or HTTP.

```
┌─────────────────────────────────────────────┐
│               Web (Blazor Server)            │  ← Pages, Components, Services
├─────────────────────────────────────────────┤
│                Infrastructure                │  ← EF Core, Identity, JWT, Cloudinary
├─────────────────────────────────────────────┤
│                 Application                  │  ← CQRS handlers, DTOs, validators
├─────────────────────────────────────────────┤
│                   Domain                     │  ← Entities, enums, interfaces (no deps)
└─────────────────────────────────────────────┘
```

**Why this structure?** Mostly to keep the domain model clean. Once you put EF Core attributes or Identity concerns into your entities, you start making decisions based on the ORM instead of the problem. Having a hard boundary also makes the Application layer testable without spinning up a database.

**A few implementation notes worth calling out:**

- Auth state lives in localStorage via JS interop — CustomAuthStateProvider reads the JWT on each render cycle. This means the token, including claims like email_verified and vatar_url, travels with the request and doesn't require an extra DB call on every page load.
- Avatar URL is stored as a JWT claim and refreshed after upload. The AvatarStateService fires an event so the header component re-renders without a reload.
- There are two user representations: AppUser (Domain entity, owns the business rules) and ApplicationUser (ASP.NET Identity). The Identity layer is only used for password hashing and the email confirmation token — not for authorization decisions.
- The FluentValidation pipeline behavior runs automatically before every MediatR handler. Validation errors surface as ValidationException, which the UI catches and displays.

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- A free SendGrid account (for email verification)
- A free Cloudinary account (for avatar uploads)

### 1. Start SQL Server

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Admin1234!" \
  -p 1433:1433 --name sqlserver-taskmanager \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

### 2. Clone and build

```bash
git clone https://github.com/nmino1984/TaskManagerPro.Blazor.git
cd TaskManagerPro.Blazor
dotnet build
```

### 3. Configure credentials

Create src/TaskManagerPro.Blazor.Web/appsettings.Development.json (this file is gitignored):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=TaskManagerProDb;User Id=sa;Password=Admin1234!;TrustServerCertificate=True;"
  },
  "Jwt": { "Key": "your-secret-key-at-least-32-chars" },
  "SendGrid": { "ApiKey": "SG.your-key-here" },
  "Cloudinary": {
    "CloudName": "your-cloud-name",
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret"
  }
}
```

### 4. Apply migrations

```bash
cd src/TaskManagerPro.Blazor.Infrastructure
dotnet ef database update --startup-project ../TaskManagerPro.Blazor.Web
```

### 5. Run

```bash
dotnet run --project src/TaskManagerPro.Blazor.Web
```

Open **http://localhost:5026** in your browser.

---

## Tests

```bash
dotnet test
```

28 tests covering Domain entities and Application handlers (CQRS commands and queries). Uses NSubstitute for faking dependencies and FluentAssertions for readable assertions.

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
│   │   ├── Common/           # Pipeline behaviors, exceptions, interfaces
│   │   └── Features/
│   │       ├── Auth/         # Login, Register, RefreshToken, UpdateProfile, ChangePassword
│   │       ├── Tasks/        # Full CRUD + status transitions
│   │       ├── Milestones/   # Create, Update, Delete, GetByTask
│   │       ├── Notifications/# GetNotifications, MarkAsRead
│   │       └── Users/        # GetAllUsers
│   │
│   ├── TaskManagerPro.Blazor.Infrastructure/
│   │   ├── Persistence/      # DbContext, Repositories, Migrations
│   │   ├── Identity/         # ApplicationUser (ASP.NET Identity)
│   │   └── Services/         # JWT, PasswordHasher, Cloudinary, Email
│   │
│   └── TaskManagerPro.Blazor.Web/
│       ├── Components/
│       │   ├── Layout/       # MainLayout (avatar in header, notification badge)
│       │   ├── Pages/        # Dashboard, Tasks, TaskDetail, Profile, Login, Register,
│       │   │                 # VerifyEmail, Milestones, Notifications
│       │   └── Dialogs/      # TaskDialog
│       └── Services/         # AuthService, AvatarStateService, NotificationCountService,
│                             # CustomAuthStateProvider
│
└── tests/
    └── TaskManagerPro.Blazor.Tests/
        ├── Domain/           # TaskItemTests, MilestoneTests
        └── Application/      # Handler tests (Create/Update Task, Login, Register...)
```

---

## What's next

- Overdue notifications via a background job (Hangfire or .NET Worker Service)
- Task filtering and search on the task list page
- Pagination on queries that currently load everything into memory
- Role-based access — right now all authenticated users have the same permissions

---

## License

[MIT](LICENSE)

---

## 🐳 Docker

The app ships with a `docker-compose.yml` that brings up the full stack — SQL Server and the Blazor app — with a single command.

### Prerequisites
- Docker Desktop running

### Run with Docker Compose

```bash
# Copy the example env file and fill in your real values
cp .env.example .env

# Start everything
docker compose up --build -d

# Follow logs
docker compose logs -f webapp

# Stop
docker compose down
```

### Environment variables

Credentials live in `.env` (gitignored — never committed). Copy `.env.example` to see what's needed:
- `SA_PASSWORD` — SQL Server sa password
- `JWT_KEY` — signing key for JWT tokens (min 32 chars)
- `SENDGRID_API_KEY`
- `CLOUDINARY_CLOUD_NAME`, `CLOUDINARY_API_KEY`, `CLOUDINARY_API_SECRET`

---

## ☸️ Kubernetes

Kubernetes manifests are in `k8s/`. Tested locally with minikube.

### Prerequisites
- Docker Desktop
- [minikube](https://minikube.sigs.k8s.io/docs/start/)
- kubectl

### Deploy to minikube

```bash
minikube start --driver=docker --cpus=2 --memory=3072

# Build and load the image into minikube's local registry
docker build -t taskmanagerproblazor-webapp:latest .
minikube image load taskmanagerproblazor-webapp:latest

# Apply manifests — order matters (PVC and DB before the app)
kubectl apply -f k8s/sqlserver-pvc.yaml
kubectl apply -f k8s/sqlserver-deployment.yaml
kubectl apply -f k8s/sqlserver-service.yaml
kubectl apply -f k8s/configmap.yaml
kubectl apply -f k8s/secret.yaml
kubectl apply -f k8s/deployment.yaml
kubectl apply -f k8s/service.yaml

# Wait for both pods to be ready
kubectl wait --for=condition=ready pod -l app=taskmanager-sqlserver --timeout=120s
kubectl wait --for=condition=ready pod -l app=taskmanager-webapp --timeout=120s

# Get the access URL (keep this terminal open — it maintains the tunnel)
minikube service taskmanager-webapp-service --url
```

### Health endpoints

- `GET /health/live` — liveness: process is up
- `GET /health/ready` — readiness: process is up and SQL Server is reachable