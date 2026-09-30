using Microsoft.EntityFrameworkCore;
using MiniTaskManager.Data;
using MiniTaskManager.Extensions;
using MiniTaskManager.Models;
using MiniTaskManager.ViewModels;

namespace MiniTaskManager.Services;

public class DashboardService(AppDbContext context) : IDashboardService
{
    public async Task<DashboardViewModel> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;

        // وظیفه عقب‌افتاده: DueDate < امروز و وضعیت آن Done نباشد.
        var totalTasks = await context.Tasks.CountAsync(cancellationToken);
        var todoCount = await context.Tasks.CountAsync(t => t.Status == TaskItemStatus.Todo, cancellationToken);
        var inProgressCount = await context.Tasks.CountAsync(t => t.Status == TaskItemStatus.InProgress, cancellationToken);
        var doneCount = await context.Tasks.CountAsync(t => t.Status == TaskItemStatus.Done, cancellationToken);
        var overdueCount = await context.Tasks.CountAsync(t => t.DueDate < today && t.Status != TaskItemStatus.Done, cancellationToken);

        var recentTasks = await context.Tasks
            .AsNoTracking()
            .Include(t => t.Project)
            .OrderByDescending(t => t.CreatedAt)
            .ThenByDescending(t => t.Id)
            .Take(5)
            .ToListAsync(cancellationToken);

        return new DashboardViewModel
        {
            TotalProjects = await context.Projects.CountAsync(cancellationToken),
            TotalTasks = totalTasks,
            TodoCount = todoCount,
            InProgressCount = inProgressCount,
            DoneCount = doneCount,
            OverdueCount = overdueCount,
            RecentTasks = recentTasks.Select(t => t.ToListItem(today)).ToList()
        };
    }
}
