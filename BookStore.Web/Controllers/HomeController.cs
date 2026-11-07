using Microsoft.AspNetCore.Mvc;
using System;

namespace BookStore.Web.Controllers;

public class HomeController : Controller
{
    // Domyślna strona główna
    public IActionResult Index()
    {
        return View();
    }

    // Akcja z zadania E do testowania filtra wyjątków
    public IActionResult Boom() => throw new InvalidOperationException("Celowy wyjątek");
    
    public IActionResult HttpStatus(int code)
    {
        // Ustawienie statusu odpowiedzi zgodnie z poleceniem
        Response.StatusCode = code;
    
        // Przekazujemy kod błędu do widoku, aby móc go wyświetlić
        return View(code); 
    }
}