using System;

namespace Slant.Linq
{
    /// <summary>
    /// Option monade
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct Option<T> : IEquatable<Option<T>>
        where T : class
    {
        private readonly T _value;

        /// <summary>
        /// Wrapped value
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public T Value
        {
            get
            {
                if (HasNoValue)
                    throw new InvalidOperationException();

                return _value;
            }
        }

        /// <summary>
        /// Get value or default value
        /// </summary>
        public T ValueOrDefault => HasValue ? _value : default(T);

        /// <summary>
        /// Determine if has value
        /// </summary>
        public bool HasValue => _value != null;

        /// <summary>
        /// Determine if has no value
        /// </summary>
        public bool HasNoValue => !HasValue;

        private Option(T value)
        {
            _value = value;
        }

        public static implicit operator Option<T>(T value)
        {
            return new Option<T>(value);
        }

        public static bool operator ==(Option<T> option, T value)
        {
            if (option.HasNoValue)
                return false;

            return option.Value.Equals(value);
        }

        public static bool operator !=(Option<T> option, T value)
        {
            return !(option == value);
        }

        public static bool operator ==(Option<T> first, Option<T> second)
        {
            return first.Equals(second);
        }

        public static bool operator !=(Option<T> first, Option<T> second)
        {
            return !(first == second);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Option<T>))
                return false;

            var other = (Option<T>)obj;
            return Equals(other);
        }

        public bool Equals(Option<T> other)
        {
            if (HasNoValue && other.HasNoValue)
                return true;

            if (HasNoValue || other.HasNoValue)
                return false;

            return _value.Equals(other._value);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            if (HasNoValue)
                return "No value";

            return Value.ToString();
        }

        public T Unwrap()
        {
            if (HasValue)
                return Value;

            return default(T);
        }

        public K Unwrap<K>(Func<T, K> selector)
        {
            if (HasValue)
                return selector(Value);

            return default(K);
        }
    }

    public static class Option
    {
        public static Option<object> None => new Option<object>();
    }
}
