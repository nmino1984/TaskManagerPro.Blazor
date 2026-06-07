# TaskManagerPro.Blazor — Project Rules for Claude Code

## Architecture
- This project follows strict Clean Architecture: Domain → Application → Infrastructure → Web
- Domain has ZERO external dependencies
- Never add EF Core, ASP.NET or any infrastructure concern to Domain layer

## C# and .NET Rules
- Always use private setters for entity Id: `public Guid Id { get; private set; }`
- Never name enums with names that conflict with .NET BCL: avoid TaskStatus, use WorkTaskStatus instead
- Always use `string.Empty` instead of `""`
- Always use `DateTime.UtcNow` never `DateTime.Now`
- Always add CancellationToken to async methods
- Use file-scoped namespaces always

## DDD Rules
- Entities must encapsulate their state — avoid public setters where possible
- Use private or internal setters for properties that should not be modified externally
- Never expose raw collections — use IReadOnlyCollection for navigation properties exposed publicly

## Code Quality
- Zero warnings policy — treat all warnings as errors
- Document public interfaces completely. Skip obvious methods — if the name explains it, no comment needed. Comments explain WHY not WHAT.
- Never use var for primitive types

## Git Rules
- NEVER execute git commands (commit, push, add, reset) under any circumstance
- Only the user decides when and what to commit

## Claude Code Behavior
- Always use --yes flag on any package install commands
- If any file already exists, overwrite it without asking
- Do not prompt for confirmation at any point
- Never wait for user input during automated tasks

## Code Style — Natural Human Style
- Do NOT add XML documentation to every method and property. Only document public interfaces, complex business logic, and non-obvious decisions. Simple CRUD methods need no comments.
- Use concise variable names where context is clear: "tasks" not "taskItemDtoCollection", "user" not "authenticatedUserEntity", "id" not "taskItemIdentifier"
- Avoid over-engineering simple things. A straightforward if/else is often better than a clever pattern
- Leave occasional TODO comments where something could genuinely be improved
- Skip method comments when the method name is already self-explanatory. LoginAsync() needs no summary saying "handles login"
- It is okay to have minor style variations between files
- Avoid redundant inline comments that just restate the code in English
- Use "var" freely when the type is obvious from context
- Not every edge case needs explicit handling — trust the framework where appropriate
- Prefer flat code over deeply nested abstractions when the logic is simple enough
