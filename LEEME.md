# TaskManagerPro.Blazor

![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![Blazor Server](https://img.shields.io/badge/Blazor-Server-7B2FBE?logo=blazor)
![Tests](https://img.shields.io/badge/tests-28%20passing-brightgreen)
![License: MIT](https://img.shields.io/badge/License-MIT-green)

Aplicación de gestión de tareas construida con Blazor Server y Clean Architecture.
El objetivo era construir algo real, no un CRUD de tutorial, sino un sistema con
autenticación completa, integraciones externas, CQRS, y una base de código que no
se convierta en un desastre a medida que crece.

---

## Qué hace

- Ciclo de vida completo de tareas: crear, asignar, cambiar estado (Pendiente, En Progreso, Completado, Cancelado), subtareas y milestones
- Asignación de tareas a miembros del equipo con notificaciones en la app y un badge en tiempo real
- Verificación de email al registrarse vía SendGrid. Token de 5 minutos, con opción de reenvío desde el login y desde la página de verificación
- Subida de avatar a través de Cloudinary. Preview inmediato en la página de perfil, el header se actualiza sin recargar la página
- Dashboard con tres gráficos MudChart: tareas por estado (donut), por prioridad (barra), tareas creadas en los últimos 6 meses (línea). Toggle entre mis tareas asignadas y todas las del sistema
- Logging estructurado a archivo rotativo diario vía Serilog
- Página de perfil: editar datos personales y cambiar contraseña

---

## Stack

| Capa | Tecnología |
|---|---|
| Frontend | Blazor Server, MudBlazor |
| Backend | .NET 10, MediatR (CQRS), FluentValidation |
| Base de datos | SQL Server (Docker), EF Core 10 |
| Auth | ASP.NET Core Identity, JWT, BCrypt |
| Email | SendGrid |
| Almacenamiento | Cloudinary (imágenes de avatar) |
| Logging | Serilog (archivo, rotación diaria) |
| Testing | xUnit, NSubstitute, FluentAssertions |

---

## Arquitectura

Clean Architecture con una regla de dependencias estrictamente hacia adentro.
El Domain no sabe nada de EF Core, Blazor ni HTTP.

```
+-----------------------------------------------+
|           Web (Blazor Server)                  |  <- Paginas, Componentes, Servicios
+-----------------------------------------------+
|            Infrastructure                      |  <- EF Core, Identity, JWT, Cloudinary
+-----------------------------------------------+
|             Application                        |  <- Handlers CQRS, DTOs, validadores
+-----------------------------------------------+
|               Domain                           |  <- Entidades, enums, interfaces (sin deps)
+-----------------------------------------------+
```

**Por qué esta estructura:** Para mantener el modelo de dominio limpio. Una vez que empezás a poner
atributos de EF Core o concerns de Identity en las entidades, las decisiones de diseño las empieza a
tomar el ORM en lugar del problema. Tener una frontera dura también hace que la capa Application sea
testeable sin levantar una base de datos.

**Algunos detalles de implementación que vale la pena mencionar:**

- El estado de autenticación vive en localStorage vía JS interop. CustomAuthStateProvider lee el JWT
  en cada ciclo de render, incluyendo claims como email_verified y avatar_url, sin necesitar una query
  extra a la base de datos en cada página.
- La URL del avatar se guarda como claim JWT y se refresca después de subir uno nuevo. AvatarStateService
  dispara un evento para que el header se re-renderice sin recargar la página.
- Hay dos representaciones del usuario: AppUser (entidad de Domain, dueña de las reglas de negocio)
  y ApplicationUser (ASP.NET Identity). Identity solo se usa para hash de contraseña y token de
  confirmación de email, no para decisiones de autorización.
- El pipeline behavior de FluentValidation corre automáticamente antes de cada handler de MediatR.
  Los errores de validación suben como ValidationException, que la UI captura y muestra.

---

## Cómo arrancarlo

### Requisitos previos

- .NET 10 SDK: https://dotnet.microsoft.com/download
- Docker Desktop: https://www.docker.com/products/docker-desktop
- Una cuenta gratuita de SendGrid (para verificación de email)
- Una cuenta gratuita de Cloudinary (para subida de avatares)

### 1. Levantar SQL Server

```bash
docker run -e ACCEPT_EULA=Y -e SA_PASSWORD=Admin1234! -p 1433:1433 --name sqlserver-taskmanager -d mcr.microsoft.com/mssql/server:2022-latest
```

### 2. Clonar y compilar

```bash
git clone https://github.com/nmino1984/TaskManagerPro.Blazor.git
cd TaskManagerPro.Blazor
dotnet build
```

### 3. Configurar credenciales

Crear src/TaskManagerPro.Blazor.Web/appsettings.Development.json (este archivo está en .gitignore):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=TaskManagerProDb;User Id=sa;Password=Admin1234!;TrustServerCertificate=True;"
  },
  "Jwt": { "Key": "tu-clave-secreta-de-al-menos-32-caracteres" },
  "SendGrid": { "ApiKey": "SG.tu-clave-aqui" },
  "Cloudinary": {
    "CloudName": "tu-cloud-name",
    "ApiKey": "tu-api-key",
    "ApiSecret": "tu-api-secret"
  }
}
```

### 4. Aplicar migraciones

```bash
cd src/TaskManagerPro.Blazor.Infrastructure
dotnet ef database update --startup-project ../TaskManagerPro.Blazor.Web
```

### 5. Correr la app

```bash
dotnet run --project src/TaskManagerPro.Blazor.Web
```

Abrir http://localhost:5026 en el navegador.

---

## Tests

```bash
dotnet test
```

28 tests que cubren entidades del Domain y handlers de Application (comandos y queries CQRS).
Usa NSubstitute para los fakes y FluentAssertions para que las aserciones sean legibles.

---

## Estructura del proyecto

```
TaskManagerPro.Blazor/
+-- src/
|   +-- TaskManagerPro.Blazor.Domain/
|   |   +-- Entities/         # TaskItem, AppUser, Milestone, Notification, SubTask
|   |   +-- Enums/            # WorkTaskStatus, TaskPriority, MilestoneStatus
|   |   +-- Interfaces/       # IRepository<T>, IUnitOfWork
|   |
|   +-- TaskManagerPro.Blazor.Application/
|   |   +-- Common/           # Pipeline behaviors, excepciones, interfaces
|   |   +-- Features/
|   |       +-- Auth/         # Login, Register, RefreshToken, UpdateProfile, ChangePassword
|   |       +-- Tasks/        # CRUD completo + transiciones de estado
|   |       +-- Milestones/   # Create, Update, Delete, GetByTask
|   |       +-- Notifications/# GetNotifications, MarkAsRead
|   |       +-- Users/        # GetAllUsers
|   |
|   +-- TaskManagerPro.Blazor.Infrastructure/
|   |   +-- Persistence/      # DbContext, Repositorios, Migraciones
|   |   +-- Identity/         # ApplicationUser (ASP.NET Identity)
|   |   +-- Services/         # JWT, PasswordHasher, Cloudinary, Email
|   |
|   +-- TaskManagerPro.Blazor.Web/
|       +-- Components/
|       |   +-- Layout/       # MainLayout (avatar en header, badge de notificaciones)
|       |   +-- Pages/        # Dashboard, Tasks, TaskDetail, Profile, Login, Register,
|       |   |                 # VerifyEmail, Milestones, Notifications
|       |   +-- Dialogs/      # TaskDialog
|       +-- Services/         # AuthService, AvatarStateService, NotificationCountService,
|                             # CustomAuthStateProvider
|
+-- tests/
    +-- TaskManagerPro.Blazor.Tests/
        +-- Domain/           # TaskItemTests, MilestoneTests
        +-- Application/      # Tests de handlers (Create/Update Task, Login, Register...)
```

---

## Lo que viene

- Notificaciones de tareas vencidas vía job en segundo plano (Hangfire o .NET Worker Service)
- Filtros y búsqueda en la lista de tareas
- Paginación en las queries que actualmente cargan todo en memoria
- Control de acceso por roles; por ahora todos los usuarios autenticados tienen los mismos permisos

---

## Licencia

[MIT](LICENSE)