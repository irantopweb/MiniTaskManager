using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MiniTaskManager.Services;
using MiniTaskManager.ViewModels;

namespace MiniTaskManager.Controllers;

public class HomeController(IDashboardService dashboardService, ILogger<HomeController> logger) : Controller
{
    // GET: /
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var dashboard = await dashboardService.GetDashboardAsync(cancellationToken);
        return View(dashboard);
    }

    // این Action هم برای خطاهای مدیریت‌نشده (UseExceptionHandler) و هم برای
    // کدهای وضعیت مثل 404 (UseStatusCodePagesWithReExecute) اجرا می‌شود.
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int? statusCode)
    {
        var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionFeature?.Error is not null)
        {
            logger.LogError(exceptionFeature.Error, "Unhandled exception while requesting {Path}", exceptionFeature.Path);
        }

        Response.StatusCode = statusCode ?? StatusCodes.Status500InternalServerError;

        return View(new ErrorViewModel
        {
            StatusCode = Response.StatusCode,
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
