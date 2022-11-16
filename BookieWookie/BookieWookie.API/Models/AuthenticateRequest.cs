namespace BookieWookie.API.Models
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Model for user authentication request.
    /// </summary>
    public class AuthenticateRequest
    {
        /// <summary>
        /// Gets or sets the username, which is required.
        /// </summary>
        [Required]
        public string? Username { get; set; }

        /// <summary>
        /// Gets or sets the user password which is required.
        /// </summary>
        [Required]
        public string? Password { get; set; }
    }
}
