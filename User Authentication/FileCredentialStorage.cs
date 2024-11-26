using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace UserAuthentication
{
    public class FileCredentialStorage : ICredentialStorage // Implements the ICredentialStorage interface
    {
        private readonly string _filePath; // Stores the file path for the credentials
        private readonly string _familyGroupsPath; // Stores the file path for the family groups
        private readonly byte[] _encryptionKey;

        public FileCredentialStorage(string filePath = "credentials.txt", string familyGroupsPath = "familygroups.txt") // Constructor for the FileCredentialStorage class
        {
            _filePath = filePath; // Stores the file path for the credentials
            _familyGroupsPath = familyGroupsPath; // Stores the file path for the family groups
            _encryptionKey = GetOrCreateEncryptionKey();
        }

        private byte[] GetOrCreateEncryptionKey()
        {
            // In production, this should be stored securely (e.g., Azure Key Vault)
            using (var aes = Aes.Create())
            {
                return aes.Key;
            }
        }

        private string EncryptData(string data)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = _encryptionKey;
                aes.GenerateIV();

                using (var encryptor = aes.CreateEncryptor())
                using (var msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(aes.IV, 0, aes.IV.Length);

                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(data);
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        private string DecryptData(string encryptedData)
        {
            byte[] fullCipher = Convert.FromBase64String(encryptedData);

            using (var aes = Aes.Create())
            {
                byte[] iv = new byte[aes.IV.Length];
                byte[] cipher = new byte[fullCipher.Length - iv.Length];

                Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
                Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

                aes.Key = _encryptionKey;
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor())
                using (var msDecrypt = new MemoryStream(cipher))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }

        public Dictionary<string, UserCredentials> LoadCredentials() // Loads the credentials
        {
            var credentials = new Dictionary<string, UserCredentials>(); // Stores the credentials
            if (File.Exists(_filePath)) // Checks if the file exists
            {
                var lines = File.ReadAllLines(_filePath); // Reads all the lines from the file
                foreach (var line in lines) // Iterates through the lines
                {
                    var parts = line.Split('|'); // Splits the line into parts
                    if (parts.Length >= 4) // Checks if the line has four parts
                    {
                        string username = parts[0]; // Stores the username
                        string password = parts[1]; // Stores the password
                        bool isAdmin = bool.Parse(parts[2]); // Stores if the user is an admin
                        string familyGroup = parts[3]; // Stores the family group

                        var user = new UserCredentials(username, password, isAdmin, familyGroup); // Creates a new user credentials object
                        credentials[username] = user; // Adds the user credentials to the dictionary
                    }
                }
            }
            return credentials; // Returns the credentials
        }

        public void SaveCredentials(Dictionary<string, UserCredentials> credentials) // Saves the credentials
        {
            var lines = credentials.Values.Select(user => $"{user.Username}|{user.PasswordHash}|{user.IsAdmin}|{user.FamilyGroup}").ToList(); // Converts the credentials to a list of lines
            File.WriteAllLines(_filePath, lines); // Writes the lines to the file
        }

        public (HashSet<string> groups, Dictionary<string, string> admins) LoadFamilyGroups() // Loads the family groups
        {
            var groups = new HashSet<string>(); // Stores the family groups
            var admins = new Dictionary<string, string>(); // Stores the family group admins

            if (File.Exists(_familyGroupsPath)) // Checks if the file exists
            {
                var lines = File.ReadAllLines(_familyGroupsPath); // Reads all the lines from the file
                foreach (var line in lines) // Iterates through the lines
                {
                    var parts = line.Split('|'); // Splits the line into parts
                    if (parts.Length >= 2) // Checks if the line has two parts
                    {
                        string groupName = parts[0]; // Stores the family group name
                        string adminUsername = parts[1]; // Stores the admin username

                        groups.Add(groupName); // Adds the family group to the set
                        admins[groupName] = adminUsername; // Adds the family group and admin to the dictionary
                    }
                }
            }
            return (groups, admins); // Returns the family groups and admins
        }

        public void SaveFamilyGroups(HashSet<string> groups, Dictionary<string, string> admins) // Saves the family groups
        {
            var lines = groups.Select(group => $"{group}|{(admins.ContainsKey(group) ? admins[group] : "")}").ToList(); // Converts the family groups and admins to a list of lines
            File.WriteAllLines(_familyGroupsPath, lines); // Writes the lines to the file
        }
    }
} 