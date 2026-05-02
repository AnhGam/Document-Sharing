namespace document_sharing_manager.Core.Data
{
    /// <summary>
    /// Holds the current user session information for both Client and Server logic.
    /// </summary>
    public static class UserSession
    {
        /// <summary>
        /// The ID of the currently authenticated user. Defaults to 1 for local/offline mode.
        /// </summary>
        public static int CurrentUserId { get; set; } = 1;

        /// <summary>
        /// The username of the currently authenticated user.
        /// </summary>
        public static string Username { get; set; } = "LocalUser";

        /// <summary>
        /// The JWT access token for the current session.
        /// </summary>
        public static string? AccessToken { get; set; }
    }
}
