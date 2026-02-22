<div dir="rtl" align="right">

# GeneratorCode - مولد الكود الذكي

<div align="left">

[English](#generatorcode---intelligent-code-generator) | [العربية](#generatorcode---مولد-الكود-الذكي)

</div>

---

## نظرة عامة

**GeneratorCode** هو تطبيق سطح مكتب مبني بـ **.NET 7 Windows Forms** لتوليد مشاريع C# كاملة من قواعد البيانات أو من تصميم نماذج المجال. يدعم وضعين أساسيين: **Database First** (من قاعدة بيانات موجودة) و **Code First** (تصميم الكيانات وتوليد مشروع EF Core مع إدارة الترحيلات). يوفر واجهة رسومية (GUI) وواجهة سطر أوامر (CLI).

## الميزات الرئيسية

### أوضاع التوليد

| الوضع | الوصف |
|-------|-------|
| **Database First** | الاتصال بقاعدة بيانات موجودة، اكتشاف الجداول والأعمدة والمفاتيح، ثم توليد مشروع كامل |
| **Code First** | تصميم نموذج المجال (كيانات، خصائص، علاقات) عبر مصمم الكيانات، ثم توليد مشروع EF Core مع إدارة الترحيلات |

### أنماط العمارة المدعومة

| النمط | الحالة |
|-------|--------|
| **Clean Architecture** | مكتمل |
| **Simple Architecture** | مكتمل |
| **Layered Architecture** | قيد التطوير |
| **CQRS Pattern** | قيد التطوير |
| **Domain-Driven Design (DDD)** | قيد التطوير |
| **Microservices Architecture** | قيد التطوير |

### قواعد البيانات المدعومة

- **SQL Server** - دعم كامل (جداول، أعمدة، مفاتيح، فهارس، triggers)
- **PostgreSQL** - دعم كامل
- **MySQL / MariaDB** - دعم كامل
- **Oracle** - دعم كامل
- **SQLite** - دعم كامل

### المكونات المولدة

- **Entities / Domain Models** - نماذج المجال
- **DTOs** - كائنات نقل البيانات
- **Repositories** - مستودعات البيانات (مع Generic Repository)
- **Services** - طبقة الخدمات
- **Controllers** - وحدات التحكم (ASP.NET Core API)
- **Validators** - التحقق من البيانات (FluentValidation)
- **Mappings** - AutoMapper Profiles
- **Unit Tests** - اختبارات الوحدة
- **Integration Tests** - اختبارات التكامل
- **Dependency Injection** - تسجيل الخدمات (Microsoft DI / Autofac)
- **ASP.NET Core Views** - صفحات Razor
- **TypeScript Models** - نماذج TypeScript

### ملفات المشروع المولدة

- ملف الحل `.sln`
- ملفات المشاريع `.csproj` مع حزم NuGet
- `Program.cs` / `Startup.cs`
- `appsettings.json`
- `.gitignore`
- `README.md`

## واجهات التطبيق

### الشاشات الرئيسية

| الشاشة | الوظيفة |
|--------|---------|
| **شاشة الاتصال** | اختيار نوع قاعدة البيانات، إدخال بيانات الاتصال، اختبار الاتصال، تحميل قواعد البيانات المتاحة، حفظ بيانات الاتصال مشفرة |
| **شاشة الجداول** | عرض الجداول والأعمدة، اختيار نمط العمارة واللغة، تحديد المكونات والطبقات المراد توليدها، خيارات DI/Async/Tests/Swagger/CRUD |
| **مصمم الكيانات** | تصميم نموذج المجال: إضافة/حذف/تعديل كيانات وخصائصها وعلاقاتها، حفظ/تحميل/تصدير/استيراد JSON، معاينة كود C# |
| **مدير الترحيلات** | توليد كود EF Core، إضافة ترحيل، تحديث قاعدة البيانات، التراجع، توليد سكريبت SQL، إزالة آخر ترحيل |
| **شاشة المعاينة** | عرض الكود المولد مع تلوين بناء الجملة، نسخ إلى الحافظة، حفظ كملف |
| **شاشة التقدم** | عرض تقدم عملية التوليد مع سجل تفصيلي وشريط تقدم |
| **شاشة الإعدادات** | مساحة الأسماء الافتراضية، مسار الحفظ، إعدادات قاعدة البيانات الافتراضية (مشفرة)، تصدير/استيراد الإعدادات JSON |
| **عارض السجلات** | عرض وتصفية السجلات حسب المستوى/التاريخ/المصدر/البحث، تصدير JSON/CSV/TXT |

### اللغات المدعومة للتوليد

- C#
- ASP.NET Web Forms
- ASP.NET MVC
- ASP.NET Core
- TypeScript

## المتطلبات

- **.NET 7.0** أو أحدث
- **Windows OS** (Windows Forms)
- قاعدة بيانات واحدة على الأقل (SQL Server / MySQL / PostgreSQL / Oracle / SQLite)

## التثبيت والتشغيل

```bash
git clone https://github.com/AbdulqaderMaofaa/GeneratorCode.git
cd GeneratorCode
dotnet build
dotnet run
```

## الاستخدام

### واجهة المستخدم الرسومية (GUI)

#### Database First
1. اختر نوع قاعدة البيانات وأدخل بيانات الاتصال
2. اختبر الاتصال واختر قاعدة البيانات
3. اضغط "Connect" لفتح شاشة الجداول
4. اختر نمط العمارة واللغة والمكونات المطلوبة
5. حدد الجداول المراد توليد الكود لها
6. اضغط "Generate" لبدء التوليد

#### Code First
1. من شاشة الاتصال اختر "Entity Designer"
2. صمم الكيانات والخصائص والعلاقات
3. احفظ النموذج أو صدّره كـ JSON
4. اضغط "Generate" لفتح مدير الترحيلات
5. أنشئ المشروع وأضف الترحيلات وحدّث قاعدة البيانات

### واجهة سطر الأوامر (CLI)

```bash
GeneratorCode.exe --server localhost --database MyDatabase --output C:\Output
```

**المعاملات:**

| المعامل | الوصف | مطلوب |
|---------|-------|-------|
| `--server` | اسم السيرفر | نعم |
| `--database` | اسم قاعدة البيانات | نعم |
| `--output` | مسار حفظ الملفات | نعم |
| `--db-type` | نوع قاعدة البيانات (SqlServer, MySQL, PostgreSQL) | لا |
| `--namespace` | مساحة الأسماء | لا |
| `--pattern` | نمط العمارة (CleanArchitecture, Simple, CQRS, DDD, etc.) | لا |
| `--enable-di` | تفعيل Dependency Injection | لا |
| `--async` | توليد عمليات غير متزامنة | لا |
| `--tests` | توليد اختبارات وحدة | لا |

### أمثلة CLI

```bash
# Clean Architecture مع DI و Async
GeneratorCode.exe --server localhost --database Northwind --pattern CleanArchitecture --namespace NorthwindApp --output C:\GeneratedCode --enable-di true --async true

# MySQL مع CQRS واختبارات
GeneratorCode.exe --server localhost --database ShopDB --db-type MySQL --pattern CQRS --namespace ShopApp --output C:\ShopCode --tests true
```

## هيكل المشروع المولد

```
GeneratedProject/
├── src/
│   ├── ProjectName.Domain/              # الكيانات، Value Objects، واجهات المستودعات
│   ├── ProjectName.Application/         # DTOs، خدمات التطبيق، Validators، Mappings
│   ├── ProjectName.Infrastructure/      # تنفيذ المستودعات، DbContext، الإعدادات
│   └── ProjectName.API/                 # Controllers، Middleware، Program.cs، Swagger
├── tests/
│   ├── ProjectName.UnitTests/           # اختبارات الوحدة
│   └── ProjectName.IntegrationTests/    # اختبارات التكامل
├── ProjectName.sln
├── appsettings.json
├── .gitignore
└── README.md
```

## هيكل الكود المصدري

```
GeneratorCode/
├── GeneratorCode.sln
└── GeneratorCode/
    ├── GeneratorCode.csproj
    ├── Program.cs
    ├── CLI/
    │   └── CommandLineInterface.cs          # معالجة سطر الأوامر
    ├── Core/
    │   ├── ArchitecturePatterns/             # أنماط العمارة (Clean, Simple, Layered, CQRS, DDD, Microservices)
    │   ├── CodeFirst/                       # EfCoreCodeFirstGenerator، TypeMappingService
    │   ├── CodeGenerators/                  # مولدات ASP.NET Core Views و TypeScript
    │   ├── DatabaseProviders/               # موفرو قواعد البيانات (SqlServer, MySQL, PostgreSql, SQLite, Oracle)
    │   ├── DependencyInjection/             # DIIntegrationService، PackagesGenerator
    │   ├── DomainModel/                     # DomainEntity، DomainProperty، DomainRelation
    │   ├── Factories/                       # مصانع أنماط العمارة وموفري قواعد البيانات
    │   ├── Helpers/                         # ConnectionStringBuilder، PasswordEncryption، ProcessRunner
    │   ├── Interfaces/                      # IDatabaseProvider، IArchitecturePattern، IMigrationService
    │   ├── Logging/                         # LoggerFactory، ExceptionMiddleware
    │   ├── Models/                          # CodeGenerationContext، TableInfo، ColumnInfo
    │   ├── Services/                        # CodeGenerationService، DomainModelService، EfCoreMigrationService
    │   └── TemplateEngine/                  # محرك القوالب
    └── GeneratorCode/
        ├── Forms/                           # شاشات التطبيق (Connection, Tables, Preview, Settings, EntityDesigner, etc.)
        └── Helpers/                         # AppTheme، DatabaseHelper، LogViewerHelper
```

## الحزم المستخدمة

| الحزمة | الاستخدام |
|--------|----------|
| MaterialSkin.2 | واجهة مستخدم حديثة (Material Design) |
| Microsoft.Data.SqlClient | الاتصال بـ SQL Server |
| Npgsql | الاتصال بـ PostgreSQL |
| MySql.Data | الاتصال بـ MySQL |
| Microsoft.Data.Sqlite | الاتصال بـ SQLite |
| Oracle.ManagedDataAccess.Core | الاتصال بـ Oracle |
| System.CommandLine | واجهة سطر الأوامر |
| System.Security.Cryptography.ProtectedData | تشفير كلمات المرور |

## نظام التسجيل (Logging)

- **المستويات**: Debug, Info, Warning, Error
- **التنسيق**: JSON منظم مع Correlation ID
- **واجهة العرض**: تصفية حسب المستوى/التاريخ/المصدر/النص
- **التصدير**: JSON, CSV, TXT
- **تفاصيل الأخطاء**: عرض Stack Trace و Inner Exceptions و بيانات إضافية

## الأمان

- تشفير كلمات المرور المحفوظة باستخدام DPAPI
- عدم تخزين بيانات الاتصال بنص واضح
- تصدير/استيراد الإعدادات بدون كلمات المرور

## المساهمة

1. Fork المشروع
2. إنشاء branch للميزة (`git checkout -b feature/AmazingFeature`)
3. Commit التغييرات (`git commit -m 'Add some AmazingFeature'`)
4. Push للـ branch (`git push origin feature/AmazingFeature`)
5. فتح Pull Request

## الترخيص

هذا المشروع مرخص تحت [MIT License](LICENSE)

## المؤلف

- **عبدالقادر موفعة** - [GitHub](https://github.com/AbdulqaderMaofaa/)

## التواصل

- **Issues**: [GitHub Issues](https://github.com/AbdulqaderMaofaa/GeneratorCode/issues)
- **Email**: maofaacom@gmail.com

</div>

---

<div dir="ltr" align="left">

# GeneratorCode - Intelligent Code Generator

<div align="right">

[العربية](#generatorcode---مولد-الكود-الذكي) | [English](#generatorcode---intelligent-code-generator)

</div>

---

## Overview

**GeneratorCode** is a **.NET 7 Windows Forms** desktop application that generates complete C# projects from existing databases or from hand-designed domain models. It supports two primary modes: **Database First** (reverse-engineer an existing database) and **Code First** (design entities in a visual designer and generate an EF Core project with migration management). Both a GUI and a CLI are provided.

## Key Features

### Generation Modes

| Mode | Description |
|------|-------------|
| **Database First** | Connect to an existing database, discover tables/columns/keys, and generate a full project |
| **Code First** | Design a domain model (entities, properties, relations) in the Entity Designer, then generate an EF Core project with migration management |

### Supported Architecture Patterns

| Pattern | Status |
|---------|--------|
| **Clean Architecture** | Complete |
| **Simple Architecture** | Complete |
| **Layered Architecture** | In Development |
| **CQRS Pattern** | In Development |
| **Domain-Driven Design (DDD)** | In Development |
| **Microservices Architecture** | In Development |

### Supported Databases

- **SQL Server** - Full support (tables, columns, keys, indexes, triggers)
- **PostgreSQL** - Full support
- **MySQL / MariaDB** - Full support
- **Oracle** - Full support
- **SQLite** - Full support

### Generated Components

- **Entities / Domain Models**
- **DTOs** - Data Transfer Objects
- **Repositories** - with Generic Repository pattern
- **Services** - Service layer
- **Controllers** - ASP.NET Core API controllers
- **Validators** - FluentValidation
- **Mappings** - AutoMapper Profiles
- **Unit Tests**
- **Integration Tests**
- **Dependency Injection** - Service registration (Microsoft DI / Autofac)
- **ASP.NET Core Views** - Razor pages
- **TypeScript Models**

### Generated Project Files

- `.sln` solution file
- `.csproj` project files with NuGet packages
- `Program.cs` / `Startup.cs`
- `appsettings.json`
- `.gitignore`
- `README.md`

## Application Screens

| Screen | Purpose |
|--------|---------|
| **Connection** | Select database type, enter credentials, test connection, load available databases, save encrypted credentials |
| **Tables** | Browse tables and columns, select architecture pattern and language, choose components and layers to generate, DI/Async/Tests/Swagger/CRUD options |
| **Entity Designer** | Design domain model: add/remove/edit entities, properties (type, required, PK, identity, max length, default, description), and relations (target, type, FK, delete behavior, navigation); save/load/export/import JSON; preview C# code |
| **Migration Manager** | Generate EF Core code, add migration, update database, rollback, generate SQL script, remove last migration, list migrations |
| **Preview** | View generated code with syntax highlighting; copy to clipboard or save as file |
| **Progress** | Real-time generation progress with detailed log and progress bar |
| **Settings** | Default namespace, output path, default database credentials (encrypted), export/import settings as JSON |
| **Log Viewer** | View and filter logs by level/date/source/search; export to JSON/CSV/TXT |

### Supported Generation Languages

- C#
- ASP.NET Web Forms
- ASP.NET MVC
- ASP.NET Core
- TypeScript

## Requirements

- **.NET 7.0** or later
- **Windows OS** (Windows Forms)
- At least one database (SQL Server / MySQL / PostgreSQL / Oracle / SQLite)

## Installation

```bash
git clone https://github.com/AbdulqaderMaofaa/GeneratorCode.git
cd GeneratorCode
dotnet build
dotnet run
```

## Usage

### GUI

#### Database First
1. Select the database type and enter connection credentials
2. Test the connection and select a database
3. Click "Connect" to open the Tables screen
4. Choose architecture pattern, language, and desired components
5. Select the tables to generate code for
6. Click "Generate" to start generation

#### Code First
1. From the Connection screen, choose "Entity Designer"
2. Design entities, properties, and relations
3. Save the model or export it as JSON
4. Click "Generate" to open the Migration Manager
5. Generate the project, add migrations, and update the database

### CLI

```bash
GeneratorCode.exe --server localhost --database MyDatabase --output C:\Output
```

**Parameters:**

| Parameter | Description | Required |
|-----------|-------------|----------|
| `--server` | Server name | Yes |
| `--database` | Database name | Yes |
| `--output` | Output path | Yes |
| `--db-type` | Database type (SqlServer, MySQL, PostgreSQL) | No |
| `--namespace` | Namespace for generated code | No |
| `--pattern` | Architecture pattern (CleanArchitecture, Simple, CQRS, DDD, etc.) | No |
| `--enable-di` | Enable Dependency Injection | No |
| `--async` | Generate async operations | No |
| `--tests` | Generate unit tests | No |

### CLI Examples

```bash
# Clean Architecture with DI and Async
GeneratorCode.exe --server localhost --database Northwind --pattern CleanArchitecture --namespace NorthwindApp --output C:\GeneratedCode --enable-di true --async true

# MySQL with CQRS and Tests
GeneratorCode.exe --server localhost --database ShopDB --db-type MySQL --pattern CQRS --namespace ShopApp --output C:\ShopCode --tests true
```

## Generated Project Structure

```
GeneratedProject/
├── src/
│   ├── ProjectName.Domain/              # Entities, Value Objects, Repository interfaces
│   ├── ProjectName.Application/         # DTOs, Application services, Validators, Mappings
│   ├── ProjectName.Infrastructure/      # Repository implementations, DbContext, Configuration
│   └── ProjectName.API/                 # Controllers, Middleware, Program.cs, Swagger
├── tests/
│   ├── ProjectName.UnitTests/
│   └── ProjectName.IntegrationTests/
├── ProjectName.sln
├── appsettings.json
├── .gitignore
└── README.md
```

## Source Code Structure

```
GeneratorCode/
├── GeneratorCode.sln
└── GeneratorCode/
    ├── GeneratorCode.csproj
    ├── Program.cs
    ├── CLI/
    │   └── CommandLineInterface.cs          # CLI argument handling
    ├── Core/
    │   ├── ArchitecturePatterns/             # Clean, Simple, Layered, CQRS, DDD, Microservices
    │   ├── CodeFirst/                       # EfCoreCodeFirstGenerator, TypeMappingService
    │   ├── CodeGenerators/                  # ASP.NET Core Views, TypeScript generators
    │   ├── DatabaseProviders/               # SqlServer, MySQL, PostgreSql, SQLite, Oracle
    │   ├── DependencyInjection/             # DIIntegrationService, PackagesGenerator
    │   ├── DomainModel/                     # DomainEntity, DomainProperty, DomainRelation
    │   ├── Factories/                       # ArchitecturePatternFactory, DatabaseProviderFactory
    │   ├── Helpers/                         # ConnectionStringBuilder, PasswordEncryption, ProcessRunner
    │   ├── Interfaces/                      # IDatabaseProvider, IArchitecturePattern, IMigrationService
    │   ├── Logging/                         # LoggerFactory, ExceptionMiddleware
    │   ├── Models/                          # CodeGenerationContext, TableInfo, ColumnInfo
    │   ├── Services/                        # CodeGenerationService, DomainModelService, EfCoreMigrationService
    │   └── TemplateEngine/                  # Template engine for code generation
    └── GeneratorCode/
        ├── Forms/                           # UI forms (Connection, Tables, Preview, Settings, EntityDesigner, etc.)
        └── Helpers/                         # AppTheme, DatabaseHelper, LogViewerHelper
```

## NuGet Packages

| Package | Purpose |
|---------|---------|
| MaterialSkin.2 | Material Design UI |
| Microsoft.Data.SqlClient | SQL Server connectivity |
| Npgsql | PostgreSQL connectivity |
| MySql.Data | MySQL connectivity |
| Microsoft.Data.Sqlite | SQLite connectivity |
| Oracle.ManagedDataAccess.Core | Oracle connectivity |
| System.CommandLine | CLI interface |
| System.Security.Cryptography.ProtectedData | Password encryption |

## Logging

- **Levels**: Debug, Info, Warning, Error
- **Format**: Structured JSON with Correlation ID
- **Viewer**: Filter by level/date/source/text
- **Export**: JSON, CSV, TXT
- **Error Details**: Stack trace, inner exceptions, additional data

## Security

- Saved passwords encrypted with DPAPI
- No plaintext credential storage
- Settings export/import excludes passwords

## Contributing

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the [MIT License](LICENSE)

## Author

- **Abdulqader Maofaa** - [GitHub](https://github.com/AbdulqaderMaofaa/)

## Contact

- **Issues**: [GitHub Issues](https://github.com/AbdulqaderMaofaa/GeneratorCode/issues)
- **Email**: maofaacom@gmail.com

</div>
