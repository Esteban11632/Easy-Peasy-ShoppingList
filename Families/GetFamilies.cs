namespace UserAuthentication
{
    public class GetFamilies : IFamily
    {
        private readonly Dictionary<string, UserCredentials> _userCredentials;

        public GetFamilies(Dictionary<string, UserCredentials> userCredentials)
        {
            _userCredentials = userCredentials ?? throw new ArgumentNullException(nameof(userCredentials));
        }

        public string GetFamilyGroup(string username)
        {
            return _userCredentials.ContainsKey(username) ? _userCredentials[username].FamilyGroup : string.Empty;
        }

        public async Task<List<string>> GetUsersInFamilyGroup(string familyGroup)
        {
            return await Task.Run(() =>
                _userCredentials
                    .Where(u => u.Value.FamilyGroup == familyGroup)
                    .Select(u => u.Key)
                    .ToList()
            );
        }
    }
}