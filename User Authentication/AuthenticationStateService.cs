namespace UserAuthentication
{
    public class AuthenticationStateService
    {
        private bool _isAuthenticated;
        private string _currentUser = string.Empty;

        public bool IsAuthenticated => _isAuthenticated;
        public string CurrentUser => _currentUser;

        public void SetAuthenticationState(bool isAuthenticated, string username = "")
        {
            _isAuthenticated = isAuthenticated;
            _currentUser = username;
        }
    }
}