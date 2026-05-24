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

            RegisterUriScheme();

            // Khởi tạo database SQLite (tạo file và bảng nếu chưa có)
            DatabaseHelper.InitializeDatabase();

            // Cấu hình API URL mặc định
            string apiBaseUrl = System.Configuration.ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://127.0.0.1:5000";
            var authClient = new document_sharing_manager.Core.Services.AuthServiceClient(apiBaseUrl);

            bool isAuthenticated = false;

            // Local login requires entering password for now.
            // If "remember me" is needed in the future, save the local UserId.

            if (!isAuthenticated)
            {
                // Show Login Form
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

