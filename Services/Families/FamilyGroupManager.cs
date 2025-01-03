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
                Console.WriteLine($"Invalid input - Username: {username}, FamilyGroup: {newFamilyGroup}"); // Logs the invalid input
                return false; // Returns false if the username or family group is null or empty
            }

            lock (_familyGroups) // Add thread safety
            {
                Console.WriteLine($"Attempting to create family group: {newFamilyGroup}"); // Logs the attempt to create the family group
                Console.WriteLine($"Current family groups: {string.Join(", ", _familyGroups)}"); // Logs the current family groups

                // If family group already exists, return false
                if (_familyGroups.Contains(newFamilyGroup))
                {
                    Console.WriteLine($"Family group already exists: {newFamilyGroup}"); // Logs the family group already exists
                    return false; // Returns false if the family group already exists
                }

                // Create new family group with admin
                _familyGroups.Add(newFamilyGroup); // Adds the new family group
                _familyGroupAdmins[newFamilyGroup] = username; // Adds the admin to the family group
                
                try
                {
                    SaveFamilyGroups(); // Saves the family groups
                    Console.WriteLine($"Successfully created family group: {newFamilyGroup} with admin: {username}"); // Logs the success of creating the family group
                    return true; // Returns true if the family group was created
                }
                catch (Exception ex) // Catches any exceptions
                {
                    Console.WriteLine($"Error saving family group: {ex.Message}"); // Logs the error saving the family group
                    // Rollback changes if save fails
                    _familyGroups.Remove(newFamilyGroup); // Removes the new family group
                    _familyGroupAdmins.Remove(newFamilyGroup); // Removes the admin from the family group
                    throw; // Throws an exception if there is an error saving the family groups
                }
            }
        }

        public bool IsFamilyGroupAdmin(string username, string familyGroup) // Checks if the user is the admin of the family group
        {
            return _familyGroupAdmins.ContainsKey(familyGroup) && // Checks if the family group exists
                   _familyGroupAdmins[familyGroup] == username; // Checks if the user is the admin
        }

        public bool FamilyGroupExists(string familyGroup)
        {
            var exists = _familyGroups.Contains(familyGroup); // Checks if the family group exists
            Console.WriteLine($"Checking if family group exists: {familyGroup} - Result: {exists}"); // Logs the result of checking if the family group exists
            return exists; // Returns true if the family group exists
        }

        public IEnumerable<string> GetAllFamilyGroups() // Returns all the family groups
        {
            return _familyGroups.ToList(); // Returns the family groups
        }

        public string GetFamilyGroupAdmin(string familyGroup) // Returns the admin of the family group
        {
            return _familyGroupAdmins.ContainsKey(familyGroup) ? // Checks if the family group exists
                   _familyGroupAdmins[familyGroup] : string.Empty; // Returns the admin of the family group
        }

        private void LoadFamilyGroups() // Loads the family groups
        {
            try
            {
                Console.WriteLine("Loading family groups..."); // Logs the loading of the family groups
                var (groups, admins) = _storage.LoadFamilyGroups(); // Loads the family groups
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

                Console.WriteLine($"Loaded family groups: {string.Join(", ", _familyGroups)}"); // Logs the loaded family groups
                Console.WriteLine($"Loaded admins: {string.Join(", ", _familyGroupAdmins.Select(x => $"{x.Key}:{x.Value}"))}"); // Logs the loaded admins
            }
            catch (Exception ex) // Catches any exceptions
            {
                Console.WriteLine($"Error loading family groups: {ex.Message}"); // Logs the error loading the family groups
                throw new Exception($"Error loading family groups: {ex.Message}"); // Throws an exception if there is an error loading the family groups
            }
        }

        private void SaveFamilyGroups() // Saves the family groups
        {
            try // Try to save the family groups
            {
                _storage.SaveFamilyGroups(_familyGroups, _familyGroupAdmins); // Saves the family groups
            }
            catch (Exception ex) // Catches any exceptions
            {
                throw new Exception($"Error saving family groups: {ex.Message}"); // Throws an exception if there is an error saving the family groups
            }
        }
    }
}