using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace UserAuthentication
{
    public class UserPassword : ILogin, IRegister // Implements the ILogin and IRegister interfaces
    {
        private readonly ICredentialStorage _storage; // Stores the credentials
        private readonly IUserValidator _validator; // Validates the credentials
        private readonly IFamilyGroupManager _familyGroupManager; // Manages the family groups
        private Dictionary<string, UserCredentials> _userCredentials; // Stores the user credentials
        private readonly LoginAttemptManager _attemptManager;
        private readonly SessionManager _sessionManager;
        private readonly SecurityAuditLogger _auditLogger;
        public event EventHandler<string>? OnAuthenticationMessage; // Event handler for the authentication message
        public UserPassword(ICredentialStorage storage, IUserValidator validator, IFamilyGroupManager familyGroupManager) // Constructor for the UserPassword class
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage)); // Stores the credentials
            _validator = validator ?? throw new ArgumentNullException(nameof(validator)); // Validates the credentials
            _familyGroupManager = familyGroupManager ?? throw new ArgumentNullException(nameof(familyGroupManager)); // Manages the family groups
            _userCredentials = new Dictionary<string, UserCredentials>(); // Stores the user credentials
            _attemptManager = new LoginAttemptManager();
            _sessionManager = new SessionManager();
            _auditLogger = new SecurityAuditLogger();
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

        public async Task SaveUser()
        {
            try
            {
                await Task.Run(() => _storage.SaveCredentials(_userCredentials));
            }
            catch (Exception ex)
            {
                throw new SecurityException($"Error saving credentials: {ex.Message}", ex);
            }
        }

        public async Task<bool> Register(string username, string password, string FamilyGroup, bool isAdmin = false)
        {
            try
            {
                if (!_validator.ValidateCredentials(username, password, FamilyGroup))
                {
                    RaiseAuthenticationMessage("Invalid credentials format");
                    return false;
                }

                if (_userCredentials.ContainsKey(username))
                {
                    RaiseAuthenticationMessage("Username already exists");
                    return false;
                }

                if (!isAdmin && !_familyGroupManager.FamilyGroupExists(FamilyGroup))
                {
                    RaiseAuthenticationMessage("Invalid family group");
                    return false;
                }

                var newUser = new UserCredentials(username, password, isAdmin, FamilyGroup);
                _userCredentials.Add(username, newUser);
                
                await Task.Run(() => SaveUser());
                
                _auditLogger.LogSecurityEvent(username, "REGISTRATION", $"User registered in {FamilyGroup} group", true);
                RaiseAuthenticationMessage($"Registration successful. User type: {(isAdmin ? "Admin" : "Regular User")}");
                return true;
            }
            catch (Exception ex)
            {
                _auditLogger.LogSecurityEvent(username, "REGISTRATION_ERROR", ex.Message, false);
                throw new SecurityException("An error occurred during registration", ex);
            }
        }

        public async Task<bool> Login(string username, string password)
        {
            try
            {
                if (_attemptManager.IsLockedOut(username))
                {
                    RaiseAuthenticationMessage("Account is temporarily locked");
                    await Task.Run(() => _auditLogger.LogSecurityEvent(username, "LOGIN_ATTEMPT", "Account locked", false));
                    return false;
                }

                if (!_userCredentials.ContainsKey(username))
                {
                    _attemptManager.RecordAttempt(username, false);
                    await Task.Run(() => _auditLogger.LogSecurityEvent(username, "LOGIN_ATTEMPT", "Invalid username", false));
                    RaiseAuthenticationMessage("Invalid credentials");
                    return false;
                }

                var userInfo = _userCredentials[username];
                var hashedPassword = await Task.Run(() => HashPassword(password, userInfo.Salt));
                
                if (hashedPassword != userInfo.PasswordHash)
                {
                    _attemptManager.RecordAttempt(username, false);
                    await Task.Run(() => _auditLogger.LogSecurityEvent(username, "LOGIN_ATTEMPT", "Invalid password", false));
                    RaiseAuthenticationMessage("Invalid credentials");
                    return false;
                }

                _attemptManager.RecordAttempt(username, true);
                var sessionId = await Task.Run(() => _sessionManager.CreateSession(username, userInfo.FamilyGroup, userInfo.IsAdmin));
                await Task.Run(() => _auditLogger.LogSecurityEvent(username, "LOGIN_SUCCESS", $"Session: {sessionId}", true));
                
                RaiseAuthenticationMessage($"Login successful. User type: {(userInfo.IsAdmin ? "Admin" : "Regular User")} in {userInfo.FamilyGroup} group");
                return true;
            }
            catch (Exception ex)
            {
                await Task.Run(() => _auditLogger.LogSecurityEvent(username, "LOGIN_ERROR", ex.Message, false));
                throw new SecurityException("An error occurred during login", ex);
            }
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

        private string HashPassword(string password, string salt)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);
            using (var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                saltBytes,
                350000, // Same iteration count as in UserCredentials
                HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32);
                return Convert.ToBase64String(hash);
            }
        }

        public async Task<List<string>> GetUsersInFamilyGroup(string familyGroup)
        {
            return await Task.Run(() => 
                _userCredentials
                    .Where(u => u.Value.FamilyGroup == familyGroup)
                    .Select(u => u.Key)
                    .ToList()
            );
        }
    }
}