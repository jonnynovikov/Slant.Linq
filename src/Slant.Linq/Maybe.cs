using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Slant.Linq
{
    /// <summary>
    /// Maybe monade
    /// 
    /// Nothing represented by an empty array
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Maybe<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> values;

        public Maybe()
        {
            this.values = new T[0];
        }

        public Maybe(T value)
        {
            this.values = new[] { value };
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public T Unwrap()
        {
            foreach (var value in values)
            {
                return value;
            }
            return default(T);
        }

        public K Unwrap<K>(Func<T, K> selector)
        {
            foreach (var value in values)
            {
                return selector(value);
            }
            return default(K);
        }
    }

    public static class Maybe
    {
        public static Maybe<T> AsMaybe<T>(this T value)
        {
            return new Maybe<T>(value);
        }

        public static Maybe<T> Empty<T>()
        {
            return new Maybe<T>();
        }
    }
}