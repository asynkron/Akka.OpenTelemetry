using Akka.Event;
using Microsoft.Extensions.Logging;
using LogLevel = Akka.Event.LogLevel;

namespace Akka.OpenTelemetry.Logging;

/// <summary>
/// ILogger wrapper for Akka.NET's <see cref="ILoggingAdapter"/> interface.
/// This bypass the actor bus-logging used by Akka.NET and instead uses the <see cref="ILogger"/> interface.
/// The reason is that we need to perform the proper logging operations on the Activity.Current span.
/// </summary>
public class OtelLogger : ILoggingAdapter
{
    private readonly ILogger Logger;

    public OtelLogger(ILogger logger)
    {
        Logger = logger;
    }
    public bool IsEnabled(LogLevel logLevel)
    {
        return Logger.IsEnabled(ToLogLevel(logLevel));
    }

    public void Log(LogLevel logLevel, Exception cause, string format)
    {
#pragma warning disable CA2254
        Logger.Log(ToLogLevel(logLevel), cause, format);
#pragma warning restore CA2254
    }

    public void Log(LogLevel logLevel, Exception cause, LogMessage message)
    {
#pragma warning disable CA2254
        Logger.Log(ToLogLevel(logLevel), cause, message.Format, message.Parameters());
#pragma warning restore CA2254
    }

    public ILogMessageFormatter Formatter { get; }

    private Microsoft.Extensions.Logging.LogLevel ToLogLevel(LogLevel logLevel) =>
        logLevel switch
        {
            LogLevel.DebugLevel => Microsoft.Extensions.Logging.LogLevel.Debug,
            LogLevel.ErrorLevel => Microsoft.Extensions.Logging.LogLevel.Error,
            LogLevel.InfoLevel => Microsoft.Extensions.Logging.LogLevel.Information,
            LogLevel.WarningLevel => Microsoft.Extensions.Logging.LogLevel.Warning,
            _ => Microsoft.Extensions.Logging.LogLevel.None
        };

    public void Debug(string format, params object[] args)
    {
        Logger.LogDebug(format, args);
    }

    public void Debug(Exception cause, string format, params object[] args)
    {
        Logger.LogDebug(cause, format, args);
    }

    public void Info(string format, params object[] args)
    {
        Logger.LogInformation(format, args);
    }

    public void Info(Exception cause, string format, params object[] args)
    {
        Logger.LogInformation(cause, format, args);
    }

    public void Warning(string format, params object[] args)
    {
        Logger.LogWarning(format, args);
    }

    public void Warning(Exception cause, string format, params object[] args)
    {
        Logger.LogWarning(cause, format, args);
    }

    public void Error(string format, params object[] args)
    {
        Logger.LogError(format, args);
    }

    public void Error(Exception cause, string format, params object[] args)
    {
        Logger.LogError(cause, format, args);
    }

    public void Log(LogLevel logLevel, string format, params object[] args)
    {
        Logger.Log(ToLogLevel(logLevel), format, args);
    }

    public void Log(LogLevel logLevel, Exception cause, string format, params object[] args)
    {
        Logger.Log(ToLogLevel(logLevel), cause, format, args);
    }

    public bool IsDebugEnabled => Logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug);
    public bool IsInfoEnabled => Logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information);
    public bool IsWarningEnabled => Logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Warning);
    public bool IsErrorEnabled => Logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Error);
}