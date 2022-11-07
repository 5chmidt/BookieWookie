using System.ComponentModel.DataAnnotations;

namespace BookieWookie.API.Authorization
{
    public class OwnedByBase : IOwnedBy
    {
        [Required] 
        [MaxLength(40)] //Guid string
        public string OwnedBy { get; private set; }

        public void SetOwnedBy(string protectKey)
        {
            OwnedBy = protectKey;
        }
    }
}
