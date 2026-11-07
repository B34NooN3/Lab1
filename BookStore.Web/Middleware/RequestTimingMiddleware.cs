using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BookStore.Web.Middleware;

public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTimingMiddleware> _log;

    public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> log)
    {
        _next = next;
        _log = log;
    }

    public async Task InvokeAsync(HttpContext ctx, RequestStats stats)
    {
        var sw = Stopwatch.StartNew();
        var n = stats.Hit();

        // nagłówki z licznikiem i czasem dodajem tuż przed odesłaniem odpowiedzi
        ctx.Response.OnStarting(() =>
        {
            ctx.Response.Headers["X-Elapsed-Ms"] = sw.ElapsedMilliseconds.ToString();
            ctx.Response.Headers["X-Request-Count"] = n.ToString();
            return Task.CompletedTask;
        });

        // DODATEK Z ZADANIA: short-circuit (ominięcie pozostałych warstw)
        if (ctx.Request.Path.StartsWithSegments("/blokada"))
        {
            ctx.Response.StatusCode = 403; // Zwróć błąd 403 Forbidden
            return; // BRAK wywołania _next(ctx) przerywa potok!
        }

        // wywołanie następnej warstwy potoku
        await _next(ctx);

        _log.LogInformation("{Method} {Path} -> {Status} w {Ms} ms", 
            ctx.Request.Method, ctx.Request.Path, ctx.Response.StatusCode, sw.ElapsedMilliseconds);
    }
}