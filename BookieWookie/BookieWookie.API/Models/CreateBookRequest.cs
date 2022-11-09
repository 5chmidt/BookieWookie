using System.ComponentModel.DataAnnotations;

namespace BookieWookie.API.Models
{
    public class CreateBookRequest
    {
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }


    }
}
