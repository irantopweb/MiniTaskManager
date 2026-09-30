using System.ComponentModel.DataAnnotations;
using MiniTaskManager.Models;

namespace MiniTaskManager.ViewModels;

/// <summary>نسخه نمایشی امن از TaskItem برای استفاده در Viewها.</summary>
public class TaskListItemViewModel
{
    public int Id { get; init; }

    [Display(Name = "عنوان")]
    public string Title { get; init; } = string.Empty;

    [Display(Name = "توضیحات")]
    public string? Description { get; init; }

    public int ProjectId { get; init; }

    [Display(Name = "پروژه")]
    public string ProjectTitle { get; init; } = string.Empty;

    [Display(Name = "اولویت")]
    public TaskPriority Priority { get; init; }

    [Display(Name = "وضعیت")]
    public TaskItemStatus Status { get; init; }

    [Display(Name = "تاریخ سررسید")]
    public DateTime? DueDate { get; init; }

    [Display(Name = "تاریخ ایجاد")]
    public DateTime CreatedAt { get; init; }

    public bool IsOverdue { get; init; }
}
