namespace UserAuthentication
{
    public interface IRegister // Interface for registering a user
    {
        bool Register(string username, string password, string FamilyGroup, bool isAdmin = false); // Method to register the user
        void SaveUser(); // Method to save the user credentials
    }
}