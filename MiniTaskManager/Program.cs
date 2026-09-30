using Microsoft.EntityFrameworkCore;
using MiniTaskManager.Data;
using MiniTaskManager.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// صفحه خطای مرتبط با EF Core فقط در محیط توسعه نمایش داده می‌شود.
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// دیتابیس SQLite در پوشه App_Data (نسبت به Content Root) نگه‌داری می‌شود تا اجرای
// برنامه از طریق Visual Studio یا dotnet run نتیجه یکسانی داشته باشد.
var dataDirectory = Path.Combine(builder.Environment.ContentRootPath, "App_Data");
Directory.CreateDirectory(dataDirectory);
var connectionString = $"Data Source={Path.Combine(dataDirectory, "minitaskmanager.db")}";

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

builder.Services.AddScoped<IDashboardService, DashboardService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
app.UseRouting();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// اعمال Migrationهای در انتظار و درج داده‌های نمونه در اولین اجرا.
await DbInitializer.InitializeAsync(app.Services);

app.Run();
