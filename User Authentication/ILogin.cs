namespace UserAuthentication.cs
{
    public interface ILogin // Interface for logging in a user
    {
        void LoadUser(); // Method to load the user credentials
        bool Login(string username, string password); // Method to login the user
        bool IsAdmin(string username); // Method to check if the user is an admin
    }
}