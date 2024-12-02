namespace UserAuthentication
{
    public class FamilyGroupManager : IFamilyGroupManager
    {
        private readonly HashSet<string> _familyGroups; // Stores the family groups
        private readonly IUserValidator _userValidator; // Validates the user
        private readonly ICredentialStorage _storage; // Stores the credentials
        private readonly Dictionary<string, string> _familyGroupAdmins; // Stores family group and its admin

        public FamilyGroupManager(IUserValidator userValidator, ICredentialStorage storage) // Constructor for the FamilyGroupManager class
        {
            _userValidator = userValidator ?? throw new ArgumentNullException(nameof(userValidator)); // Validates the user
            _storage = storage ?? throw new ArgumentNullException(nameof(storage)); // Stores the credentials
            _familyGroups = new HashSet<string>(StringComparer.OrdinalIgnoreCase); // Make case-insensitive
            _familyGroupAdmins = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); // Make case-insensitive
            LoadFamilyGroups(); // Loads the family groups
        }

        public bool CreateFamilyGroup(string username, string newFamilyGroup) // Creates a new family group
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(newFamilyGroup)) // Checks if the username or family group is null or empty
            {
                Console.WriteLine($"Invalid input - Username: {username}, FamilyGroup: {newFamilyGroup}");
                return false;
            }

            lock (_familyGroups) // Add thread safety
            {
                Console.WriteLine($"Attempting to create family group: {newFamilyGroup}");
                Console.WriteLine($"Current family groups: {string.Join(", ", _familyGroups)}");

                // If family group already exists, return false
                if (_familyGroups.Contains(newFamilyGroup))
                {
                    Console.WriteLine($"Family group already exists: {newFamilyGroup}");
                    return false;
                }

                // Create new family group with admin
                _familyGroups.Add(newFamilyGroup);
                _familyGroupAdmins[newFamilyGroup] = username;
                
                try
                {
                    SaveFamilyGroups();
                    Console.WriteLine($"Successfully created family group: {newFamilyGroup} with admin: {username}");
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving family group: {ex.Message}");
                    // Rollback changes if save fails
                    _familyGroups.Remove(newFamilyGroup);
                    _familyGroupAdmins.Remove(newFamilyGroup);
                    throw;
                }
            }
        }

        public bool IsFamilyGroupAdmin(string username, string familyGroup) // Checks if the user is the admin of the family group
        {
            return _familyGroupAdmins.ContainsKey(familyGroup) &&
                   _familyGroupAdmins[familyGroup] == username; // Checks if the family group exists and if the user is the admin
        }

        public bool FamilyGroupExists(string familyGroup)
        {
            var exists = _familyGroups.Contains(familyGroup);
            Console.WriteLine($"Checking if family group exists: {familyGroup} - Result: {exists}");
            return exists;
        }

        public IEnumerable<string> GetAllFamilyGroups()
        {
            return _familyGroups.ToList(); // Returns the family groups
        }

        public string GetFamilyGroupAdmin(string familyGroup)
        {
            return _familyGroupAdmins.ContainsKey(familyGroup) ?
                   _familyGroupAdmins[familyGroup] : string.Empty; // Returns the admin of the family group
        }

        private void LoadFamilyGroups()
        {
            try
            {
                Console.WriteLine("Loading family groups...");
                var (groups, admins) = _storage.LoadFamilyGroups();
                _familyGroups.Clear(); // Clears the family groups
                _familyGroupAdmins.Clear(); // Clears the family group admins

                foreach (var group in groups) // Adds the family groups
                {
                    _familyGroups.Add(group); // Adds the family groups
                }

                foreach (var admin in admins) // Adds the family group admins
                {
                    _familyGroupAdmins[admin.Key] = admin.Value; // Adds the family group admins
                }

                Console.WriteLine($"Loaded family groups: {string.Join(", ", _familyGroups)}");
                Console.WriteLine($"Loaded admins: {string.Join(", ", _familyGroupAdmins.Select(x => $"{x.Key}:{x.Value}"))}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading family groups: {ex.Message}");
                throw new Exception($"Error loading family groups: {ex.Message}");
            }
        }

        private void SaveFamilyGroups() // Saves the family groups
        {
            try
            {
                _storage.SaveFamilyGroups(_familyGroups, _familyGroupAdmins); // Saves the family groups
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving family groups: {ex.Message}"); // Throws an exception if there is an error saving the family groups
            }
        }
    }
}