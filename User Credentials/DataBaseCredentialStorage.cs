using Easy_Peasy_ShoppingList.Data;
using Easy_Peasy_ShoppingList.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Security.Cryptography;

namespace UserAuthentication
{
    public class DatabaseCredentialStorage : ICredentialStorage
    {
        private readonly ShoppingListDbContext _context;
        private readonly byte[] _encryptionKey;

        public DatabaseCredentialStorage(ShoppingListDbContext context)
        {
            _context = context;
            _encryptionKey = GetOrCreateEncryptionKey();
        }

        private byte[] GetOrCreateEncryptionKey()
        {
            // In production, this should be stored securely (e.g., Azure Key Vault)
            using (var aes = Aes.Create())
            {
                return aes.Key;
            }
        }

        public Dictionary<string, UserCredentials> LoadCredentials()
        {
            var credentials = new Dictionary<string, UserCredentials>();
            
            var users = _context.UserCredentials.ToList();
            foreach (var user in users)
            {
                var userCred = new UserCredentials(
                    user.Username,
                    user.PasswordHash,
                    user.Salt,
                    user.IsAdmin,
                    user.FamilyGroup
                );
                credentials[user.Username] = userCred;
            }

            return credentials;
        }

        public void SaveCredentials(Dictionary<string, UserCredentials> credentials)
        {
            try
            {
                // Remove users that no longer exist in the credentials dictionary
                var existingUsers = _context.UserCredentials.ToList();
                foreach (var user in existingUsers)
                {
                    if (!credentials.ContainsKey(user.Username))
                    {
                        _context.UserCredentials.Remove(user);
                    }
                }

                // Update or add users
                foreach (var cred in credentials.Values)
                {
                    var user = _context.UserCredentials
                        .FirstOrDefault(u => u.Username == cred.Username);

                    if (user == null)
                    {
                        user = new UserCredentialEntity
                        {
                            Username = cred.Username,
                            PasswordHash = cred.PasswordHash,
                            Salt = cred.Salt,
                            IsAdmin = cred.IsAdmin,
                            FamilyGroup = cred.FamilyGroup
                        };
                        _context.UserCredentials.Add(user);
                    }
                    else
                    {
                        user.PasswordHash = cred.PasswordHash;
                        user.Salt = cred.Salt;
                        user.IsAdmin = cred.IsAdmin;
                        user.FamilyGroup = cred.FamilyGroup;
                    }
                }

                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                // Log the error
                throw new SecurityException("Failed to save credentials", ex);
            }
        }

        public (HashSet<string> groups, Dictionary<string, string> admins) LoadFamilyGroups()
        {
            try
            {
                Console.WriteLine("Loading family groups from database...");
                
                var groups = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var admins = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                var familyGroups = _context.FamilyGroups.ToList();
                Console.WriteLine($"Found {familyGroups.Count} family groups in database");

                foreach (var group in familyGroups)
                {
                    Console.WriteLine($"Loading group: {group.GroupName} with admin: {group.AdminUsername}");
                    groups.Add(group.GroupName);
                    admins[group.GroupName] = group.AdminUsername;
                }

                return (groups, admins);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading family groups: {ex.Message}");
                throw;
            }
        }

        public void SaveFamilyGroups(HashSet<string> groups, Dictionary<string, string> admins)
        {
            try
            {
                Console.WriteLine($"Saving family groups. New groups count: {groups.Count}");
                
                // First, log existing groups in database
                var existingGroups = _context.FamilyGroups.ToList();
                Console.WriteLine($"Existing groups in database: {string.Join(", ", existingGroups.Select(g => g.GroupName))}");

                // Check if the database provider supports transactions
                var supportsTransactions = !(_context.Database.ProviderName?.Contains("InMemory") ?? false);
                
                IDbContextTransaction? transaction = null;
                try
                {
                    if (supportsTransactions)
                    {
                        transaction = _context.Database.BeginTransaction();
                    }

                    // Remove groups that no longer exist
                    foreach (var group in existingGroups)
                    {
                        if (!groups.Contains(group.GroupName, StringComparer.OrdinalIgnoreCase))
                        {
                            Console.WriteLine($"Removing group: {group.GroupName}");
                            _context.FamilyGroups.Remove(group);
                        }
                    }

                    // Update or add groups
                    foreach (var groupName in groups)
                    {
                        var group = existingGroups
                            .FirstOrDefault(g => string.Equals(g.GroupName, groupName, StringComparison.OrdinalIgnoreCase));

                        if (group == null)
                        {
                            Console.WriteLine($"Adding new group: {groupName}");
                            var adminUsername = admins.FirstOrDefault(a => 
                                string.Equals(a.Key, groupName, StringComparison.OrdinalIgnoreCase)).Value;
                                
                            group = new FamilyGroupEntity
                            {
                                GroupName = groupName,
                                AdminUsername = adminUsername
                            };
                            _context.FamilyGroups.Add(group);
                        }
                        else
                        {
                            Console.WriteLine($"Updating group: {groupName}");
                            var adminUsername = admins.FirstOrDefault(a => 
                                string.Equals(a.Key, groupName, StringComparison.OrdinalIgnoreCase)).Value;
                            group.AdminUsername = adminUsername;
                        }
                    }

                    _context.SaveChanges();
                    
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                    
                    Console.WriteLine("Successfully saved family groups");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during save: {ex.Message}");
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }
                    throw;
                }
                finally
                {
                    transaction?.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving family groups: {ex.Message}");
                throw new SecurityException("Failed to save family groups", ex);
            }
        }
    }
} 