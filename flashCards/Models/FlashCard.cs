namespace flashCards.Models
{
    public class FlashcardPack
    {
        public int Id { get; set; }
        public string PackName { get; set; }

        // Navigation property
        public ICollection<Flashcard> Flashcards { get; set; }
    }

    public class Flashcard
    {
        public int Id { get; set; }
        public string ImageURL { get; set; }
        public string Question { get; set; }
        public string SubQuestion { get; set; }
        public string Answer { get; set; }

        // Foreign key
        public int FlashcardPackId { get; set; }

        // Navigation property
        public FlashcardPack FlashcardPack { get; set; }
    }

}
