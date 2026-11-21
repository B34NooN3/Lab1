using BookStore.Web.Data;
using BookStore.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Web.Services;

public class BookService : IBookService
{
    private readonly AppDbContext _db;

    public BookService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<Book>> GetPagedAsync(int page, int pageSize, string? search = null, string? sort = null, string? dir = null)
    {
        var query = _db.Books.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(b => b.Title.Contains(search) || b.Isbn.Contains(search));
        }

        var desc = string.Equals(dir, "desc", StringComparison.OrdinalIgnoreCase);

        query = sort?.ToLower() switch
        {
            "title" => desc ? query.OrderByDescending(b => b.Title) : query.OrderBy(b => b.Title),
            "price" => desc ? query.OrderByDescending(b => (double)b.Price) : query.OrderBy(b => (double)b.Price),
            "date"  => desc ? query.OrderByDescending(b => b.PublicationDate) : query.OrderBy(b => b.PublicationDate),
            _       => query.OrderBy(b => b.Id)
        };

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Book> { Items = items, Page = page, PageSize = pageSize, TotalCount = total };
    }

    public async Task<Book?> GetByIdAsync(int id)
        => await _db.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);

    public async Task CreateAsync(Book book)
    {
        _db.Books.Add(book);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Book book)
    {
        _db.Books.Update(book);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var book = await _db.Books.FindAsync(id);
        if (book != null)
        {
            _db.Books.Remove(book);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<bool> IsbnExistsAsync(string isbn, int? excludeId = null)
        => await _db.Books.AnyAsync(b => b.Isbn == isbn && (!excludeId.HasValue || b.Id != excludeId.Value));
}