using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniTaskManager.Models;

namespace MiniTaskManager.ViewModels;

/// <summary>فیلترهای صفحه لیست وظایف؛ هم به‌عنوان ورودی GET بایند می‌شود هم Dropdownها را نگه می‌دارد.</summary>
public class TaskFilterViewModel
{
    [Display(Name = "جستجو")]
    public string? Search { get; set; }

    [Display(Name = "پروژه")]
    public int? ProjectId { get; set; }

    [Display(Name = "وضعیت")]
    public TaskItemStatus? Status { get; set; }

    [Display(Name = "اولویت")]
    public TaskPriority? Priority { get; set; }

    [Display(Name = "ترتیب")]
    public string? SortBy { get; set; }

    [Display(Name = "جهت")]
    public bool SortDescending { get; set; }

    public IEnumerable<SelectListItem>? Projects { get; set; }
}
