namespace UserAuthentication
{
    using System.Collections.Concurrent;

    public class RegisterManager : IRegister
    {
        private readonly ICredentialStorage _storage;
        private readonly IUserValidator _validator;
        private readonly IFamilyGroupManager _familyGroupManager;
        private readonly ConcurrentDictionary<string, UserCredentials> _userCredentials;
        private readonly SecurityAuditLogger _auditLogger;
        private readonly SemaphoreSlim _saveLock = new SemaphoreSlim(1, 1);
        public event EventHandler<string>? OnAuthenticationMessage;

        public RegisterManager(ICredentialStorage storage, IUserValidator validator, IFamilyGroupManager familyGroupManager)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _familyGroupManager = familyGroupManager ?? throw new ArgumentNullException(nameof(familyGroupManager));
            _userCredentials = new ConcurrentDictionary<string, UserCredentials>(StringComparer.OrdinalIgnoreCase);
            _auditLogger = new SecurityAuditLogger();
            LoadUser();
        }

        private void LoadUser()
        {
            try
            {
                var credentials = _storage.LoadCredentials();
                foreach (var cred in credentials)
                {
                    _userCredentials.TryAdd(cred.Key, cred.Value);
                }
            }
            catch (Exception ex)
            {
                RaiseAuthenticationMessage($"Error loading credentials: {ex.Message}");
                throw;
            }
        }

        public async Task SaveUser()
        {
            try
            {
                await _saveLock.WaitAsync();
                await Task.Run(() => _storage.SaveCredentials(_userCredentials.ToDictionary(k => k.Key, v => v.Value)));
            }
            catch (Exception ex)
            {
                throw new SecurityException($"Error saving credentials: {ex.Message}", ex);
            }
            finally
            {
                _saveLock.Release();
            }
        }

        public async Task<bool> Register(string username, string password, string familyGroup, bool isAdmin = false)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(familyGroup))
            {
                RaiseAuthenticationMessage("Username, password, and family group cannot be empty");
                return false;
            }

            try
            {
                if (!_validator.ValidateCredentials(username, password, familyGroup))
                {
                    RaiseAuthenticationMessage("Invalid credentials format");
                    return false;
                }

                // Check family group status first
                if (isAdmin)
                {
                    // For admin users, check if family group already exists
                    if (_familyGroupManager.FamilyGroupExists(familyGroup))
                    {
                        RaiseAuthenticationMessage("Failed to create family group - it may already exist");
                        return false;
                    }
                }
                else
                {
                    // For regular users, verify family group exists
                    if (!_familyGroupManager.FamilyGroupExists(familyGroup))
                    {
                        RaiseAuthenticationMessage("Invalid family group");
                        return false;
                    }
                }

                // Now try to add the user
                if (!_userCredentials.TryAdd(username, new UserCredentials(username, password, isAdmin, familyGroup)))
                {
                    RaiseAuthenticationMessage("Username already exists");
                    return false;
                }

                try
                {
                    // Create family group for admin after user is created
                    if (isAdmin)
                    {
                        if (!_familyGroupManager.CreateFamilyGroup(username, familyGroup))
                        {
                            _userCredentials.TryRemove(username, out _);
                            RaiseAuthenticationMessage("Failed to create family group");
                            return false;
                        }
                    }

                    await SaveUser();
                    
                    _auditLogger.LogSecurityEvent(username, "REGISTRATION", $"User registered in {familyGroup} group", true);
                    RaiseAuthenticationMessage($"Registration successful. User type: {(isAdmin ? "Admin" : "Regular User")}");
                    return true;
                }
                catch
                {
                    _userCredentials.TryRemove(username, out _);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _auditLogger.LogSecurityEvent(username, "REGISTRATION_ERROR", ex.Message, false);
                throw new SecurityException("An error occurred during registration", ex);
            }
        }

        private void RaiseAuthenticationMessage(string message)
        {
            OnAuthenticationMessage?.Invoke(this, message);
        }
    }
}