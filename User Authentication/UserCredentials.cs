namespace UserAuthentication
{
    public class UserCredentials // Stores the user credentials
    {
        public string Username { get; } // Stores the username
        public string Password { get; } // Stores the password
        public bool IsAdmin { get; } // Stores if the user is an admin
        public string FamilyGroup { get; } // Stores the family group

        public UserCredentials(string username, string password, bool isAdmin, string familyGroup) // Constructor for the UserCredentials class
        {
            Username = username ?? throw new ArgumentNullException(nameof(username)); // Stores the username
            Password = password ?? throw new ArgumentNullException(nameof(password)); // Stores the password
            FamilyGroup = familyGroup ?? throw new ArgumentNullException(nameof(familyGroup)); // Stores the family group
            IsAdmin = isAdmin; // Stores if the user is an admin
        }
    }
}