﻿using System.Diagnostics;

namespace TrafficCourts.Diagnostics;

/// <summary>
/// Timer operation. When timer operation is disposed, the elapsed time will be recorded.
/// </summary>
public interface ITimerOperation : IDisposable
{
    /// <summary>
    /// Indicate the operation resulted in an error adding the appropriate tags to the operation.
    /// </summary>
    /// <param name="exception"></param>
    void Error(Exception exception);

    /// <summary>
    /// Adds a tag to the operation.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    void AddTag(string key, object? value);

    /// <summary>
    /// Gets a copy of the operation's tags
    /// </summary>
    TagList Tags { get; }
}
