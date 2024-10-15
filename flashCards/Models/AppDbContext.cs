using flashCards.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<FlashcardPack> FlashcardPacks { get; set; }
    public DbSet<Flashcard> Flashcards { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FlashcardPack>()
            .HasMany(f => f.Flashcards)
            .WithOne(p => p.FlashcardPack)
            .HasForeignKey(p => p.FlashcardPackId);
    }
}
