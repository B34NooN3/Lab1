using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace BookStore.Web.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _log;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> log) => _log = log;

    public void OnException(ExceptionContext context)
    {
        // 1. Logowanie błędu
        _log.LogError(context.Exception, "Wystąpił nieobsłużony wyjątek");

        // 2. Obsługa w zależności od ścieżki (zgodnie z instrukcją)
        if (context.HttpContext.Request.Path.StartsWithSegments("/api"))
        {
            context.Result = new ObjectResult(new ProblemDetails
            {
                Status = 500,
                Title = "Błąd serwera",
                Detail = context.Exception.Message
            }) { StatusCode = 500 };
        }
        else
        {
            context.Result = new ViewResult { ViewName = "Error" };
            context.HttpContext.Response.StatusCode = 500;
        }

        // 3. Oznaczenie wyjątku jako obsłużonego, by potok nie rzucił nim dalej
        context.ExceptionHandled = true;
    }
}