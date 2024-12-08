public class SessionManager
{
    private readonly Dictionary<string, SessionInfo> _sessions = new(); // Dictionary to store the sessions
    private readonly object _lockObject = new(); // Lock object for thread safety
    private const int SessionTimeoutMinutes = 30; // Session timeout in minutes

    public class SessionInfo // Class to store the session information
    {
        public required string Username { get; set; } // Username
        public required string SessionId { get; set; } // Session ID
        public DateTime LastActivity { get; set; } // Last activity
        public required string FamilyGroup { get; set; } // Family group
        public bool IsAdmin { get; set; } // Is admin
    }

    public string CreateSession(string username, string familyGroup, bool isAdmin) // Method to create a session
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(familyGroup)) // Checks if the username or family group is null or empty
        {
            throw new SecurityException("Username and family group are required for session creation"); // Throws a security exception if the username or family group is null or empty
        }

        lock (_lockObject) // Locks the object for thread safety
        {
            var sessionId = GenerateSessionId(); // Generates a session ID
            _sessions[sessionId] = new SessionInfo // Creates a new session
            {
                Username = username, // Sets the username
                SessionId = sessionId, // Sets the session ID
                LastActivity = DateTime.UtcNow, // Sets the last activity
                FamilyGroup = familyGroup, // Sets the family group
                IsAdmin = isAdmin // Sets if the user is an admin
            };
            return sessionId; // Returns the session ID
        }
    }

    public bool ValidateSession(string sessionId) // Method to validate a session
    {
        lock (_lockObject) // Locks the object for thread safety
        {
            if (string.IsNullOrEmpty(sessionId) || !_sessions.TryGetValue(sessionId, out SessionInfo? session)) // Checks if the session ID is null or empty or if the session ID is not in the dictionary
                return false; // Returns false if the session ID is null or empty or if the session ID is not in the dictionary

            if (DateTime.UtcNow > session.LastActivity.AddMinutes(SessionTimeoutMinutes)) // Checks if the current time is greater than the last activity time plus the session timeout
            {
                _sessions.Remove(sessionId); // Removes the session from the dictionary
                return false; // Returns false if the session has timed out
            }

            session.LastActivity = DateTime.UtcNow; // Updates the last activity time
            return true; // Returns true if the session is valid
        }
    }

    public SessionInfo? GetSession(string sessionId) // Method to get a session
    {
        lock (_lockObject) // Locks the object for thread safety
        {
            return _sessions.TryGetValue(sessionId, out SessionInfo? session) ? session : null; // Returns the session if it exists, otherwise returns null
        }
    }

    private string GenerateSessionId() // Method to generate a session ID
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray()); // Generates a session ID
    }
} 