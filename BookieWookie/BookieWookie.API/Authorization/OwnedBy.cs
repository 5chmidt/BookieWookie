using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BookieWookie.API.Authorization
{
    public class OwnedByBase : IOwnedBy
    {
        [Required] 
        [MaxLength(40)] //Guid string
        [JsonIgnore]
        public string OwnedBy { get; private set; }

        public void SetOwnedBy(string protectKey)
        {
            OwnedBy = protectKey;
        }
    }
}
