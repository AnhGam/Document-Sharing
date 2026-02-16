using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace study_document_manager.Services
{
    /// <summary>
    /// Information about an available update
    /// </summary>
    public class UpdateInfo
    {
        public bool HasUpdate { get; set; }
        public string NewVersion { get; set; }
        public string DownloadUrl { get; set; }
        public string ReleasePageUrl { get; set; }
        public string ReleaseNotes { get; set; }
    }

    /// <summary>
    /// Checks GitHub Releases API for new versions
    /// </summary>
    public static class UpdateChecker
    {
        private const string RepoOwner = "hayato-shino05";
        private const string RepoName = "study-document-manager";
        private static readonly string ApiUrl =
            $"https://api.github.com/repos/{RepoOwner}/{RepoName}/releases/latest";

        /// <summary>
        /// Check for updates asynchronously. Returns null on any error (offline, etc.)
        /// </summary>
        public static async Task<UpdateInfo> CheckForUpdateAsync()
        {
            try
            {
                using (var client = new WebClient())
                {
                    // GitHub API requires User-Agent header
                    client.Headers.Add("User-Agent", $"StudyDocumentManager/{AppVersion.Current}");
                    client.Headers.Add("Accept", "application/vnd.github.v3+json");
                    client.Encoding = System.Text.Encoding.UTF8;

                    // Force TLS 1.2 for GitHub API
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    string json = await client.DownloadStringTaskAsync(new Uri(ApiUrl));
                    return ParseResponse(json);
                }
            }
            catch
            {
                // Silently fail if no internet or API error
                return null;
            }
        }

        private static UpdateInfo ParseResponse(string json)
        {
            try
            {
                var serializer = new JavaScriptSerializer();
                var release = serializer.Deserialize<Dictionary<string, object>>(json);

                string tagName = release.ContainsKey("tag_name")
                    ? release["tag_name"]?.ToString() ?? "" : "";
                string body = release.ContainsKey("body")
                    ? release["body"]?.ToString() ?? "" : "";
                string htmlUrl = release.ContainsKey("html_url")
                    ? release["html_url"]?.ToString() ?? "" : "";

                // Find Setup.exe download URL from assets
                string setupUrl = null;
                if (release.ContainsKey("assets") && release["assets"] is System.Collections.ArrayList assets)
                {
                    foreach (var item in assets)
                    {
                        if (item is Dictionary<string, object> asset)
                        {
                            string name = asset.ContainsKey("name")
                                ? asset["name"]?.ToString() ?? "" : "";
                            if (name.EndsWith("_Setup.exe", StringComparison.OrdinalIgnoreCase))
                            {
                                setupUrl = asset.ContainsKey("browser_download_url")
                                    ? asset["browser_download_url"]?.ToString() : null;
                                break;
                            }
                        }
                    }
                }

                // Compare versions
                bool hasUpdate = AppVersion.IsNewer(tagName);

                return new UpdateInfo
                {
                    HasUpdate = hasUpdate,
                    NewVersion = tagName,
                    DownloadUrl = setupUrl,
                    ReleasePageUrl = htmlUrl,
                    ReleaseNotes = body
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
