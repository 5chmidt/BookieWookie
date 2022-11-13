using System.ComponentModel.DataAnnotations;

namespace BookieWookie.API.Models
{
    public class BookParameters
    {
        public int? Id { get; set; }

        public int? AuthorId { get; set; }

        public string? AuthorPseudonym { get; set; }

        public string? AuthorFirstName { get; set; }

        public string? AuthorLastName { get; set; }

        public DateOnly? CreatedOn { get; set; }

        public DateTime? CreatedAfter { get; set; }

        public DateTime? CreateBefore { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }
    }
}
