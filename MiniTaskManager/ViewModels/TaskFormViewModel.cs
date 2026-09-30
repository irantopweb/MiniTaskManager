using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniTaskManager.Models;

namespace MiniTaskManager.ViewModels;

/// <summary>
/// ViewModel فرم ایجاد/ویرایش وظیفه؛ تنها فیلدهای قابل ویرایش را در خود دارد
/// تا از Overposting جلوگیری شود (فیلدهایی مثل CreatedAt هرگز بایند نمی‌شوند).
/// </summary>
public class TaskFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "انتخاب پروژه الزامی است.")]
    [Display(Name = "پروژه")]
    public int? ProjectId { get; set; }

    [Required(ErrorMessage = "عنوان وظیفه الزامی است.")]
    [StringLength(150, ErrorMessage = "حداکثر طول عنوان ۱۵۰ کاراکتر است.")]
    [Display(Name = "عنوان")]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "حداکثر طول توضیحات ۲۰۰۰ کاراکتر است.")]
    [Display(Name = "توضیحات")]
    public string? Description { get; set; }

    [Display(Name = "اولویت")]
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    [Display(Name = "وضعیت")]
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;

    [Display(Name = "تاریخ سررسید")]
    public DateTime? DueDate { get; set; }

    public IEnumerable<SelectListItem>? Projects { get; set; }
}
