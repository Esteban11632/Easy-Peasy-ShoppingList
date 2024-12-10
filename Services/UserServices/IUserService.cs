using System.Collections.Generic;
using System.Threading.Tasks;
using Easy_Peasy_ShoppingList.Models;

namespace UserAuthentication
{
    public interface IUserService
    {
        Task<string> GetFamilyGroupAsync(string username);
        Task<bool> ChangeUsernameAsync(string currentUsername, string newUsername);

        // New Methods for Managing Permissions
        Task<List<FamilyMemberModel>> GetFamilyMembersAsync(string familyGroup);
        Task<bool> GrantAdminPermissionAsync(string username);
        Task<bool> RevokeAdminPermissionAsync(string username);
    }
}