using System.Threading.Tasks;

namespace UserAuthentication
{
    public interface IUserService
    {
        Task<string> GetFamilyGroupAsync(string username);
        Task<bool> ChangeUsernameAsync(string currentUsername, string newUsername);
    }
}