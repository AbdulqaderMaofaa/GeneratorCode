<div dir="rtl" align="right">

# 🚀 GeneratorCode - مولد الكود الذكي

<div align="left">

[English](#-generatorcode---intelligent-code-generator) | [العربية](#-generatorcode---مولد-الكود-الذكي)

</div>

---

## 📋 نظرة عامة

**GeneratorCode** هو تطبيق سطح مكتب متقدم لتوليد الكود البرمجي ديناميكياً بناءً على قواعد البيانات وأنماط العمارة البرمجية المختلفة. يساعد المطورين على إنشاء هياكل مشاريع متكاملة وفق أفضل الممارسات المعمارية في دقائق معدودة.

## ✨ الميزات الرئيسية

### 🏗️ أنماط العمارة المدعومة
- **Clean Architecture** - عمارة نظيفة مع فصل الطبقات
- **Layered Architecture** - عمارة متعددة الطبقات
- **CQRS Pattern** - فصل الأوامر والاستعلامات
- **Domain-Driven Design (DDD)** - التصميم الموجه بالمجال
- **Microservices Architecture** - عمارة الخدمات الصغيرة
- **Simple Architecture** - عمارة بسيطة للمشاريع الصغيرة

### 🗄️ قواعد البيانات المدعومة
- **SQL Server** - دعم كامل لـ Microsoft SQL Server
- **MySQL** - دعم لـ MySQL و MariaDB
- **PostgreSQL** - دعم لـ PostgreSQL

### 🔧 المكونات المولدة
- ✅ **Entities/Domain Models** - نماذج المجال
- ✅ **DTOs** - كائنات نقل البيانات
- ✅ **Repositories** - مستودعات البيانات
- ✅ **Services** - طبقة الخدمات
- ✅ **Controllers** - وحدات التحكم (API)
- ✅ **Validators** - أدوات التحقق من البيانات
- ✅ **Mappings** - AutoMapper Profiles
- ✅ **Unit Tests** - اختبارات الوحدة
- ✅ **Integration Tests** - اختبارات التكامل

### 🎯 ميزات إضافية
- 🔐 **Dependency Injection** - دعم Microsoft DI و Autofac
- 📝 **Logging System** - نظام تسجيل شامل مع واجهة عرض
- 🔍 **Preview Mode** - معاينة الكود قبل التوليد
- 🎨 **GUI & CLI** - واجهة رسومية وسطر أوامر
- 🔄 **Async/Await Support** - دعم العمليات غير المتزامنة
- 📊 **Swagger Integration** - تكامل مع Swagger/OpenAPI
- 🛡️ **Input Validation** - التحقق من المدخلات

## 📦 متطلبات التشغيل

- **.NET 7.0** أو أحدث
- **Windows OS** (Windows Forms)
- **قاعدة بيانات** (SQL Server / MySQL / PostgreSQL)

## 🚀 البدء السريع

### التثبيت

1. استنسخ المستودع:
```bash
git clone https://github.com/yourusername/GeneratorCode.git
cd GeneratorCode
```

2. افتح المشروع في Visual Studio أو Rider

3. قم ببناء المشروع:
```bash
dotnet build
```

4. شغل التطبيق:
```bash
dotnet run
```

### الاستخدام الأساسي

#### واجهة المستخدم الرسومية (GUI)

1. افتح التطبيق
2. أدخل معلومات الاتصال بقاعدة البيانات
3. اختر نمط العمارة المطلوب
4. حدد الجداول المراد توليد الكود لها
5. اضغط "Generate" وانتظر النتيجة

#### واجهة سطر الأوامر (CLI)

```bash
GeneratorCode.exe --server localhost --database MyDatabase --output C:\Output
```

**معاملات CLI:**
- `--server`: اسم السيرفر
- `--database`: اسم قاعدة البيانات
- `--db-type`: نوع قاعدة البيانات (SqlServer, MySQL, PostgreSQL)
- `--namespace`: مساحة الأسماء للكود المولد
- `--pattern`: نمط العمارة (CleanArchitecture, CQRS, DDD, etc.)
- `--output`: مسار حفظ الملفات
- `--enable-di`: تفعيل Dependency Injection
- `--async`: توليد عمليات غير متزامنة
- `--tests`: توليد اختبارات وحدة

## 📖 أمثلة الاستخدام

### مثال 1: توليد كود Clean Architecture

```bash
GeneratorCode.exe \
  --server localhost \
  --database Northwind \
  --pattern CleanArchitecture \
  --namespace NorthwindApp \
  --output C:\GeneratedCode \
  --enable-di true \
  --async true
```

### مثال 2: توليد كود CQRS مع اختبارات

```bash
GeneratorCode.exe \
  --server localhost \
  --database ShopDB \
  --pattern CQRS \
  --namespace ShopApp \
  --output C:\ShopCode \
  --tests true
```

## 🏛️ هيكل المشروع المولد

```
GeneratedProject/
├── src/
│   ├── ProjectName.Domain/          # طبقة المجال
│   ├── ProjectName.Application/     # طبقة التطبيق
│   ├── ProjectName.Infrastructure/  # طبقة البنية التحتية
│   └── ProjectName.API/            # طبقة العرض (API)
├── tests/
│   ├── ProjectName.UnitTests/      # اختبارات الوحدة
│   └── ProjectName.IntegrationTests/ # اختبارات التكامل
└── ProjectName.sln                 # ملف الحل
```

## 🔧 التكوين

### إعدادات قاعدة البيانات

يمكن حفظ إعدادات الاتصال بقاعدة البيانات بشكل آمن مع تشفير كلمات المرور.

### إعدادات التوليد

- اختيار المكونات المراد توليدها
- تخصيص مساحة الأسماء
- تحديد مسار الحفظ
- خيارات Dependency Injection

## 📝 نظام التسجيل (Logging)

التطبيق يحتوي على نظام تسجيل شامل:
- **مستويات التسجيل**: Debug, Info, Warning, Error
- **تنسيق JSON** - سجلات منظمة
- **واجهة عرض** - عرض السجلات وتصفيتها
- **تصدير السجلات** - تصدير إلى JSON, CSV, TXT

## 🤝 المساهمة

نرحب بمساهماتكم! يرجى:

1. عمل Fork للمشروع
2. إنشاء branch للميزة (`git checkout -b feature/AmazingFeature`)
3. عمل Commit للتغييرات (`git commit -m 'Add some AmazingFeature'`)
4. عمل Push للـ branch (`git push origin feature/AmazingFeature`)
5. فتح Pull Request

## 📄 الترخيص

هذا المشروع مرخص تحت [MIT License](LICENSE)

## 👥 المؤلفون

- **عبدالقادر موفعة** - *Initial work* - [Profile](https://github.com/AbdulqaderMaofaa/)

## 🙏 شكر وتقدير

- شكراً لجميع المساهمين في هذا المشروع
- شكراً لمجتمع .NET المفتوح المصدر

## 📞 التواصل

- **Issues**: [GitHub Issues](https://github.com/AbdulqaderMaofaa/GeneratorCode/issues)
- **Email**: maofaacom@gmail.com

---

<div align="center">

**صنع بـ ❤️ باستخدام .NET**

[⭐ Star على GitHub](https://github.com/AbdulqaderMaofaa/GeneratorCode) | [📖 الوثائق](docs/) | [🐛 الإبلاغ عن مشكلة](https://github.com/AbdulqaderMaofaa/GeneratorCode/issues)

</div>

</div>

---

<div dir="ltr" align="left">

# 🚀 GeneratorCode - Intelligent Code Generator

<div align="right">

[العربية](#-generatorcode---مولد-الكود-الذكي) | [English](#-generatorcode---intelligent-code-generator)

</div>

---

## 📋 Overview

**GeneratorCode** is an advanced desktop application for dynamically generating code based on databases and various software architecture patterns. It helps developers create complete project structures following best architectural practices in just minutes.

## ✨ Key Features

### 🏗️ Supported Architecture Patterns
- **Clean Architecture** - Clean architecture with layer separation
- **Layered Architecture** - Multi-layered architecture
- **CQRS Pattern** - Command Query Responsibility Segregation
- **Domain-Driven Design (DDD)** - Domain-driven design
- **Microservices Architecture** - Microservices architecture
- **Simple Architecture** - Simple architecture for small projects

### 🗄️ Supported Databases
- **SQL Server** - Full support for Microsoft SQL Server
- **MySQL** - Support for MySQL and MariaDB
- **PostgreSQL** - Support for PostgreSQL

### 🔧 Generated Components
- ✅ **Entities/Domain Models** - Domain models
- ✅ **DTOs** - Data Transfer Objects
- ✅ **Repositories** - Data repositories
- ✅ **Services** - Service layer
- ✅ **Controllers** - API controllers
- ✅ **Validators** - Data validation
- ✅ **Mappings** - AutoMapper Profiles
- ✅ **Unit Tests** - Unit tests
- ✅ **Integration Tests** - Integration tests

### 🎯 Additional Features
- 🔐 **Dependency Injection** - Microsoft DI and Autofac support
- 📝 **Logging System** - Comprehensive logging with viewer interface
- 🔍 **Preview Mode** - Preview code before generation
- 🎨 **GUI & CLI** - Graphical and command-line interfaces
- 🔄 **Async/Await Support** - Asynchronous operations support
- 📊 **Swagger Integration** - Swagger/OpenAPI integration
- 🛡️ **Input Validation** - Input validation

## 📦 Requirements

- **.NET 7.0** or later
- **Windows OS** (Windows Forms)
- **Database** (SQL Server / MySQL / PostgreSQL)

## 🚀 Quick Start

### Installation

1. Clone the repository:
```bash
git clone https://github.com/yourusername/GeneratorCode.git
cd GeneratorCode
```

2. Open the project in Visual Studio or Rider

3. Build the project:
```bash
dotnet build
```

4. Run the application:
```bash
dotnet run
```

### Basic Usage

#### Graphical User Interface (GUI)

1. Open the application
2. Enter database connection information
3. Select the desired architecture pattern
4. Choose tables to generate code for
5. Click "Generate" and wait for the result

#### Command Line Interface (CLI)

```bash
GeneratorCode.exe --server localhost --database MyDatabase --output C:\Output
```

**CLI Parameters:**
- `--server`: Server name
- `--database`: Database name
- `--db-type`: Database type (SqlServer, MySQL, PostgreSQL)
- `--namespace`: Namespace for generated code
- `--pattern`: Architecture pattern (CleanArchitecture, CQRS, DDD, etc.)
- `--output`: Output path for files
- `--enable-di`: Enable Dependency Injection
- `--async`: Generate async operations
- `--tests`: Generate unit tests

## 📖 Usage Examples

### Example 1: Generate Clean Architecture Code

```bash
GeneratorCode.exe \
  --server localhost \
  --database Northwind \
  --pattern CleanArchitecture \
  --namespace NorthwindApp \
  --output C:\GeneratedCode \
  --enable-di true \
  --async true
```

### Example 2: Generate CQRS Code with Tests

```bash
GeneratorCode.exe \
  --server localhost \
  --database ShopDB \
  --pattern CQRS \
  --namespace ShopApp \
  --output C:\ShopCode \
  --tests true
```

## 🏛️ Generated Project Structure

```
GeneratedProject/
├── src/
│   ├── ProjectName.Domain/          # Domain layer
│   ├── ProjectName.Application/     # Application layer
│   ├── ProjectName.Infrastructure/  # Infrastructure layer
│   └── ProjectName.API/            # Presentation layer (API)
├── tests/
│   ├── ProjectName.UnitTests/      # Unit tests
│   └── ProjectName.IntegrationTests/ # Integration tests
└── ProjectName.sln                 # Solution file
```

## 🔧 Configuration

### Database Settings

Database connection settings can be saved securely with encrypted passwords.

### Generation Settings

- Select components to generate
- Customize namespace
- Set output path
- Dependency Injection options

## 📝 Logging System

The application includes a comprehensive logging system:
- **Log Levels**: Debug, Info, Warning, Error
- **JSON Format** - Structured logs
- **Viewer Interface** - View and filter logs
- **Export Logs** - Export to JSON, CSV, TXT

## 🤝 Contributing

Contributions are welcome! Please:

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the [MIT License](LICENSE)

## 👥 Authors

- **Abdulqader Maofaa** - *Initial work* - [Profile](https://github.com/AbdulqaderMaofaa/)

## 🙏 Acknowledgments

- Thanks to all contributors to this project
- Thanks to the open-source .NET community

## 📞 Contact

- **Issues**: [GitHub Issues](https://github.com/AbdulqaderMaofaa/GeneratorCode/issues)
- **Email**: maofaacom@gmail.com

---

<div align="center">

**Made with ❤️ using .NET**

[⭐ Star on GitHub](https://github.com/AbdulqaderMaofaa/GeneratorCode) | [📖 Documentation](docs/) | [🐛 Report Bug](https://github.com/AbdulqaderMaofaa/GeneratorCode/issues)

</div>

</div>
