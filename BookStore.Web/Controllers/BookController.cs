using System.Diagnostics;
using System.Text;
using BookStore.Web.Filters;
using Microsoft.AspNetCore.Mvc;
using BookStore.Web.Models;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace BookStore.Web.Controllers;
[ServiceFilter(typeof(LogActionFilter))] 
public class BookController : Controller
{
    //Lab 3
    private readonly IActionDescriptorCollectionProvider _provider;

    public BookController(IActionDescriptorCollectionProvider provider)
    {
        _provider = provider;
    }
    // GET /Book/Hello  → zwykły tekst
    public IActionResult Hello() => Content("Witaj w BookStore!", "text/plain; charset=utf-8");

    // GET /Book/Status?code=418 → dowolny kod statusu
    public IActionResult Status(int code = 200) => StatusCode(code);
    //Lab 3 routing
    [HttpGet("Book/Details/{id:int}")]
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
        return Content(Request.Headers.UserAgent.ToString());
    }

    public IActionResult Health()
    {
        return Json(new { status = "ok", time = DateTime.Now });
    }

    public IActionResult Index()
    {
        return View(BookStoreMemory.Books);
    }

    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Create(BookFormViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        BookStoreMemory.Books.Add(new Book
        {
            Id = BookStoreMemory.Books.Max(b => b.Id) + 1,
            Title = vm.Title,
            Isbn = vm.Isbn,
            PublicationDate = vm.PublicationDate,
            NumPages = vm.NumPages,
            Price = vm.Price,
            Author = vm.Author
        });

        TempData["Success"] = $"Dodano książkę „{vm.Title}”.";
        return RedirectToAction(nameof(Index));
    }
    //Lab 3
    public IActionResult ByYear(int year)
    {
        // Filtruje książki, których data wydania (PublicationDate) zgadza się z podanym rokiem
        var books = BookStoreMemory.Books.Where(b => b.PublicationDate.HasValue && b.PublicationDate.Value.Year == year).ToList();
    
        // Zwraca przefiltrowaną listę korzystając z istniejącego widoku Index
        return View("Index", books); 
    }

    public IActionResult ByIsbn(string isbn)
    {
        var book = BookStoreMemory.Books.FirstOrDefault(b => b.Isbn == isbn);

        if (book == null)
        {
            return NotFound();
        }
        return RedirectToAction("Details", new { id = book.Id }); 
    }
    
    public IActionResult Docs(string path)
    {
        return Content($"path = {path}");
    }
    
    public IActionResult Routes()
    {
        var routes = _provider.ActionDescriptors.Items.Select(x => new
        {
            Action = x.RouteValues["Action"],
            Controller = x.RouteValues["Controller"],
            Name = x.AttributeRouteInfo?.Name,
            Template = x.AttributeRouteInfo?.Template,
            HttpMethods = x.ActionConstraints?.OfType<HttpMethodActionConstraint>().FirstOrDefault()?.HttpMethods
        }).ToList();

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("ADRES (Szablon) | KONTROLER.AKCJA | METODA HTTP");
        sb.AppendLine("-------------------------------------------------");

        foreach (var r in routes)
        {
            // Pobieranie metody HTTP (np. GET, POST)
            string httpMethod = r.HttpMethods != null ? string.Join(", ", r.HttpMethods) : "Dowolna";
        
            // Formatowanie wiersza
            sb.AppendLine($"{r.Template ?? "Trasa konwencjonalna"} | {r.Controller}.{r.Action} | {httpMethod}");
        }

        return Content(sb.ToString(), "text/plain; charset=utf-8");
    }
}
