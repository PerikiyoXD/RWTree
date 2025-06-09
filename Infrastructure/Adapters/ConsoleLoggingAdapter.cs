using RWTree.Application.Ports;

namespace RWTree.Infrastructure.Adapters;

/// <summary>
/// Console logging adapter for development/debugging
/// </summary>
public sealed class ConsoleLoggingAdapter : ILoggingPort
{
    public void LogInformation(string message, params object[] args)
    {
        var formattedMessage = FormatMessage(message, args);
        Console.WriteLine($"[INFO] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {formattedMessage}");
    }

    public void LogWarning(string message, params object[] args)
    {
        var formattedMessage = FormatMessage(message, args);
        Console.WriteLine($"[WARN] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {formattedMessage}");
    }

    public void LogError(string message, Exception? exception = null, params object[] args)
    {
        var formattedMessage = FormatMessage(message, args);
        var errorMessage = $"[ERROR] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {formattedMessage}";

        if (exception != null)
        {
            errorMessage += $"\nException: {exception}";
        }

        Console.WriteLine(errorMessage);
    }

    public void LogDebug(string message, params object[] args)
    {
        var formattedMessage = FormatMessage(message, args);
        Console.WriteLine($"[DEBUG] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {formattedMessage}");
    }

    private static string FormatMessage(string message, params object[] args)
    {
        if (args.Length == 0)
            return message;

        try
        {
            // Handle structured logging format (e.g., "{FilePath}")
            if (message.Contains('{') && message.Contains('}') && !message.Contains("{0}"))
            {
                // Simple replacement for structured logging
                var result = message;
                for (int i = 0; i < args.Length; i++)
                {
                    // Replace first occurrence of any {PropertyName} with the argument
                    var startIndex = result.IndexOf('{');
                    if (startIndex >= 0)
                    {
                        var endIndex = result.IndexOf('}', startIndex);
                        if (endIndex > startIndex)
                        {
                            result = result.Substring(0, startIndex) + args[i]?.ToString() +
                                     result.Substring(endIndex + 1);
                        }
                    }
                }

                return result;
            }
            else
            {
                // Handle standard string.Format (e.g., "{0}")
                return string.Format(message, args);
            }
        }
        catch (FormatException)
        {
            // Fallback: just append args
            return $"{message} [{string.Join(", ", args)}]";
        }
    }
}