using Easy_Peasy_ShoppingList.Data;
using Easy_Peasy_ShoppingList.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Security.Cryptography;

namespace UserAuthentication
{
    public class DatabaseCredentialStorage : ICredentialStorage // implements ICredentialStorage interface
    {
        private readonly ShoppingListDbContext _context; // context for the database
        private readonly byte[] _encryptionKey; // encryption key for the credentials

        public DatabaseCredentialStorage(ShoppingListDbContext context) // constructor for the class
        {
            _context = context; // sets the context for the database
            _encryptionKey = GetOrCreateEncryptionKey(); // gets or creates the encryption key
        }

        private byte[] GetOrCreateEncryptionKey() // method to get or create the encryption key
        {
            // In production, this should be stored securely (e.g., Azure Key Vault)
            using (var aes = Aes.Create()) // creates a new AES object
            {
                return aes.Key; // returns the key
            }
        }

        public Dictionary<string, UserCredentials> LoadCredentials() // method to load the credentials
        {
            var credentials = new Dictionary<string, UserCredentials>(); // creates a new dictionary for the credentials
            
            var users = _context.UserCredentials.ToList(); // gets the users from the database
            foreach (var user in users) // iterates through the users
            {
                var userCred = new UserCredentials( // creates a new user credentials object
                    user.Username, // username
                    user.PasswordHash, // password hash
                    user.Salt, // salt
                    user.IsAdmin, // is admin
                    user.FamilyGroup // family group
                );
                credentials[user.Username] = userCred; // adds the user credentials to the dictionary
            }

            return credentials; // returns the credentials
        }

        public void SaveCredentials(Dictionary<string, UserCredentials> credentials) // method to save the credentials
        {
            try
            {
                // Remove users that no longer exist in the credentials dictionary
                var existingUsers = _context.UserCredentials.ToList(); // gets the existing users from the database
                foreach (var user in existingUsers) // iterates through the existing users
                {
                    if (!credentials.ContainsKey(user.Username)) // checks if the user is not in the credentials dictionary
                    {
                        _context.UserCredentials.Remove(user); // removes the user from the database
                    }
                }

                // Update or add users
                foreach (var cred in credentials.Values) // iterates through the credentials
                {
                    var user = _context.UserCredentials // gets the user from the database
                        .FirstOrDefault(u => u.Username == cred.Username); // gets the user by username

                    if (user == null) // checks if the user is null
                    {
                        user = new UserCredentialEntity // creates a new user credential entity
                        {
                            Username = cred.Username, // username
                            PasswordHash = cred.PasswordHash, // password hash
                            Salt = cred.Salt, // salt
                            IsAdmin = cred.IsAdmin, // is admin
                            FamilyGroup = cred.FamilyGroup // family group
                        };
                        _context.UserCredentials.Add(user); // adds the user to the database
                    }
                    else
                    {
                        user.PasswordHash = cred.PasswordHash; // password hash
                        user.Salt = cred.Salt; // salt
                        user.IsAdmin = cred.IsAdmin; // is admin
                        user.FamilyGroup = cred.FamilyGroup; // family group
                    }
                }

                _context.SaveChanges(); // saves the changes to the database
            }
            catch (DbUpdateException ex) // catches the exception
            {
                // Log the error
                throw new SecurityException("Failed to save credentials", ex); // throws a new security exception
            }
        }

        public (HashSet<string> groups, Dictionary<string, string> admins) LoadFamilyGroups() // method to load the family groups
        {
            try
            {
                Console.WriteLine("Loading family groups from database...");
                
                var groups = new HashSet<string>(StringComparer.OrdinalIgnoreCase); // creates a new hash set for the groups
                var admins = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); // creates a new dictionary for the admins

                var familyGroups = _context.FamilyGroups.ToList(); // gets the family groups from the database
                Console.WriteLine($"Found {familyGroups.Count} family groups in database"); // displays the number of family groups in the database

                foreach (var group in familyGroups)
                {
                    Console.WriteLine($"Loading group: {group.GroupName} with admin: {group.AdminUsername}");
                    groups.Add(group.GroupName); // adds the group to the hash set
                    admins[group.GroupName] = group.AdminUsername; // adds the admin to the dictionary
                }

                return (groups, admins); // returns the groups and admins
            }
            catch (Exception ex) // catches the exception
            {
                Console.WriteLine($"Error loading family groups: {ex.Message}"); // displays the error message
                throw; // throws the exception
            }
        }

        public void SaveFamilyGroups(HashSet<string> groups, Dictionary<string, string> admins) // method to save the family groups
        {
            try
            {
                Console.WriteLine($"Saving family groups. New groups count: {groups.Count}"); // displays the number of new groups
                
                // First, log existing groups in database
                var existingGroups = _context.FamilyGroups.ToList(); // gets the existing groups from the database
                Console.WriteLine($"Existing groups in database: {string.Join(", ", existingGroups.Select(g => g.GroupName))}"); // displays the existing groups in the database

                // Check if the database provider supports transactions
                var supportsTransactions = !(_context.Database.ProviderName?.Contains("InMemory") ?? false); // checks if the database provider supports transactions
                
                IDbContextTransaction? transaction = null; // creates a new transaction
                try
                {
                    if (supportsTransactions)
                    {
                        transaction = _context.Database.BeginTransaction(); // begins the transaction
                    }

                    // Remove groups that no longer exist
                    foreach (var group in existingGroups)
                    {
                        if (!groups.Contains(group.GroupName, StringComparer.OrdinalIgnoreCase)) // checks if the group is not in the hash set
                        {
                            Console.WriteLine($"Removing group: {group.GroupName}"); // displays the group that is being removed
                            _context.FamilyGroups.Remove(group); // removes the group from the database
                        }
                    }

                    // Update or add groups
                    foreach (var groupName in groups) // iterates through the groups
                    {
                        var group = existingGroups // gets the group from the database
                            .FirstOrDefault(g => string.Equals(g.GroupName, groupName, StringComparison.OrdinalIgnoreCase)); // gets the group by group name

                        if (group == null) // checks if the group is null
                        {
                            Console.WriteLine($"Adding new group: {groupName}");
                            var adminUsername = admins.FirstOrDefault(a => 
                                string.Equals(a.Key, groupName, StringComparison.OrdinalIgnoreCase)).Value; // gets the admin username
                                
                            group = new FamilyGroupEntity // creates a new family group entity
                            {
                                GroupName = groupName, // group name
                                AdminUsername = adminUsername // admin username
                            };
                            _context.FamilyGroups.Add(group); // adds the group to the database
                        }
                        else
                        {
                            Console.WriteLine($"Updating group: {groupName}"); // displays the group that is being updated
                            var adminUsername = admins.FirstOrDefault(a => 
                                string.Equals(a.Key, groupName, StringComparison.OrdinalIgnoreCase)).Value; // gets the admin username
                            group.AdminUsername = adminUsername; // sets the admin username
                        }
                    }

                    _context.SaveChanges(); // saves the changes to the database
                    
                    if (transaction != null) // checks if the transaction is not null
                    {
                        transaction.Commit(); // commits the transaction
                    }
                    
                    Console.WriteLine("Successfully saved family groups"); // displays a success message
                }
                catch (Exception ex) // catches the exception
                {
                    Console.WriteLine($"Error during save: {ex.Message}"); // displays the error message
                    if (transaction != null) // checks if the transaction is not null
                    {
                        transaction.Rollback(); // rolls back the transaction
                    }
                    throw; // throws the exception
                }
                finally // finally block
                {
                    transaction?.Dispose(); // disposes of the transaction
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving family groups: {ex.Message}"); // displays the error message
                throw new SecurityException("Failed to save family groups", ex); // throws a new security exception
            }
        }
    }
} 