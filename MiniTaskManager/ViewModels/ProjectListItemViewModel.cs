using System.ComponentModel.DataAnnotations;

namespace MiniTaskManager.ViewModels;

public class ProjectListItemViewModel
{
    public int Id { get; init; }

    [Display(Name = "عنوان")]
    public string Title { get; init; } = string.Empty;

    [Display(Name = "توضیحات")]
    public string? Description { get; init; }

    [Display(Name = "تاریخ ایجاد")]
    public DateTime CreatedAt { get; init; }

    [Display(Name = "فعال")]
    public bool IsActive { get; init; }

    public int TaskCount { get; init; }

    public int DoneTaskCount { get; init; }
}
