using System.ComponentModel.DataAnnotations;

namespace BookieWookie.API.Models
{
    public class UserRequest
    {
        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Password { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Pseudonym { get; set; }
    }
}
