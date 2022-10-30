namespace BookieWookie.API.Models
{
    using System.ComponentModel.DataAnnotations;

    public class AuthenticationRequest
    {
        [Required]
        public string? UserName { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
