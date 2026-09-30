using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniTaskManager.Data;
using MiniTaskManager.Extensions;
using MiniTaskManager.Models;
using MiniTaskManager.ViewModels;

namespace MiniTaskManager.Controllers;

public class ProjectsController(AppDbContext context, ILogger<ProjectsController> logger) : Controller
{
    // GET: /Projects
    public async Task<IActionResult> Index()
    {
        var projects = await context.Projects
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProjectListItemViewModel
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                IsActive = p.IsActive,
                TaskCount = p.Tasks.Count(),
                DoneTaskCount = p.Tasks.Count(t => t.Status == TaskItemStatus.Done)
            })
            .ToListAsync();

        return View(projects);
    }

    // GET: /Projects/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null or < 1)
        {
            return NotFound();
        }

        var project = await context.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (project is null)
        {
            return NotFound();
        }

        var today = DateTime.Today;
        var tasks = await context.Tasks
            .AsNoTracking()
            .Where(t => t.ProjectId == id)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        var viewModel = new ProjectDetailsViewModel
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            CreatedAt = project.CreatedAt,
            IsActive = project.IsActive,
            TodoCount = tasks.Count(t => t.Status == TaskItemStatus.Todo),
            InProgressCount = tasks.Count(t => t.Status == TaskItemStatus.InProgress),
            DoneCount = tasks.Count(t => t.Status == TaskItemStatus.Done),
            Tasks = tasks.Select(t => t.ToListItem(today)).ToList()
        };

        return View(viewModel);
    }

    // GET: /Projects/Create
    public IActionResult Create() => View(new ProjectFormViewModel());

    // POST: /Projects/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var project = new Project
        {
            Title = model.Title.Trim(),
            Description = model.Description.NullIfWhiteSpace(),
            CreatedAt = DateTime.Now,
            IsActive = model.IsActive
        };

        try
        {
            context.Projects.Add(project);
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to create project {ProjectTitle}", project.Title);
            ModelState.AddModelError(string.Empty, "هنگام ذخیره پروژه خطایی رخ داد. لطفاً دوباره تلاش کنید.");
            return View(model);
        }

        TempData["SuccessMessage"] = $"پروژه «{project.Title}» با موفقیت ایجاد شد.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Projects/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null or < 1)
        {
            return NotFound();
        }

        var project = await context.Projects.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (project is null)
        {
            return NotFound();
        }

        return View(new ProjectFormViewModel
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            IsActive = project.IsActive
        });
    }

    // POST: /Projects/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProjectFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var project = await context.Projects.FirstOrDefaultAsync(p => p.Id == id);
        if (project is null)
        {
            return NotFound();
        }

        project.Title = model.Title.Trim();
        project.Description = model.Description.NullIfWhiteSpace();
        project.IsActive = model.IsActive;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            if (!await context.Projects.AnyAsync(p => p.Id == id))
            {
                return NotFound();
            }

            logger.LogError(ex, "Concurrency conflict while updating project {ProjectId}", id);
            ModelState.AddModelError(string.Empty, "پروژه توسط کاربر دیگری تغییر کرده است. لطفاً دوباره تلاش کنید.");
            return View(model);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to update project {ProjectId}", id);
            ModelState.AddModelError(string.Empty, "هنگام ذخیره تغییرات خطایی رخ داد. لطفاً دوباره تلاش کنید.");
            return View(model);
        }

        TempData["SuccessMessage"] = $"پروژه «{project.Title}» با موفقیت به‌روزرسانی شد.";
        return RedirectToAction(nameof(Details), new { id });
    }

    // GET: /Projects/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null or < 1)
        {
            return NotFound();
        }

        var project = await context.Projects
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProjectListItemViewModel
            {
                Id = p.Id,
                Title = p.Title,
                CreatedAt = p.CreatedAt,
                TaskCount = p.Tasks.Count()
            })
            .FirstOrDefaultAsync();

        if (project is null)
        {
            return NotFound();
        }

        return View(project);
    }

    // POST: /Projects/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var project = await context.Projects.FindAsync(id);
        if (project is not null)
        {
            // با Cascade Delete، وظایف مرتبط با پروژه هم حذف می‌شوند.
            context.Projects.Remove(project);
            await context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"پروژه «{project.Title}» و وظایف مرتبط با آن حذف شد.";
        }

        return RedirectToAction(nameof(Index));
    }
}
