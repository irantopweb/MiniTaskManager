using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MiniTaskManager.Models;

namespace MiniTaskManager.Data;

/// <summary>
/// اعمال Migrationهای در انتظار و درج داده‌های نمونه در اولین اجرای برنامه.
/// </summary>
public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        logger.LogInformation("Applying pending database migrations...");
        await context.Database.MigrateAsync();

        if (await context.Projects.AnyAsync())
        {
            logger.LogInformation("Database already contains data; skipping seed.");
            return;
        }

        logger.LogInformation("Seeding sample data...");
        context.Projects.AddRange(BuildSampleData());
        await context.SaveChangesAsync();
        logger.LogInformation("Sample data seeded successfully.");
    }

    private static List<Project> BuildSampleData()
    {
        var now = DateTime.Now;
        var today = DateTime.Today;

        var onlineStore = new Project
        {
            Title = "وب‌سایت فروشگاه اینترنتی",
            Description = "پیاده‌سازی فروشگاه آنلاین شامل سبد خرید، جستجوی محصولات و درگاه پرداخت.",
            CreatedAt = now.AddDays(-30),
            IsActive = true
        };

        var financeApp = new Project
        {
            Title = "اپلیکیشن مدیریت مالی شخصی",
            Description = "اپلیکیشن موبایل برای ثبت و تحلیل هزینه‌های روزانه و تولید گزارش‌های ماهانه.",
            CreatedAt = now.AddDays(-15),
            IsActive = true
        };

        var bookingSystem = new Project
        {
            Title = "سامانه رزرو جلسات",
            Description = "سامانه داخلی برای رزرو اتاق جلسات و ارسال یادآوری به کارمندان.",
            CreatedAt = now.AddDays(-8),
            IsActive = false
        };

        onlineStore.Tasks.Add(new TaskItem { Title = "طراحی صفحه اصلی", Description = "طراحی رابط کاربری صفحه اصلی بر اساس پروتوتایپ.", Priority = TaskPriority.High, Status = TaskItemStatus.Done, DueDate = today.AddDays(-18), CreatedAt = now.AddDays(-25) });
        onlineStore.Tasks.Add(new TaskItem { Title = "پیاده‌سازی سبد خرید", Description = "افزودن/حذف محصول، محاسبه مبلغ کل و اعمال کد تخفیف.", Priority = TaskPriority.High, Status = TaskItemStatus.InProgress, DueDate = today.AddDays(5), CreatedAt = now.AddDays(-12) });
        onlineStore.Tasks.Add(new TaskItem { Title = "اتصال درگاه پرداخت", Description = null, Priority = TaskPriority.Medium, Status = TaskItemStatus.Todo, DueDate = today.AddDays(14), CreatedAt = now.AddDays(-10) });
        onlineStore.Tasks.Add(new TaskItem { Title = "رفع باگ جستجوی محصولات", Description = "جستجو در دسته‌بندی‌ها نتیجه درستی برنمی‌گرداند.", Priority = TaskPriority.High, Status = TaskItemStatus.Todo, DueDate = today.AddDays(-2), CreatedAt = now.AddDays(-6) });
        onlineStore.Tasks.Add(new TaskItem { Title = "بهینه‌سازی سرعت لود صفحات", Description = null, Priority = TaskPriority.Low, Status = TaskItemStatus.Todo, DueDate = today.AddDays(21), CreatedAt = now.AddDays(-3) });

        financeApp.Tasks.Add(new TaskItem { Title = "طراحی داشبورد حساب کاربری", Description = "نمودار مصرف ماهانه و دسته‌بندی هزینه‌ها.", Priority = TaskPriority.Medium, Status = TaskItemStatus.Done, DueDate = today.AddDays(-6), CreatedAt = now.AddDays(-14) });
        financeApp.Tasks.Add(new TaskItem { Title = "ایجاد گزارش‌های ماهانه", Description = "خروجی PDF و اشتراک‌گذاری گزارش.", Priority = TaskPriority.Medium, Status = TaskItemStatus.InProgress, DueDate = today.AddDays(3), CreatedAt = now.AddDays(-9) });
        financeApp.Tasks.Add(new TaskItem { Title = "پیاده‌سازی ورود دو مرحله‌ای", Description = "تأیید با پیامک برای افزایش امنیت.", Priority = TaskPriority.High, Status = TaskItemStatus.Todo, DueDate = today.AddDays(-1), CreatedAt = now.AddDays(-5) });
        financeApp.Tasks.Add(new TaskItem { Title = "خروجی اکسل تراکنش‌ها", Description = null, Priority = TaskPriority.Low, Status = TaskItemStatus.Todo, DueDate = today.AddDays(25), CreatedAt = now.AddDays(-2) });

        bookingSystem.Tasks.Add(new TaskItem { Title = "طراحی تقویم رزرو", Description = "نمای هفتگی و ماهانه اتاق‌ها.", Priority = TaskPriority.Medium, Status = TaskItemStatus.InProgress, DueDate = today.AddDays(7), CreatedAt = now.AddDays(-4) });
        bookingSystem.Tasks.Add(new TaskItem { Title = "ارسال ایمیل یادآوری جلسه", Description = null, Priority = TaskPriority.Medium, Status = TaskItemStatus.Todo, DueDate = today.AddDays(2), CreatedAt = now.AddDays(-2) });
        bookingSystem.Tasks.Add(new TaskItem { Title = "آموزش استفاده از سامانه", Description = "تهیه راهنمای متنی و ویدئویی برای کارمندان.", Priority = TaskPriority.Low, Status = TaskItemStatus.Done, DueDate = today.AddDays(-9), CreatedAt = now.AddDays(-13) });

        return [onlineStore, financeApp, bookingSystem];
    }
}
