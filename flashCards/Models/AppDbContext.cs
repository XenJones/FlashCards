using Microsoft.EntityFrameworkCore;
using flashCards.Models;

public class AppDbContext : DbContext
{
    public DbSet<FlashCard> FlashCard { get; set; }
    public DbSet<User> User { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}
