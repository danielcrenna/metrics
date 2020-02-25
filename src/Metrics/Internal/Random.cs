using System;
using System.Security.Cryptography;

namespace Metrics.Internal
{
    /// <summary>
    ///     Provides statistically relevant random number generation
    /// </summary>
    internal class Random
    {
        private static readonly RandomNumberGenerator Inner;

        static Random()
        {
            Inner = RandomNumberGenerator.Create();
        }

        public static long NextLong()
        {
            var buffer = new byte[sizeof(long)];
            Inner.GetBytes(buffer);
            var value = BitConverter.ToInt64(buffer, 0);
            return value;
        }
    }
}