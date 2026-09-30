using System.ComponentModel.DataAnnotations;

namespace MiniTaskManager.Models;

/// <summary>اولویت یک وظیفه.</summary>
public enum TaskPriority
{
    [Display(Name = "کم")]
    Low = 1,

    [Display(Name = "متوسط")]
    Medium = 2,

    [Display(Name = "زیاد")]
    High = 3
}
