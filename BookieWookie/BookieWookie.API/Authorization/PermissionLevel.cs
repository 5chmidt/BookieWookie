namespace BookieWookie.API.Authorization
{
    /// <summary>
    /// Enumerated permissions in acending order of access.
    /// </summary>
    public enum PermissionLevel
    {
        /// <summary>
        /// 0 - No permissions to execurte actions.
        /// </summary>
        None,

        /// <summary>
        /// Read permission.
        /// </summary>
        Get,

        /// <summary>
        /// Update permission.
        /// </summary>
        Update,

        /// <summary>
        /// Create permission.
        /// </summary>
        Create,

        /// <summary>
        /// Delete permission.
        /// </summary>
        Delete,

        /// <summary>
        /// Highest level of permisison (grant to admin users only).
        /// </summary>
        Admin
    }
}
