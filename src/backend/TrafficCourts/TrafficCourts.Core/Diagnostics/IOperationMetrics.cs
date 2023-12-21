using System.Runtime.CompilerServices;

namespace TrafficCourts.Diagnostics;

/// <summary>
/// Provides count and duration metrics on an operation.
/// </summary>
public interface IOperationMetrics
{
    /// <summary>
    /// Begins a named operation
    /// </summary>
    /// <param name="operation">The name of the operation. If not supplied, the caller's method name will be used.</param>
    /// <returns>
    /// A disposable <see cref="ITimerOperation"/> that when disposed records the operation completion.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="operation"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="operation"/> is empty.</exception>
    ITimerOperation BeginOperation([CallerMemberName] string operation = "");
}
