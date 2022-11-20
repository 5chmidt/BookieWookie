using System.ComponentModel.DataAnnotations;

namespace BookieWookie.API.Models
{
    /// <summary>
    /// User request used to update a user.
    /// </summary>
    public class UpdateUserRequest
    {
        /// <summary>
        /// Unique identifier for user.
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Username must be less than 50 charectors and only contain letters and numbers.
        /// </summary>
        [Required]
        public string? Username { get; set; }

        /// <summary>
        /// Password must meet complexity requirements.
        /// </summary>
        [Required]
        public string? Password { get; set; }

        /// <summary>
        /// Author's first name.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Author's last name.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Author's pseudonym.
        /// </summary>
        public string? Pseudonym { get; set; }
    }
}
