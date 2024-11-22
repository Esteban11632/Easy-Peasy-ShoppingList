namespace UserAuthentication.cs
{
        public interface ICredentialStorage
    {
        Dictionary<string, (string password, bool isAdmin)> LoadCredentials();
        void SaveCredentials(Dictionary<string, (string password, bool isAdmin)> credentials);
    }
}