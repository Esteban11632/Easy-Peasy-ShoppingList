using System;

public class SecurityException : Exception
{
    public SecurityException(string message, Exception? innerException = null) 
        : base(message, innerException)
    {
        LogDetailedError(message, innerException);
    }

    private void LogDetailedError(string message, Exception? innerException)
    {
        try
        {
            var securityLogger = new SecurityAuditLogger();
            securityLogger.LogSecurityEvent(
                username: "SYSTEM", // Or pass through actual username if available
                eventType: "SecurityException",
                details: $"Message: {message}, Inner Exception: {innerException?.Message ?? "None"}",
                wasSuccessful: false
            );
        }
        catch (Exception ex)
        {
            // Fail silently but log to debug - we don't want logging failures to affect the exception flow
            System.Diagnostics.Debug.WriteLine($"Failed to log security exception: {ex.Message}");
        }
    }
} 