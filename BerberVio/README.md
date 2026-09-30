# BerberVio

ASP.NET Core ile geliştirilmiş kuaför randevu yönetim sistemi.

---

## Özellikler

- Müşteri randevu alma ve yönetimi
- Çalışan ve hizmet yönetimi
- Rol bazlı yetkilendirme (Admin / Müşteri)
- JWT Authentication
- Ayrı API katmanı (Swagger UI ile)

---

## Mimari

N-Tier Architecture — katmanlar birbirinden bağımsız:

```
BerberVio/
├── BerberVio              # MVC — UI katmanı (Views, Controllers)
├── BerberVio.API          # REST API (JWT auth, Swagger)
├── BerberVio.Business     # İş mantığı (Interfaces + Services)
├── BerberVio.DataAccessLayer  # Veri erişimi (Repository Pattern, Migrations)
└── BerberVio.Entities     # Entity sınıfları
```

---

## Teknoloji

| | |
|---|---|
| Framework | ASP.NET Core |
| ORM | Entity Framework Core (Code First) |
| Veritabanı | MSSQL |
| Auth | ASP.NET Core Identity + JWT Bearer |
| API Dok. | Swagger / OpenAPI |
| Mimari | N-Tier, Repository Pattern, DI |

---

## Kurulum

**Gereksinimler:** .NET SDK, MSSQL Server

```bash
git clone https://github.com/ridvancomez/BerberVio.git
cd BerberVio/BerberVio
```

`appsettings.json` dosyasındaki connection string'i güncelle:

```json
"ConnectionStrings": {
  "BerberVioDb": "Server=.;Database=BerberVioDB;Trusted_Connection=True;"
}
```

Migration uygula ve çalıştır:

```bash
dotnet ef database update
dotnet run
```

API için `BerberVio.API` projesini çalıştır:

```bash
cd BerberVio.API
dotnet run
# Swagger: https://localhost:{port}/swagger
```
