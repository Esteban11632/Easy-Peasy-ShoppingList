using Easy_Peasy_ShoppingList.Data;
using Easy_Peasy_ShoppingList.Models;
using Microsoft.EntityFrameworkCore;
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
            var groups = _context.FamilyGroups
                .Select(g => g.GroupName)
                .ToHashSet();

            var admins = _context.FamilyGroups
                .ToDictionary(g => g.GroupName, g => g.AdminUsername);

            return (groups, admins);
        }

        public void SaveFamilyGroups(HashSet<string> groups, Dictionary<string, string> admins)
        {
            // Remove groups that no longer exist
            var existingGroups = _context.FamilyGroups.ToList();
            foreach (var group in existingGroups)
            {
                if (!groups.Contains(group.GroupName))
                {
                    _context.FamilyGroups.Remove(group);
                }
            }

            // Update or add groups
            foreach (var groupName in groups)
            {
                var group = _context.FamilyGroups
                    .FirstOrDefault(g => g.GroupName == groupName);

                if (group == null)
                {
                    group = new FamilyGroupEntity
                    {
                        GroupName = groupName,
                        AdminUsername = admins.ContainsKey(groupName) ? admins[groupName] : string.Empty
                    };
                    _context.FamilyGroups.Add(group);
                }
                else
                {
                    group.AdminUsername = admins.ContainsKey(groupName) ? admins[groupName] : string.Empty;
                }
            }

            _context.SaveChanges();
        }
    }
} 