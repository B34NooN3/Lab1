using System.ComponentModel.DataAnnotations;

namespace BookStore.Web.Models;

public class Book
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = "";

    [Required]
    [StringLength(20)]
    public string Isbn { get; set; } = "";

    public DateTime? PublicationDate { get; set; } // gravity.db: publication_date

    public int NumPages { get; set; }              // gravity.db: num_pages

    public decimal Price { get; set; }

    [Required]
    [StringLength(150)]
    public string Author { get; set; } = "";       // tymczasowo tekst; w Lab 5 zamienimy na relację N:M
}

