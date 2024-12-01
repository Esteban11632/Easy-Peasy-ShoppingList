using System;
using System.Collections.Generic;
using System.Linq;

public class LoginAttemptManager
{
    private readonly Dictionary<string, List<DateTime>> _loginAttempts = new();
    private readonly Dictionary<string, DateTime> _lockoutUntil = new();
    
    private const int MaxAttempts = 5;
    private const int LockoutMinutes = 15;
    private const int AttemptWindowMinutes = 5;

    public bool IsLockedOut(string username)
    {
        if (_lockoutUntil.TryGetValue(username, out DateTime lockoutTime))
        {
            if (DateTime.UtcNow < lockoutTime)
            {
                return true;
            }
            _lockoutUntil.Remove(username);
        }
        return false;
    }

    public void RecordAttempt(string username, bool wasSuccessful)
    {
        if (wasSuccessful)
        {
            _loginAttempts.Remove(username);
            return;
        }

        if (!_loginAttempts.ContainsKey(username))
        {
            _loginAttempts[username] = new List<DateTime>();
        }

        _loginAttempts[username].Add(DateTime.UtcNow);
        _loginAttempts[username] = _loginAttempts[username]
            .Where(attempt => attempt > DateTime.UtcNow.AddMinutes(-AttemptWindowMinutes))
            .ToList();

        if (_loginAttempts[username].Count >= MaxAttempts)
        {
            _lockoutUntil[username] = DateTime.UtcNow.AddMinutes(LockoutMinutes);
            _loginAttempts.Remove(username);
        }
    }
} 