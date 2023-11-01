namespace Synthesis.Core.Extensions;

/// <summary>
/// Provides a way to fire and forget a Task, ignoring its completion status and any exceptions it might throw.
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// Ignores the completion status and exceptions of a Task, allowing it to run asynchronously without blocking the caller.
    /// </summary>
    /// <param name="task">The Task to be forgotten.</param>
    public static void Forget(this Task task)
    {
        if (task.IsCompleted)
            return;

        _ = ForgetAwaited(task);
        
        return;

        async Task ForgetAwaited(Task t)
        {
            try
            {
                await t.ConfigureAwait(false);
            }
            catch
            {
                // Ignored exception
            }
        }
    }
}