namespace UserAuthentication
{
    public interface IFamilyGroupManager
    {
        bool CreateFamilyGroup(string adminUsername, string newFamilyGroup); // Method to create a family group
        bool FamilyGroupExists(string familyGroup); // Method to check if a family group exists
        IEnumerable<string> GetAllFamilyGroups(); // Method to get all family groups
    }
}