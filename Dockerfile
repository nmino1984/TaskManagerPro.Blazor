# syntax=docker/dockerfile:1

# --- build stage ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# .csproj files first — keeps restore layer cached when only code changes.
# Tests project excluded intentionally (not needed in the final image).
COPY src/TaskManagerPro.Blazor.Domain/TaskManagerPro.Blazor.Domain.csproj src/TaskManagerPro.Blazor.Domain/
COPY src/TaskManagerPro.Blazor.Application/TaskManagerPro.Blazor.Application.csproj src/TaskManagerPro.Blazor.Application/
COPY src/TaskManagerPro.Blazor.Infrastructure/TaskManagerPro.Blazor.Infrastructure.csproj src/TaskManagerPro.Blazor.Infrastructure/
COPY src/TaskManagerPro.Blazor.Web/TaskManagerPro.Blazor.Web.csproj src/TaskManagerPro.Blazor.Web/

RUN dotnet restore src/TaskManagerPro.Blazor.Web/TaskManagerPro.Blazor.Web.csproj

COPY src/TaskManagerPro.Blazor.Domain/ src/TaskManagerPro.Blazor.Domain/
COPY src/TaskManagerPro.Blazor.Application/ src/TaskManagerPro.Blazor.Application/
COPY src/TaskManagerPro.Blazor.Infrastructure/ src/TaskManagerPro.Blazor.Infrastructure/
COPY src/TaskManagerPro.Blazor.Web/ src/TaskManagerPro.Blazor.Web/

RUN dotnet publish src/TaskManagerPro.Blazor.Web/TaskManagerPro.Blazor.Web.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# --- runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TaskManagerPro.Blazor.Web.dll"]
