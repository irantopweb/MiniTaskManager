using System.ComponentModel.DataAnnotations;
using System.Reflection;
using MiniTaskManager.Models;

namespace MiniTaskManager.Extensions;

public static class EnumExtensions
{
    /// <summary>نام نمایشی عضو Enum را بر اساس ویژگی [Display] برمی‌گرداند.</summary>
    public static string GetDisplayName(this Enum value)
    {
        var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
        return member?.GetCustomAttribute<DisplayAttribute>()?.Name ?? value.ToString();
    }

    /// <summary>کلاس Badge مناسب برای هر وضعیت وظیفه.</summary>
    public static string GetBadgeClass(this TaskItemStatus status) => status switch
    {
        TaskItemStatus.Todo => "bg-secondary",
        TaskItemStatus.InProgress => "bg-warning text-dark",
        TaskItemStatus.Done => "bg-success",
        _ => "bg-light text-dark"
    };

    /// <summary>کلاس Badge مناسب برای هر اولویت وظیفه.</summary>
    public static string GetBadgeClass(this TaskPriority priority) => priority switch
    {
        TaskPriority.Low => "bg-info text-dark",
        TaskPriority.Medium => "bg-primary",
        TaskPriority.High => "bg-danger",
        _ => "bg-light text-dark"
    };
}
