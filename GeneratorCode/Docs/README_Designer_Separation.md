````md
# 🧩 دعم اختيار ReadRepository & WriteRepository & UnitOfWork  
## داخل GeneratorCode (في وضع Code First و Database First)

---

# 🎯 الهدف

إتاحة خيار للمستخدم أثناء التوليد لتحديد نمط الوصول للبيانات:

- ✔️ Generic Repository
- ✔️ Read Repository
- ✔️ Write Repository
- ✔️ Unit Of Work
- ✔️ أو بدون Repository (DbContext مباشر)

ليصبح النظام مرنًا وقابلًا للتخصيص حسب أسلوب المعمارية.

---

# 🧠 الرؤية المعمارية

بدل أن يكون التوليد ثابتًا، نضيف طبقة إعدادات:

```csharp
public class DataAccessOptions
{
    public bool UseRepositoryPattern { get; set; }
    public bool UseReadRepository { get; set; }
    public bool UseWriteRepository { get; set; }
    public bool UseUnitOfWork { get; set; }
}
````

ثم يتم تمريرها إلى Code Generator ليحدد ما الذي سيتم توليده.

---

# 🏗 السيناريوهات المدعومة

| الخيار                | الناتج                                       |
| --------------------- | -------------------------------------------- |
| لا شيء                | استخدام DbContext مباشرة                     |
| Generic Repository    | IGenericRepository + GenericRepository       |
| Read/Write Repository | IOrderReadRepository + IOrderWriteRepository |
| UnitOfWork فقط        | IUnitOfWork + تنفيذ مركزي                    |
| Read/Write + UoW      | أفضل ممارسة مع CQRS                          |
| كل شيء                | Full Clean Architecture Data Layer           |

---

# 📆 خطة التنفيذ التفصيلية

---

# 📆 المرحلة 1 — تحديث إعدادات النظام

## 1️⃣ إضافة DataAccessPattern Enum

```csharp
public enum DataAccessPattern
{
    DbContextOnly,
    GenericRepository,
    ReadWriteRepository,
    ReadWriteWithUnitOfWork
}
```

## 2️⃣ تحديث GenerationContext

```csharp
public DataAccessPattern DataAccessPattern { get; set; }
```

## 3️⃣ تحديث UI

إضافة ComboBox في إعدادات التوليد:

```
اختر نمط الوصول للبيانات:
- DbContext مباشر
- Generic Repository
- Read/Write Repository
- Read/Write + UnitOfWork
```

---

# 📆 المرحلة 2 — توليد Generic Repository

## 🎯 عند اختيار GenericRepository

### توليد Interface

```csharp
public interface IGenericRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}
```

### توليد Implementation

```csharp
public class GenericRepository<T> : IGenericRepository<T>
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;
}
```

---

# 📆 المرحلة 3 — توليد Read / Write Repository (CQRS Friendly)

## 🎯 عند اختيار ReadWriteRepository

لكل Entity يتم توليد:

### 1️⃣ Read Repository

```csharp
public interface IOrderReadRepository
{
    Task<Order> GetByIdAsync(int id);
    Task<List<Order>> GetAllAsync();
}
```

### 2️⃣ Write Repository

```csharp
public interface IOrderWriteRepository
{
    Task AddAsync(Order entity);
    void Update(Order entity);
    void Delete(Order entity);
}
```

### 3️⃣ Implementations

* OrderReadRepository
* OrderWriteRepository

---

# 📆 المرحلة 4 — دعم Unit Of Work

## 🎯 عند اختيار ReadWriteWithUnitOfWork

### توليد Interface

```csharp
public interface IUnitOfWork : IDisposable
{
    IOrderReadRepository OrderRead { get; }
    IOrderWriteRepository OrderWrite { get; }

    Task<int> SaveChangesAsync();
}
```

### توليد Implementation

```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    public Task<int> SaveChangesAsync()
        => _context.SaveChangesAsync();
}
```

---

# 📆 المرحلة 5 — دمج مع Dependency Injection

توليد ملف:

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IOrderReadRepository, OrderReadRepository>();
        services.AddScoped<IOrderWriteRepository, OrderWriteRepository>();
        return services;
    }
}
```

---

# 📆 المرحلة 6 — تعديل Code Generator Architecture

## إنشاء Strategy Pattern

```csharp
public interface IDataAccessGenerator
{
    Task GenerateAsync(DomainModel model);
}
```

Implementations:

* DbContextOnlyGenerator
* GenericRepositoryGenerator
* ReadWriteRepositoryGenerator
* ReadWriteWithUowGenerator

ثم داخل Pipeline:

```csharp
var generator = _factory.Create(context.DataAccessPattern);
await generator.GenerateAsync(domainModel);
```

---

# 🧠 أفضل ممارسة مقترحة

للمشاريع المتوسطة والكبيرة:

> ✔️ Read/Write Repository + UnitOfWork
> ✔️ فصل Queries عن Commands
> ✔️ جاهز للانتقال إلى CQRS لاحقًا

---

# ⚖️ مقارنة الأنماط

| النمط              | التعقيد | المرونة | قابلية الاختبار |
| ------------------ | ------- | ------- | --------------- |
| DbContext مباشر    | منخفض   | منخفض   | متوسط           |
| Generic Repository | متوسط   | جيد     | جيد             |
| Read/Write         | متوسط   | عالي    | عالي            |
| Read/Write + UoW   | أعلى    | احترافي | ممتاز           |

---

# 📊 تأثير الإضافة على النظام

* لا يؤثر على Database First
* يتم توليده فقط عند الاختيار
* قابل للتوسعة لاحقًا (Specification Pattern)
* يدعم Clean Architecture

---

# 🚀 إضافات مستقبلية ممكنة

* دعم Specification Pattern
* دعم Caching Layer
* دعم Decorator Logging
* دعم TransactionScope اختياري
* دعم Soft Delete تلقائي داخل WriteRepository

---

# 🏆 النتيجة النهائية

بعد تنفيذ هذه الخطة سيصبح GeneratorCode:

* مولد Code First
* مولد Migrations
* مولد Repository Layer احترافي
* يدعم CQRS
* يدعم Unit Of Work
* جاهز لـ Clean Architecture

---


* 🔥 تصميم Clean Architecture كامل متكامل
* 🧠 دعم CQRS + MediatR
* 📦 توليد مشروع Web API كامل مع كل الطبقات
* 🏗 تصميم Layered + Modular Architecture

اختر الاتجاه الذي تريد 👌

```
```
