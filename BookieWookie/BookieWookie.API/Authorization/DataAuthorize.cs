using Microsoft.EntityFrameworkCore;

namespace BookieWookie.API.Authorization
{
    public static class DataAuthorize
    {
        /// <summary>
        /// This is called in the overridden SaveChanges in the application's DbContext
        /// Its job is to call the SetOwnedBy on entities that have it and are being created
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userId"></param>
        public static void MarkCreatedItemAsOwnedBy(this DbContext context, string userId)
        {
            foreach (var entityEntry in context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added))
            {
                if (entityEntry.Entity is IOwnedBy entityToMark)
                {
                    entityToMark.SetOwnedBy(userId);
                }
            }
        }
    }
}
