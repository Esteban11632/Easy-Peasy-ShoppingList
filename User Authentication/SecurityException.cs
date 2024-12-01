using System;

public class SecurityException : Exception
{
    public SecurityException(string message, Exception? innerException = null) 
        : base(message, innerException)
    {
        // Log the detailed error internally but don't expose it to the user
        LogDetailedError(message, innerException);
    }

    private void LogDetailedError(string message, Exception? innerException)
    {
        // Implement detailed error logging here
    }
} 