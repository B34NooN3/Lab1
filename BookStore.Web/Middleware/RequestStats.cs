using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BookStore.Web.Middleware;

public class RequestStats
{
    private long _total;
    public long Total => Interlocked.Read(ref _total);
    public long Hit() => Interlocked.Increment(ref _total);

    // Bezpieczny dla wątków słownik zliczający ścieżki
    public ConcurrentDictionary<string, long> PathCounts { get; } = new();

    public void RecordPath(string path)
    {
        // Dodaje nową ścieżkę z wartością 1 lub inkrementuje obecną
        PathCounts.AddOrUpdate(path, 1, (key, oldValue) => oldValue + 1);
    }

    public Dictionary<string, long> GetTop5Paths()
    {
        return PathCounts
            .OrderByDescending(x => x.Value)
            .Take(5)
            .ToDictionary(x => x.Key, x => x.Value);
    }
}