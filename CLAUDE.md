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
- Always add XML documentation comments to public interfaces and classes
- Never use var for primitive types

## Git Rules
- NEVER execute git commands (commit, push, add, reset) under any circumstance
- Only the user decides when and what to commit

## Documentation Rules
- Every public class must have XML summary comment explaining its purpose and responsibility
- Every public method must have XML summary comment
- Every public interface must document each member
- Comments must explain WHY, not WHAT — avoid restating the code
- Use /// <summary> format always
- After creating or modifying any file, verify it has proper XML documentation
