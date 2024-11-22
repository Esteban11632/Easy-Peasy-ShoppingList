using System.ComponentModel.DataAnnotations;

namespace UserAuthentication.cs
{
    public class UserPassword : ILogin, IRegister
    {
        private readonly ICredentialStorage _storage;
        private readonly IUserValidator _validator;
        private Dictionary<string, (string password, bool isAdmin)> _userCredentials;

        public event EventHandler<string>? OnAuthenticationMessage;

        public UserPassword(ICredentialStorage storage, IUserValidator validator)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _userCredentials = new Dictionary<string, (string password, bool isAdmin)>();
        }

        public void LoadUser()
        {
            try
            {
                _userCredentials = _storage.LoadCredentials();
                if (!_userCredentials.Any())
                {
                    CreateDefaultAdmin();
                }
            }
            catch (Exception ex)
            {
                RaiseAuthenticationMessage($"Error loading credentials: {ex.Message}");
                throw;
            }
        }

        public void SaveUser()
        {
            try
            {
                _storage.SaveCredentials(_userCredentials);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving credentials: {ex.Message}");
            }
        }

        public bool Register(string username, string password, bool isAdmin = false)
        {
            if (!_validator.ValidateCredentials(username, password))
            {
                RaiseAuthenticationMessage("Invalid credentials format");
                return false;
            }

            if (_userCredentials.ContainsKey(username))
            {
                RaiseAuthenticationMessage("Username already exists");
                return false;
            }

            _userCredentials.Add(username, (password, isAdmin));
            SaveUser();
            RaiseAuthenticationMessage($"Registration successful. User type: {(isAdmin ? "Admin" : "Regular User")}");
            return true;
        }

        public bool Login(string username, string password)
        {
            if (!_userCredentials.ContainsKey(username))
            {
                RaiseAuthenticationMessage("Invalid username");
                return false;
            }

            var userInfo = _userCredentials[username];
            if (password != userInfo.password)
            {
                RaiseAuthenticationMessage("Invalid password");
                return false;
            }

            RaiseAuthenticationMessage($"Login successful. User type: {(userInfo.isAdmin ? "Admin" : "Regular User")}");
            return true;
        }

        public bool IsAdmin(string username)
        {
            return _userCredentials.ContainsKey(username) && _userCredentials[username].isAdmin;
        }

        private void CreateDefaultAdmin()
        {
            Register("admin", "admin123", true);
        }

        private void RaiseAuthenticationMessage(string message)
        {
            OnAuthenticationMessage?.Invoke(this, message);
        }
    }
}