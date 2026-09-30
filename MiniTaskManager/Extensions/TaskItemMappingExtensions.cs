using MiniTaskManager.Models;
using MiniTaskManager.ViewModels;

namespace MiniTaskManager.Extensions;

public static class TaskItemMappingExtensions
{
    /// <summary>تبدیل موجودیت TaskItem به ViewModel نمایشی؛ Entity مستقیماً به View ارسال نمی‌شود.</summary>
    public static TaskListItemViewModel ToListItem(this TaskItem task, DateTime today) => new()
    {
        Id = task.Id,
        Title = task.Title,
        Description = task.Description,
        ProjectId = task.ProjectId,
        ProjectTitle = task.Project?.Title ?? "-",
        Priority = task.Priority,
        Status = task.Status,
        DueDate = task.DueDate,
        CreatedAt = task.CreatedAt,
        IsOverdue = task.DueDate is { } dueDate && dueDate.Date < today && task.Status != TaskItemStatus.Done
    };
}
