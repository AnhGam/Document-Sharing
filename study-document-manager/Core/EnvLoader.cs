using System;
using System.Collections.Generic;
using System.IO;

namespace study_document_manager.Core
{
    public static class EnvLoader
    {
        private static Dictionary<string, string> _envVariables = new Dictionary<string, string>();
        private static bool _isLoaded = false;

        public static void Load(string filePath = null)
        {
            if (_isLoaded) return;

            if (string.IsNullOrEmpty(filePath))
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                filePath = Path.Combine(baseDir, ".env");
                
                if (!File.Exists(filePath))
                {
                    string projectDir = Directory.GetParent(baseDir)?.Parent?.Parent?.FullName;
                    if (projectDir != null)
                    {
                        filePath = Path.Combine(projectDir, ".env");
                    }
                }
            }

            if (!File.Exists(filePath))
            {
                return;
            }

            foreach (var line in File.ReadAllLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                var parts = line.Split(new[] { '=' }, 2);
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();
                    
                    if (value.StartsWith("\"") && value.EndsWith("\""))
                    {
                        value = value.Substring(1, value.Length - 2);
                    }

                    _envVariables[key] = value;
                    Environment.SetEnvironmentVariable(key, value);
                }
            }

            _isLoaded = true;
        }

        public static string Get(string key, string defaultValue = "")
        {
            if (_envVariables.TryGetValue(key, out string value))
            {
                return value;
            }
            
            var envValue = Environment.GetEnvironmentVariable(key);
            return !string.IsNullOrEmpty(envValue) ? envValue : defaultValue;
        }

        public static string GetConnectionString()
        {
            var server = Get("DB_SERVER");
            var database = Get("DB_NAME");
            var user = Get("DB_USER");
            var password = Get("DB_PASSWORD");
            var encrypt = Get("DB_ENCRYPT", "True");
            var trustCert = Get("DB_TRUST_CERT", "True");

            if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(database))
            {
                return null;
            }

            return $"Server={server};Database={database};User Id={user};Password={password};Encrypt={encrypt};TrustServerCertificate={trustCert};";
        }
    }
}
