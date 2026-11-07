using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace BookStore.Web.Filters;

public class LogActionFilter : IActionFilter
{
    private readonly ILogger<LogActionFilter> _log;
    public LogActionFilter(ILogger<LogActionFilter> log) => _log = log;

    // wejście
    public void OnActionExecuting(ActionExecutingContext c) =>
        _log.LogInformation("START {Action}({Args})", c.ActionDescriptor.DisplayName,
            string.Join(", ", c.ActionArguments.Select(a => $"{a.Key}={a.Value}")));

    // wyjście
    public void OnActionExecuted(ActionExecutedContext c) =>
        _log.LogInformation("KONIEC {Action}", c.ActionDescriptor.DisplayName);
}