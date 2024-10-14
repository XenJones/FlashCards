using Microsoft.EntityFrameworkCore;
using flashCards.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<FlashCard> FlashCard { get; set; }
}
