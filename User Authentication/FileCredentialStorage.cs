using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UserAuthentication.cs
{
    public class FileCredentialStorage : ICredentialStorage
    {
        private readonly string _filePath;

        public FileCredentialStorage(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        public Dictionary<string, (string password, bool isAdmin)> LoadCredentials()
        {
            var credentials = new Dictionary<string, (string password, bool isAdmin)>();
            
            if (!File.Exists(_filePath))
            {
                File.Create(_filePath).Close();
                return credentials;
            }

            string[] lines = File.ReadAllLines(_filePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                if (parts.Length == 3)
                {
                    bool isAdmin = bool.Parse(parts[2]);
                    credentials[parts[0]] = (parts[1], isAdmin);
                }
            }

            return credentials;
        }

        public void SaveCredentials(Dictionary<string, (string password, bool isAdmin)> credentials)
        {
            var lines = credentials
                .Select(kvp => $"{kvp.Key},{kvp.Value.password},{kvp.Value.isAdmin}")
                .ToList();

            File.WriteAllLines(_filePath, lines);
        }
    }
} 