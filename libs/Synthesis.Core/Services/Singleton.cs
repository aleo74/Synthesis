namespace Synthesis.Core.Services;

/// <summary>
/// Represents a generic singleton pattern implementation.
/// </summary>
/// <typeparam name="T">The type of the singleton instance.</typeparam>
public abstract class Singleton<T>
    where T : class, new()
{
    private static readonly Lazy<T> LazyInstance = new(() => new T(), LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// Gets the single instance of the <typeparamref name="T"/> type.
    /// </summary>
    public static T Instance =>
        LazyInstance.Value;
}