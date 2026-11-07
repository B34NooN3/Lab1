using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.Areas.Admin.Controllers;

[Area("Admin")] // Kluczowy atrybut definiujący przynależność do obszaru
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}