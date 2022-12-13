using System.Diagnostics;

namespace TrafficCourts.Common.Diagnostics
{
    /// <summary>
    /// Non allocating stopwatch. Code from .NET 7 Stopwatch class.
    /// </summary>
    internal static class ValueStopwatch
    {
        private const long TicksPerMillisecond = 10000;
        private const long TicksPerSecond = TicksPerMillisecond * 1000;
        private static readonly double s_tickFrequency = (double)TicksPerSecond / Stopwatch.Frequency;

        public static long GetTimestamp() => Stopwatch.GetTimestamp();

        public static TimeSpan GetElapsedTime(long startingTimestamp) => GetElapsedTime(startingTimestamp, GetTimestamp());

        // https://github.com/dotnet/dotnet/blob/dbf3542eaa7b3ee59359791a7a7fdb177e8641f7/src/runtime/src/libraries/System.Private.CoreLib/src/System/Diagnostics/Stopwatch.cs#L128
        public static TimeSpan GetElapsedTime(long startingTimestamp, long endingTimestamp) =>
            new TimeSpan((long)((endingTimestamp - startingTimestamp) * s_tickFrequency));
    }
}
