namespace UserAuthentication
{
    public interface ILogin
    {
        Task<bool> Login(string username, string password);
        bool IsAdmin(string username);
        string GetFamilyGroup(string username);
        Task<List<string>> GetUsersInFamilyGroup(string familyGroup);
    }
}