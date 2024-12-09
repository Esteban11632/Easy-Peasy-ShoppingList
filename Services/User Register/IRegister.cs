namespace UserAuthentication
{
    public interface IRegister // Interface for registering a user
    {
        event EventHandler<string> OnAuthenticationMessage;
        Task<bool> Register(string username, string password, string FamilyGroup, bool isAdmin = false); // Method to register the user
        Task SaveUser(); // Method to save the user credentials
    }
}