using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BookStore.Web.Areas.Admin.Controllers;

[Area("Admin")] // Bardzo ważne!
public class AuthorsController : Controller
{
    // Symulowana lista autorów w pamięci (zgodnie z poleceniem na tym etapie laboratorium)
    private static List<string> AuthorsList = new List<string> 
    { 
        "Stephen King", 
        "J.K. Rowling", 
        "Stanisław Lem" 
    };

    // Wyświetla listę autorów
    public IActionResult Index()
    {
        return View(AuthorsList);
    }

    // Wyświetla formularz dodawania
    public IActionResult Create()
    {
        return View();
    }

    // Odbiera dane z formularza i dodaje do listy
    [HttpPost]
    public IActionResult Create(string name)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            AuthorsList.Add(name);
        }
        return RedirectToAction(nameof(Index));
    }
}