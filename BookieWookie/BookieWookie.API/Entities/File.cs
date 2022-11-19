namespace BookieWookie.API.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Model for storing uploaded files.
    /// </summary>
    public class File
    {
        /// <summary>
        /// Unique key for each file.
        /// </summary>
        [Key, NotNull]
        public int FileId { get; set; }

        /// <summary>
        /// Description of what the file is used for, IE CoverPage.
        /// </summary>
        public string? Purpose { get; set; }

        /// <summary>
        /// The file path as it was stored on the API server.
        /// </summary>
        [Required, NotNull, JsonIgnore]
        public string Path { get; set; }

        /// <summary>
        /// File name as it was uploaded to the API.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Timestamp when the file was uploaded.
        /// </summary>
        public DateTime Uploaded { get; set; }

        /// <summary>
        /// User that uploaded the file.
        /// <see cref="User"/>
        /// </summary>
        [JsonIgnore]
        public User User { get; set; }

        /// <summary>
        /// Foriegn key to connect to user table.
        /// </summary>
        public int UserId { get; set; }
    }
}
