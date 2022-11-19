namespace BookieWookie.API.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Model for storing user date.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Unique key for user object.
        /// </summary>
        [Key]
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// User's first name.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// User's last name.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Username, must be unique per user.
        /// </summary>
        [Required, NotNull]
        public string Username { get; set; }

        /// <summary>
        /// Author's Pseudonym.
        /// </summary>
        public string? Pseudonym { get; set; }

        /// <summary>
        /// Randomly generated number used to salt password hash for secure storage.
        /// </summary>
        [JsonIgnore]
        public byte[] Salt { get; set; }

        /// <summary>
        /// Hashed user password, to be compaired upon authentication request.
        /// </summary>
        [JsonIgnore]
        public byte[] Hash { get; set; }

        /// <summary>
        /// Collection of books published by a user.
        /// <see cref="Book"/>
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<Book> Books { get; set; }

        /// <summary>
        /// Collection of files uploaded by a user.
        /// <see cref="File"/>
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<File> Files { get; set; }
    }
}
