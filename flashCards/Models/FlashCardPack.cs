using System.ComponentModel.DataAnnotations;

namespace flashCards.Models
{
    public class FlashCardPack
    {
        public int Id { get; set; }

        [Required]
        public string PackName { get; set; }
        public List<Flashcard> Flashcards { get; set; } = new List<Flashcard>();
    }
}
