using System;
using System.IO;
using System.Collections.Generic;

namespace document_sharing_manager.Core.Services
{
    /// <summary>
    /// Utility class to dynamically load and read variables from .env files.
    /// </summary>
    public static class EnvReader
    {
        private static readonly Dictionary<string, string> _envVars = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static bool _loaded = false;

        public static string GetValue(string key, string defaultValue = "")
        {
            if (!_loaded)
            {
                LoadEnv();
            }
            return _envVars.TryGetValue(key, out var val) ? val : defaultValue;
        }

        private static void LoadEnv()
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string[] checkPaths = new[]
                {
                    Path.Combine(baseDir, ".env"),
                    Path.Combine(baseDir, @"..\..\..\.env"),
                    Path.Combine(baseDir, @"..\..\..\..\.env")
                };

                foreach (var path in checkPaths)
                {
                    string fullPath = Path.GetFullPath(path);
                    if (File.Exists(fullPath))
                    {
                        foreach (var line in File.ReadAllLines(fullPath))
                        {
                            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                                continue;

                            int eqIdx = line.IndexOf('=');
                            if (eqIdx > 0)
                            {
                                string key = line.Substring(0, eqIdx).Trim();
                                string val = line.Substring(eqIdx + 1).Trim();
                                _envVars[key] = val;
                            }
                        }
                        _loaded = true;
                        break;
                    }
                }
            }
            catch { }
            _loaded = true; // Mark as loaded even if it failed so we don't retry endlessly
        }
    }
}
