using System.ComponentModel.DataAnnotations;

namespace UserAuthentication.cs
{
    public class UserPassword : ILogin, IRegister // Implements the ILogin and IRegister interfaces
    {
        private readonly ICredentialStorage _storage; // Stores the credentials
        private readonly IUserValidator _validator; // Validates the credentials
        private Dictionary<string, (string password, bool isAdmin)> _userCredentials; // Stores the user credentials

        public event EventHandler<string>? OnAuthenticationMessage; // Event handler for the authentication message

        public UserPassword(ICredentialStorage storage, IUserValidator validator) // Constructor for the UserPassword class
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage)); // Stores the credentials
            _validator = validator ?? throw new ArgumentNullException(nameof(validator)); // Validates the credentials
            _userCredentials = new Dictionary<string, (string password, bool isAdmin)>(); // Stores the user credentials
        }

        public void LoadUser() // Load credentials of the users
        {
            try
            {
                _userCredentials = _storage.LoadCredentials(); // Store the credentials in _userCredentials
            }
            catch (Exception ex) // Exception
            {
                RaiseAuthenticationMessage($"Error loading credentials: {ex.Message}"); // Indicates there is an error in the loading of credentials
                throw;
            }
        }

        public void SaveUser() // Save user credentials in file
        {
            try
            {
                _storage.SaveCredentials(_userCredentials); // Store the credentials
            }
            catch (Exception ex) // Exception
            {
                throw new Exception($"Error saving credentials: {ex.Message}"); // Error in saving the credentials
            }
        }

        public bool Register(string username, string password, bool isAdmin = false) // Resgiste the user in the files
        {
            if (!_validator.ValidateCredentials(username, password)) // Validate the credentials to register in the files
            {
                RaiseAuthenticationMessage("Invalid credentials format"); // If they are invalid, throw an authentication message
                return false;
            }

            if (_userCredentials.ContainsKey(username)) // Checks if the user already exists
            {
                RaiseAuthenticationMessage("Username already exists"); // Throw an authentication message that the user already exists
                return false;
            }

            _userCredentials.Add(username, (password, isAdmin)); // Save the new credentials in the dictionary _userCredentials
            SaveUser(); // Save credentials in the file
            RaiseAuthenticationMessage($"Registration successful. User type: {(isAdmin ? "Admin" : "Regular User")}"); // Throw an authentication message that the save was succesfull
            return true;
        }

        public bool Login(string username, string password) // Makes the user login to their account
        {
            if (!_userCredentials.ContainsKey(username)) // Check if the user is correct
            {
                RaiseAuthenticationMessage("Invalid username"); // Throws a invalid username message authentication
                return false;
            }

            var userInfo = _userCredentials[username]; // Puts the user name in userInfo
            if (password != userInfo.password) // Checks the input password in the userInfo password key
            {
                RaiseAuthenticationMessage("Invalid password"); // Throws an authentication message of invalid password
                return false;
            }

            RaiseAuthenticationMessage($"Login successful. User type: {(userInfo.isAdmin ? "Admin" : "Regular User")}"); // Throws an authentication message of login successful
            return true;
        }

        public bool IsAdmin(string username) // Checks if the user is an admin
        {
            return _userCredentials.ContainsKey(username) && _userCredentials[username].isAdmin; // Returns true if the user is an admin
        }

        private void RaiseAuthenticationMessage(string message) // Raises an authentication message
        {
            OnAuthenticationMessage?.Invoke(this, message); // Invokes the authentication message
        }
    }
}