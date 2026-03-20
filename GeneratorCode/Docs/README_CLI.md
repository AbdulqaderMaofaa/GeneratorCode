# واجهة سطر الأوامر لمولد الكود المتطور

## الاستخدام الأساسي

```bash
GeneratorCode.exe --server localhost --database MyDatabase --output C:\Output
```

## المعاملات المتاحة

| المعامل | الوصف | القيمة الافتراضية |
|---------|--------|-------------------|
| --server | اسم السيرفر | - |
| --database | اسم قاعدة البيانات | - |
| --db-type | نوع قاعدة البيانات (SqlServer, MySQL, PostgreSQL) | SqlServer |
| --namespace | مساحة الأسماء للكود المولد | GeneratedCode |
| --pattern | نمط البنية المعمارية | CleanArchitecture |
| --output | مسار حفظ الملفات المولدة | - |
| --enable-di | تفعيل Dependency Injection | true |
| --async | توليد عمليات غير متزامنة | true |
| --tests | توليد اختبارات وحدة | false |

## أمثلة

### توليد كود لقاعدة بيانات SQL Server
```bash
GeneratorCode.exe --server localhost --database Northwind --output C:\Output
```

### توليد كود لقاعدة بيانات MySQL مع نمط CQRS
```bash
GeneratorCode.exe --server localhost --database shop --db-type MySQL --pattern CQRS --output C:\Output
```

### توليد كود مع اختبارات وحدة
```bash
GeneratorCode.exe --server localhost --database MyApp --tests true --output C:\Output
```

### توليد كود بدون Dependency Injection
```bash
GeneratorCode.exe --server localhost --database Legacy --enable-di false --output C:\Output
```

## ملاحظات
- يجب تحديد المعاملات الإلزامية: server, database, output
- يمكن استخدام -h أو --help لعرض المساعدة
- في حالة عدم تحديد أي معاملات، سيتم تشغيل واجهة المستخدم الرسومية 