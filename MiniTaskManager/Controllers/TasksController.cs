using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiniTaskManager.Data;
using MiniTaskManager.Extensions;
using MiniTaskManager.Models;
using MiniTaskManager.ViewModels;

namespace MiniTaskManager.Controllers;

public class TasksController(AppDbContext context, ILogger<TasksController> logger) : Controller
{
    // GET: /Tasks?search=&projectId=&status=&priority=&sortBy=&sortDescending=
    public async Task<IActionResult> Index(TaskFilterViewModel filter)
    {
        // در بارگذاری اولیه (بدون هیچ پارامتری)، جدیدترین وظایف در ابتدا نمایش داده شوند.
        if (Request.Query.Count == 0)
        {
            filter = new TaskFilterViewModel { SortBy = "createdAt", SortDescending = true };
        }

        filter.SortBy = filter.SortBy?.Trim().ToLowerInvariant() switch
        {
            "duedate" => "dueDate",
            "priority" => "priority",
            "createdat" => "createdAt",
            _ => "createdAt"
        };

        filter.Projects = await context.Projects
            .AsNoTracking()
            .OrderBy(p => p.Title)
            .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Title })
            .ToListAsync();

        var today = DateTime.Today;
        var tasks = await context.Tasks
            .AsNoTracking()
            .Include(t => t.Project)
            .ApplyFilter(filter)
            .ApplySort(filter.SortBy, filter.SortDescending)
            .ToListAsync();

        var viewModel = new TaskListViewModel
        {
            Filter = filter,
            Items = tasks.Select(t => t.ToListItem(today)).ToList()
        };

        return View(viewModel);
    }

    // GET: /Tasks/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null or < 1)
        {
            return NotFound();
        }

        var task = await context.Tasks
            .AsNoTracking()
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task is null)
        {
            return NotFound();
        }

        return View(task.ToListItem(DateTime.Today));
    }

    // GET: /Tasks/Create?projectId=5
    public async Task<IActionResult> Create(int? projectId)
    {
        var model = new TaskFormViewModel();
        if (projectId is > 0)
        {
            model.ProjectId = projectId;
        }

        await PopulateProjectsAsync(model);
        return View(model);
    }

    // POST: /Tasks/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TaskFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateProjectsAsync(model);
            return View(model);
        }

        if (!await ProjectExistsAsync(model.ProjectId))
        {
            return await RejectInvalidProjectAsync(model);
        }

        var task = new TaskItem
        {
            ProjectId = model.ProjectId!.Value,
            Title = model.Title.Trim(),
            Description = model.Description.NullIfWhiteSpace(),
            Priority = model.Priority,
            Status = model.Status,
            DueDate = model.DueDate,
            CreatedAt = DateTime.Now
        };

        try
        {
            context.Tasks.Add(task);
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to create task {TaskTitle}", task.Title);
            ModelState.AddModelError(string.Empty, "هنگام ذخیره وظیفه خطایی رخ داد. لطفاً دوباره تلاش کنید.");
            await PopulateProjectsAsync(model);
            return View(model);
        }

        TempData["SuccessMessage"] = $"وظیفه «{task.Title}» با موفقیت ایجاد شد.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Tasks/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null or < 1)
        {
            return NotFound();
        }

        var task = await context.Tasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        if (task is null)
        {
            return NotFound();
        }

        var model = new TaskFormViewModel
        {
            Id = task.Id,
            ProjectId = task.ProjectId,
            Title = task.Title,
            Description = task.Description,
            Priority = task.Priority,
            Status = task.Status,
            DueDate = task.DueDate
        };

        await PopulateProjectsAsync(model);
        return View(model);
    }

    // POST: /Tasks/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TaskFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            await PopulateProjectsAsync(model);
            return View(model);
        }

        if (!await ProjectExistsAsync(model.ProjectId))
        {
            return await RejectInvalidProjectAsync(model);
        }

        var task = await context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
        if (task is null)
        {
            return NotFound();
        }

        task.ProjectId = model.ProjectId!.Value;
        task.Title = model.Title.Trim();
        task.Description = model.Description.NullIfWhiteSpace();
        task.Priority = model.Priority;
        task.Status = model.Status;
        task.DueDate = model.DueDate;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            if (!await context.Tasks.AnyAsync(t => t.Id == id))
            {
                return NotFound();
            }

            logger.LogError(ex, "Concurrency conflict while updating task {TaskId}", id);
            ModelState.AddModelError(string.Empty, "وظیفه توسط کاربر دیگری تغییر کرده است. لطفاً دوباره تلاش کنید.");
            await PopulateProjectsAsync(model);
            return View(model);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to update task {TaskId}", id);
            ModelState.AddModelError(string.Empty, "هنگام ذخیره تغییرات خطایی رخ داد. لطفاً دوباره تلاش کنید.");
            await PopulateProjectsAsync(model);
            return View(model);
        }

        TempData["SuccessMessage"] = $"وظیفه «{task.Title}» با موفقیت به‌روزرسانی شد.";
        return RedirectToAction(nameof(Details), new { id });
    }

    // GET: /Tasks/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null or < 1)
        {
            return NotFound();
        }

        var task = await context.Tasks
            .AsNoTracking()
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task is null)
        {
            return NotFound();
        }

        return View(task.ToListItem(DateTime.Today));
    }

    // POST: /Tasks/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var task = await context.Tasks.FindAsync(id);
        if (task is not null)
        {
            context.Tasks.Remove(task);
            await context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"وظیفه «{task.Title}» حذف شد.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> ProjectExistsAsync(int? projectId)
        => projectId is > 0 && await context.Projects.AnyAsync(p => p.Id == projectId);

    private async Task<IActionResult> RejectInvalidProjectAsync(TaskFormViewModel model)
    {
        ModelState.AddModelError(nameof(TaskFormViewModel.ProjectId), "پروژه انتخاب‌شده معتبر نیست.");
        await PopulateProjectsAsync(model);
        return View(model);
    }

    private async Task PopulateProjectsAsync(TaskFormViewModel model)
    {
        model.Projects = await context.Projects
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.Title)
            .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Title })
            .ToListAsync();
    }
}
