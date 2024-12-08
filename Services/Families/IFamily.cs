namespace UserAuthentication
{
    public interface IFamily
    {
        string GetFamilyGroup(string username); // Method to get the family group
        Task<List<string>> GetUsersInFamilyGroup(string familyGroup); // Method to get the users in the family group
    }
}