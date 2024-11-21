namespace UserAuthentication.cs
{
    public interface IRegister // Interface for registering a user
    {
        bool Register(); // Method to register the user
        void SaveCredentials(); // Method to save the user credentials
    }
}