namespace BookieWookie.API.Authorization
{
    public interface IOwnedBy
    {
        string OwnedBy { get; }
        void SetOwnedBy(string protectKey);
    }
}
