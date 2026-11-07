using System.Threading;

namespace BookStore.Web.Middleware;

public class RequestStats
{
    private long _total;
    // odczyt stanu licznika
    public long Total => Interlocked.Read(ref _total);
    // inkrementacja licznika
    public long Hit() => Interlocked.Increment(ref _total);
}