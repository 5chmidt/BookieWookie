using System.ComponentModel.DataAnnotations;

namespace BookieWookie.API.Models
{
    /// <summary>
    /// Model to request a file property update.
    /// </summary>
    public class UpdateFileRequest
    {
        /// <summary>
        /// Required, the unique identifier for the file being updated.
        /// </summary>
        [Required]
        public int FileId { get; set; }

        /// <summary>
        /// What the file is used for IE CoverPage, AuthorPhoto etc.
        /// </summary>
        public string? Purpose { get; set; }
    }
}
