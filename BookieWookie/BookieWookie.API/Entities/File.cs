namespace BookieWookie.API.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json.Serialization;

    public class File
    {
        [Key, NotNull]
        public int FileId { get; set; }

        public string? Purpose { get; set; }

        [Required, NotNull, JsonIgnore]
        public string Path { get; set; }

        public string FileName { get; set; }

        public DateTime Uploaded { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        public int UserId { get; set; }
    }
}
