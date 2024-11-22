namespace UserAuthentication.cs
{
    public interface IUserValidator
    {
        bool ValidateCredentials(string username, string password);
    }
}