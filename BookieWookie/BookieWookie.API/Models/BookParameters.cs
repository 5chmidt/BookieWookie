using System.ComponentModel.DataAnnotations;

namespace BookieWookie.API.Models
{
    /// <summary>
    /// Query parameters for book search.
    /// </summary>
    public class BookParameters
    {
        /// <summary>
        /// If not null filter by book id.
        /// </summary>
        public int? BookId { get; set; }

        /// <summary>
        /// If not null filter by author id.
        /// </summary>
        public int? AuthorId { get; set; }

        /// <summary>
        /// If not null filter by author pseudonym.
        /// </summary>
        public string? AuthorPseudonym { get; set; }

        /// <summary>
        /// If not null filter by author first name.
        /// </summary>
        public string? AuthorFirstName { get; set; }

        /// <summary>
        /// If not null filter by author last name.
        /// </summary>
        public string? AuthorLastName { get; set; }

        /// <summary>
        /// If not null filter by date created.
        /// </summary>
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// If not null filter books created after.
        /// </summary>
        public DateTime? CreatedAfter { get; set; }

        /// <summary>
        /// If not null filter books created before.
        /// </summary>
        public DateTime? CreateBefore { get; set; }

        /// <summary>
        /// If not null filter by the exact matching title.
        /// </summary>
        public string? TitleEquals { get; set; }

        /// <summary>
        /// If not null filter if title contains entry.
        /// </summary>
        public string? TitleContains { get; set; }

        /// <summary>
        /// If not null filter if description contains entry.
        /// </summary>
        public string? Description { get; set; }
    }
}
