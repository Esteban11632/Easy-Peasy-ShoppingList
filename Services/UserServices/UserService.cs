using System;
using System.Threading.Tasks;
using Easy_Peasy_ShoppingList.Shared;
using Easy_Peasy_ShoppingList.Models;
using Easy_Peasy_ShoppingList.Services;
using UserAuthentication;

namespace UserAuthentication
{
    public class UserService : IUserService
    {
        private readonly IFamily _familyService;
        private readonly ICredentialStorage _credentialStorage;

        public UserService(IFamily familyService, ICredentialStorage credentialStorage)
        {
            _familyService = familyService ?? throw new ArgumentNullException(nameof(familyService));
            _credentialStorage = credentialStorage ?? throw new ArgumentNullException(nameof(credentialStorage));
        }

        /// <summary>
        /// Retrieves the family group for a given username.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <returns>The family group name.</returns>
        public Task<string> GetFamilyGroupAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Username cannot be null or empty.", nameof(username));
            }

            string familyGroup = _familyService.GetFamilyGroup(username);
            return Task.FromResult(familyGroup);
        }

        /// <summary>
        /// Changes the username of a user.
        /// </summary>
        /// <param name="currentUsername">The current username.</param>
        /// <param name="newUsername">The new desired username.</param>
        /// <returns>True if the username was changed successfully; otherwise, false.</returns>
        public Task<bool> ChangeUsernameAsync(string currentUsername, string newUsername)
        {
            if (string.IsNullOrWhiteSpace(currentUsername))
            {
                throw new ArgumentException("Current username cannot be null or empty.", nameof(currentUsername));
            }

            if (string.IsNullOrWhiteSpace(newUsername))
            {
                throw new ArgumentException("New username cannot be null or empty.", nameof(newUsername));
            }

            try
            {
                var credentials = _credentialStorage.LoadCredentials();

                // Check if the current username exists and the new username is not already taken
                if (!credentials.ContainsKey(currentUsername))
                {
                    Console.Error.WriteLine($"Username '{currentUsername}' does not exist.");
                    return Task.FromResult(false);
                }

                if (credentials.ContainsKey(newUsername))
                {
                    Console.Error.WriteLine($"Username '{newUsername}' is already taken.");
                    return Task.FromResult(false);
                }

                // Retrieve the user's credentials
                var userCredentials = credentials[currentUsername];

                // Remove the old username entry
                credentials.Remove(currentUsername);

                // Create a new UserCredentials instance with the new username
                var updatedUser = new UserCredentials(
                    username: newUsername,
                    passwordHash: userCredentials.PasswordHash,
                    salt: userCredentials.Salt,
                    isAdmin: userCredentials.IsAdmin,
                    familyGroup: userCredentials.FamilyGroup
                );

                // Add the new username entry
                credentials[newUsername] = updatedUser;

                // Save the updated credentials
                _credentialStorage.SaveCredentials(credentials);

                Console.WriteLine($"Username changed from '{currentUsername}' to '{newUsername}' successfully.");
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error changing username: {ex.Message}");
                return Task.FromResult(false);
            }
        }
    }
}