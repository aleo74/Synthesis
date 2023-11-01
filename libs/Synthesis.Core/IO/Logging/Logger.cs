using System.Collections.Frozen;
using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;
using Synthesis.Core.Extensions;
using Synthesis.Core.IO.Logging.Infrastructure;

namespace Synthesis.Core.IO.Logging;

/// <summary>
/// A custom logger implementation that writes log messages to the console with formatting and coloring.
/// </summary>
public sealed class Logger(string categoryName) : ILogger
{
    private static readonly FrozenSet<string> ParameterColors = new[]
    {
        "\x1B[96m",
        "\x1B[95m",
        "\x1B[92m",
        "\x1B[93m",
        "\x1B[94m",
    }.ToFrozenSet();
    
    private readonly TextWriter _writer = Console.Out;
    private readonly TextWriter _errorWriter = Console.Error;

    /// <summary>
    /// Creates a scope for the logger.
    /// </summary>
    /// <typeparam name="TState">The type of state for the logger scope.</typeparam>
    /// <param name="state">The state information for the logger scope.</param>
    /// <returns>A disposable object that represents the logger scope.</returns>
    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        return NullDisposable.Instance;
    }

    /// <summary>
    /// Checks whether logging at the specified log level is enabled.
    /// </summary>
    /// <param name="logLevel">The log level to check.</param>
    /// <returns><c>true</c> if logging at the specified level is enabled; otherwise, <c>false</c>.</returns>
    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel is >= LogLevel.Debug and <= LogLevel.Critical;
    }

    /// <summary>
    /// Logs a message with the specified log level, event ID, state, exception, and formatter function.
    /// </summary>
    /// <typeparam name="TState">The type of state for the log message.</typeparam>
    /// <param name="logLevel">The log level of the message.</param>
    /// <param name="eventId">The event ID associated with the message.</param>
    /// <param name="state">The state information for the log message.</param>
    /// <param name="exception">The exception associated with the message, if any.</param>
    /// <param name="formatter">A function to format the log message.</param>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;
        
        if (state is not IReadOnlyCollection<KeyValuePair<string, object?>> pairs)
            return;
        
        var items = pairs.ToDictionary(x => x.Key, x => x.Value!, StringComparer.Ordinal);
        
        if (!items.Remove("{OriginalFormat}", out var originalFormat))
            return;
        
        var formatted = Format(DateTime.Now, logLevel, categoryName, originalFormat.ToString(), items);

        _ = logLevel is LogLevel.Error or LogLevel.Critical
            ? _errorWriter.WriteLineAsync(formatted)
            : _writer.WriteLineAsync(formatted);
    }
    
    private static string Format(DateTime timestamp, LogLevel level, string categoryName, ReadOnlySpan<char> template, IDictionary<string, object> items)
    {
        var builder = new StringBuilder();
        var colorIndex = 0;
        var index = 0;
        
        builder
            .Append(string.Concat("[", timestamp.ToString("hh:mm:ss", CultureInfo.InvariantCulture), "] "))
            .Append(string.Concat("[", level.ToString()[..3].ToUpper(CultureInfo.InvariantCulture), "] "))
            .Append(string.Concat("(", categoryName, ") "));

        while (index < template.Length)
        {
            var startIndex = template.IndexOf('{', index);

            if (startIndex is -1)
            {
                builder.Append(template[index..]);
                break;
            }

            var endIndex = template.IndexOf('}', startIndex);

            if (endIndex is -1)
            {
                builder.Append(template[index..]);
                break;
            }
            
            builder.Append(template.Slice(index, startIndex - index));

            var parameter = template.Slice(startIndex + 1, endIndex - startIndex - 1);
            
            var formatIndex = parameter.IndexOf(':');
            
            var value = GetValue(items, formatIndex is -1 ? parameter : parameter[..formatIndex]);
            
            if (value is IFormattable table && formatIndex is not -1)
                value = table.ToString(parameter[(formatIndex + 1)..].ToString(), CultureInfo.InvariantCulture);

            builder.Append(string.Concat(GetNextParameterColor(ref colorIndex), value, "\x1B[0m"));

            index = endIndex + 1;
        }
        
        return builder.ToString();
    }

    private static object GetValue(IDictionary<string, object> items, ReadOnlySpan<char> expectedKey)
    {
        foreach (var (key, value) in items)
        {
            if (expectedKey.SequenceEqual(key))
                return value;
        }
        
        throw new KeyNotFoundException("Key not found in dictionary.");
    }
    
    private static string GetNextParameterColor(ref int index)
    {
        var color = ParameterColors.ElementAt(index);
            
        index = (index + 1) % ParameterColors.Count;
            
        return color;
    }
}