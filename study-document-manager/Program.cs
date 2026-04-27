using study_document_manager.Core.Data;
using study_document_manager.Core;
using study_document_manager.Documents;
using System;
using System.Windows.Forms;

namespace study_document_manager
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

