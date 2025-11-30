using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            
            // Hiển thị LoginForm trước
            LoginForm loginForm = new LoginForm();
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                // Nếu đăng nhập thành công, mở Form1
                Application.Run(new Dashboard());
            }
            // Nếu không đăng nhập hoặc thoát, app sẽ đóng
        }
    }
}
