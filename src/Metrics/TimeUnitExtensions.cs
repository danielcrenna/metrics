using System;

namespace Metrics
{
    /// <summary>
    ///     Provides enum methods for timing values
    /// </summary>
    public static class TimeUnitExtensions
    {
        private static readonly long[] _timings = {1000L, 1000L * 1000L, 1000L * 1000L * 1000L};

        public static string Abbreviate(this TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.Nanoseconds:
                    return "ns";
                case TimeUnit.Microseconds:
                    return "us";
                case TimeUnit.Milliseconds:
                    return "ms";
                case TimeUnit.Seconds:
                    return "s";
                case TimeUnit.Minutes:
                    return "m";
                case TimeUnit.Hours:
                    return "h";
                case TimeUnit.Days:
                    return "d";
                default:
                    throw new ArgumentOutOfRangeException(nameof(unit));
            }
        }

        public static long ToNanos(this TimeUnit source, long interval)
        {
            return Convert(source, interval, TimeUnit.Nanoseconds);
        }

        public static long ToMicros(this TimeUnit source, long interval)
        {
            return Convert(source, interval, TimeUnit.Microseconds);
        }

        public static long ToMillis(this TimeUnit source, long interval)
        {
            return Convert(source, interval, TimeUnit.Milliseconds);
        }

        public static long ToSeconds(this TimeUnit source, long interval)
        {
            return Convert(source, interval, TimeUnit.Seconds);
        }

        public static long ToMinutes(this TimeUnit source, long interval)
        {
            return Convert(source, interval, TimeUnit.Minutes);
        }

        public static long ToHours(this TimeUnit source, long interval)
        {
            return Convert(source, interval, TimeUnit.Hours);
        }

        public static long ToDays(this TimeUnit source, long interval)
        {
            return Convert(source, interval, TimeUnit.Days);
        }

        public static long Convert(this TimeUnit source, long duration, TimeUnit target)
        {
            switch (source)
            {
                case TimeUnit.Nanoseconds:
                    switch (target)
                    {
                        case TimeUnit.Nanoseconds:
                            return duration;
                        case TimeUnit.Microseconds:
                            return duration / _timings[0];
                        case TimeUnit.Milliseconds:
                            return duration / _timings[1];
                        case TimeUnit.Seconds:
                            return NanosecondsToSeconds(duration);
                        case TimeUnit.Minutes:
                            return NanosecondsToSeconds(duration) / 60;
                        case TimeUnit.Hours:
                            return NanosecondsToSeconds(duration) / 60 / 60;
                        case TimeUnit.Days:
                            return NanosecondsToSeconds(duration) / 60 / 60 / 24;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(target));
                    }

                case TimeUnit.Microseconds:
                    switch (target)
                    {
                        case TimeUnit.Nanoseconds:
                            return duration * _timings[0];
                        case TimeUnit.Microseconds:
                            return duration;
                        case TimeUnit.Milliseconds:
                            return duration / _timings[0];
                        case TimeUnit.Seconds:
                            return MicrosecondsToSeconds(duration);
                        case TimeUnit.Minutes:
                            return MicrosecondsToSeconds(duration) / 60;
                        case TimeUnit.Hours:
                            return MicrosecondsToSeconds(duration) / 60 / 60;
                        case TimeUnit.Days:
                            return MicrosecondsToSeconds(duration) / 60 / 60 / 24;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(target));
                    }

                case TimeUnit.Milliseconds:
                    switch (target)
                    {
                        case TimeUnit.Nanoseconds:
                            return duration * _timings[1];
                        case TimeUnit.Microseconds:
                            return duration * _timings[0];
                        case TimeUnit.Milliseconds:
                            return duration;
                        case TimeUnit.Seconds:
                            return MillisecondsToSeconds(duration);
                        case TimeUnit.Minutes:
                            return MillisecondsToSeconds(duration) / 60;
                        case TimeUnit.Hours:
                            return MillisecondsToSeconds(duration) / 60 / 60;
                        case TimeUnit.Days:
                            return MillisecondsToSeconds(duration) / 60 / 60 / 24;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(target));
                    }

                case TimeUnit.Seconds:
                    switch (target)
                    {
                        case TimeUnit.Nanoseconds:
                            return duration * _timings[2];
                        case TimeUnit.Microseconds:
                            return duration * _timings[1];
                        case TimeUnit.Milliseconds:
                            return duration * _timings[0];
                        case TimeUnit.Seconds:
                            return duration;
                        case TimeUnit.Minutes:
                            return duration / 60;
                        case TimeUnit.Hours:
                            return duration / 60 / 60;
                        case TimeUnit.Days:
                            return duration / 60 / 60 / 24;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(target));
                    }

                case TimeUnit.Minutes:
                    switch (target)
                    {
                        case TimeUnit.Nanoseconds:
                            return duration * 60 * _timings[2];
                        case TimeUnit.Microseconds:
                            return duration * 60 * _timings[1];
                        case TimeUnit.Milliseconds:
                            return duration * 60 * _timings[0];
                        case TimeUnit.Seconds:
                            return duration * 60;
                        case TimeUnit.Minutes:
                            return duration;
                        case TimeUnit.Hours:
                            return duration / 60;
                        case TimeUnit.Days:
                            return duration / 60 / 24;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(target));
                    }

                case TimeUnit.Hours:
                    switch (target)
                    {
                        case TimeUnit.Nanoseconds:
                            return duration * 60 * 60 * _timings[2];
                        case TimeUnit.Microseconds:
                            return duration * 60 * 60 * _timings[1];
                        case TimeUnit.Milliseconds:
                            return duration * 60 * 60 * _timings[0];
                        case TimeUnit.Seconds:
                            return duration * 60 * 60;
                        case TimeUnit.Minutes:
                            return duration * 60;
                        case TimeUnit.Hours:
                            return duration;
                        case TimeUnit.Days:
                            return duration / 24;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(target));
                    }

                case TimeUnit.Days:
                    switch (target)
                    {
                        case TimeUnit.Nanoseconds:
                            return duration * 24 * 60 * 60 * _timings[2];
                        case TimeUnit.Microseconds:
                            return duration * 24 * 60 * 60 * _timings[1];
                        case TimeUnit.Milliseconds:
                            return duration * 24 * 60 * 60 * _timings[0];
                        case TimeUnit.Seconds:
                            return duration * 24 * 60 * 60;
                        case TimeUnit.Minutes:
                            return duration * 24 * 60;
                        case TimeUnit.Hours:
                            return duration * 24;
                        case TimeUnit.Days:
                            return duration;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(target));
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(source));
            }
        }

        private static long NanosecondsToSeconds(long duration)
        {
            return duration / _timings[2];
        }

        private static long MicrosecondsToSeconds(long duration)
        {
            return duration / _timings[1];
        }

        private static long MillisecondsToSeconds(long duration)
        {
            return duration / _timings[0];
        }
    }
}