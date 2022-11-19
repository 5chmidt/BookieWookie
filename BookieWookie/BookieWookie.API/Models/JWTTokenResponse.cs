namespace BookieWookie.API.Models
{
    /// <summary>
    /// JWT token responce object will be null or contain a token.
    /// </summary>
    public class JWTTokenResponse
    {
        /// <summary>
        /// If authentication was successful will contain a bearer token.
        /// </summary>
        public string? Token { get; set; }
    }
}
