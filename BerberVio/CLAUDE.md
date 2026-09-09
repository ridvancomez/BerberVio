
# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

BerberVio is an ASP.NET Core MVC web application (net6.0), currently at the default `dotnet new mvc` scaffold with no customizations applied yet — no database, no authentication, no tests. Expect to be building most features from scratch.

## Commands

​```
dotnet build                 # build the project
dotnet run                   # run the app (see launch URLs below)
dotnet watch run             # run with hot reload
​```
There is no test project yet; add one (e.g. `dotnet new xunit`) before writing tests.

Launch profiles (`Properties/launchSettings.json`) run the app at `https://localhost:7050` / `http://localhost:5270` in the `Development` environment.

## Architecture

Standard ASP.NET Core MVC layout, wired up in `Program.cs`:

- `Controllers/` — MVC controllers (currently just `HomeController`).
- `Models/` — view models / data models.
- `Views/` — Razor views, organized by controller name under `Views/<Controller>/`, with shared layout in `Views/Shared/_Layout.cshtml`.
- `wwwroot/` — static assets; third-party front-end libraries (Bootstrap, jQuery, jQuery Validation) are vendored under `wwwroot/lib/`.
- `appsettings.json` / `appsettings.Development.json` — configuration, loaded via the standard ASP.NET Core configuration pipeline.

Routing is conventional MVC routing (`{controller=Home}/{action=Index}/{id?}`), configured in `Program.cs` — new controllers/actions are picked up automatically without extra route registration unless attribute routing is introduced.

## Project plan (BerberVio)

- .NET 6.0 kullanılıyor
- Mimari: N-Tier — önce basit controller yapısı, sonra DAL/BL katmanları (Repository Pattern) eklenecek
- Admin panel Areas/Admin altında ayrı bir Area olacak
- Entity'ler: Customer, Employee, Service, Appointment, WorkingHours
- Web UI: Start Bootstrap "Small Business" teması kullanılacak
- Admin UI: Start Bootstrap "SB Admin" teması kullanılacak
- Veri erişimi önce EF Core ile direkt, ileride API üzerinden yapılacak
- SignalR entegrasyonu (anlık randevu bildirimi) proje sonuna bırakıldı