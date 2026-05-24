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

            // Cấu hình giao thức bảo mật để gọi API HTTPS không bị lỗi SSL
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            RegisterUriScheme();

            // Khởi tạo database SQLite (tạo file và bảng nếu chưa có)
            DatabaseHelper.InitializeDatabase();

            // --- TỰ ĐỘNG CHẠY SERVER API NGẦM ---
            System.Diagnostics.Process apiProcess = null;
            try
            {
                string startupPath = Application.StartupPath;
                string[] possiblePaths = {
                    System.IO.Path.Combine(startupPath, "document-sharing-manager-api.exe"),
                    System.IO.Path.GetFullPath(System.IO.Path.Combine(startupPath, @"..\..\..\document-sharing-manager-api\bin\Debug\net8.0\document-sharing-manager-api.exe")),
                    System.IO.Path.GetFullPath(System.IO.Path.Combine(startupPath, @"..\..\..\document-sharing-manager-api\bin\Release\net8.0\document-sharing-manager-api.exe"))
                };

                string apiExePath = null;
                foreach (var path in possiblePaths)
                {
                    if (System.IO.File.Exists(path)) { apiExePath = path; break; }
                }

                if (apiExePath != null)
                {
                    apiProcess = new System.Diagnostics.Process();
                    apiProcess.StartInfo.FileName = apiExePath;
                    apiProcess.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(apiExePath);
                    apiProcess.StartInfo.UseShellExecute = false;
                    apiProcess.StartInfo.CreateNoWindow = true;

                    // Load .env if it exists
                    string envPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(startupPath, @"..\..\..\.env"));
                    if (System.IO.File.Exists(envPath))
                    {
                        foreach (var line in System.IO.File.ReadAllLines(envPath))
                        {
                            if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("#") && line.Contains("="))
                            {
                                var parts = line.Split(new[] { '=' }, 2);
                                apiProcess.StartInfo.EnvironmentVariables[parts[0].Trim()] = parts[1].Trim();
                            }
                        }
                    }

                    apiProcess.Start();
                }
            }
            catch { }
            // ------------------------------------

            try
            {
                // Cấu hình API URL mặc định
                string apiBaseUrl = System.Configuration.ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://127.0.0.1:5000";
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
                // Ensure API process is killed when app closes
                if (apiProcess != null)
                {
                    try { if (!apiProcess.HasExited) apiProcess.Kill(); } catch { }
                    try { apiProcess.Dispose(); } catch { }
                }
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

