using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MudBlazor.Services;
using TaskManagerPro.Blazor.Application.Features.Tasks.Commands.CreateTask;
using TaskManagerPro.Blazor.Infrastructure.Extensions;
using TaskManagerPro.Blazor.Infrastructure.Identity;
using TaskManagerPro.Blazor.Infrastructure.Persistence.Context;
using TaskManagerPro.Blazor.Infrastructure.Persistence.Seeding;
using TaskManagerPro.Blazor.Web.Components;
using TaskManagerPro.Blazor.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Razor / Blazor Server
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// MudBlazor
builder.Services.AddMudServices();

// Infrastructure (DbContext, Identity, UnitOfWork, PasswordHasher, JwtGenerator, ValidationBehavior)
builder.Services.AddInfrastructure(builder.Configuration);

// MediatR — scans the Application assembly for all IRequestHandler implementations
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateTaskCommand).Assembly));

// FluentValidation — scans the Application assembly for all AbstractValidator implementations
builder.Services.AddValidatorsFromAssembly(typeof(CreateTaskCommand).Assembly);

// JWT middleware authentication (for API endpoints if added later)
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

// Blazor component-level authorization — required for [Authorize] on Razor components
builder.Services.AddAuthorizationCore();

// CustomAuthStateProvider registered once as a concrete type so AuthService can
// inject it directly; the second registration forwards the base-class contract
// to the same scoped instance rather than creating a second object.
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(
    sp => sp.GetRequiredService<CustomAuthStateProvider>());

builder.Services.AddScoped<AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Seed demo data on every startup — idempotent, skips if data already exists
using (IServiceScope scope = app.Services.CreateScope())
{
    ApplicationDbContext context =
        scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    UserManager<ApplicationUser> userManager =
        scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    await DataSeeder.SeedAsync(context, userManager);
}

app.Run();
