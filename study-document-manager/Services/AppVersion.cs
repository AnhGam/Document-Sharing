namespace study_document_manager.Services
{
    /// <summary>
    /// Centralized application version management
    /// </summary>
    public static class AppVersion
    {
        public const string Current = "2.0.2";

        /// <summary>
        /// Compare two semantic version strings (e.g. "2.0.1" vs "2.1.0")
        /// Returns: positive if latest > current, 0 if equal, negative if current > latest
        /// </summary>
        public static int Compare(string current, string latest)
        {
            var cur = ParseVersion(current);
            var lat = ParseVersion(latest);

            if (cur.Major != lat.Major) return lat.Major - cur.Major;
            if (cur.Minor != lat.Minor) return lat.Minor - cur.Minor;
            return lat.Patch - cur.Patch;
        }

        public static bool IsNewer(string latest)
        {
            return Compare(Current, latest) > 0;
        }

        private static (int Major, int Minor, int Patch) ParseVersion(string version)
        {
            // Strip leading 'v' if present
            if (version.StartsWith("v") || version.StartsWith("V"))
                version = version.Substring(1);

            var parts = version.Split('.');
            int major = parts.Length > 0 && int.TryParse(parts[0], out int m) ? m : 0;
            int minor = parts.Length > 1 && int.TryParse(parts[1], out int n) ? n : 0;
            int patch = parts.Length > 2 && int.TryParse(parts[2], out int p) ? p : 0;

            return (major, minor, patch);
        }
    }
}
