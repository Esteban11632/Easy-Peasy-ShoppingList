namespace UserAuthentication
{
    public interface ICredentialStorage
    {
        Dictionary<string, UserCredentials> LoadCredentials(); // Method to load the credentials
        void SaveCredentials(Dictionary<string, UserCredentials> credentials); // Method to save the credentials
        (HashSet<string> groups, Dictionary<string, string> admins) LoadFamilyGroups(); // Method to load the family groups
        void SaveFamilyGroups(HashSet<string> groups, Dictionary<string, string> admins); // Method to save the family groups
    }
}