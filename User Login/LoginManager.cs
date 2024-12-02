using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace UserAuthentication
{
    public class LoginManager : ILogin
    {
        private readonly ICredentialStorage _storage;
        private Dictionary<string, UserCredentials> _userCredentials;
        private readonly LoginAttemptManager _attemptManager;
        private readonly SessionManager _sessionManager;
        private readonly SecurityAuditLogger _auditLogger;
        public event EventHandler<string>? OnAuthenticationMessage;

        public LoginManager(ICredentialStorage storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _userCredentials = new Dictionary<string, UserCredentials>();
            _attemptManager = new LoginAttemptManager();
            _sessionManager = new SessionManager();
            _auditLogger = new SecurityAuditLogger();
            LoadUser();
        }

        private void LoadUser()
        {
            try
            {
                _userCredentials = _storage.LoadCredentials();
            }
            catch (Exception ex)
            {
                RaiseAuthenticationMessage($"Error loading credentials: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> Login(string username, string password)
        {
            try
            {
                Console.WriteLine($"Login attempt for user: {username}");
                
                if (_attemptManager.IsLockedOut(username))
                {
                    Console.WriteLine("Account is locked");
                    RaiseAuthenticationMessage("Account is temporarily locked");
                    await Task.Run(() => _auditLogger.LogSecurityEvent(username, "LOGIN_ATTEMPT", "Account locked", false));
                    return false;
                }

                if (!_userCredentials.ContainsKey(username))
                {
                    Console.WriteLine($"User not found: {username}");
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

        private string HashPassword(string password, string salt)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);
            using (var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                saltBytes,
                350000,
                HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32);
                return Convert.ToBase64String(hash);
            }
        }

        private void RaiseAuthenticationMessage(string message)
        {
            OnAuthenticationMessage?.Invoke(this, message);
        }

        public bool IsAdmin(string username) // Checks if the user is an admin
        {
            return _userCredentials.ContainsKey(username) && _userCredentials[username].IsAdmin; // Returns true if the user is an admin
        }

        public string GetFamilyGroup(string username) // Gets the family group of the user
        {
            return _userCredentials.ContainsKey(username) ? _userCredentials[username].FamilyGroup : string.Empty; // Returns the family group of the user
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