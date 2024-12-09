using System;
using System.IO;
using System.Text.Json;

public class SecurityAuditLogger
{
    private readonly string _logPath;
    private readonly object _lockObject = new();
    private const int MAX_FILE_SIZE = 10 * 1024 * 1024; // 10MB

    public SecurityAuditLogger(string logPath = "security_audit.log")
    {
        _logPath = logPath;
        EnsureLogDirectoryExists();
    }

    private void EnsureLogDirectoryExists()
    {
        var directory = Path.GetDirectoryName(_logPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
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

        try
        {
            lock (_lockObject)
            {
                RotateLogFileIfNeeded();
                File.AppendAllText(_logPath, 
                    $"{JsonSerializer.Serialize(logEntry)}{Environment.NewLine}");
            }
        }
        catch (Exception ex)
        {
            // Consider logging to a fallback location or raising an event
            System.Diagnostics.Debug.WriteLine($"Failed to log security event: {ex.Message}");
        }
    }

    private void RotateLogFileIfNeeded()
    {
        if (!File.Exists(_logPath)) return;

        var fileInfo = new FileInfo(_logPath);
        if (fileInfo.Length >= MAX_FILE_SIZE)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var newPath = Path.Combine(
                Path.GetDirectoryName(_logPath) ?? "",
                $"{Path.GetFileNameWithoutExtension(_logPath)}_{timestamp}{Path.GetExtension(_logPath)}");
            
            File.Move(_logPath, newPath);
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