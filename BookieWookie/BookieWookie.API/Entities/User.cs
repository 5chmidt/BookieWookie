namespace BookieWookie.API.Entities
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    public class User
    {
        [Required]
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [Required]
        public string Username { get; set; }

        public string? Pseudonym { get; set; }

        [JsonIgnore]
        public byte[] Salt { get; set; }
        [JsonIgnore]
        public byte[] Hash { get; set; }
    }
}
