using System;
using System.Data;
using System.Data.SqlClient;

namespace study_document_manager
{
    /// <summary>
    /// Partial class của DatabaseHelper - phần User Authentication
    /// </summary>
    public partial class DatabaseHelper
    {
        #region User Management & Authentication

        /// <summary>
        /// Xác thực người dùng
        /// </summary>
        public static DataRow AuthenticateUser(string username, string password)
        {
            string query = "SELECT * FROM users WHERE username = @username";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@username", username)
            };

            DataTable dt = ExecuteQuery(query, parameters);
            
            if (dt.Rows.Count > 0)
            {
                DataRow user = dt.Rows[0];
                string storedHash = user["password_hash"].ToString();
                
                // Verify password với BCrypt
                if (BCrypt.Net.BCrypt.Verify(password, storedHash))
                {
                    return user;
                }
            }

            return null;
        }

        /// <summary>
        /// Kiểm tra username đã tồn tại
        /// </summary>
        public static bool CheckUsernameExists(string username)
        {
            string query = "SELECT COUNT(*) FROM users WHERE username = @username";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@username", username)
            };

            object result = ExecuteScalar(query, parameters);
            return Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// Kiểm tra email đã tồn tại
        /// </summary>
        public static bool CheckEmailExists(string email)
        {
            string query = "SELECT COUNT(*) FROM users WHERE email = @email";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@email", email)
            };

            object result = ExecuteScalar(query, parameters);
            return Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// Đăng ký người dùng mới
        /// </summary>
        public static bool RegisterUser(string username, string password, string fullName, string email, string role)
        {
            // Hash password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            string query = @"INSERT INTO users (username, password_hash, full_name, email, role) 
                           VALUES (@username, @passwordHash, @fullName, @email, @role)";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@username", username),
                new SqlParameter("@passwordHash", passwordHash),
                new SqlParameter("@fullName", fullName),
                new SqlParameter("@email", string.IsNullOrEmpty(email) ? (object)DBNull.Value : email),
                new SqlParameter("@role", role)
            };

            int result = ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        /// <summary>
        /// Cập nhật last_login
        /// </summary>
        public static bool UpdateLastLogin(int userId)
        {
            string query = "UPDATE users SET last_login = GETDATE() WHERE id = @userId";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@userId", userId)
            };

            int result = ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        /// <summary>
        /// Lấy danh sách tất cả users (admin only)
        /// </summary>
        public static DataTable GetAllUsers()
        {
            string query = @"SELECT id, username, full_name, email, role, is_active, 
                           created_date, last_login FROM users ORDER BY created_date DESC";
            return ExecuteQuery(query);
        }

        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        public static bool ChangePassword(int userId, string oldPassword, string newPassword)
        {
            // Lấy hash cũ
            string query = "SELECT password_hash FROM users WHERE id = @userId";
            SqlParameter[] getParams = new SqlParameter[]
            {
                new SqlParameter("@userId", userId)
            };

            DataTable dt = ExecuteQuery(query, getParams);
            if (dt.Rows.Count == 0)
                return false;

            string storedHash = dt.Rows[0]["password_hash"].ToString();

            // Verify old password
            if (!BCrypt.Net.BCrypt.Verify(oldPassword, storedHash))
                return false;

            // Hash new password và update
            string newHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            string updateQuery = "UPDATE users SET password_hash = @newHash WHERE id = @userId";
            
            SqlParameter[] updateParams = new SqlParameter[]
            {
                new SqlParameter("@newHash", newHash),
                new SqlParameter("@userId", userId)
            };

            int result = ExecuteNonQuery(updateQuery, updateParams);
            return result > 0;
        }

        /// <summary>
        /// Khóa/Mở khóa user (admin only)
        /// </summary>
        public static bool ToggleUserActive(int userId, bool isActive)
        {
            string query = "UPDATE users SET is_active = @isActive WHERE id = @userId";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@isActive", isActive),
                new SqlParameter("@userId", userId)
            };

            int result = ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        /// <summary>
        /// Xóa user (admin only)
        /// </summary>
        public static bool DeleteUser(int userId)
        {
            string query = "DELETE FROM users WHERE id = @userId";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@userId", userId)
            };

            int result = ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        /// <summary>
        /// Admin reset password (không cần old password)
        /// </summary>
        public static bool AdminResetPassword(int userId, string newPassword)
        {
            // Hash new password
            string newHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            string query = "UPDATE users SET password_hash = @newHash WHERE id = @userId";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@newHash", newHash),
                new SqlParameter("@userId", userId)
            };

            int result = ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        /// <summary>
        /// Update user role (admin only)
        /// </summary>
        public static bool UpdateUserRole(int userId, string newRole)
        {
            string query = "UPDATE users SET role = @newRole, updated_date = GETDATE(), updated_by = @updatedBy WHERE id = @userId";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@newRole", newRole),
                new SqlParameter("@userId", userId),
                new SqlParameter("@updatedBy", UserSession.UserId)
            };

            int result = ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        /// <summary>
        /// Cập nhật thông tin profile của user (tự sửa)
        /// </summary>
        public static bool UpdateUserProfile(int userId, string fullName, string email)
        {
            string query = "UPDATE users SET full_name = @fullName, email = @email WHERE id = @userId";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@fullName", fullName),
                new SqlParameter("@email", string.IsNullOrEmpty(email) ? (object)DBNull.Value : email),
                new SqlParameter("@userId", userId)
            };

            int result = ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        /// <summary>
        /// Đổi mật khẩu (user tự đổi, cần xác thực mật khẩu cũ)
        /// </summary>
        public static (bool Success, string Message) ChangePasswordSelf(int userId, string currentPassword, string newPassword)
        {
            string query = "SELECT password_hash FROM users WHERE id = @userId";
            SqlParameter[] getParams = new SqlParameter[]
            {
                new SqlParameter("@userId", userId)
            };

            DataTable dt = ExecuteQuery(query, getParams);
            if (dt.Rows.Count == 0)
                return (false, "Không tìm thấy tài khoản!");

            string storedHash = dt.Rows[0]["password_hash"].ToString();

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, storedHash))
                return (false, "Mật khẩu hiện tại không đúng!");

            string newHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            string updateQuery = "UPDATE users SET password_hash = @newHash WHERE id = @userId";
            
            SqlParameter[] updateParams = new SqlParameter[]
            {
                new SqlParameter("@newHash", newHash),
                new SqlParameter("@userId", userId)
            };

            int result = ExecuteNonQuery(updateQuery, updateParams);
            return result > 0 
                ? (true, "Đổi mật khẩu thành công!") 
                : (false, "Không thể cập nhật mật khẩu!");
        }

        #endregion
    }
}
