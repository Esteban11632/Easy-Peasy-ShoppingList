namespace UserAuthentication.cs
{
    public interface ILogin // Interface for logging in a user
    {
        void LoadCredentials(); // Method to load the user credentials
        bool Login(); // Method to login the user
    }
}