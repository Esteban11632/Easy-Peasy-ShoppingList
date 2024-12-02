using System;
using System.Collections.Generic;
using System.Linq;

namespace UserAuthentication
{
    public class LoginAttemptManager // Class for managing login attempts
    {
        private readonly Dictionary<string, List<DateTime>> _loginAttempts = new(); // Dictionary to store the login attempts
        private readonly Dictionary<string, DateTime> _lockoutUntil = new(); // Dictionary to store the lockout times
        
        private const int MaxAttempts = 5; // Maximum number of login attempts
        private const int LockoutMinutes = 15; // Lockout duration in minutes
        private const int AttemptWindowMinutes = 5; // Attempt window duration in minutes

        public bool IsLockedOut(string username) // Method to check if the user is locked out
        {
            if (_lockoutUntil.TryGetValue(username, out DateTime lockoutTime)) // Tries to get the lockout time for the user
            {
                if (DateTime.UtcNow < lockoutTime) // Checks if the current time is less than the lockout time
                {
                    return true; // Returns true if the user is locked out
                }
                _lockoutUntil.Remove(username); // Removes the lockout time for the user
            }
            return false; // Returns false if the user is not locked out
        }

        public void RecordAttempt(string username, bool wasSuccessful) // Method to record the login attempt
        {
            if (wasSuccessful) // Checks if the login attempt was successful
            {
                _loginAttempts.Remove(username); // Removes the login attempts for the user
                return; // Returns if the login attempt was successful
            }

            if (!_loginAttempts.ContainsKey(username)) // Checks if the user is in the login attempts dictionary
            {
                _loginAttempts[username] = new List<DateTime>(); // Creates a new list of login attempts for the user
            }

            _loginAttempts[username].Add(DateTime.UtcNow); // Adds the current time to the login attempts list for the user
            _loginAttempts[username] = _loginAttempts[username] // Filters the login attempts for the user
                .Where(attempt => attempt > DateTime.UtcNow.AddMinutes(-AttemptWindowMinutes)) // Filters the login attempts for the user
                .ToList();

            if (_loginAttempts[username].Count >= MaxAttempts) // Checks if the user has reached the maximum number of login attempts
            {
                _lockoutUntil[username] = DateTime.UtcNow.AddMinutes(LockoutMinutes); // Sets the lockout time for the user
                _loginAttempts.Remove(username); // Removes the login attempts for the user
            }
        }
    }
} 