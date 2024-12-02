using System;
using System.Security.Cryptography;

namespace UserAuthentication
{
    public class UserCredentials
    {
        public string Salt { get; } // Stores the salt
        public string PasswordHash { get; } // Stores the password hash
        public string Username { get; } // Stores the username
        public bool IsAdmin { get; } // Stores if the user is an admin
        public string FamilyGroup { get; } // Stores the family group

        private const int Iterations = 350000; // Stores the number of iterations for the hashing algorithm

        public UserCredentials(string username, string password, bool isAdmin, string familyGroup) // Constructor for the UserCredentials class
        {
            Salt = GenerateSalt(); // Generates a salt
            Username = username ?? throw new ArgumentNullException(nameof(username)); // Stores the username
            PasswordHash = HashPassword(password, Salt); // Stores the hashed password
            FamilyGroup = familyGroup ?? throw new ArgumentNullException(nameof(familyGroup)); // Stores the family group
            IsAdmin = isAdmin; // Stores if the user is an admin
        }

        public UserCredentials(string username, string passwordHash, string salt, bool isAdmin, string familyGroup)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
            Salt = salt ?? throw new ArgumentNullException(nameof(salt));
            FamilyGroup = familyGroup ?? throw new ArgumentNullException(nameof(familyGroup));
            IsAdmin = isAdmin;
        }

        private string GenerateSalt() // Generates a salt
        {
            byte[] salt = new byte[32]; // Creates a byte array with a length of 32
            RandomNumberGenerator.Fill(salt); // Fills the byte array with random bytes
            return Convert.ToBase64String(salt); // Converts the byte array to a base64 string and returns it
        }

        private string HashPassword(string password, string salt) // Hashes the password
        {
            byte[] saltBytes = Convert.FromBase64String(salt); // Converts the salt to a byte array
            using (var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                saltBytes,
                Iterations,
                HashAlgorithmName.SHA256)) // Creates a new Rfc2898DeriveBytes object with the password, salt, iterations, and hash algorithm name
            {
                byte[] hash = pbkdf2.GetBytes(32); // Gets the hashed password as a byte array
                return Convert.ToBase64String(hash); // Converts the byte array to a base64 string and returns it
            }
        }
    }
}