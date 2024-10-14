using System.ComponentModel.DataAnnotations;

namespace flashCards.Models
{
    public class FlashCard
    {
        public int Id { get; set; }
        public string ImageURL { get; set; }

        [Required]
        public string Question { get; set; }
        public string SubQuestion { get; set; }

        [Required]
        public string Answer { get; set; }
    }
}
