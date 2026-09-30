using System.ComponentModel.DataAnnotations;

namespace MiniTaskManager.Models;

/// <summary>وضعیت انجام یک وظیفه.</summary>
public enum TaskItemStatus
{
    [Display(Name = "در انتظار انجام")]
    Todo = 1,

    [Display(Name = "در حال انجام")]
    InProgress = 2,

    [Display(Name = "انجام شده")]
    Done = 3
}
