namespace MiniTaskManager.ViewModels;

public class ProjectDetailsViewModel
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public DateTime CreatedAt { get; init; }

    public bool IsActive { get; init; }

    public int TodoCount { get; init; }

    public int InProgressCount { get; init; }

    public int DoneCount { get; init; }

    public IReadOnlyList<TaskListItemViewModel> Tasks { get; init; } = [];
}
