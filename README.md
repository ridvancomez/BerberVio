# BerberVio

Kuaför ve güzellik salonları için randevu yönetim sistemi. ASP.NET Core ile N-Tier mimari kullanılarak geliştirildi; amaç hem gerçek bir salon randevu akışını (müşteri, çalışan, hizmet, çalışma saatleri) uçtan uca çözmek hem de backend mimarisini (Repository Pattern, katmanlı yapı) portfolyo projesi olarak göstermek.

## Teknoloji Stack

- ASP.NET Core (.NET 6)
- Entity Framework Core
- MSSQL
- Repository Pattern
- Bootstrap tabanlı admin paneli (ayrı bir Area olarak)

## Proje Yapısı

```
BerberVio/
├── BerberVio.Entities/          # Domain modelleri (Customer, Employee, Service, Appointment, WorkingHours)
├── BerberVio.DataAccessLayer/   # DbContext, Repository implementasyonları, migrations
├── BerberVio.Business/          # Servisler, iş kuralları
├── BerberVio.API/                # API katmanı
└── BerberVio/                    # Web (Controllers, Views, Areas/Admin, wwwroot)
```

## Mimari Kararlar

- **N-Tier Architecture**: Entities, DataAccess, Business ve Web katmanları birbirinden ayrıldı.
- **Repository Pattern**: Veri erişimi repository arayüzleri üzerinden soyutlandı.
- **Admin Paneli**: Randevu/çalışan/hizmet yönetimi ayrı bir ASP.NET Core Area olarak tutuluyor.

## Domain Modeli (Özet)

- **Customer** — müşteri bilgileri
- **Employee** — çalışan/berber bilgileri
- **Service** — sunulan hizmetler (saç kesimi, sakal tıraşı vb.)
- **Appointment** — randevu kaydı (müşteri, çalışan, hizmet, tarih/saat)
- **WorkingHours** — çalışan bazlı haftalık çalışma saatleri

## Kurulum

```bash
git clone https://github.com/ridvancomez/BerberVio.git
cd BerberVio
dotnet restore
dotnet ef database update --project BerberVio.DataAccessLayer --startup-project BerberVio
dotnet run --project BerberVio
```

## Durum

🚧 Aktif geliştirme aşamasında.
