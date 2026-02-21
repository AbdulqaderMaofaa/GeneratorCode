```md
# 🏗 خطة متكاملة لإضافة دعم Code First + توليد Migrations تلقائيًا  
## لمشروع GeneratorCode

---

# 🎯 الهدف الاستراتيجي

تمكين GeneratorCode من دعم مسار جديد بالكامل:

> المستخدم يصمم الكيانات داخل النظام →  
> يتم توليد كود EF Core →  
> يتم توليد Migration تلقائيًا →  
> يتم تطبيقه على قاعدة البيانات.

ليصبح النظام ثنائي الاتجاه:

```

Database First  ⇄  Code First

```

---

# 🧠 الرؤية المعمارية

حاليًا النظام يعمل كالتالي:

```

Database → Metadata → Code Generation

```

سيتم إضافة مسار جديد:

```

Entity Designer → Domain Model → DbContext → Migration → Database Update

````

وسيتم دعم وضعين تشغيل:

```csharp
public enum GenerationMode
{
    DatabaseFirst,
    CodeFirst
}
````

---

# 🧱 المكونات الجديدة المطلوبة

## 1️⃣ Domain Modeling Engine

محرك داخلي لبناء نموذج المجال (Entities + Relations).

## 2️⃣ Entity Designer (WinForms UI)

واجهة مرئية لإنشاء الجداول والخصائص والعلاقات.

## 3️⃣ EF Core Code Generator

توليد:

* Entities
* DbContext
* Fluent Configuration

## 4️⃣ Migration Engine

إدارة:

* Add Migration
* Update Database
* Rollback
* Script Generation

## 5️⃣ Integration Layer

دمج Code First داخل Pipeline الحالي.

---

# 📆 خطة التنفيذ التفصيلية (8 أسابيع)

---

# 📆 الأسبوع 1 — تأسيس Domain Modeling Engine

## 🎯 الهدف

إنشاء تمثيل داخلي كامل للكيانات والعلاقات.

## 🛠 المهام التقنية

### إنشاء Models داخل Core/DomainModel

```csharp
public class DomainModel
{
    public List<DomainEntity> Entities { get; set; } = new();
}
```

```csharp
public class DomainEntity
{
    public string Name { get; set; }
    public List<DomainProperty> Properties { get; set; } = new();
    public List<DomainRelation> Relations { get; set; } = new();
}
```

```csharp
public class DomainProperty
{
    public string Name { get; set; }
    public string Type { get; set; }
    public bool IsRequired { get; set; }
    public bool IsPrimaryKey { get; set; }
    public bool IsIdentity { get; set; }
    public int? MaxLength { get; set; }
}
```

```csharp
public class DomainRelation
{
    public string SourceEntity { get; set; }
    public string TargetEntity { get; set; }
    public RelationType RelationType { get; set; }
}
```

---

### إنشاء Services

* `IDomainModelService`
* `DomainModelService`

المسؤوليات:

* إضافة/حذف/تعديل Entity
* إدارة العلاقات
* التحقق من صحة النموذج

---

### دعم الحفظ والاسترجاع

* حفظ إلى `Resources/domain-model.json`
* تحميل النموذج عند بدء التشغيل

---

# 📆 الأسبوع 2 — تطوير Entity Designer (WinForms)

## 🎯 الهدف

تمكين المستخدم من تصميم قاعدة البيانات بصريًا.

## 🛠 المهام

### إنشاء نافذة جديدة

```
Forms/FrmEntityDesigner.cs
```

### دعم العمليات التالية

* إنشاء Entity
* إضافة Property
* تحديد:

  * النوع (int, string, decimal, DateTime...)
  * Required
  * Primary Key
  * Identity
* تعريف العلاقات (One-to-Many / Many-to-Many)

### إضافة Validation Layer

* منع تكرار أسماء الخصائص
* منع أكثر من PK بدون Composite Support
* التحقق من العلاقات

### دعم التصدير والاستيراد

* Export JSON
* Import JSON

---

# 📆 الأسبوع 3 — توليد كود EF Core (Code First Generator)

## 🎯 الهدف

تحويل DomainModel إلى مشروع EF Core جاهز.

## 🛠 المهام

### إنشاء:

* `ICodeFirstGenerator`
* `EfCoreCodeFirstGenerator`

### توليد:

#### 1️⃣ Entity Classes

* دعم Data Annotations
* دعم Navigation Properties

#### 2️⃣ DbContext

```csharp
public class AppDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; }
}
```

#### 3️⃣ Fluent Configuration داخل OnModelCreating

```csharp
modelBuilder.Entity<Order>()
    .HasOne(o => o.Customer)
    .WithMany(c => c.Orders)
    .HasForeignKey(o => o.CustomerId);
```

---

### تحديث ملفات .csproj تلقائيًا

إضافة:

* Microsoft.EntityFrameworkCore
* Microsoft.EntityFrameworkCore.Tools
* Provider حسب قاعدة البيانات

---

# 📆 الأسبوع 4 — دمج Migration Engine

## 🎯 الهدف

إدارة Migrations برمجيًا.

## 🛠 الخيار المعتمد (موصى به)

استخدام dotnet CLI عبر Process:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

### إنشاء Abstraction

```csharp
public interface IMigrationService
{
    Task AddMigrationAsync(string name);
    Task UpdateDatabaseAsync();
    Task RollbackAsync(string migrationName);
    Task GenerateScriptAsync(string from, string to);
}
```

---

### تنفيذ:

* `EfCoreMigrationService`
* التقاط stdout/stderr
* تسجيل النتائج في Logging System

---

# 📆 الأسبوع 5 — إدارة الإصدارات والتحكم الكامل

## 🎯 الهدف

تحكم احترافي في دورة حياة المخطط.

## 🛠 المهام

* عرض قائمة Migrations الحالية
* دعم:

  * Remove Last Migration
  * Rollback To Version
* عرض SQL Script قبل التنفيذ
* دعم Generate Script Only Mode
* دعم Apply Migration Only Mode

---

# 📆 الأسبوع 6 — دمج Code First في Pipeline

## 🎯 الهدف

تكامل كامل مع النظام الحالي.

## 🛠 المهام

### تحديث GenerationContext

إضافة:

```csharp
public GenerationMode Mode { get; set; }
```

### تعديل Pipeline

إذا Mode == CodeFirst:

* Skip LoadMetadataStage
* Execute LoadDomainModelStage

### تحديث CLI

```
--mode codefirst
--migration-name Init
--apply-migration
```

---

# 📆 الأسبوع 7 — تحسينات احترافية

## 🔥 إضافات

### 1️⃣ Migration Diff Preview

عرض الفرق بين النسختين قبل التنفيذ.

### 2️⃣ Seed Data Generator

توليد Seed داخل OnModelCreating.

### 3️⃣ Soft Delete + Auditing Support

إضافة BaseEntity تلقائيًا.

### 4️⃣ Snapshot Export

تصدير المخطط الكامل إلى JSON.

---

# 📆 الأسبوع 8 — تحسين الأداء والأمان

## 🛠 المهام

### الأداء

* Async Process Execution
* Parallel Code Generation
* Cache للـ DomainModel

### الأمان

* Path Sanitization
* Validation ضد Code Injection
* منع تنفيذ أوامر خارجية غير آمنة

---

# 🧩 اعتبارات معمارية مهمة

## 1️⃣ فصل Migration Engine

لا تربطه مباشرة بـ CLI.
استخدم Abstraction واضحة.

## 2️⃣ دعم Multi-Provider

يمرر:

* ProviderName
* ConnectionString
* TargetFramework

## 3️⃣ تجنب Tight Coupling

Code First يجب ألا يؤثر على Database First.

---

# 📊 مؤشرات الأداء (KPIs)

| المقياس                | الهدف      |
| ---------------------- | ---------- |
| إنشاء مشروع Code First | < 30 ثانية |
| توليد Migration        | < 5 ثواني  |
| Update Database        | بدون أخطاء |
| UI                     | بدون تجمّد |
| Test Coverage          | ≥ 60%      |

---

# 🏆 النتيجة النهائية

بعد تنفيذ الخطة سيكون GeneratorCode:

* يدعم Database First
* يدعم Code First
* يولد Migrations تلقائيًا
* يطبق التحديثات مباشرة
* يدير إصدارات المخطط
* منصة متكاملة لإدارة دورة حياة قاعدة البيانات

---

# 🚀 المرحلة التالية المقترحة بعد الإكمال

* Hybrid Sync (Code ↔ Database)
* Visual ER Diagram Designer
* Live Schema Diff Engine
* CI/CD Migration Mode
* Multi-Project Code First

---

**انتهت الخطة الكاملة.**

```
```
