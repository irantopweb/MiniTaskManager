# Mini Task Manager

یک نمونه پروژه (Sample Project) برای مدیریت ساده پروژه‌ها و وظایف، ساخته‌شده با **ASP.NET Core 10 MVC**، **Entity Framework Core** و **SQLite**. هدف این پروژه نمایش کیفیت کدنویسی، معماری Separation of Concerns، استفاده صحیح از EF Core و طراحی یک رابط کاربری ساده، تمیز و Responsive با Bootstrap 5 است.

> [!IMPORTANT]
> این پروژه به‌صورت آزمایشی با کمک هوش مصنوعی و مدل **GLM 5.3 Flash**، بر اساس یک پرامپت ساده برای ارزیابی توانایی مدل در تولید یک برنامه کامل، ساخته شده است. کد، معماری و مستندات پروژه خروجی همین آزمایش هستند و پیش از استفاده در محیط Production باید از نظر امنیت، کارایی و نیازهای کسب‌وکار بازبینی شوند.

## فهرست مطالب

- [تکنولوژی‌ها](#تکنولوژیها)
- [درباره آزمایش هوش مصنوعی](#درباره-آزمایش-هوش-مصنوعی)
- [ساختار پروژه](#ساختار-پروژه)
- [معماری و تصمیمات طراحی](#معماری-و-تصمیمات-طراحی)
- [مدل داده](#مدل-داده)
- [امکانات](#امکانات)
- [نحوه اجرا](#نحوه-اجرا)
- [Migrationها](#migrationها)
- [داده‌های نمونه (Seed)](#داده‌های-نمونه-seed)

## تکنولوژی‌ها

| بخش | تکنولوژی |
|---|---|
| Framework | ASP.NET Core 10 MVC (.NET 10) |
| ORM | Entity Framework Core 10 |
| پایگاه داده | SQLite |
| رابط کاربری | Razor Views + Bootstrap 5 (RTL) + Bootstrap Icons |
| اعتبارسنجی سمت کلاینت | jQuery Validation / Unobtrusive Validation |

## درباره آزمایش هوش مصنوعی

هدف اصلی این مخزن، تست عملی مدل **GLM 5.3 Flash** با یک پرامپت ساده و مشاهده توانایی آن در تولید اجزای یک پروژه وب است؛ از ساختار و مدل داده تا CRUD، رابط کاربری RTL، Migration، Seed و مستندات. بنابراین این مخزن علاوه بر یک نمونه Task Manager، نتیجه یک آزمایش شفاف در زمینه توسعه نرم‌افزار با کمک AI نیز محسوب می‌شود.

- تولید اولیه پروژه با هوش مصنوعی انجام شده است.
- مدل مورد آزمایش: **GLM 5.3 Flash**
- ورودی مدل: یک پرامپت ساده برای ساخت Mini Task Manager
- هدف: ارزیابی خروجی مدل در یک سناریوی واقعی توسعه وب
- وضعیت: پروژه نمونه و آموزشی؛ نه یک محصول آماده Production

## ساختار پروژه

```
MiniTaskManager/
├── Controllers/            # HomeController, ProjectsController, TasksController
├── Models/                 # Project, TaskItem, TaskPriority, TaskItemStatus
├── ViewModels/             # مدل‌های ورودی/خروجی مخصوص هر View (بدون Overposting)
├── Data/
│   ├── AppDbContext.cs     # DbContext و تنظیمات Fluent API
│   ├── DbInitializer.cs    # اجرای Migration + Seed داده نمونه
│   └── Migrations/         # Migrationهای EF Core
├── Services/
│   ├── IDashboardService.cs
│   └── DashboardService.cs # منطق محاسبه آمار داشبورد
├── Extensions/
│   ├── EnumExtensions.cs        # نام نمایشی و کلاس Badge برای Enumها
│   ├── StringExtensions.cs      # کمکی برای Trim/Null کردن رشته‌ها
│   ├── TaskQueryExtensions.cs   # فیلتر و مرتب‌سازی IQueryable<TaskItem>
│   └── TaskItemMappingExtensions.cs # تبدیل Entity به ViewModel
├── Views/                  # صفحات Razor (Home, Projects, Tasks, Shared)
└── wwwroot/                 # Bootstrap, jQuery, CSS سفارشی
```

## معماری و تصمیمات طراحی

- **بدون Repository Pattern**: با توجه به کوچک بودن پروژه و استفاده مستقیم و ساده از `AppDbContext` در کنترلرها (که خودش یک Unit of Work و Abstraction روی دیتابیس است)، افزودن یک لایه Repository صرفاً پیچیدگی بدون مزیت واقعی ایجاد می‌کرد؛ طبق دستور کارفرما از آن صرف‌نظر شد.
- **ViewModel جدا برای هر View**: برای جلوگیری از Overposting و همچنین جداسازی مدل Domain از مدل نمایشی (`TaskFormViewModel`, `ProjectFormViewModel`, `TaskListItemViewModel` و ...). هیچ Entity مستقیماً Bind یا در View استفاده نمی‌شود.
- **DI و Async/Await در همه لایه‌ها**: `AppDbContext`، `IDashboardService` از طریق DI تزریق می‌شوند و تمام دسترسی‌های دیتابیس به‌صورت `async` (با `AsNoTracking` برای Queryهای فقط‌خواندنی) نوشته شده‌اند.
- **Extension Methodها برای منطق تکراری**: فیلتر/مرتب‌سازی Task (`ApplyFilter`, `ApplySort`) و تبدیل Entity به ViewModel (`ToListItem`) به‌صورت Extension Method روی `IQueryable<TaskItem>` نوشته شده تا کنترلر تنها Orchestration را انجام دهد.
- **Error Handling**: تمام عملیات نوشتن (`Create`/`Edit`/`Delete`) در `try/catch` با مدیریت `DbUpdateException` و `DbUpdateConcurrencyException` قرار دارند و پیام خطای مناسب در ModelState برگردانده می‌شود؛ خطاهای مدیریت‌نشده به صفحه `Home/Error` هدایت می‌شوند (هم برای Exception و هم برای کدهای وضعیت مثل 404).
- **Cascade Delete**: با تنظیم `OnDelete(DeleteBehavior.Cascade)` روی رابطه Project → TaskItem، حذف یک پروژه به‌صورت خودکار وظایف مرتبط را نیز حذف می‌کند (این رفتار در صفحه Delete پروژه به کاربر اطلاع داده می‌شود).
- **دیتابیس در `App_Data`**: مسیر فایل SQLite نسبت به `ContentRootPath` محاسبه می‌شود تا اجرا از طریق Visual Studio یا `dotnet run` از هر مسیری رفتار یکسانی داشته باشد.
- **RTL و فارسی‌سازی کامل UI**: `Layout` با `dir="rtl"` و `lang="fa"`، متن‌های فارسی، تاریخ‌های میلادی با فرمت `yyyy/MM/dd` (برای سادگی، بدون افزودن وابستگی تقویم شمسی).

## مدل داده

### Project

| فیلد | نوع | توضیح |
|---|---|---|
| Id | int | کلید اصلی |
| Title | string (≤150) | الزامی |
| Description | string? (≤2000) | اختیاری |
| CreatedAt | DateTime | زمان ایجاد |
| IsActive | bool | فعال/غیرفعال |

### TaskItem

| فیلد | نوع | توضیح |
|---|---|---|
| Id | int | کلید اصلی |
| ProjectId | int | کلید خارجی (الزامی) |
| Title | string (≤150) | الزامی |
| Description | string? (≤2000) | اختیاری |
| Priority | enum: Low, Medium, High | |
| Status | enum: Todo, InProgress, Done | |
| DueDate | DateTime? | تاریخ سررسید (اختیاری) |
| CreatedAt | DateTime | زمان ایجاد |

رابطه **One-to-Many** بین `Project` و `TaskItem` با Cascade Delete برقرار است.

## امکانات

1. **داشبورد** (`/`): تعداد کل پروژه‌ها، کل وظایف، وظایف Todo/InProgress/Done، تعداد وظایف عقب‌افتاده (`DueDate < امروز` و `Status != Done`) و ۵ وظیفه آخر ایجادشده.
2. **مدیریت پروژه‌ها** (`/Projects`): لیست با تعداد وظایف هر پروژه، ایجاد/ویرایش/حذف، صفحه جزئیات با لیست وظایف مرتبط.
3. **مدیریت وظایف** (`/Tasks`): ایجاد/ویرایش/حذف/جزئیات، انتخاب پروژه از Dropdown.
4. **جستجو و فیلتر هم‌زمان**: بر اساس عنوان (Contains)، پروژه، وضعیت و اولویت — همه با هم قابل ترکیب هستند (Query String).
5. **مرتب‌سازی**: بر اساس تاریخ ایجاد، DueDate و Priority (صعودی/نزولی با کلیک روی هدر ستون).
6. **Validation سمت سرور**: `Required`, `StringLength(150)` روی عنوان‌ها، الزامی بودن انتخاب پروژه برای وظیفه، و نمایش خطاها در UI با `asp-validation-for` و `ValidationSummary`.
7. **UI**: Bootstrap 5 (نسخه RTL)، Badge رنگی برای Status و Priority، منوی اصلی (Dashboard/Projects/Tasks)، Responsive (Grid System بوت‌استرپ).
8. **SQLite + EF Core Migrations + Seed خودکار** در اولین اجرا.

## نحوه اجرا

### پیش‌نیاز

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Git (برای دریافت سورس از مخزن)

### با خط فرمان

```powershell
git clone https://github.com/irantopweb/MiniTaskManager.git
cd MiniTaskManager
dotnet restore
dotnet run --project MiniTaskManager/MiniTaskManager.csproj
```

اگر سورس پروژه از قبل روی سیستم شما قرار دارد:

```powershell
cd MiniTaskManager
dotnet restore
dotnet run
```

سپس در مرورگر آدرس نمایش‌داده‌شده در کنسول (به‌صورت پیش‌فرض `http://localhost:5245` یا `https://localhost:7255`) را باز کنید. آدرس‌ها در `MiniTaskManager/Properties/launchSettings.json` قابل تغییر هستند.

### با Visual Studio

فایل `MiniTaskManager.slnx` را باز کرده و روی دکمه Run/F5 کلیک کنید (پروژه به‌صورت پیش‌فرض Startup Project است).

> در اولین اجرا، برنامه به‌صورت خودکار Migrationهای در انتظار را روی یک فایل SQLite در پوشه `App_Data` (کنار فایل‌های پروژه) اعمال کرده و چند پروژه/وظیفه نمونه Seed می‌کند. برای شروع از صفر، برنامه را متوقف و پوشه `MiniTaskManager/App_Data` را حذف کنید؛ این پوشه در اجرای بعدی دوباره ساخته می‌شود.

## Migrationها

Migration اولیه (`InitialCreate`) از قبل در مسیر `Data/Migrations` ایجاد شده و در Program.cs با `context.Database.MigrateAsync()` به‌صورت خودکار در هر اجرا اعمال می‌شود (نیازی به اجرای دستی `dotnet ef database update` نیست).

برای ایجاد Migration جدید (در صورت تغییر مدل‌ها):

```powershell
cd MiniTaskManager
dotnet ef migrations add <MigrationName> --output-dir Data/Migrations
```

## داده‌های نمونه (Seed)

در اولین اجرا (وقتی جدول `Projects` خالی باشد)، ۳ پروژه نمونه با مجموعاً ۱۲ وظیفه (با وضعیت‌ها، اولویت‌ها و سررسیدهای متنوع — از جمله چند وظیفه عمداً عقب‌افتاده برای تست شمارنده داشبورد) در `DbInitializer.cs` درج می‌شوند.



