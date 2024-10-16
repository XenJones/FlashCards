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
            .HasMany(fp => fp.Flashcards)
            .WithOne(f => f.FlashcardPack)
            .HasForeignKey(f => f.FlashcardPackId);
    }
}