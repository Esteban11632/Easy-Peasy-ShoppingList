namespace UserAuthentication
{
    public interface ILogin
    {
        event EventHandler<string> OnAuthenticationMessage; // Event handler for the authentication message
        Task<bool> Login(string username, string password); // Method to login
        bool IsAdmin(string username); // Method to check if the user is an admin
    }
}