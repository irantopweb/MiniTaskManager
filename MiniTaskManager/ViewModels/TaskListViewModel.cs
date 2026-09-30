namespace MiniTaskManager.ViewModels;

/// <summary>مدل کامل صفحه لیست وظایف (فیلترها + نتایج).</summary>
public class TaskListViewModel
{
    public TaskFilterViewModel Filter { get; init; } = new();

    public IReadOnlyList<TaskListItemViewModel> Items { get; init; } = [];
}
