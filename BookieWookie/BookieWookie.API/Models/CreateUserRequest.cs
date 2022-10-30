using System.ComponentModel.DataAnnotations;

namespace BookieWookie.API.Models
{
    public class CreateUserRequest
    {
        [Required]
        public string? UserName { get; set; }

        [Required]
        public string? Password { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Pseudonym { get; set; }
    }
}
