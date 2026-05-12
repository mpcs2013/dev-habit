# Phase 1 — Solution Skeleton (with Aspire)

**Goal:** Create the empty Clean Architecture project structure. No business logic yet.

**Why second:** It lets you see and lock in the dependency graph early, before code makes refactoring harder.

This phase creates an intentionally “empty” solution: projects exist, references are wired correctly, and CI builds/tests—*but you haven’t implemented features yet*.

> **Numbering note (keep as-is):** The issue list remains **1.1–1.9**. Two additional activities are included under existing issues so numbering doesn’t change:
> - **Architecture tests** are implemented under **1.8**.
> - **CI update** is implemented under **1.9**.

---

## By the end of Phase 1, you’ll have

- One solution file: `CurrencyTracker.sln`
- `src/` and `tests/` folders
- 7 application projects:
  - `CurrencyTracker.Domain`
  - `CurrencyTracker.Application`
  - `CurrencyTracker.Infrastructure`
  - `CurrencyTracker.Api`
  - `CurrencyTracker.Worker`
  - `CurrencyTracker.ServiceDefaults`
  - `CurrencyTracker.AppHost`
- 4 test projects:
  - `CurrencyTracker.Domain.UnitTests`
  - `CurrencyTracker.Application.UnitTests`
  - `CurrencyTracker.Infrastructure.IntegrationTests`
  - `CurrencyTracker.Architecture.Tests` (**architecture guardrails**)
- All project references wired in the correct direction
- `dotnet build` passing with no business code
- `dotnet test` passing (architecture test included)
- CI updated to run restore/build/test (and format check if configured)

---

## Quick mental model

A solution is a **container** of projects.
A project is a `.csproj` file that produces either a library (`.dll`) or executable (`.exe`).
Projects can reference other projects; references define build order and allowed compile-time coupling.

### Clean Architecture dependency rule
Dependencies may only flow *inward*:

```
Domain (top, no dependencies)
  ↑
Application (depends on Domain)
  ↑
Infrastructure (depends on Application + Domain)
  ↑
Api & Worker (depend on Application + Infrastructure + ServiceDefaults)
  ↑
AppHost (orchestrates Api/Worker locally)
```

If `Domain` references `Application`, you’ve broken the rule. We’ll add an **architecture test** to catch that.

---

## Before you start

Make sure:
- [ ] Phase 0 is complete (main is clean)
- [ ] You’re on `main` and up to date:
  ```bash
  git checkout main
  git pull
  ```
- [ ] .NET 10 SDK installed:
  ```bash
  dotnet --version
  ```

---

## Standard workflow for every issue (GitHub + local)

For each issue below:

1. **Create GitHub issue** (title/body/labels/milestone)
2. **Create branch** locally
3. Do the work via terminal (commands provided)
4. Verify locally (`dotnet build` / `dotnet test`)
5. Commit + push
6. Open PR
7. Self-review
8. Merge

---

# Issues (keep as-is)

| #   | Title | What you'll learn |
| --- | --- | --- |
| 1.1 | Create empty solution `CurrencyTracker.sln` with `src/` and `tests/` folders | Solution organization |
| 1.2 | Add `CurrencyTracker.Domain` class library project (no deps) | Pure domain layer |
| 1.3 | Add `CurrencyTracker.Application` class library; reference Domain | Layer dependency direction |
| 1.4 | Add `CurrencyTracker.Infrastructure` class library; reference Application + Domain | Where I/O lives |
| 1.5 | Add `CurrencyTracker.Api` Web API minimal project; reference Application + Infrastructure | Composition root for HTTP |
| 1.6 | Add `CurrencyTracker.Worker` worker project; reference Application + Infrastructure | Composition root for background jobs |
| 1.7 | Add unit test projects for Domain and Application (xUnit + FluentAssertions + NSubstitute) | Test project setup |
| 1.8 | Add architecture test project using `NetArchTest.Rules` with one test asserting Domain has no outbound refs | Enforcing architecture in CI |
| 1.9 | Wire all project references and confirm `dotnet build` succeeds | MSBuild references |

---

# 1.1 — Create empty solution + `src/` + `tests/`

## GitHub issue
**Title:** Create `CurrencyTracker.sln` with `src/` and `tests/` folders

**Acceptance criteria**
- [ ] `CurrencyTracker.sln` exists at repo root
- [ ] `src/` exists
- [ ] `tests/` exists
- [ ] `dotnet sln list` shows zero projects
- [ ] `dotnet build` succeeds (does nothing yet)

## Branch
```bash
git checkout -b feat/01-solution-skeleton
```

## Step-by-step

### 1) Create the solution file
```bash
dotnet new sln --name CurrencyTracker
```
**What this does:** creates `CurrencyTracker.sln` at repo root.

### 2) Create folder structure
```bash
mkdir src
mkdir tests
```
**What this does:** establishes a conventional layout.

### 3) Verify solution is empty
```bash
dotnet sln list
```
Expected output:
```
Project(s)
----------
```

### 4) Verify build
```bash
dotnet build
```
Expected output includes:
```
No projects found in solution.
```

## Commit + push
```bash
git add CurrencyTracker.sln src/ tests/
git commit -m "chore: create solution file and folder structure"
git push -u origin feat/01-solution-skeleton
```

## PR
Open PR → self-review → merge.

---

# 1.2 — Add `CurrencyTracker.Domain` (no dependencies)

## GitHub issue
**Title:** Add `CurrencyTracker.Domain` class library (no dependencies)

**Acceptance criteria**
- [ ] `src/CurrencyTracker.Domain/CurrencyTracker.Domain.csproj` exists
- [ ] Targets `net10.0`
- [ ] Added to solution
- [ ] No NuGet references
- [ ] `dotnet build` succeeds

## Branch
```bash
git checkout main
git pull
git checkout -b feat/01-domain-project
```

## Step-by-step

### 1) Create the project
```bash
dotnet new classlib --name CurrencyTracker.Domain --output src/CurrencyTracker.Domain
```

### 2) Delete template class
```bash
rm src/CurrencyTracker.Domain/Class1.cs
```

### 3) Add to solution
```bash
dotnet sln add src/CurrencyTracker.Domain/
```

### 4) Add a marker type (for architecture tests)
Create `src/CurrencyTracker.Domain/DomainAssemblyMarker.cs`:

```csharp
namespace CurrencyTracker.Domain;

/// <summary>
/// Marker type used by tests to locate the Domain assembly without requiring real domain code.
/// </summary>
public sealed class DomainAssemblyMarker;
```

### 5) Build
```bash
dotnet build
```

## Commit + push
```bash
git add src/CurrencyTracker.Domain/ CurrencyTracker.sln
git commit -m "feat: add CurrencyTracker.Domain project"
git push -u origin feat/01-domain-project
```

## PR
Open PR → self-review → merge.

---

# 1.3 — Add `CurrencyTracker.Application` (references Domain)

## GitHub issue
**Title:** Add `CurrencyTracker.Application` project referencing Domain

**Acceptance criteria**
- [ ] Project exists at `src/CurrencyTracker.Application/`
- [ ] References `CurrencyTracker.Domain`
- [ ] Added to solution
- [ ] `dotnet build` succeeds
- [ ] No NuGet packages yet

## Branch
```bash
git checkout main
git pull
git checkout -b feat/01-application-project
```

## Step-by-step

### 1) Create the project
```bash
dotnet new classlib --name CurrencyTracker.Application --output src/CurrencyTracker.Application
```

### 2) Delete template class
```bash
rm src/CurrencyTracker.Application/Class1.cs
```

### 3) Add to solution
```bash
dotnet sln add src/CurrencyTracker.Application/
```

### 4) Add project reference to Domain
```bash
dotnet add src/CurrencyTracker.Application/ reference src/CurrencyTracker.Domain/
```
**What this does:** adds a `<ProjectReference .../>` to `CurrencyTracker.Application.csproj`.

### 5) Build
```bash
dotnet build
```

## Commit + push
```bash
git add src/CurrencyTracker.Application/ CurrencyTracker.sln
git commit -m "feat: add CurrencyTracker.Application with Domain reference"
git push -u origin feat/01-application-project
```

## PR
Open PR → self-review → merge.

---

# 1.4 — Add `CurrencyTracker.Infrastructure` (references Application + Domain)

## GitHub issue
**Title:** Add `CurrencyTracker.Infrastructure` project referencing Application and Domain

**Acceptance criteria**
- [ ] Project exists at `src/CurrencyTracker.Infrastructure/`
- [ ] References Application and Domain
- [ ] Added to solution
- [ ] `dotnet build` succeeds

## Branch
```bash
git checkout main
git pull
git checkout -b feat/01-infrastructure-project
```

## Step-by-step

### 1) Create the project
```bash
dotnet new classlib --name CurrencyTracker.Infrastructure --output src/CurrencyTracker.Infrastructure
```

### 2) Delete template class
```bash
rm src/CurrencyTracker.Infrastructure/Class1.cs
```

### 3) Add to solution
```bash
dotnet sln add src/CurrencyTracker.Infrastructure/
```

### 4) Add references
```bash
dotnet add src/CurrencyTracker.Infrastructure/ reference src/CurrencyTracker.Application/
dotnet add src/CurrencyTracker.Infrastructure/ reference src/CurrencyTracker.Domain/
```

### 5) Build
```bash
dotnet build
```

## Commit + push
```bash
git add src/CurrencyTracker.Infrastructure/ CurrencyTracker.sln
git commit -m "feat: add CurrencyTracker.Infrastructure with references"
git push -u origin feat/01-infrastructure-project
```

## PR
Open PR → self-review → merge.

---

# 1.5 — Add `CurrencyTracker.Api` (Minimal API; references Application + Infrastructure)

## GitHub issue
**Title:** Add `CurrencyTracker.Api` Web API minimal project; reference Application + Infrastructure

**Acceptance criteria**
- [ ] Project exists at `src/CurrencyTracker.Api/`
- [ ] Project type is `Microsoft.NET.Sdk.Web`
- [ ] References Application and Infrastructure
- [ ] Has `Program.cs`
- [ ] `dotnet build` succeeds

## Branch
```bash
git checkout main
git pull
git checkout -b feat/01-api-project
```

## Step-by-step

### 1) Create the project
```bash
dotnet new webapi --name CurrencyTracker.Api --output src/CurrencyTracker.Api
```

### 2) Remove template sample code (recommended)
```bash
rm src/CurrencyTracker.Api/WeatherForecast.cs
rm src/CurrencyTracker.Api/Controllers/WeatherForecastController.cs
rmdir src/CurrencyTracker.Api/Controllers
```

### 3) Add to solution
```bash
dotnet sln add src/CurrencyTracker.Api/
```

### 4) Add references
```bash
dotnet add src/CurrencyTracker.Api/ reference src/CurrencyTracker.Application/
dotnet add src/CurrencyTracker.Api/ reference src/CurrencyTracker.Infrastructure/
```

### 5) Build
```bash
dotnet build
```

## Commit + push
```bash
git add src/CurrencyTracker.Api/ CurrencyTracker.sln
git commit -m "feat: add CurrencyTracker.Api with references"
git push -u origin feat/01-api-project
```

## PR
Open PR → self-review → merge.

---

# 1.6 — Add `CurrencyTracker.Worker` (references Application + Infrastructure)

## GitHub issue
**Title:** Add `CurrencyTracker.Worker` background job project

**Acceptance criteria**
- [ ] Project exists at `src/CurrencyTracker.Worker/`
- [ ] Project type is Worker Service (`Microsoft.NET.Sdk.Worker`)
- [ ] References Application and Infrastructure
- [ ] Has `Program.cs` and `Worker.cs` from template
- [ ] `dotnet build` succeeds

## Branch
```bash
git checkout main
git pull
git checkout -b feat/01-worker-project
```

## Step-by-step

### 1) Create the project
```bash
dotnet new worker --name CurrencyTracker.Worker --output src/CurrencyTracker.Worker
```

### 2) Add to solution
```bash
dotnet sln add src/CurrencyTracker.Worker/
```

### 3) Add references
```bash
dotnet add src/CurrencyTracker.Worker/ reference src/CurrencyTracker.Application/
dotnet add src/CurrencyTracker.Worker/ reference src/CurrencyTracker.Infrastructure/
```

### 4) IMPORTANT: keep template `Worker.cs` for Phase 1
The worker template registers `Worker` in `Program.cs`. If you delete `Worker.cs` now, the build will fail. Replace/delete it later when you implement the real job.

### 5) Build
```bash
dotnet build
```

## Commit + push
```bash
git add src/CurrencyTracker.Worker/ CurrencyTracker.sln
git commit -m "feat: add CurrencyTracker.Worker with references"
git push -u origin feat/01-worker-project
```

## PR
Open PR → self-review → merge.

---

# 1.7 — Add `CurrencyTracker.AppHost` (Aspire orchestration)

## GitHub issue
**Title:** Add `CurrencyTracker.AppHost` for .NET Aspire orchestration

**Acceptance criteria**
- [ ] Project exists at `src/CurrencyTracker.AppHost/`
- [ ] Has template `AppHost.cs`
- [ ] `dotnet build` succeeds

## Branch
```bash
git checkout main
git pull
git checkout -b feat/01-apphost-project
```

## Step-by-step

### 1) Create AppHost
```bash
dotnet new aspire-apphost --name CurrencyTracker.AppHost --output src/CurrencyTracker.AppHost
```

### 2) Add to solution
```bash
dotnet sln add src/CurrencyTracker.AppHost/
```

### 3) Build
```bash
dotnet build
```

> If the template expects `CurrencyTracker.ServiceDefaults` and build fails, that’s OK—proceed to 1.8 (or combine 1.7 + 1.8 into a single PR).

## Commit + push
```bash
git add src/CurrencyTracker.AppHost/ CurrencyTracker.sln
git commit -m "feat: add CurrencyTracker.AppHost for Aspire orchestration"
git push -u origin feat/01-apphost-project
```

## PR
Open PR → self-review → merge.

---

# 1.8 — Add `CurrencyTracker.ServiceDefaults` + architecture tests

This issue includes **two** deliverables to keep numbering unchanged:
1) `ServiceDefaults` project used by Api/Worker (Aspire pattern)
2) **Architecture test project** using `NetArchTest.Rules`

## GitHub issue
**Title:** Add `CurrencyTracker.ServiceDefaults` + architecture tests (NetArchTest)

**Acceptance criteria**
- [ ] `src/CurrencyTracker.ServiceDefaults/` exists
- [ ] Api and Worker reference `ServiceDefaults`
- [ ] Api and Worker call `AddServiceDefaults()`
- [ ] `tests/CurrencyTracker.Architecture.Tests/` exists
- [ ] One test: Domain has no outbound references
- [ ] `dotnet test` passes

## Branch
```bash
git checkout main
git pull
git checkout -b feat/01-servicedefaults-and-architecture-tests
```

## Step-by-step

## Part A — ServiceDefaults

### 1) Create the project
```bash
dotnet new classlib --name CurrencyTracker.ServiceDefaults --output src/CurrencyTracker.ServiceDefaults
rm src/CurrencyTracker.ServiceDefaults/Class1.cs
dotnet sln add src/CurrencyTracker.ServiceDefaults/
```

### 2) Add `Extensions.cs`
Create `src/CurrencyTracker.ServiceDefaults/Extensions.cs`:

```csharp
namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Shared cross-cutting defaults for services (Api, Worker).
/// Real implementation comes later (telemetry, health, resilience, etc.).
/// </summary>
public static class ServiceDefaultsExtensions
{
    public static IHostBuilder AddServiceDefaults(this IHostBuilder builder)
    {
        // Placeholder for later phases.
        return builder;
    }
}
```

### 3) Reference ServiceDefaults from Api and Worker
```bash
dotnet add src/CurrencyTracker.Api/ reference src/CurrencyTracker.ServiceDefaults/
dotnet add src/CurrencyTracker.Worker/ reference src/CurrencyTracker.ServiceDefaults/
```

### 4) Update Api Program.cs
In `src/CurrencyTracker.Api/Program.cs`, right after builder creation:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Host.AddServiceDefaults();
```

### 5) Update Worker Program.cs
In `src/CurrencyTracker.Worker/Program.cs`, chain `.AddServiceDefaults()`:

```csharp
IHost host = Host.CreateDefaultBuilder(args)
    .AddServiceDefaults()
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
```

### 6) Build
```bash
dotnet build
```

## Part B — Architecture tests (NetArchTest)

### 7) Create the project
```bash
dotnet new xunit --name CurrencyTracker.Architecture.Tests --output tests/CurrencyTracker.Architecture.Tests
rm tests/CurrencyTracker.Architecture.Tests/UnitTest1.cs
dotnet sln add tests/CurrencyTracker.Architecture.Tests/
```

### 8) Add NetArchTest.Rules package
Add to `Directory.Packages.props`:

```xml
<PackageVersion Include="NetArchTest.Rules" Version="1.3.2" />
```

Add package + reference Domain:
```bash
dotnet add tests/CurrencyTracker.Architecture.Tests/ package NetArchTest.Rules
dotnet add tests/CurrencyTracker.Architecture.Tests/ reference src/CurrencyTracker.Domain/
```

### 9) Add the test file
Create `tests/CurrencyTracker.Architecture.Tests/ArchitectureTests.cs`:

```csharp
using CurrencyTracker.Domain;
using NetArchTest.Rules;

namespace CurrencyTracker.Architecture.Tests;

public class ArchitectureTests
{
    [Fact]
    public void Domain_ShouldHaveNoDependenciesOnOuterLayers()
    {
        var domainAssembly = typeof(DomainAssemblyMarker).Assembly;

        var result = Types
            .InAssembly(domainAssembly)
            .Should()
            .NotHaveDependencyOnAny(
                "CurrencyTracker.Application",
                "CurrencyTracker.Infrastructure",
                "CurrencyTracker.Api",
                "CurrencyTracker.Worker"
            )
            .GetResult();

        Assert.True(result.IsSuccessful, string.Join(Environment.NewLine, result.FailingTypes));
    }
}
```

### 10) Run tests
```bash
dotnet test
```

## Commit + push
```bash
git add src/CurrencyTracker.ServiceDefaults/ src/CurrencyTracker.Api/Program.cs src/CurrencyTracker.Worker/Program.cs \
  tests/CurrencyTracker.Architecture.Tests/ Directory.Packages.props CurrencyTracker.sln

git commit -m "feat: add ServiceDefaults and architecture tests"
git push -u origin feat/01-servicedefaults-and-architecture-tests
```

## PR
Open PR → self-review → merge.

---

# 1.9 — Add test projects + wire references + update CI

This issue includes **two** deliverables to keep numbering unchanged:
1) unit/integration test projects + packages
2) CI update to run restore/build/test (and format check if configured)

## GitHub issue
**Title:** Add test projects, wire references, and update CI

**Acceptance criteria**
- [ ] Test projects exist:
  - [ ] `tests/CurrencyTracker.Domain.UnitTests/`
  - [ ] `tests/CurrencyTracker.Application.UnitTests/`
  - [ ] `tests/CurrencyTracker.Infrastructure.IntegrationTests/`
- [ ] Each test project references the layer it tests
- [ ] xUnit + FluentAssertions + NSubstitute available via `Directory.Packages.props`
- [ ] `dotnet test` passes
- [ ] CI runs `dotnet restore`, `dotnet build`, `dotnet test` (and format check if configured)

## Branch
```bash
git checkout main
git pull
git checkout -b feat/01-tests-and-ci
```

## Step-by-step

## Part A — Test projects

### 1) Domain.UnitTests
```bash
dotnet new xunit --name CurrencyTracker.Domain.UnitTests --output tests/CurrencyTracker.Domain.UnitTests
rm tests/CurrencyTracker.Domain.UnitTests/UnitTest1.cs
dotnet sln add tests/CurrencyTracker.Domain.UnitTests/
dotnet add tests/CurrencyTracker.Domain.UnitTests/ reference src/CurrencyTracker.Domain/
```

### 2) Application.UnitTests
```bash
dotnet new xunit --name CurrencyTracker.Application.UnitTests --output tests/CurrencyTracker.Application.UnitTests
rm tests/CurrencyTracker.Application.UnitTests/UnitTest1.cs
dotnet sln add tests/CurrencyTracker.Application.UnitTests/
dotnet add tests/CurrencyTracker.Application.UnitTests/ reference src/CurrencyTracker.Application/
```

### 3) Infrastructure.IntegrationTests
```bash
dotnet new xunit --name CurrencyTracker.Infrastructure.IntegrationTests --output tests/CurrencyTracker.Infrastructure.IntegrationTests
rm tests/CurrencyTracker.Infrastructure.IntegrationTests/UnitTest1.cs
dotnet sln add tests/CurrencyTracker.Infrastructure.IntegrationTests/
dotnet add tests/CurrencyTracker.Infrastructure.IntegrationTests/ reference src/CurrencyTracker.Infrastructure/
```

### 4) Add test packages to `Directory.Packages.props`
Add (or ensure present) these versions:

```xml
<ItemGroup>
  <!-- Testing -->
  <PackageVersion Include="FluentAssertions" Version="6.12.0" />
  <PackageVersion Include="NSubstitute" Version="5.1.0" />
</ItemGroup>
```

> xUnit + Microsoft.NET.Test.Sdk are usually already present from the xUnit template. If you are using central package management, you can also pin those versions here as well.

### 5) Add FluentAssertions + NSubstitute references to each test project
```bash
dotnet add tests/CurrencyTracker.Domain.UnitTests/ package FluentAssertions
dotnet add tests/CurrencyTracker.Domain.UnitTests/ package NSubstitute

dotnet add tests/CurrencyTracker.Application.UnitTests/ package FluentAssertions
dotnet add tests/CurrencyTracker.Application.UnitTests/ package NSubstitute

dotnet add tests/CurrencyTracker.Infrastructure.IntegrationTests/ package FluentAssertions
dotnet add tests/CurrencyTracker.Infrastructure.IntegrationTests/ package NSubstitute
```

### 6) Run tests
```bash
dotnet test
```

## Part B — Wire references sanity check

### 7) Confirm solution projects
```bash
dotnet sln list
```

### 8) Build
```bash
dotnet build -c Release
```

## Part C — CI update

### 9) Update workflow `.github/workflows/ci.yml`
Uncomment/enable restore/build/test now that projects exist.

Recommended sequence:
- Restore
- (Optional) Format check
- Build
- Test

> If your format step uses `dotnet csharpier`, ensure CI restores tools first (e.g., `dotnet tool restore`) if Phase 0 set it up as a local tool.

### 10) Verify locally once more
```bash
dotnet build -c Release
dotnet test -c Release
```

## Commit + push
```bash
git add tests/ Directory.Packages.props CurrencyTracker.sln .github/workflows/ci.yml

git commit -m "test/ci: add test projects and enable build/test in CI"
git push -u origin feat/01-tests-and-ci
```

## PR
Open PR → watch Actions run → merge.

---

# Phase 1 — Definition of done

- [ ] All issues/PRs merged
- [ ] `dotnet sln list` shows all projects
- [ ] `dotnet build -c Release` succeeds
- [ ] `dotnet test -c Release` succeeds (architecture test passes)
- [ ] Architecture test fails if you intentionally break the rule (e.g., add a Domain reference to Application)
- [ ] CI runs green on PRs

---

## Summary of what you built

```
CurrencyTracker.sln
├── src/
│   ├── CurrencyTracker.Domain/               (no deps)
│   ├── CurrencyTracker.Application/          (→ Domain)
│   ├── CurrencyTracker.Infrastructure/       (→ Application, Domain)
│   ├── CurrencyTracker.Api/                  (→ Application, Infrastructure, ServiceDefaults)
│   ├── CurrencyTracker.Worker/               (→ Application, Infrastructure, ServiceDefaults)
│   ├── CurrencyTracker.AppHost/              (Aspire orchestration)
│   └── CurrencyTracker.ServiceDefaults/      (shared config for Api, Worker)
└── tests/
    ├── CurrencyTracker.Domain.UnitTests/
    ├── CurrencyTracker.Application.UnitTests/
    ├── CurrencyTracker.Infrastructure.IntegrationTests/
    └── CurrencyTracker.Architecture.Tests/
```

**Dependency flow:**
```
Domain
  ↑
Application
  ↑
Infrastructure
  ↑
Api & Worker
  ↑
AppHost
```
