using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Synthesis.Core.IO.Logging;

/// <summary>
/// A logger provider that creates and caches logger instances based on category names.
/// </summary>
public sealed class LoggerProvider : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, ILogger> _cache = new(StringComparer.Ordinal);

    /// <summary>
    /// Creates a logger instance for the specified category name, or retrieves an existing one from the cache.
    /// </summary>
    /// <param name="categoryName">The category name for which a logger is requested.</param>
    /// <returns>An ILogger instance for the specified category name.</returns>
    public ILogger CreateLogger(string categoryName)
    {
        if (!_cache.ContainsKey(categoryName))
            _cache.TryAdd(categoryName, new Logger(categoryName));

        return _cache[categoryName];
    }
    
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose() { }
}