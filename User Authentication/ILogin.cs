namespace UserAuthentication.cs
{
    public interface ILogin // Interface for logging in a user
    {
        void LoadCredentials(); // Method to load the user credentials
        bool Login(string username, string password); // Method to login the user
    }
}