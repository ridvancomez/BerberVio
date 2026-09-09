
# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

BerberVio is a berber (barber shop) appointment management system, implemented as a .NET 6.0 N-Tier solution: a class-library stack (Entities → DataAccessLayer → Business) shared by a separate JWT-secured Web API project and an ASP.NET Core MVC web app. The MVC app hosts its own end-user pages plus a `Cms` Area for admin management, and talks to the API over HTTP rather than using EF Core directly for feature data.

The solution has 5 projects (`BerberVio.sln`):

| Project | Type | Role |
|---|---|---|
| `BerberVio.Entities` | class library | POCOs + Identity entities (`AppUser`, `AppRole`) |
| `BerberVio.DataAccessLayer` | class library | `AppDbContext`, EF Core migrations, Repository pattern (`IGenericRepository<T>` + per-entity repositories) |
| `BerberVio.Business` | class library | Service layer (`IGenericService<T>` + per-entity services) wrapping the repositories |
| `BerberVio.API` | ASP.NET Core Web API | JWT-bearer secured REST API over the Business layer; Swagger UI |
| `BerberVio` (root) | ASP.NET Core MVC | End-user site + `Areas/Cms` admin area; cookie-based Identity auth for its own session, and a JWT (obtained from the API) for calling the API |

There is no test project yet.

## Commands

```
dotnet build BerberVio.sln       # build everything
dotnet run --project BerberVio.API      # run the API (Swagger at /swagger)
dotnet run --project BerberVio           # run the MVC web app
dotnet watch run --project BerberVio     # web app with hot reload
```

The web app depends on the API being up for login and for any Cms Area data operations (e.g. Employee CRUD) — run both projects together during development.

Launch profiles (`Properties/launchSettings.json`):
- Web app: `https://localhost:7050` / `http://localhost:5270`
- API (`BerberVio.API/Properties/launchSettings.json`): `https://localhost:7100` / `http://localhost:5187`, opens Swagger UI on launch

EF Core migrations live in `BerberVio.DataAccessLayer/Migrations` and target `AppDbContext`. Run `dotnet ef database update` from a project that references both `BerberVio.DataAccessLayer` and has the connection string available (e.g. `BerberVio.API`), or `dotnet ef migrations add <Name> -p BerberVio.DataAccessLayer -s BerberVio.API` to add a new one.

## Architecture

### Data layer (`BerberVio.Entities` → `BerberVio.DataAccessLayer` → `BerberVio.Business`)

- Entities implemented so far: `Employee` (Id, Name, PhoneNumber, ImagePath, Appointments), `Service` (Id, Name, Price — `decimal(18,2)`), `Appointment` (Id, AppointmentDate, EmployeeId/Employee, ServiceId/Service — both FKs `DeleteBehavior.Restrict`). `Customer` and `WorkingHours` from the original plan are **not yet implemented**.
- Identity: `AppUser : IdentityUser` (+ `Name`, `Surname`), `AppRole : IdentityRole`. `AppDbContext : IdentityDbContext<AppUser, AppRole, string>` (`BerberVio.DataAccessLayer/Context/AppDbContext.cs`) — feature tables and ASP.NET Identity tables live in the same database/context.
- Repository pattern: `IGenericRepository<T>` plus `IEmployeeRepository`/`IServiceRepository`/`IAppointmentRepository`, implemented in `BerberVio.DataAccessLayer/Repositories/`.
- Service layer: `IGenericService<T>` plus `IEmployeeService`/`IServiceService`/`IAppointmentService`, implemented in `BerberVio.Business/Services/`, injected via DI in both `BerberVio.API` and the main web app.
- Migrations applied so far: `InitialCreate` (Employee/Service/Appointment tables), `AddEmployeeImagePath`, `AddIdentityTables` (adds ASP.NET Identity schema).

### `BerberVio.API`

- `[Authorize]` + JWT bearer auth (`Program.cs`); issues tokens from `Controllers/AuthController.cs` (login/register against Identity, signs a JWT using the `Jwt` config section — `Jwt:Key` is stored in User Secrets, not in `appsettings.json`).
- REST controllers: `AuthController`, `EmployeeController` (`api/employee`), `ServiceController`, `AppointmentController`, `UserController` (`api/user`, manages `AppUser` via `UserManager`) — standard CRUD over the Business layer.
- Swagger/OpenAPI enabled in Development with a Bearer security scheme; CORS is wide open (`AllowAll` policy) for the web app to call it cross-origin/port.

### `BerberVio` (main MVC web app)

- Uses ASP.NET Core Identity **directly** (own `AppDbContext` + `SignInManager`/`UserManager`) for its own cookie-based login session — see `Controllers/AccountController.cs` (`Login`/`Register`/`Logout`).
- On successful login, `AccountController` *also* calls `POST api/auth/login` on the API and stores the returned JWT in an `HttpOnly` cookie named `jwt`.
- `Services/ApiClient.cs` is a thin `HttpClient` wrapper (`GetAsync`/`PostAsync`/`PutAsync`/`DeleteAsync`) that reads that `jwt` cookie and attaches it as a `Bearer` token on every call to the API. This is how Cms Area controllers fetch/mutate data — they do not go through the Business/DataAccessLayer projects directly even though the web app has those DLLs loaded.
- `Models/` holds view models: `LoginViewModel`, `RegisterViewModel`, `EmployeeViewModel`, `ApiAuthResponse` (deserializes the API's JWT response).
- `Views/` — Razor views by controller, shared layout in `Views/Shared/_Layout.cshtml`. Home/Account pages use the vendored "Small Business" Start Bootstrap theme (`wwwroot/css/styles.css`; source assets under `wwwroot/design-source/small-business/`).
- `wwwroot/lib/` — vendored Bootstrap, jQuery, jQuery Validation (default scaffold libs).
- `wwwroot/uploads/employees/` — employee photos uploaded via the Cms Employee form.

### `Areas/Cms` (admin area)

- Named `Cms`, **not** `Admin` as originally planned. `[Area("Cms")]`; `BaseController` applies `[Authorize]` to every controller in the area.
- `DashboardController` — placeholder `Index` view only.
- `EmployeeController` — full CRUD (Index/Create/Edit/Delete) against `api/employee` via `ApiClient`, plus local image upload (saves to `wwwroot/uploads/employees/`, path stored on the `Employee` entity).
- Layout (`Areas/Cms/Views/Shared/_CmsLayout.cshtml`) follows the "SB Admin" Start Bootstrap theme (design source under `wwwroot/design-source/`); `Service`/`Appointment` do not have Cms UI yet even though their API/Business/DAL layers exist.

Routing is conventional MVC routing with an area-aware pattern registered first in `Program.cs`:
```
{area:exists}/{controller=Home}/{action=Index}/{id?}
{controller=Home}/{action=Index}/{id?}
```

### Configuration

- `appsettings.json` (both web app and API) — `ConnectionStrings:BerberVioDb` (SQL Server, `Trusted_Connection=True`), `ApiSettings:BaseUrl` (web app → API).
- `BerberVio.API/appsettings.json` also has a `Jwt` section (`Issuer`, `Audience`, `ExpireMinutes`); the signing `Jwt:Key` itself is kept in .NET User Secrets (`UserSecretsId` in `BerberVio.API.csproj`), not committed.

## Project plan (BerberVio) — original vs. current

- .NET 6.0 kullanılıyor ✅
- Mimari: N-Tier (Entities/DataAccessLayer/Business + ayrı API) ✅ — planlanandan bir adım ötede: web app EF Core'u doğrudan kullanmak yerine API üzerinden veri çekiyor (bkz. `ApiClient`), sadece kendi Identity oturumu için EF Core'u doğrudan kullanıyor.
- Admin panel: planda `Areas/Admin` deniyordu, gerçekte `Areas/Cms` olarak uygulandı.
- Entity'ler: `Employee`, `Service`, `Appointment` tamam; `Customer` ve `WorkingHours` **henüz yok**. Identity (`AppUser`/`AppRole`) plana ek olarak eklendi.
- Web UI: Start Bootstrap "Small Business" teması ana sitede kullanılıyor ✅
- Admin UI: Start Bootstrap "SB Admin" teması Cms Area'da kullanılıyor ✅ (şu an sadece Dashboard placeholder + Employee CRUD var; Service/Appointment CRUD ekranları yok)
- Veri erişimi: planda "önce EF Core direkt, sonra API" deniyordu; gerçekte API en baştan var ve web app Employee verisini API üzerinden çekiyor (JWT ile), sadece Identity oturumu EF Core direkt.
- SignalR entegrasyonu (anlık randevu bildirimi): henüz yok, planlandığı gibi sona bırakıldı.
