using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BookieWookie.API.Entities
{
    public class File
    {
        [Key, NotNull]
        public int FileId { get; set; }

        [Required, NotNull]
        public string Purpose { get; set; }

        [Required, NotNull]
        public string Path { get; set; }

        public DateTime Uploaded { get; set; }
    }
}
