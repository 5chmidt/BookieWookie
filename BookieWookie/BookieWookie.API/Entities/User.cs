namespace BookieWookie.API.Entities
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json.Serialization;

    public class User
    {
        [Key]
        [Required]
        public int UserId { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        [Required, NotNull]
        public string Username { get; set; }

        public string? Pseudonym { get; set; }

        [JsonIgnore]
        public byte[] Salt { get; set; }

        [JsonIgnore]
        public byte[] Hash { get; set; }

        [JsonIgnore]
        public ICollection<Book> Books { get; set; }
    }
}
