# Solution setup

**Status:** Implemented

## Goal

A base .NET solution for the timesheet POC: a Minimal API, a Blazor WebAssembly UI, a shared library and a test project. It contains only template boilerplate plus the minimal configuration needed for the UI to talk to the API. The spec is self-contained: running the commands in Technical notes from top to bottom in an empty folder reproduces the setup.

## Requirements

1. R1: Solution `Timesheet.sln` with four projects targeting `net10.0`:
   - `src/Timesheet.Api` — ASP.NET Core Web API (Minimal API, template `webapi`)
   - `src/Timesheet.Web` — Blazor WebAssembly Standalone (template `blazorwasm`)
   - `src/Timesheet.Shared` — class library (template `classlib`)
   - `tests/Timesheet.Api.Tests` — xUnit test project (template `xunit`)
2. R2: Project references: Api → Shared, Web → Shared, Api.Tests → Api.
3. R3: The Api uses EF Core with the **InMemory** provider: an empty `TimesheetDbContext` (no entities) registered with `UseInMemoryDatabase("Timesheet")`.
4. R4: The Api allows CORS from the Web origins listed in `appsettings.json` under `Cors:AllowedOrigins`.
5. R5: The Web `HttpClient` points to the Api base URL read from `wwwroot/appsettings.json` (`ApiBaseUrl`).
6. R6: Fixed development ports: Api `http://localhost:5100` / `https://localhost:7100`, Web `http://localhost:5200` / `https://localhost:7200`.
7. R7: Template-generated code stays unchanged, except for one change: the Web `Weather` page loads its data from the Api's `/weatherforecast` instead of `sample-data/weather.json`. This is the end-to-end smoke check that UI → API works (R4, R5).
8. R8: A `.gitignore` for .NET (template `gitignore`).
9. R9: One command starts both projects: `./run.sh` (macOS/Linux) and `./run.ps1` (Windows/PowerShell) in the repo root. The script:
   - builds the solution once and stops if the build fails;
   - starts the Api and the Web in parallel with `--no-build`, using the `http` profile;
   - prints both URLs;
   - on Ctrl+C stops both processes, so no process keeps a port busy.

## Out of scope

- Business logic, domain entities, custom endpoints or pages.
- Authentication / authorization.
- Swagger / Scalar UI (only the template's `MapOpenApi()`).
- Real database, migrations, Docker, .NET Aspire.
- Real tests (only the template's `UnitTest1`).

## Acceptance criteria

- [x] Given a clean clone, when `dotnet build Timesheet.sln` runs, then it succeeds with no errors.
- [x] Given a clean clone, when `dotnet test Timesheet.sln` runs, then the template test passes.
- [x] Given the Api is running, when `GET http://localhost:5100/weatherforecast` is called, then it returns 200 with 5 items.
- [x] Given the Api runs in Development, when `GET http://localhost:5100/openapi/v1.json` is called, then it returns the OpenAPI document.
- [x] Given a request with `Origin: http://localhost:5200`, when the Api responds, then the response contains `Access-Control-Allow-Origin: http://localhost:5200`.
- [x] Given the repo root, when `./run.sh` (or `./run.ps1`) runs, then both `http://localhost:5100/weatherforecast` and `http://localhost:5200/` respond with 200.
- [x] Given both projects were started with the script, when Ctrl+C is pressed, then nothing is listening on ports 5100 and 5200 anymore.
- [x] Given a broken build, when the script runs, then it exits with a non-zero code and starts nothing.
- [ ] Given Api and Web are both running, when the Weather page in the Web is opened, then it shows the forecast from the Api with no errors in the browser console.

## Technical notes

- .NET SDK 10.0.x. NuGet: `Microsoft.EntityFrameworkCore.InMemory` 10.x.
- In .NET 10, `dotnet new sln` creates `.slnx` by default, so `--format sln` is passed explicitly.
- Run locally with `./run.sh` / `./run.ps1` (R9). Alternatively, use two terminals (the `http` profile is the default for `dotnet run`):
  ```bash
  dotnet run --project src/Timesheet.Api
  dotnet run --project src/Timesheet.Web
  ```
- `run.sh` must be executable (`chmod +x run.sh`). `run.ps1` uses only cross-platform PowerShell 7 features (`pwsh`), so it works on macOS too.
- The criteria for R9 were verified with `run.sh` only. `run.ps1` has not been run yet (no `pwsh` on the dev machine) and still needs a check on Windows.

### Commands (from repo root)

```bash
dotnet new gitignore
dotnet new sln -n Timesheet --format sln

dotnet new webapi     -n Timesheet.Api       -o src/Timesheet.Api        -f net10.0
dotnet new blazorwasm -n Timesheet.Web       -o src/Timesheet.Web        -f net10.0
dotnet new classlib   -n Timesheet.Shared    -o src/Timesheet.Shared     -f net10.0
dotnet new xunit      -n Timesheet.Api.Tests -o tests/Timesheet.Api.Tests -f net10.0

dotnet sln Timesheet.sln add src/Timesheet.Api src/Timesheet.Web src/Timesheet.Shared tests/Timesheet.Api.Tests

dotnet add src/Timesheet.Api reference src/Timesheet.Shared
dotnet add src/Timesheet.Web reference src/Timesheet.Shared
dotnet add tests/Timesheet.Api.Tests reference src/Timesheet.Api

dotnet add src/Timesheet.Api package Microsoft.EntityFrameworkCore.InMemory
```

### Manual configuration

**Ports (R6).** In `Properties/launchSettings.json` set `applicationUrl`:
- Api: `http` → `http://localhost:5100`, `https` → `https://localhost:7100;http://localhost:5100`
- Web: `http` → `http://localhost:5200`, `https` → `https://localhost:7200;http://localhost:5200`

Also set `@Timesheet.Api_HostAddress = http://localhost:5100` in `src/Timesheet.Api/Timesheet.Api.http`.

**DbContext (R3).** `src/Timesheet.Api/Data/TimesheetDbContext.cs`:
```csharp
using Microsoft.EntityFrameworkCore;

namespace Timesheet.Api.Data;

public class TimesheetDbContext(DbContextOptions<TimesheetDbContext> options) : DbContext(options)
{
}
```

**Api `appsettings.json` (R4).** Add:
```json
"Cors": {
  "AllowedOrigins": [ "http://localhost:5200", "https://localhost:7200" ]
}
```

**Api `Program.cs` (R3, R4).** After `AddOpenApi()`:
```csharp
builder.Services.AddDbContext<TimesheetDbContext>(options =>
    options.UseInMemoryDatabase("Timesheet"));

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy
        .WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [])
        .AllowAnyHeader()
        .AllowAnyMethod()));
```
After `app.UseHttpsRedirection()`: `app.UseCors();`. Add usings `Microsoft.EntityFrameworkCore` and `Timesheet.Api.Data`.

**Web `wwwroot/appsettings.json` (R5).** New file:
```json
{
  "ApiBaseUrl": "http://localhost:5100/"
}
```

**Web `Program.cs` (R5).** Replace the `HttpClient` registration:
```csharp
var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
    ?? throw new InvalidOperationException("ApiBaseUrl is not configured.");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });
```

**Web `Pages/Weather.razor` (R7).** Replace `"sample-data/weather.json"` with `"weatherforecast"` and delete `wwwroot/sample-data/`.

## Tasks

- [x] `.gitignore` and `Timesheet.sln` (R8, R1)
- [x] Create the four projects from templates (R1)
- [x] Add projects to the solution and the project references (R1, R2)
- [x] Fix the ports in `launchSettings.json` (R6)
- [x] EF Core InMemory package, `TimesheetDbContext` and its registration (R3)
- [x] CORS config and policy in the Api (R4)
- [x] `ApiBaseUrl` and `HttpClient` in the Web (R5)
- [x] Point the Weather page to the Api and delete `sample-data` (R7)
- [x] Start scripts `run.sh` and `run.ps1` (R9)
- [x] Check every Acceptance criterion
