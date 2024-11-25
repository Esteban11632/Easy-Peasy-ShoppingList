using System.ComponentModel.DataAnnotations;

namespace UserAuthentication
{
    public class UserPassword : ILogin, IRegister // Implements the ILogin and IRegister interfaces
    {
        private readonly ICredentialStorage _storage; // Stores the credentials
        private readonly IUserValidator _validator; // Validates the credentials
        private readonly IFamilyGroupManager _familyGroupManager; // Manages the family groups
        private Dictionary<string, UserCredentials> _userCredentials; // Stores the user credentials
        public event EventHandler<string>? OnAuthenticationMessage; // Event handler for the authentication message
        public UserPassword(ICredentialStorage storage, IUserValidator validator, IFamilyGroupManager familyGroupManager) // Constructor for the UserPassword class
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage)); // Stores the credentials
            _validator = validator ?? throw new ArgumentNullException(nameof(validator)); // Validates the credentials
            _familyGroupManager = familyGroupManager ?? throw new ArgumentNullException(nameof(familyGroupManager)); // Manages the family groups
            _userCredentials = new Dictionary<string, UserCredentials>(); // Stores the user credentials
            LoadUser(); // Loads the user credentials
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

        public async Task<bool> Register(string username, string password, string FamilyGroup, bool isAdmin = false) // Resgiste the user in the files, need explicit true for admin to be admin
        {
            if (!_validator.ValidateCredentials(username, password, FamilyGroup)) // Validate the credentials to register in the files
            {
                RaiseAuthenticationMessage("Invalid credentials format"); // If they are invalid, throw an authentication message
                return false;
            }

            if (_userCredentials.ContainsKey(username)) // Checks if the user already exists
            {
                RaiseAuthenticationMessage("Username already exists"); // Throw an authentication message that the user already exists
                return false;
            }

            // Only check for existing family group if the user is NOT an admin
            if (!isAdmin && !_familyGroupManager.FamilyGroupExists(FamilyGroup))
            {
                RaiseAuthenticationMessage("Invalid family group");
                return false;
            }

            var newUser = new UserCredentials(username, password, isAdmin, FamilyGroup); // Creates a new user
            _userCredentials.Add(username, newUser); // Adds the new user to the dictionary
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
            if (password != userInfo.Password) // Checks the input password in the userInfo password key
            {
                RaiseAuthenticationMessage("Invalid password"); // Throws an authentication message of invalid password
                return false;
            }

            RaiseAuthenticationMessage($"Login successful. User type: {(userInfo.IsAdmin ? "Admin" : "Regular User")} " + $"in {userInfo.FamilyGroup} group"); // Throws an authentication message of login successful
            return true;
        }

        public bool IsAdmin(string username) // Checks if the user is an admin
        {
            return _userCredentials.ContainsKey(username) && _userCredentials[username].IsAdmin; // Returns true if the user is an admin
        }

        public string GetFamilyGroup(string username) // Gets the family group of the user
        {
            return _userCredentials.ContainsKey(username) ? _userCredentials[username].FamilyGroup : string.Empty; // Returns the family group of the user
        }

        private void RaiseAuthenticationMessage(string message) // Raises an authentication message
        {
            OnAuthenticationMessage?.Invoke(this, message); // Invokes the authentication message
        }

        public bool IsUserInFamilyGroup(string username, string familyGroup) // Checks if the user is in the family group
        {
            return _userCredentials.ContainsKey(username) &&
                _userCredentials[username].FamilyGroup == familyGroup; // Returns true if the user is in the family group
        }

        public bool IsFamilyGroupAdmin(string username, string familyGroup) // Checks if the user is the admin of the family group
        {
            return IsAdmin(username) && IsUserInFamilyGroup(username, familyGroup); // Returns true if the user is the admin of the family group
        }
    }
}