public class SessionManager
{
    private readonly Dictionary<string, SessionInfo> _sessions = new();
    private const int SessionTimeoutMinutes = 30;

    public class SessionInfo
    {
        public required string Username { get; set; }
        public required string SessionId { get; set; }
        public DateTime LastActivity { get; set; }
        public required string FamilyGroup { get; set; }
        public bool IsAdmin { get; set; }
    }

    public string CreateSession(string username, string familyGroup, bool isAdmin)
    {
        var sessionId = GenerateSessionId();
        _sessions[sessionId] = new SessionInfo
        {
            Username = username,
            SessionId = sessionId,
            LastActivity = DateTime.UtcNow,
            FamilyGroup = familyGroup,
            IsAdmin = isAdmin
        };
        return sessionId;
    }

    public bool ValidateSession(string sessionId)
    {
        if (string.IsNullOrEmpty(sessionId) || !_sessions.TryGetValue(sessionId, out SessionInfo? session) || session == null)
            return false;

        if (DateTime.UtcNow > session.LastActivity.AddMinutes(SessionTimeoutMinutes))
        {
            _sessions.Remove(sessionId);
            return false;
        }

        session.LastActivity = DateTime.UtcNow;
        return true;
    }

    private string GenerateSessionId()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }
} 