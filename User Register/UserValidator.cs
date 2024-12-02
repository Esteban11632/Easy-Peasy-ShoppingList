using System;
using System.Linq;

namespace UserAuthentication
{

    public class UserValidator : IUserValidator
    {
        public bool ValidateCredentials(string username, string password, string familyGroup)
        {
            try
            {
                // Validate all inputs
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(familyGroup))
                {
                    throw new SecurityException("Username, password, and family group cannot be empty");
                }

                ValidateUsername(username);
                ValidatePassword(password);
                ValidateFamilyGroup(familyGroup);

                return true;
            }
            catch (SecurityException)
            {
                return false;
            }
        }

        private void ValidateUsername(string username)
        {
            if (username.Length < 3 || username.Length > 20)
            {
                throw new SecurityException("Username must be between 3 and 20 characters");
            }

            if (!username.All(c => char.IsLetterOrDigit(c) || c == '_'))
            {
                throw new SecurityException("Username can only contain letters, numbers, and underscores");
            }
        }

        private void ValidatePassword(string password)
        {
            if (password.Length < 8 || password.Length > 128) // Updated length requirements
            {
                throw new SecurityException("Password must be between 8 and 128 characters");
            }

            bool hasUpperCase = password.Any(char.IsUpper);
            bool hasLowerCase = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecialChar = password.Any(c => !char.IsLetterOrDigit(c));

            if (!hasUpperCase || !hasLowerCase || !hasDigit || !hasSpecialChar)
            {
                throw new SecurityException("Password must contain uppercase, lowercase, numbers, and special characters");
            }
        }

        private void ValidateFamilyGroup(string familyGroup)
        {
            if (familyGroup.Length < 3 || familyGroup.Length > 30)
            {
                throw new SecurityException("Family group name must be between 3 and 30 characters");
            }

            if (!familyGroup.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == ' '))
            {
                throw new SecurityException("Family group can only contain letters, numbers, underscores, hyphens, and spaces");
            }
        }
    }
}