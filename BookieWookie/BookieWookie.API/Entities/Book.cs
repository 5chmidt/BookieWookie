using System.ComponentModel.DataAnnotations;

namespace BookieWookie.API.Entities
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public int UserId { get; set; }

        public User User { get; set; }
    }
}
