namespace MiniTaskManager.ViewModels;

public class DashboardViewModel
{
    public int TotalProjects { get; init; }

    public int TotalTasks { get; init; }

    public int TodoCount { get; init; }

    public int InProgressCount { get; init; }

    public int DoneCount { get; init; }

    public int OverdueCount { get; init; }

    public IReadOnlyList<TaskListItemViewModel> RecentTasks { get; init; } = [];
}
