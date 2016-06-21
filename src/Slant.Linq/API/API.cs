using Monad;

namespace Slant
{
    public class Match<T>
    {
        private T value;

        internal Match(T value)
        {
            this.value = value;
        }
    }

    public static class API
    {
        public static Match<T> Match<T>(T value)
        {
            return new Match<T>(value);
        }
    }

    public delegate Option<R> Case<T, R>();
}