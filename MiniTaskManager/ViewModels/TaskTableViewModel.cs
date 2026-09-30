namespace MiniTaskManager.ViewModels;

/// <summary>مدل جدول وظایف که بین صفحات مختلف به اشتراک گذاشته می‌شود.</summary>
public class TaskTableViewModel
{
    public IReadOnlyList<TaskListItemViewModel> Items { get; init; } = [];

    public bool ShowProject { get; init; } = true;

    public string EmptyMessage { get; init; } = "هنوز وظیفه‌ای ثبت نشده است.";

    /// <summary>
    /// وقتی مقدار داشته باشد (صفحه لیست وظایف)، هدر ستون‌های مرتب‌سازی‌پذیر لینک‌دار می‌شوند.
    /// </summary>
    public TaskFilterViewModel? Filter { get; init; }
}
