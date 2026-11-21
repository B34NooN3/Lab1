using BookStore.Web.Models;

namespace BookStore.Web.Services;

public interface IBookService
{
    Task<PagedResult<Book>> GetPagedAsync(int page, int pageSize, string? search = null, string? sort = null, string? dir = null);
    Task<Book?> GetByIdAsync(int id);
    Task CreateAsync(Book book);
    Task UpdateAsync(Book book);
    Task DeleteAsync(int id);
    Task<bool> IsbnExistsAsync(string isbn, int? excludeId = null);
}