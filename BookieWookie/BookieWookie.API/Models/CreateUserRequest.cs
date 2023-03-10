using System.ComponentModel.DataAnnotations;

namespace BookieWookie.API.Models
{
    /// <summary>
    /// User request used to create a user.
    /// </summary>
    public class CreateUserRequest
    {
        /// <summary>
        /// Username, must be unique among accounts can only contain letters and number.
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Password, must meet complexity criteria.
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// Author's first name.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Author's last name.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Author's psuedonym.
        /// </summary>
        public string? Pseudonym { get; set; }
    }
}
