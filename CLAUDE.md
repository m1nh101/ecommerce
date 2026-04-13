# CLAUDE.md - [Ecommerce]

## Overview
Build a backend-focused e-commerce system to practice Domain-Driven Design (DDD), modeling core domains like Order, Payment, and Inventory with clear business logic and real-world workflows.

## Tech Stack
- .NET 10, ASP.NET Core API
- Entity Framework Core 10 with PostgreSQL
- Mediator for CQRS (source-generated, https://github.com/martinothamar/Mediator)
- FluentValidation for request validation
- Scalar for OpenAPI documentation
- NUnit + FluentAssertions for testing
- Use ASP.NET Identity for user management and authentication

## Project Structure
- `src/Api/` - Endpoints, middleware, DI configuration
- `src/Application/` - Commands, queries, handlers, validators
- `src/Domain/` - Entities, value objects, enums, domain events
- `src/Infrastructure/` - EF Core, external services, repositories
- `tests/UnitTests/` - Domain and application layer tests
- `tests/IntegrationTests/` - API and database tests

## Core Principles
- Always prioritize business domain modeling over technical implementation.
- Business logic MUST reside in the Domain layer, not in controllers or services.
- Avoid anemic domain models (no logic in entities).

## Commands
- Build: `dotnet build`
- Test: `dotnet test`
- Run API: `dotnet run --project src/Api`
- Add Migration: `dotnet ef migrations add <Name> -p src/Infrastructure -s src/Api`
- Update Database: `dotnet ef database update -p src/Infrastructure -s src/Api`
- Format: `dotnet format`

## Architecture Rules
- Domain layer has ZERO external dependencies, business logic, aggregates, domain events
- Application layer defines interfaces, Infrastructure implements them, use cases / orchestration and should not contain business logic
- Infrastructure layer (DB, messaging, external services)
- All database access goes through EF Core DbContext (no repository pattern)
- Use Mediator for all command/query handling
- API layer is thin - endpoint definitions only

## Domain-Driven Design Rules
- Identify and model Aggregates (e.g., Order, Payment, Inventory).
- All state changes MUST go through Aggregate Roots.
- Use rich domain models (methods with behavior, not just data).

## Code Conventions

### Naming
- Commands: `Create[Entity]Command`, `Update[Entity]Command`
- Queries: `Get[Entity]Query`, `List[Entities]Query`
- Handlers: `[Command/Query]Handler`
- DTOs: `Create[Entity]Request`

### EFCore Convention
- Use `snake_case` for columns, tables when create migration

### Patterns We Use
- Primary constructors for DI
- Records for DTOs and commands
- Result<T> pattern for error handling (no exceptions for flow control)
- File-scoped namespaces
- Always pass CancellationToken to async methods
- Specification pattern for reuseable application logic and EF query filter

### Patterns We DON'T Use (Never Suggest)
- Repository pattern (use EF Core directly)
- AutoMapper (write explicit mappings)
- Exceptions for business logic errors
- Stored procedures

## Validation
- All request validation in FluentValidation validators
- Validators auto-registered via assembly scanning
- Validation runs in Mediator pipeline behavior

## Testing
- Unit tests: Domain logic and handlers
- Integration tests: Full API endpoint testing with WebApplicationFactory
- Use FluentAssertions for readable assertions
- Test naming: `[Method]_[Scenario]_[ExpectedResult]`

## Git Workflow
- Branch naming: `feature/`, `fix/`, `hotfix/`
- Commit format: `type: description` (feat, fix, refactor, test, docs)
- Always create a branch before changes
- Run tests before committing

