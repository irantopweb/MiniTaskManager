using System.ComponentModel.DataAnnotations;

namespace MiniTaskManager.ViewModels;

/// <summary>ViewModel فرم ایجاد/ویرایش پروژه (جلوگیری از Overposting).</summary>
public class ProjectFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "عنوان پروژه الزامی است.")]
    [StringLength(150, ErrorMessage = "حداکثر طول عنوان ۱۵۰ کاراکتر است.")]
    [Display(Name = "عنوان")]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "حداکثر طول توضیحات ۲۰۰۰ کاراکتر است.")]
    [Display(Name = "توضیحات")]
    public string? Description { get; set; }

    [Display(Name = "فعال")]
    public bool IsActive { get; set; } = true;
}
