using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace BookStore.Web.Filters;

public class MeasureTimeAttribute : ActionFilterAttribute
{
    private Stopwatch _stopwatch;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        _stopwatch = Stopwatch.StartNew();
        base.OnActionExecuting(context);
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        _stopwatch.Stop();
        context.HttpContext.Response.Headers["X-Action-Ms"] = _stopwatch.ElapsedMilliseconds.ToString();
        base.OnActionExecuted(context);
    }
}