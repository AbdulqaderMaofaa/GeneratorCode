# مولّد الكود المتطور (GeneratorCode)

## نبذة عامة

**GeneratorCode** هو تطبيق سطح مكتب مبني بتقنية **Windows Forms** على إطار عمل **.NET 7.0**، مصمّم لتوليد مشاريع C# كاملة البنية انطلاقًا من قاعدة بيانات موجودة. يتصل التطبيق بقاعدة البيانات، يقرأ بنية الجداول (الأعمدة، المفاتيح الأساسية والخارجية، الفهارس، المشغّلات)، ثم يولّد مشروع C# متكامل وفق النمط المعماري الذي يختاره المستخدم.

يوفّر التطبيق واجهتين للاستخدام:

- **واجهة رسومية (GUI):** نوافذ WinForms تفاعلية للاتصال بقاعدة البيانات، اختيار الجداول، تحديد الخيارات، معاينة الكود، وتوليده.
- **واجهة سطر أوامر (CLI):** لتشغيل التوليد من الطرفية بدون واجهة رسومية عبر معاملات محددة.

---

## التقنيات والحزم المستخدمة

| التقنية / الحزمة | الإصدار | الوظيفة |
|---|---|---|
| .NET | 7.0-windows | إطار العمل الأساسي |
| Windows Forms | مدمج | واجهة المستخدم الرسومية |
| Microsoft.Data.SqlClient | 5.1.0 | الاتصال بـ SQL Server |
| Npgsql | 9.0.3 | الاتصال بـ PostgreSQL |
| MySql.Data | 9.3.0 | الاتصال بـ MySQL |
| System.CommandLine | 2.0.0-beta4 | تحليل معاملات سطر الأوامر (CLI) |
| System.CommandLine.NamingConventionBinder | 2.0.0-beta4 | ربط المعاملات تلقائيًا |
| MaterialSkin.2 | 2.3.1 | تنسيق وتجميل واجهة المستخدم |
| System.Text.Json | مدمج | قراءة وكتابة ملف الإعدادات (JSON) |

---

## قواعد البيانات المدعومة

- **SQL Server** — عبر `Microsoft.Data.SqlClient`
- **PostgreSQL** — عبر `Npgsql`
- **MySQL** — عبر `MySql.Data`

لكل قاعدة بيانات مزوّد مخصص (`IDatabaseProvider`) يتولى:
- جلب قائمة الجداول مع بياناتها الوصفية
- جلب الأعمدة مع أنواعها وخصائصها
- جلب المفاتيح الأساسية والخارجية
- تحويل أنواع البيانات من قاعدة البيانات إلى أنواع C#
- بناء نص الاتصال (Connection String)
- اختبار الاتصال

---

## الأنماط المعمارية المدعومة

| النمط المعماري | الحالة | الوصف |
|---|---|---|
| **Clean Architecture** | مكتمل | بنية نظيفة بأربع طبقات: Domain, Application, Infrastructure, Presentation مع كافة الملفات |
| **Simple Architecture** | مكتمل | بنية بسيطة: Models, DAL, Business مع عمليات CRUD مباشرة |
| **Layered Architecture** | قيد التطوير | بنية متعددة الطبقات |
| **CQRS** | قيد التطوير | فصل القراءة عن الكتابة |
| **Domain-Driven Design** | قيد التطوير | تصميم موجّه بالمجال |
| **Microservices** | قيد التطوير | بنية الخدمات المصغّرة |

---

## الملفات والمكوّنات المولّدة

عند اختيار **Clean Architecture** (المكتمل)، يولّد التطبيق البنية التالية:

### طبقة Domain
- كيانات (Entities) مع Data Annotations
- واجهات المستودعات (IRepository)

### طبقة Application
- كائنات نقل البيانات (DTOs)
- واجهات الخدمات (IService)
- المدققات (Validators) بنمط FluentValidation
- ملفات الربط (Mapping Profiles) بنمط AutoMapper

### طبقة Infrastructure
- تنفيذ المستودعات (Repository Implementation)
- سياق قاعدة البيانات (DbContext) لـ Entity Framework Core
- إعدادات البرنامج (Program.cs, Startup.cs)
- ملفات المشروع (.csproj) مع التبعيات

### طبقة Presentation
- المتحكمات (API Controllers) بنمط RESTful
- نماذج العرض (ViewModels)
- صفحات العرض (Views) — Index, Create, Edit, Details, Delete

### ملفات إضافية
- ملف الحل (Solution .sln)
- اختبارات الوحدة (Unit Tests)
- اختبارات التكامل (Integration Tests)
- ملف `.gitignore`
- ملف `README.md`
- ملف `global.json`

---

## بنية المشروع (هيكل المجلدات)

```
GeneratorCode/
├── GeneratorCode.sln                        # ملف الحل
└── GeneratorCode/                           # مشروع التطبيق
    ├── GeneratorCode.csproj                 # ملف المشروع
    ├── Program.cs                           # نقطة البداية
    │
    ├── Core/                                # الطبقة الأساسية (بدون واجهة مستخدم)
    │   ├── Interfaces/                      # الواجهات
    │   │   ├── IArchitecturePattern.cs      # واجهة الأنماط المعمارية
    │   │   ├── IDatabaseProvider.cs         # واجهة مزوّدات قواعد البيانات
    │   │   └── ITemplateEngine.cs           # واجهة محرك القوالب
    │   │
    │   ├── Models/                          # النماذج وكائنات البيانات
    │   │   ├── TableInfo.cs                 # معلومات الجدول
    │   │   ├── ColumnInfo.cs                # معلومات العمود
    │   │   ├── ForeignKeyInfo.cs            # معلومات المفتاح الخارجي
    │   │   ├── IndexInfo.cs                 # معلومات الفهرس
    │   │   ├── TriggerInfo.cs               # معلومات المشغّل
    │   │   ├── CodeGenerationContext.cs     # سياق التوليد
    │   │   ├── CodeGenerationResult.cs      # نتيجة التوليد
    │   │   ├── GeneratedFile.cs             # ملف مولّد
    │   │   ├── GenerationOptions.cs         # خيارات التوليد
    │   │   ├── DatabaseType.cs              # أنواع قواعد البيانات
    │   │   ├── DIOptions.cs                 # خيارات حقن التبعيات
    │   │   ├── PreviewResult.cs             # نتيجة المعاينة
    │   │   └── ProgrammingLanguage.cs       # لغات البرمجة المدعومة
    │   │
    │   ├── Services/                        # الخدمات
    │   │   └── CodeGenerationService.cs     # خدمة التوليد الرئيسية
    │   │
    │   ├── ArchitecturePatterns/             # تنفيذ الأنماط المعمارية
    │   │   ├── BaseArchitecturePattern.cs   # الصنف الأساسي
    │   │   ├── CleanArchitecturePattern.cs  # نمط Clean Architecture
    │   │   ├── SimpleArchitecturePattern.cs # نمط بسيط
    │   │   ├── LayeredArchitecturePattern.cs
    │   │   ├── CQRSPattern.cs
    │   │   ├── DomainDrivenDesignPattern.cs
    │   │   └── MicroservicesArchitecturePattern.cs
    │   │
    │   ├── DatabaseProviders/               # مزوّدات قواعد البيانات
    │   │   ├── SqlServerProvider.cs
    │   │   ├── PostgreSqlProvider.cs
    │   │   └── MySqlProvider.cs
    │   │
    │   ├── DependencyInjection/             # توليد حقن التبعيات
    │   │   ├── MicrosoftDIProvider.cs
    │   │   ├── AutofacProvider.cs
    │   │   ├── PackagesGenerator.cs         # توليد ملفات .csproj
    │   │   ├── ServiceExtensionsGenerator.cs
    │   │   └── DIIntegrationService.cs
    │   │
    │   ├── Factories/                       # المصانع
    │   │   ├── ArchitecturePatternFactory.cs
    │   │   ├── DatabaseProviderFactory.cs
    │   │   └── DIProviderFactory.cs
    │   │
    │   ├── TemplateEngine/                  # محرك القوالب
    │   │   └── SimpleTemplateEngine.cs
    │   │
    │   ├── Helpers/                         # أدوات مساعدة
    │   │   ├── ConnectionStringBuilder.cs   # بناء نص الاتصال
    │   │   └── PasswordEncryption.cs        # تشفير كلمات المرور
    │   │
    │   └── Logging/                         # نظام التسجيل
    │       ├── ILogger.cs
    │       ├── FileLogger.cs
    │       ├── LoggerFactory.cs
    │       ├── LogFileManager.cs
    │       ├── LogReaderService.cs
    │       ├── LogEntry.cs
    │       ├── LogLevel.cs
    │       └── ExceptionMiddleware.cs       # معالج الاستثناءات العام
    │
    ├── Forms/                               # نماذج واجهة المستخدم
    │   ├── FrmConnection.cs                 # نافذة الاتصال بقاعدة البيانات
    │   ├── FrmTabls.cs                      # نافذة اختيار الجداول والتوليد
    │   ├── FrmSettings.cs                   # نافذة الإعدادات
    │   ├── FrmPreview.cs                    # نافذة معاينة الكود
    │   ├── FrmProgress.cs                   # نافذة التقدم
    │   ├── FrmLogViewer.cs                  # عارض السجلات
    │   └── FrmLogDetails.cs                 # تفاصيل السجل
    │
    ├── Helpers/                             # أدوات مساعدة للواجهة
    │   ├── DatabaseHelper.cs                # مساعد قاعدة البيانات
    │   └── LogViewerHelper.cs               # مساعد عرض السجلات
    │
    ├── CLI/                                 # واجهة سطر الأوامر
    │   └── CommandLineInterface.cs
    │
    ├── Templates/                           # القوالب
    │   └── CleanArchitecture/               # قوالب Clean Architecture
    │       ├── Solution.template
    │       ├── Domain/
    │       ├── Application/
    │       ├── Infrastructure/
    │       └── Presentation/
    │
    ├── Properties/                          # خصائص التطبيق
    │   └── Settings.cs                      # إدارة الإعدادات
    │
    └── Resources/                           # الموارد
        └── settings.json                    # ملف الإعدادات
```

---

## المعمارية الداخلية

### نمط التصميم

التطبيق مبني على مبادئ **SOLID** ويستخدم الأنماط التالية:

- **Factory Pattern** — مصانع لإنشاء مزوّدات قواعد البيانات، الأنماط المعمارية، ومزوّدات حقن التبعيات
- **Strategy Pattern** — كل نمط معماري وكل مزوّد قاعدة بيانات هو استراتيجية مستقلة خلف واجهة موحّدة
- **Template Method Pattern** — `BaseArchitecturePattern` يوفّر سلوكًا أساسيًا تتخصص به الأنماط الفرعية
- **Singleton Pattern** — `LoggerFactory.Default` و `Settings.Default`

### الواجهات الرئيسية

| الواجهة | الوظيفة |
|---|---|
| `IArchitecturePattern` | توليد الكود وفق نمط معماري، معاينة، توليد الطبقات |
| `IDatabaseProvider` | جلب بنية قاعدة البيانات (جداول، أعمدة، مفاتيح)، اختبار الاتصال |
| `ITemplateEngine` | تحميل ومعالجة وعرض القوالب |
| `IDependencyInjectionProvider` | توليد إعدادات حقن التبعيات |

### المصانع

| المصنع | الوظيفة |
|---|---|
| `ArchitecturePatternFactory` | إنشاء النمط المعماري المطلوب بالاسم |
| `DatabaseProviderFactory` | إنشاء مزوّد قاعدة البيانات حسب النوع |
| `DIProviderFactory` | إنشاء مزوّد حقن التبعيات (Microsoft DI أو Autofac) |

### الخدمة الرئيسية — `CodeGenerationService`

هذه الخدمة هي القلب النابض للتطبيق، وتتولى:

1. التحقق من صحة سياق التوليد (`ValidateContext`)
2. إنشاء النمط المعماري عبر المصنع
3. استدعاء `pattern.Generate(context)` لتوليد الملفات
4. دمج إعدادات حقن التبعيات إن كانت مفعّلة
5. حفظ الملفات المولّدة على القرص (`SaveGeneratedFilesAsync`)
6. توليد ملفات البنية التحتية (Solution, Startup, Program, .gitignore, README)
7. توليد الطبقات المعمارية (Infrastructure, Application, Domain, Presentation)

---

## تدفق العمل

### عبر الواجهة الرسومية (GUI)

```
FrmConnection                    FrmTabls                           الملفات المولّدة
┌─────────────────┐    ┌──────────────────────────┐    ┌─────────────────────┐
│ 1. اختيار نوع   │    │ 4. عرض الجداول           │    │ مشروع C# كامل:     │
│    قاعدة البيانات│───>│ 5. اختيار الجداول        │───>│ - Entities          │
│ 2. إدخال بيانات │    │ 6. اختيار النمط المعماري  │    │ - DTOs              │
│    الاتصال       │    │ 7. تحديد الخيارات        │    │ - Repositories      │
│ 3. اختبار       │    │ 8. معاينة أو توليد       │    │ - Services          │
│    الاتصال       │    │                          │    │ - Controllers       │
└─────────────────┘    └──────────────────────────┘    │ - Tests             │
                                                       │ - Solution files    │
                                                       └─────────────────────┘
```

1. **FrmConnection** — يدخل المستخدم بيانات الاتصال (نوع قاعدة البيانات، الخادم، المنفذ، اسم المستخدم، كلمة المرور)، يختبر الاتصال، ثم ينتقل إلى النافذة التالية.
2. **FrmTabls** — يتم تحميل قائمة الجداول من قاعدة البيانات. يختار المستخدم الجداول المطلوبة، النمط المعماري، لغة البرمجة، مساحة الأسماء (Namespace)، مسار الحفظ، وخيارات التوليد (كيانات، DTOs، مستودعات، خدمات، متحكمات، اختبارات، مدققات، Swagger، حقن التبعيات، CRUD).
3. **المعاينة (اختياري)** — يمكن معاينة الكود المولّد في نافذة **FrmPreview** قبل الحفظ.
4. **التوليد** — يتم استدعاء `CodeGenerationService.GenerateCodeAsync` الذي ينشئ المشروع الكامل على القرص.

### عبر سطر الأوامر (CLI)

```
GeneratorCode.exe --server localhost --database MyDB --db-type SqlServer
                  --namespace MyProject --pattern CleanArchitecture
                  --output C:\Output --enable-di --tests
```

المعاملات المتاحة:

| المعامل | الوظيفة | القيمة الافتراضية |
|---|---|---|
| `--server` | اسم الخادم | — |
| `--database` | اسم قاعدة البيانات | — |
| `--db-type` | نوع قاعدة البيانات | SqlServer |
| `--namespace` | مساحة الأسماء | GeneratedCode |
| `--pattern` | النمط المعماري | CleanArchitecture |
| `--output` | مسار الحفظ | — |
| `--enable-di` | تفعيل حقن التبعيات | true |
| `--async` | عمليات غير متزامنة | true |
| `--tests` | توليد اختبارات وحدة | false |

---

## الميزات الرئيسية

### 1. دعم قواعد بيانات متعددة
يدعم التطبيق ثلاث قواعد بيانات رئيسية (SQL Server, PostgreSQL, MySQL) مع إمكانية إضافة المزيد عبر تنفيذ واجهة `IDatabaseProvider` وتسجيلها في `DatabaseProviderFactory`.

### 2. أنماط معمارية قابلة للتوسيع
كل نمط معماري هو تنفيذ مستقل لواجهة `IArchitecturePattern`، مما يسمح بإضافة أنماط جديدة بدون تعديل الكود الموجود (مبدأ Open/Closed).

### 3. محرك قوالب مرن
`SimpleTemplateEngine` يدعم:
- متغيرات بسيطة: `{{variableName}}`
- حلقات تكرار: `{{#each items}}...{{/each}}`
- شروط: `{{#if condition}}...{{else}}...{{/if}}`
- تحميل القوالب من مجلد `Templates/` أو `%AppData%\GeneratorCode\Templates`

### 4. معاينة الكود قبل الحفظ
يتيح التطبيق معاينة جميع الملفات المولّدة في نافذة مخصصة (`FrmPreview`) قبل كتابتها على القرص.

### 5. حقن التبعيات (Dependency Injection)
يولّد التطبيق إعدادات حقن التبعيات للمشروع المولّد مع دعم:
- **Microsoft DI** — الحاوية الافتراضية لـ .NET
- **Autofac** — حاوية متقدمة

يشمل ذلك توليد ملفات `ServiceExtensions`، تسجيل الخدمات والمستودعات، وإعدادات `DbContext`.

### 6. توليد ملفات المشروع الكاملة
إلى جانب كود المصدر، يولّد التطبيق:
- ملفات `.csproj` لكل طبقة مع حزم NuGet المناسبة ومراجع المشاريع
- ملف الحل `.sln` مع جميع المشاريع
- ملف `Program.cs` و `Startup.cs` للمشروع المولّد
- ملف `.gitignore` و `README.md`

### 7. نظام تسجيل أحداث متكامل
- تسجيل بمستويات متعددة: Trace, Debug, Information, Warning, Error, Critical
- حفظ السجلات بتنسيق JSON في `%AppData%\GeneratorCode\Logs`
- عارض سجلات مدمج في الواجهة (`FrmLogViewer`, `FrmLogDetails`)
- معالج استثناءات عام (`ExceptionMiddleware`) يلتقط الأخطاء غير المعالجة

### 8. تشفير كلمات المرور
`PasswordEncryption` يشفّر كلمات مرور قواعد البيانات عند حفظها في الإعدادات لحمايتها.

### 9. إعدادات قابلة للتخصيص
نافذة إعدادات (`FrmSettings`) تسمح بتحديد:
- مساحة الأسماء الافتراضية ومسار الحفظ
- بيانات الاتصال الافتراضية لكل نوع قاعدة بيانات (اسم المستخدم، كلمة المرور، المنفذ)
- تفعيل أو تعطيل حقن التبعيات، التحقق، والاختبارات

---

## الإعدادات والتكوين

### ملف الإعدادات (`Resources/settings.json`)

```json
{
  "DatabaseType": "PostgreSQL",
  "EnableDI": false,
  "EnableValidation": false,
  "EnableTesting": false,
  "DefaultNamespace": "",
  "DefaultOutputPath": "",
  "PostgreSqlDefaultUsername": "postgres",
  "PostgreSqlDefaultPassword": "",
  "PostgreSqlDefaultPort": "5432",
  "SqlServerDefaultUsername": "sa",
  "SqlServerDefaultPassword": "",
  "MySqlDefaultUsername": "",
  "MySqlDefaultPassword": ""
}
```

يُدار الملف عبر صنف `Settings` الذي يوفّر:
- تحميل الإعدادات من ملف JSON عند بدء التشغيل
- حفظ التغييرات إلى الملف عند التعديل
- خاصية `Default` كنقطة وصول عامة (Singleton)

### مسار السجلات

```
%AppData%\GeneratorCode\Logs\
```

---

## نظام القوالب

القوالب محفوظة في مجلد `Templates/` ومنظّمة حسب النمط المعماري. حاليًا يتوفر مجلد `CleanArchitecture/` الكامل:

```
Templates/CleanArchitecture/
├── Solution.template              # قالب ملف الحل (.sln)
├── README.template.md             # قالب README
├── .gitignore.template            # قالب .gitignore
├── Domain/
│   ├── Entity.template            # قالب الكيان
│   └── IRepository.template       # قالب واجهة المستودع
├── Application/
│   ├── DTO.template               # قالب كائن نقل البيانات
│   ├── IService.template          # قالب واجهة الخدمة
│   ├── Validation/
│   │   └── Validator.template     # قالب المدقق
│   └── Mapping/
│       └── MappingProfile.template # قالب ملف الربط
├── Infrastructure/
│   ├── Repository.template        # قالب تنفيذ المستودع
│   ├── Program.template           # قالب Program.cs
│   └── Startup.template           # قالب Startup.cs
└── Presentation/
    ├── Controllers/
    │   └── Controller.template    # قالب المتحكم
    ├── ViewModels/
    │   └── ViewModel.template     # قالب نموذج العرض
    └── Views/
        ├── Index.template         # قالب صفحة القائمة
        ├── Create.template        # قالب صفحة الإنشاء
        ├── Edit.template          # قالب صفحة التعديل
        ├── Details.template       # قالب صفحة التفاصيل
        └── Delete.template        # قالب صفحة الحذف
```

تستخدم القوالب عناصر نائبة (Placeholders) مثل `{{namespace}}`، `{{entityName}}`، `{{properties}}` يتم استبدالها بالبيانات الفعلية أثناء التوليد.

---

## خيارات التوليد المتاحة

| الخيار | الوصف |
|---|---|
| الكيانات (Entities) | توليد أصناف C# تمثّل جداول قاعدة البيانات |
| كائنات نقل البيانات (DTOs) | أصناف لنقل البيانات بين الطبقات |
| المستودعات (Repositories) | واجهات وتنفيذات للوصول إلى البيانات |
| الخدمات (Services) | طبقة منطق الأعمال |
| المتحكمات (Controllers) | متحكمات API بنمط RESTful |
| الاختبارات (Unit Tests) | اختبارات وحدة للخدمات والمستودعات |
| المدققات (Validators) | قواعد التحقق من صحة البيانات |
| Swagger/OpenAPI | توثيق API |
| حقن التبعيات (DI) | تسجيل الخدمات وإعداد الحاوية |
| عمليات CRUD | توليد عمليات الإنشاء والقراءة والتحديث والحذف |
| بنية المشروع | ملفات Startup, Program, .gitignore, README, Solution |
| الطبقات | Infrastructure, Application, Domain, Presentation |

---

## نقطة البداية (`Program.cs`)

عند تشغيل التطبيق:

1. يتم تهيئة نظام التسجيل (`LoggerFactory`)
2. يتم تفعيل معالج الاستثناءات العام (`ExceptionMiddleware`)
3. يتم إنشاء الخدمات الأساسية:
   - `SimpleTemplateEngine` — محرك القوالب
   - `ArchitecturePatternFactory` — مصنع الأنماط المعمارية
   - `DatabaseProviderFactory` — مصنع مزوّدات قواعد البيانات
   - `DIProviderFactory` — مصنع مزوّدات حقن التبعيات
   - `CodeGenerationService` — خدمة التوليد الرئيسية
4. إذا وُجدت معاملات سطر أوامر → يعمل بوضع CLI
5. إذا لم توجد → يعمل بوضع GUI ويفتح نافذة `FrmConnection`

---

## نوافذ الواجهة الرسومية

| النافذة | الوظيفة |
|---|---|
| **FrmConnection** | إدخال بيانات الاتصال بقاعدة البيانات واختبارها |
| **FrmTabls** | عرض الجداول، اختيار الخيارات، تشغيل التوليد أو المعاينة |
| **FrmSettings** | إدارة الإعدادات الافتراضية (مساحة أسماء، مسارات، بيانات اتصال) |
| **FrmPreview** | عرض الكود المولّد قبل حفظه على القرص |
| **FrmProgress** | شريط تقدم أثناء عملية التوليد |
| **FrmLogViewer** | عرض وتصفية سجلات التطبيق |
| **FrmLogDetails** | عرض تفاصيل سجل محدد |

---

## القابلية للتوسيع

التطبيق مصمّم ليكون قابلًا للتوسيع بسهولة:

- **إضافة نمط معماري جديد:** تنفيذ `IArchitecturePattern` وتسجيله في `ArchitecturePatternFactory`
- **إضافة قاعدة بيانات جديدة:** تنفيذ `IDatabaseProvider` وتسجيله في `DatabaseProviderFactory`
- **إضافة حاوية حقن تبعيات:** تنفيذ `IDependencyInjectionProvider` وتسجيله في `DIProviderFactory`
- **إضافة قوالب جديدة:** إنشاء مجلد قوالب جديد تحت `Templates/` مع ملفات `.template`
