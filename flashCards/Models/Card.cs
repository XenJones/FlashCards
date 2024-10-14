namespace flashCards.Models;

public class Card
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? ImageUrl { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public DateTime Date { get; set; }
    
}