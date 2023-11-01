using Synthesis.Core.Services;

namespace Synthesis.Core.IO.Logging.Infrastructure;

public sealed class NullDisposable : Singleton<NullDisposable>, IDisposable
{
    public void Dispose()
    {
    }
}