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
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Khởi tạo database SQLite (tạo file và bảng nếu chưa có)
            DatabaseHelper.InitializeDatabase();

            // Cấu hình API URL mặc định
            string apiBaseUrl = System.Configuration.ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://127.0.0.1:5000";
            var authClient = new document_sharing_manager.Core.Services.AuthServiceClient(apiBaseUrl);

            bool isAuthenticated = false;

            // Thử load token cũ và refresh
            var tokens = document_sharing_manager.Services.SecureStorage.LoadTokens();
            string refreshToken = tokens.refreshToken;
            if (!string.IsNullOrEmpty(refreshToken))
            {
                // Thử refresh token
                if (authClient.RefreshTokensAsync(refreshToken).GetAwaiter().GetResult())
                {
                    // Refresh thành công -> Lưu token mới
                    document_sharing_manager.Services.SecureStorage.SaveTokens(
                        document_sharing_manager.Core.Data.UserSession.AccessToken,
                        document_sharing_manager.Core.Data.UserSession.RefreshToken);
                    
                    // Reset and Initialize DB for the specific user
                    document_sharing_manager.Core.Data.DatabaseHelper.ResetConnection();
                    document_sharing_manager.Core.Data.DatabaseHelper.InitializeDatabase();

                    isAuthenticated = true;
                }
            }

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
                Application.Run(new Dashboard(authClient));
            }
            else
            {
                Application.Exit();
            }
        }
    }
}

