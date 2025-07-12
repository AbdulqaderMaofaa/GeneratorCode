# دعم Dependency Injection في مولد الكود

## نظرة عامة

تم إضافة دعم شامل لـ Dependency Injection في مولد الكود، مما يجعل الكود المولد جاهز للاستخدام في التطبيقات الحديثة مع أفضل الممارسات.

## الميزات المتاحة

### 🔧 DI Containers المدعومة
- **Microsoft.Extensions.DependencyInjection** (افتراضي)
- **Autofac**
- إمكانية إضافة موفري DI إضافيين

### 📦 الملفات المولدة

#### 1. Service Extensions
```csharp
public static class UserServiceExtensions
{
    public static IServiceCollection AddUserServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database Configuration
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        
        // Repository Registration
        services.AddScoped<IUserRepository, UserRepository>();
        
        // Service Registration
        services.AddScoped<IUserService, UserService>();
        
        // MediatR Registration (للـ Clean Architecture)
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UserServiceExtensions).Assembly));
        
        // AutoMapper Registration
        services.AddAutoMapper(typeof(UserServiceExtensions).Assembly);
        
        return services;
    }
}
```

#### 2. Startup Configuration
```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSwaggerGen();
        
        // Application Services
        services.AddUserServices(Configuration);
    }
    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Pipeline configuration
    }
}
```

#### 3. Project File (.csproj)
يتم توليد ملف csproj يحتوي على جميع التبعيات المطلوبة:
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="6.0.0" />
    <PackageReference Include="MediatR.Extensions.Microsoft.DependencyInjection" Version="10.0.0" />
    <PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="11.0.0" />
    <!-- المزيد من التبعيات -->
  </ItemGroup>
</Project>
```

#### 4. Configuration Files
- `appsettings.json` - إعدادات التطبيق
- `launchSettings.json` - إعدادات التشغيل

### 🏗️ التكامل مع الأنماط المعمارية

#### Clean Architecture
```csharp
// يتم تسجيل الخدمات حسب الطبقات
services.AddScoped<IRepository<>, Repository<>>();
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly));
services.AddAutoMapper(Assembly);
```

#### CQRS Pattern
```csharp
// دعم MediatR للـ Commands والـ Queries
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly));
services.AddScoped<ICommandHandler<>, CommandHandler<>>();
services.AddScoped<IQueryHandler<>, QueryHandler<>>();
```

## كيفية الاستخدام

### 1. تكوين خيارات DI
```csharp
var context = new CodeGenerationContext
{
    // ... باقي الإعدادات
    
    DIOptions = new DIOptions
    {
        EnableDI = true,
        PreferredContainer = DIContainerType.MicrosoftDI,
        GenerateServiceExtensions = true,
        EnableLogging = true,
        EnableConfiguration = true,
        EnableHealthChecks = true
    }
};
```

### 2. توليد الكود
```csharp
var codeGenerationService = new CodeGenerationService(
    patternFactory,
    databaseFactory,
    diProviderFactory,  // جديد
    templateEngine
);

var result = await codeGenerationService.GenerateCodeAsync(context);
```

### 3. استخدام Autofac
```csharp
DIOptions = new DIOptions
{
    EnableDI = true,
    PreferredContainer = DIContainerType.Autofac,
    GenerateModuleClasses = true
}
```

## الخيارات المتاحة

### DIOptions
| الخاصية | الوصف | القيمة الافتراضية |
|---------|--------|------------------|
| `EnableDI` | تمكين DI | `true` |
| `PreferredContainer` | نوع Container المفضل | `MicrosoftDI` |
| `GenerateServiceExtensions` | توليد Service Extensions | `true` |
| `GenerateModuleClasses` | توليد Module Classes (Autofac) | `false` |
| `EnableAutoRegistration` | تمكين Auto-registration | `false` |
| `EnableValidation` | تمكين Validation | `true` |
| `EnableLogging` | تمكين Logging | `true` |
| `EnableConfiguration` | تمكين Configuration | `true` |
| `EnableHealthChecks` | تمكين Health Checks | `false` |

## التبعيات المطلوبة

### Microsoft DI
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.Configuration
- Microsoft.EntityFrameworkCore.SqlServer
- AutoMapper.Extensions.Microsoft.DependencyInjection
- MediatR.Extensions.Microsoft.DependencyInjection

### Autofac
- Autofac
- Autofac.Extensions.DependencyInjection

## مثال شامل

```csharp
// إنشاء السياق مع DI
var context = new CodeGenerationContext
{
    ConnectionString = "Server=.;Database=MyDB;Trusted_Connection=true;",
    DatabaseType = DatabaseType.SqlServer,
    TableName = "Users",
    EntityName = "User",
    Namespace = "MyApp",
    OutputPath = @"C:\Output\MyApp",
    ArchitecturePattern = "CleanArchitecture",
    
    DIOptions = new DIOptions
    {
        EnableDI = true,
        PreferredContainer = DIContainerType.MicrosoftDI,
        GenerateServiceExtensions = true,
        EnableLogging = true,
        EnableHealthChecks = true
    }
};

// توليد الكود
var result = await codeGenerationService.GenerateCodeAsync(context);

// الملفات المولدة ستشمل:
// - UserServiceExtensions.cs
// - Startup.cs
// - MyApp.csproj
// - appsettings.json
// - جميع ملفات الطبقات مع DI support
```

## الفوائد

✅ **جاهز للاستخدام**: الكود المولد جاهز للتشغيل فوراً  
✅ **أفضل الممارسات**: يتبع أحدث معايير .NET  
✅ **قابل للتوسعة**: سهولة إضافة DI containers جديدة  
✅ **مرونة**: دعم لأنماط معمارية متعددة  
✅ **شامل**: يشمل جميع ملفات التكوين المطلوبة  

## الخلاصة

يوفر النظام الجديد دعماً شاملاً لـ Dependency Injection مما يجعل الكود المولد متوافقاً مع أحدث معايير التطوير ويوفر الوقت والجهد في إعداد التطبيقات الجديدة. 