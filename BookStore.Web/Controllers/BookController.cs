using System.Text;
using BookStore.Web.Filters;
using BookStore.Web.Models;
using BookStore.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace BookStore.Web.Controllers;

[ServiceFilter(typeof(LogActionFilter))]
public class BookController : Controller
{
    // Lab 3 + Lab 4
    private readonly IActionDescriptorCollectionProvider _provider;
    private readonly IBookService _books;

    public BookController(IActionDescriptorCollectionProvider provider, IBookService books)
    {
        _provider = provider;
        _books = books;
    }

    // GET /Book/Hello → zwykły tekst
    public IActionResult Hello() => Content("Witaj w BookStore!", "text/plain; charset=utf-8");

    // GET /Book/Status?code=418 → dowolny kod statusu
    public IActionResult Status(int code = 200) => StatusCode(code);

    // Lab 3 routing + Lab 4 (pobieranie z bazy)
    [HttpGet("Book/Details/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var book = await _books.GetByIdAsync(id);
        if (book == null)
            return NotFound();

        return View(book);
    }

    // (1) query string: /Book/Echo?name=Ala&count=3
    public IActionResult Echo([FromQuery] string name = "nieznajomy", [FromQuery] int count = 1) =>
        Content(string.Join("\n", Enumerable.Repeat($"Cześć, {name}!", Math.Clamp(count, 1, 20))));

    // (2) zmienna ścieżki: /Book/Square/7
    public IActionResult Square(int id) => Content($"{id}^2 = {id * id}");

    // (3) ciało żądania JSON: POST /Book/Import
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

    public IActionResult Sum(int a, int b) => Content((a + b).ToString());

    public IActionResult UserAgent() => Content(Request.Headers.UserAgent.ToString());

    public IActionResult Health() => Json(new { status = "ok", time = DateTime.Now });

    // === LAB 4: PEŁNY CRUD ===

    public async Task<IActionResult> Index(int page = 1, int pageSize = 5, string? search = null, string? sort = null, string? dir = null)
    {
        if (page < 1) page = 1;
        ViewBag.Search = search;
        ViewBag.Sort = sort;
        ViewBag.Dir = dir;

        var paged = await _books.GetPagedAsync(page, pageSize, search, sort, dir);
        return View(paged);
    }

    public IActionResult Create()
    {
        return View(new BookFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookFormViewModel vm)
    {
        if (await _books.IsbnExistsAsync(vm.Isbn))
            ModelState.AddModelError(nameof(vm.Isbn), "Książka z takim numerem ISBN już istnieje.");

        if (!ModelState.IsValid)
            return View(vm);

        var book = new Book
        {
            Title = vm.Title,
            Isbn = vm.Isbn,
            PublicationDate = vm.PublicationDate,
            NumPages = vm.NumPages,
            Price = vm.Price,
            Author = vm.Author
        };

        await _books.CreateAsync(book);
        TempData["Success"] = $"Dodano książkę „{vm.Title}”.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var book = await _books.GetByIdAsync(id);
        if (book == null)
            return NotFound();

        var vm = new BookFormViewModel
        {
            Id = book.Id,
            Title = book.Title,
            Isbn = book.Isbn,
            PublicationDate = book.PublicationDate,
            NumPages = book.NumPages,
            Price = book.Price,
            Author = book.Author
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BookFormViewModel vm)
    {
        if (id != vm.Id)
            return BadRequest();

        if (await _books.IsbnExistsAsync(vm.Isbn, id))
            ModelState.AddModelError(nameof(vm.Isbn), "Inna książka ma już ten numer ISBN.");

        if (!ModelState.IsValid)
            return View(vm);

        var book = new Book
        {
            Id = vm.Id,
            Title = vm.Title,
            Isbn = vm.Isbn,
            PublicationDate = vm.PublicationDate,
            NumPages = vm.NumPages,
            Price = vm.Price,
            Author = vm.Author
        };

        await _books.UpdateAsync(book);
        TempData["Success"] = $"Zaktualizowano książkę „{vm.Title}”.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var book = await _books.GetByIdAsync(id);
        if (book == null)
            return NotFound();

        return View(book);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _books.DeleteAsync(id);
        TempData["Success"] = "Usunięto książkę.";
        return RedirectToAction(nameof(Index));
    }

    // === LAB 3 ===

    public IActionResult Docs(string path) => Content($"path = {path}");

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

        var sb = new StringBuilder();
        sb.AppendLine("ADRES (Szablon) | KONTROLER.AKCJA | METODA HTTP");
        sb.AppendLine("-------------------------------------------------");

        foreach (var r in routes)
        {
            string httpMethod = r.HttpMethods != null ? string.Join(", ", r.HttpMethods) : "Dowolna";
            sb.AppendLine($"{r.Template ?? "Trasa konwencjonalna"} | {r.Controller}.{r.Action} | {httpMethod}");
        }

        return Content(sb.ToString(), "text/plain; charset=utf-8");
    }

    [Route("archive/{year:int}/{month:int:range(1,12)}")]
    public IActionResult Archive(int year, int month)
    {
        return Content($"Wyświetlam książki z roku: {year}");
    }
}