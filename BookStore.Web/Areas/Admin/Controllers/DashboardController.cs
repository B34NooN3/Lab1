using BookStore.Web.Middleware;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class DashboardController : Controller
{
    private readonly RequestStats _stats;

    public DashboardController(RequestStats stats)
    {
        _stats = stats;
    }

    public IActionResult Index()
    {
        var topPaths = _stats.GetTop5Paths();
        return View(topPaths);
    }
}