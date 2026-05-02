#nullable enable
using System;

namespace document_sharing_manager.Core.Domain
{
    public enum UserRole
    {
        Admin,
        Manager,
        User
    }

    public class User : BaseEntity
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User;
        public bool IsActive { get; set; } = true;
    }
}
