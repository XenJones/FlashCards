using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace flashCards.Models
{
    [Index(nameof(PackName), IsUnique = true)]

    public class FlashcardPack
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Pack Name is required")]
        public string PackName { get; set; }

        public ICollection<Flashcard>? Flashcards { get; set; }
        
        public Guid UserId { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid? EditedBy { get; set; }
    }

    public class Flashcard
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Image URL is required")]
        public string ImageURL { get; set; }

        [Required(ErrorMessage = "Question is required")]
        public string Question { get; set; }

        public string? SubQuestion { get; set; }

        [Required(ErrorMessage = "Answer is required")]
        public string Answer { get; set; }

        public Guid FlashcardPackId { get; set; }

        // Make the navigation property optional
        public FlashcardPack? FlashcardPack { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid? EditedBy { get; set; }
    }

}
