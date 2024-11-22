namespace UserAuthentication.cs
{
    public class UserValidator : IUserValidator
    {
        public bool ValidateCredentials(string username, string password)
        {
            // Check if username or password is null or empty
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            // Username validation rules
            if (username.Length < 3 || username.Length > 20)
            {
                return false;
            }

            // Password validation rules
            if (password.Length < 6 || password.Length > 20)
            {
                return false;
            }

            // Check for valid characters in username (letters, numbers, underscore only)
            if (!username.All(c => char.IsLetterOrDigit(c) || c == '_'))
            {
                return false;
            }

            // Additional password strength requirements
            bool hasUpperCase = password.Any(char.IsUpper);
            bool hasLowerCase = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);

            return hasUpperCase && hasLowerCase && hasDigit;
        }
    }
}