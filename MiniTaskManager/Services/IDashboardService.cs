using MiniTaskManager.ViewModels;

namespace MiniTaskManager.Services;

public interface IDashboardService
{
    /// <summary>آمار کلی داشبورد و ۵ وظیفه آخر را محاسبه و برمی‌گرداند.</summary>
    Task<DashboardViewModel> GetDashboardAsync(CancellationToken cancellationToken = default);
}
