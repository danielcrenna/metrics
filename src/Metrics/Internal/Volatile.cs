﻿using System.Threading;

namespace Metrics.Internal
{
    /// <summary>
    ///     Provides support for volatile operations around a typed value
    /// </summary>
    internal struct Volatile<T>
    {
        private object _value;

        private Volatile(T value) : this()
        {
            Set(value);
        }

        public void Set(T value)
        {
            Thread.VolatileWrite(ref _value, value);
        }

        public T Get()
        {
            return (T) Thread.VolatileRead(ref _value);
        }

        public static implicit operator Volatile<T>(T value)
        {
            return new Volatile<T>(value);
        }

        public static implicit operator T(Volatile<T> value)
        {
            return value.Get();
        }

        public override string ToString()
        {
            return Get().ToString();
        }
    }
}