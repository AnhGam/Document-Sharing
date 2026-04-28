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

            // Mở Dashboard trực tiếp - không cần đăng nhập
            Application.Run(new Dashboard());
        }
    }
}

