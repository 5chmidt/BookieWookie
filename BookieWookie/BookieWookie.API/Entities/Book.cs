using BookieWookie.API.Authorization;

namespace BookieWookie.API.Entities
{
    public class Book : OwnedByBase
    {
        public int Id { get; set; }

        public int Title { get; set; }
    }
}
