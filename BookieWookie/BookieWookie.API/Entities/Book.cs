using BookieWookie.API.Authorization;
using System.ComponentModel.DataAnnotations;

namespace BookieWookie.API.Entities
{
    public class Book : OwnedByBase
    {
        public int Id { get; set; }

        [Required]
        public int AuthorId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public int Title { get; set; }

        public string Description { get; set; }
    }
}
