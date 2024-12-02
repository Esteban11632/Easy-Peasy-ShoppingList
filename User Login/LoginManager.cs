using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace UserAuthentication
{
    public class LoginManager : ILogin, IFamily // Implements the ILogin and IFamily interfaces
    {
        private readonly ICredentialStorage _storage; // Stores the credentials
        private Dictionary<string, UserCredentials> _userCredentials; // Stores the user credentials
        private readonly LoginAttemptManager _attemptManager; // Stores the login attempts
        private readonly SessionManager _sessionManager; // Stores the session manager
        private readonly SecurityAuditLogger _auditLogger; // Stores the audit logger
        public event EventHandler<string>? OnAuthenticationMessage; // Event handler for the authentication message

        public LoginManager(ICredentialStorage storage) // Constructor for the LoginManager class
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage)); // Stores the credentials
            _userCredentials = new Dictionary<string, UserCredentials>(); // Stores the user credentials
            _attemptManager = new LoginAttemptManager(); // Stores the login attempts
            _sessionManager = new SessionManager(); // Stores the session manager
            _auditLogger = new SecurityAuditLogger(); // Stores the audit logger
            LoadUser(); // Loads the user
        }

        private void LoadUser() // Loads the user
        {
            try // Try to load the user
            {
                _userCredentials = _storage.LoadCredentials(); // Loads the user credentials
            }
            catch (Exception ex) // Catches any exceptions
            {
                RaiseAuthenticationMessage($"Error loading credentials: {ex.Message}"); // Logs the error loading the credentials
                throw;
            }
        }

        public async Task<bool> Login(string username, string password) // Logs the login attempt
        {
            try // Try to login
            {
                Console.WriteLine($"Login attempt for user: {username}"); // Logs the login attempt
                
                if (_attemptManager.IsLockedOut(username)) // Checks if the user is locked out
                {
                    Console.WriteLine("Account is locked"); // Logs the account is locked
                    RaiseAuthenticationMessage("Account is temporarily locked"); // Logs the account is locked
                    await Task.Run(() => _auditLogger.LogSecurityEvent(username, "LOGIN_ATTEMPT", "Account locked", false)); // Logs the login attempt
                    return false; // Returns false if the user is locked out
                }

                if (!_userCredentials.ContainsKey(username)) // Checks if the user is not found
                {
                    Console.WriteLine($"User not found: {username}"); // Logs the user not found
                    _attemptManager.RecordAttempt(username, false); // Records the login attempt
                    await Task.Run(() => _auditLogger.LogSecurityEvent(username, "LOGIN_ATTEMPT", "Invalid username", false)); // Logs the login attempt
                    RaiseAuthenticationMessage("Invalid credentials"); // Logs the invalid credentials
                    return false; // Returns false if the user is not found
                }

                var userInfo = _userCredentials[username]; // Stores the user information
                var hashedPassword = await Task.Run(() => HashPassword(password, userInfo.Salt)); // Hashes the password
                
                if (hashedPassword != userInfo.PasswordHash) // Checks if the password is incorrect
                {
                    _attemptManager.RecordAttempt(username, false); // Records the login attempt
                    await Task.Run(() => _auditLogger.LogSecurityEvent(username, "LOGIN_ATTEMPT", "Invalid password", false)); // Logs the login attempt
                    RaiseAuthenticationMessage("Invalid credentials"); // Logs the invalid credentials
                    return false; // Returns false if the password is incorrect
                }

                _attemptManager.RecordAttempt(username, true); // Records the login attempt
                var sessionId = await Task.Run(() => _sessionManager.CreateSession(username, userInfo.FamilyGroup, userInfo.IsAdmin)); // Creates the session
                await Task.Run(() => _auditLogger.LogSecurityEvent(username, "LOGIN_SUCCESS", $"Session: {sessionId}", true)); // Logs the login success
                
                RaiseAuthenticationMessage($"Login successful. User type: {(userInfo.IsAdmin ? "Admin" : "Regular User")} in {userInfo.FamilyGroup} group"); // Logs the login successful
                return true; // Returns true if the login is successful
            }
            catch (Exception ex)
            {
                await Task.Run(() => _auditLogger.LogSecurityEvent(username, "LOGIN_ERROR", ex.Message, false)); // Logs the login error
                throw new SecurityException("An error occurred during login", ex); // Throws an exception if there is an error during login
            }
        }

        private string HashPassword(string password, string salt) // Hashes the password
        {
            byte[] saltBytes = Convert.FromBase64String(salt); // Converts the salt to bytes
            using (var pbkdf2 = new Rfc2898DeriveBytes( // Creates a new Rfc2898DeriveBytes object
                password, // The password to hash
                saltBytes, // The salt to use
                350000, // Number of iterations
                HashAlgorithmName.SHA256)) // The hash algorithm to use
            {
                byte[] hash = pbkdf2.GetBytes(32); // Gets the hash
                return Convert.ToBase64String(hash); // Converts the hash to a base64 string
            }
        }

        private void RaiseAuthenticationMessage(string message) // Raises the authentication message
        {
            OnAuthenticationMessage?.Invoke(this, message); // Invokes the authentication message
        }

        public bool IsAdmin(string username) // Checks if the user is an admin
        {
            return _userCredentials.ContainsKey(username) && _userCredentials[username].IsAdmin; // Returns true if the user is an admin
        }

        public string GetFamilyGroup(string username) // Gets the family group of the user
        {
            return _userCredentials.ContainsKey(username) ? _userCredentials[username].FamilyGroup : string.Empty; // Returns the family group of the user
        }

        public async Task<List<string>> GetUsersInFamilyGroup(string familyGroup) // Gets the users in the family group
        {
            return await Task.Run(() =>
                _userCredentials // Gets the users in the family group
                    .Where(u => u.Value.FamilyGroup == familyGroup) // Filters the users in the family group
                    .Select(u => u.Key) // Selects the keys of the users in the family group
                    .ToList() // Converts the users in the family group to a list
            );
        }
    }
}