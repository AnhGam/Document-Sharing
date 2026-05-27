using document_sharing_manager.Core.Data;
using document_sharing_manager.Core;
using document_sharing_manager.Documents;
using System;
using System.Windows.Forms;

namespace document_sharing_manager
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Cấu hình giao thức bảo mật để gọi API HTTPS không bị lỗi SSL bảo mật hơn
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            System.Net.ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
            {
                if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None) return true;
                
                string host = null;
                if (sender is System.Net.HttpWebRequest req)
                {
                    host = req.RequestUri.Host;
                }
                else if (sender is System.Net.Http.HttpRequestMessage httpReq)
                {
                    host = httpReq.RequestUri?.Host;
                }
                else if (sender != null)
                {
                    try
                    {
                        var prop = sender.GetType().GetProperty("RequestUri");
                        if (prop != null && prop.GetValue(sender) is Uri uri)
                        {
                            host = uri.Host;
                        }
                    }
                    catch { }
                }

                if (host != null)
                {
                    if (host.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
                        host.Equals("127.0.0.1") ||
                        host.EndsWith(".lhr.life", StringComparison.OrdinalIgnoreCase) ||
                        host.EndsWith(".trycloudflare.com", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            };

            RegisterUriScheme();

            // Khởi tạo database SQLite (tạo file và bảng nếu chưa có)
            DatabaseHelper.InitializeDatabase();

            try
            {
                // Cấu hình API URL mặc định: Đọc từ biến API_URL trong tệp .env, nếu không có sẽ lấy từ App.config hoặc fallback về localhost
                string configApiUrl = System.Configuration.ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://127.0.0.1:5000";
                string apiBaseUrl = document_sharing_manager.Core.Services.EnvReader.GetValue("API_URL", configApiUrl);
                
                var authClient = new document_sharing_manager.Core.Services.AuthServiceClient(apiBaseUrl);

                bool isAuthenticated = false;

                if (!isAuthenticated)
                {
                    using var loginForm = new document_sharing_manager.Management.LoginForm(authClient);
                    if (loginForm.ShowDialog() == DialogResult.OK)
                    {
                        isAuthenticated = true;
                    }
                }

                if (isAuthenticated)
                {
                    var dashboard = new Dashboard(authClient);
                    if (args.Length > 0 && args[0].StartsWith("docshare://"))
                    {
                        dashboard.HandleDeepLink(args[0]);
                    }
                    Application.Run(dashboard);
                }
                else
                {
                    Application.Exit();
                }
            }
            finally
            {
                // Clean shutdown of local resources if any
            }
        }

        private static void RegisterUriScheme()
        {
            try
            {
                const string SchemeName = "docshare";
                using var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey($@"Software\Classes\{SchemeName}");
                key.SetValue("", $"URL:{SchemeName} Protocol");
                key.SetValue("URL Protocol", "");

                using var commandKey = key.CreateSubKey(@"shell\open\command");
                string appPath = Application.ExecutablePath;
                commandKey.SetValue("", $"\"{appPath}\" \"%1\"");
            }
            catch { }
        }
    }
}

