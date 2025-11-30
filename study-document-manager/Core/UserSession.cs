using System;

namespace study_document_manager
{
    /// <summary>
    /// Quản lý session người dùng hiện tại
    /// Chế độ cá nhân hóa: Admin/User (không còn Teacher/Student)
    /// </summary>
    public static class UserSession
    {
        public static int UserId { get; set; }
        public static string Username { get; set; }
        public static string FullName { get; set; }
        public static string Email { get; set; }
        public static string Role { get; set; }
        public static DateTime LoginTime { get; set; }

        /// <summary>
        /// Kiểm tra đã đăng nhập chưa
        /// </summary>
        public static bool IsLoggedIn
        {
            get { return UserId > 0; }
        }

        /// <summary>
        /// Kiểm tra là Admin
        /// </summary>
        public static bool IsAdmin
        {
            get { return Role == "Admin"; }
        }

        /// <summary>
        /// Kiểm tra là User thường
        /// </summary>
        public static bool IsUser
        {
            get { return Role == "User"; }
        }

        // ============ DEPRECATED - Giữ lại để tương thích ngược ============
        // Các property này sẽ trả về false, code cũ vẫn chạy được
        
        /// <summary>
        /// [DEPRECATED] Không còn role Teacher - luôn trả về false
        /// </summary>
        [Obsolete("Role Teacher đã bị xóa. Sử dụng IsUser hoặc IsAdmin.")]
        public static bool IsTeacher
        {
            get { return false; }
        }

        /// <summary>
        /// [DEPRECATED] Không còn role Student - luôn trả về false
        /// </summary>
        [Obsolete("Role Student đã bị xóa. Sử dụng IsUser hoặc IsAdmin.")]
        public static bool IsStudent
        {
            get { return false; }
        }
        // ===================================================================

        /// <summary>
        /// Có quyền thêm/sửa/xóa tài liệu của mình không (tất cả user đều có)
        /// </summary>
        public static bool CanEdit
        {
            get { return IsLoggedIn; }
        }

        /// <summary>
        /// Có quyền quản lý danh mục không (tất cả user đều quản lý danh mục của mình)
        /// </summary>
        public static bool CanManageCategories
        {
            get { return IsLoggedIn; }
        }

        /// <summary>
        /// Có quyền quản lý người dùng không (chỉ Admin)
        /// </summary>
        public static bool CanManageUsers
        {
            get { return IsAdmin; }
        }

        /// <summary>
        /// Kiểm tra có quyền sửa/xóa tài liệu cụ thể không
        /// Chế độ cá nhân: mỗi user chỉ sửa được tài liệu của mình
        /// </summary>
        public static bool CanEditDocument(int documentUserId)
        {
            // Mọi user chỉ sửa được tài liệu của mình (kể cả Admin)
            return documentUserId == UserId;
        }

        /// <summary>
        /// Đăng xuất
        /// </summary>
        public static void Logout()
        {
            UserId = 0;
            Username = string.Empty;
            FullName = string.Empty;
            Email = string.Empty;
            Role = string.Empty;
        }
    }
}
