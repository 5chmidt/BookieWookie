using System.ComponentModel.DataAnnotations;

namespace BookieWookie.API.Models
{
    /// <summary>
    /// Model for an authenticated user to create a new book entry.
    /// </summary>
    public class CreateBookRequest
    {
        /// <summary>
        /// Gets or sets the book title, (required).
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the book description (optional).
        /// </summary>
        public string Description { get; set; }
    }
}
