using System;
using System.Data.SQLite;
using document_sharing_manager.Core.Data;
using document_sharing_manager.Core.Security;

namespace document_sharing_manager.Core.Services
{
    public class LocalAuthService
    {
        public bool RegisterLocal(string username, string password, string displayName, string email)
        {
            try
            {
                // Ensure main DB exists and tables are created
                DatabaseHelper.ResetConnection();
                DatabaseHelper.InitializeDatabase();

                string hash = PasswordHasher.HashPassword(password);
                
                string query = "INSERT INTO tai_khoan (ten_dang_nhap, mat_khau, ho_ten, email, vai_tro, thoi_gian_tao) " +
                               "VALUES (@username, @password, @fullname, @email, 'Admin', @createdAt)";
                
                SQLiteParameter[] parameters = {
                    new("@username", username),
                    new("@password", hash),
                    new("@fullname", displayName),
                    new("@email", email),
                    new("@createdAt", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))
                };

                int rows = DatabaseHelper.ExecuteNonQuery(query, parameters);
                return rows > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Local Registration failed: {ex.Message}");
                return false;
            }
        }

        public (bool Success, int UserId) LoginLocal(string username, string password)
        {
            try
            {
                DatabaseHelper.ResetConnection();
                DatabaseHelper.InitializeDatabase();

                string query = "SELECT id, mat_khau FROM tai_khoan WHERE ten_dang_nhap = @username";
                SQLiteParameter[] parameters = { new("@username", username) };

                using var dataTable = DatabaseHelper.ExecuteQuery(query, parameters);
                if (dataTable.Rows.Count > 0)
                {
                    string hash = dataTable.Rows[0]["mat_khau"].ToString();
                    if (PasswordHasher.VerifyPassword(hash, password))
                    {
                        int id = Convert.ToInt32(dataTable.Rows[0]["id"]);
                        return (true, id);
                    }
                }
                return (false, 0);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Local Login failed: {ex.Message}");
                return (false, 0);
            }
        }
    }
}
