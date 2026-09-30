using MiniTaskManager.Models;
using MiniTaskManager.ViewModels;

namespace MiniTaskManager.Extensions;

public static class TaskQueryExtensions
{
    /// <summary>فیلترهای جستجو، پروژه، وضعیت و اولویت را (هم‌زمان و اختیاری) اعمال می‌کند.</summary>
    public static IQueryable<TaskItem> ApplyFilter(this IQueryable<TaskItem> query, TaskFilterViewModel filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var searchTerm = filter.Search.Trim();
            query = query.Where(t => t.Title.Contains(searchTerm));
        }

        if (filter.ProjectId is > 0)
        {
            query = query.Where(t => t.ProjectId == filter.ProjectId);
        }

        if (filter.Status is not null)
        {
            query = query.Where(t => t.Status == filter.Status);
        }

        if (filter.Priority is not null)
        {
            query = query.Where(t => t.Priority == filter.Priority);
        }

        return query;
    }

    /// <summary>مرتب‌سازی بر اساس ستون و جهت انتخاب‌شده؛ حالت پیش‌فرض، تاریخ ایجاد است.</summary>
    public static IQueryable<TaskItem> ApplySort(this IQueryable<TaskItem> query, string sortBy, bool sortDescending)
        => (sortBy, sortDescending) switch
        {
            ("dueDate", false) => query.OrderBy(t => t.DueDate).ThenBy(t => t.CreatedAt),
            ("dueDate", true) => query.OrderByDescending(t => t.DueDate).ThenByDescending(t => t.CreatedAt),
            ("priority", false) => query.OrderBy(t => t.Priority).ThenBy(t => t.CreatedAt),
            ("priority", true) => query.OrderByDescending(t => t.Priority).ThenByDescending(t => t.CreatedAt),
            (_, false) => query.OrderBy(t => t.CreatedAt).ThenBy(t => t.Id),
            _ => query.OrderByDescending(t => t.CreatedAt).ThenByDescending(t => t.Id)
        };
}
