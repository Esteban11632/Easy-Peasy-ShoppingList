namespace UserAuthentication
{
    public interface IUserValidator
    {
        bool ValidateCredentials(string username, string password, string FamilyGroup);
    }
}