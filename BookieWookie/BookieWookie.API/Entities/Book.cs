namespace BookieWookie.API.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Entity framework class for book table.
    /// </summary>
    public class Book
    {
        /// <summary>
        /// Gets or sets the integer, primary key for book table.
        /// </summary>
        [Key]
        public int BookId { get; set; }

        /// <summary>
        /// Gets or sets the date-time when the book added to the database.
        /// </summary>
        [Required, NotNull]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the book's title.
        /// </summary>
        [Required, NotNull]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the book's content description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the fileId of the cover photo.
        /// </summary>
        public int? FileId { get; set; }

        /// <summary>
        /// Gets or sets the file 
        /// </summary>
        [JsonIgnore]
        public virtual File File { get; set; }

        /// <summary>
        /// Gets or sets the UserId to link the book with the author's user.
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the User object for the author.
        /// </summary>
        [JsonIgnore]
        public virtual User User { get; set; }
    }
}
