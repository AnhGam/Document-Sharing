using System;

namespace study_document_manager.Core.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; private set; }
        public string PasswordHash { get; private set; }

        public User(string username, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty", nameof(username));
            
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

            Username = username;
            PasswordHash = passwordHash;
        }

        public void UpdatePassword(string newHash)
        {
            if (string.IsNullOrWhiteSpace(newHash))
                throw new ArgumentException("New password hash cannot be empty", nameof(newHash));

            PasswordHash = newHash;
            MarkAsUpdated();
        }
    }
}

