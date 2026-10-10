using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using BookStore.Web.Models;

namespace BookStore.Web.Controllers;

public class BookController : Controller
{
    // GET /Book/Hello  → zwykły tekst
    public IActionResult Hello() => Content("Witaj w BookStore!", "text/plain; charset=utf-8");

    // GET /Book/Status?code=418 → dowolny kod statusu
    public IActionResult Status(int code = 200) => StatusCode(code);

    // GET /Book/Details/3 → widok
    public IActionResult Details(int id)
    {
        ViewBag.Id = id;
        return View();
    }
// (1) query string:  /Book/Echo?name=Ala&count=3
    public IActionResult Echo([FromQuery] string name = "nieznajomy", [FromQuery] int count = 1) =>
        Content(string.Join("\n", Enumerable.Repeat($"Cześć, {name}!", Math.Clamp(count, 1, 20))));

// (2) zmienna ścieżki:  /Book/Square/7   (segment {id} z domyślnej trasy)
    public IActionResult Square(int id) => Content($"{id}^2 = {id * id}");

// (3) ciało żądania JSON:  POST /Book/Import
    [HttpPost]
    public IActionResult Import([FromBody] BookDto dto) =>
        Json(new { received = dto.Title, dto.Isbn, dto.Price });

// (4) surowy obiekt Request
    public IActionResult Inspect()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Method: {Request.Method}");
        sb.AppendLine($"Path:   {Request.Path}{Request.QueryString}");
        foreach (var h in Request.Headers) sb.AppendLine($"{h.Key}: {h.Value}");
        return Content(sb.ToString(), "text/plain; charset=utf-8");
    }

    public IActionResult Sum(int a, int b)
    {
        int wynik = a + b;
        return Content(wynik.ToString());
    }

    public IActionResult UserAgent()
    {
        return Content (Request.Headers.UserAgent.ToString());
    }

    public IActionResult Health()
    {
        return Json(new { status = "ok", time = DateTime.Now });
    }
}

