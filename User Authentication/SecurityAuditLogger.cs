using System;
using System.IO;
using System.Text.Json;

public class SecurityAuditLogger
{
    private readonly string _logPath;
    private readonly object _lockObject = new();

    public SecurityAuditLogger(string logPath = "security_audit.log")
    {
        _logPath = logPath;
    }

    public void LogSecurityEvent(string username, string eventType, string details, bool wasSuccessful)
    {
        var logEntry = new
        {
            Timestamp = DateTime.UtcNow,
            Username = username,
            EventType = eventType,
            Details = details,
            Success = wasSuccessful,
            IpAddress = GetCurrentIpAddress(),
            UserAgent = GetCurrentUserAgent()
        };

        lock (_lockObject)
        {
            File.AppendAllText(_logPath, 
                $"{JsonSerializer.Serialize(logEntry)}{Environment.NewLine}");
        }
    }

    private string GetCurrentIpAddress()
    {
        // Implement based on your web framework
        return "IP_ADDRESS";
    }

    private string GetCurrentUserAgent()
    {
        // Implement based on your web framework
        return "USER_AGENT";
    }
} 