using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Slant.Linq
{
	/// <summary>
	/// Another good idea by Tomas Petricek.
	/// See http://tomasp.net/blog/dynamic-linq-queries.aspx for information on how it's used.
	/// </summary>
	public static class Linq
	{
        /// <summary>
        /// Returns the given anonymous method as a lambda expression
        /// </summary>
        public static Expression<Func<TResult>> Expr<TResult>(Expression<Func<TResult>> expr)
        {
            return expr;
        }


	    /// <summary>
	    /// Simplify predicate assignment
	    /// </summary>
	    /// <param name="src"></param>
	    /// <param name="expr"></param>
	    /// <typeparam name="T"></typeparam>
	    /// <returns></returns>
	    public static Expression<Func<T, bool>> Predicate<T>(this LinqBuilder<T> src, Expression<Func<T, bool>> expr)
        {
            return expr;
        }

        /// <summary>
        /// Simplify getter assignment
        /// </summary>
        /// <param name="src"></param>
        /// <param name="expr"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, TResult>> Getter<T, TResult>(this LinqBuilder<T> src, Expression<Func<T, TResult>> expr)
        {
            return expr;
        }

        /// <summary>
        /// Returns the given anonymous method as a lambda expression
        /// </summary>
        public static Expression<Func<T, TResult>> Expr<T, TResult>(Expression<Func<T, TResult>> expr)
        {
            return expr;
        }

        /// <summary>
        /// Returns the given anonymous method as a lambda expression
        /// </summary>
        public static Expression<Func<T1, T2, TResult>> Expr<T1, T2, TResult>(Expression<Func<T1, T2, TResult>> expr)
        {
            return expr;
        }

        /// <summary>
        /// Returns the given anonymous function as a Func delegate
        /// </summary>
        public static Func<TResult> Func<TResult>(Func<TResult> expr)
        {
            return expr;
        }

        /// <summary>
        /// Returns the given anonymous function as a Func delegate
        /// </summary>
		public static Func<T, TResult> Func<T, TResult> (Func<T, TResult> expr)
		{
			return expr;
		}

        /// <summary>
        /// Returns the given anonymous function as a Func delegate
        /// </summary>
        public static Func<T1, T2, TResult> Func<T1, T2, TResult>(Func<T1, T2, TResult> expr)
        {
            return expr;
        }
	}

    /// <summary>
    /// F#-style let declarations
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Let<T>
    {
        /// <summary>
        /// Predicate
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Predicate(Expression<Func<T, bool>> expr)
        {
            return expr;
        }

        /// <summary>
        /// Getter
        /// </summary>
        /// <param name="expr"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, TResult>> Getter<TResult>(Expression<Func<T, TResult>> expr)
        {
            return expr;
        }

        /// <summary>
        /// Expression
        /// </summary>
        /// <param name="expr"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, TResult>> Expr<TResult>(Expression<Func<T, TResult>> expr)
        {
            return expr;
        }
    }

    /// <summary>
    /// Helper class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LinqBuilder<T>
    {
        internal static LinqBuilder<T> Instance;

        /// <summary>
        /// Get LinqBuilder for a given type
        /// </summary>
        /// <returns></returns>
        public static LinqBuilder<T> Get()
        {
            return Instance ?? (Instance = new LinqBuilder<T>());
        }
    }

    /// <summary>
    /// Useful extensions
    /// </summary>
    public static class LinqBuilderExtensions
    {
        /// <summary>
        /// Get LinqBuilder for a given type
        /// </summary>
        /// <param name="o"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static LinqBuilder<T> Linq<T>(this T o)
        {
            return LinqBuilder<T>.Get();
        }

        /// <summary>
        /// Return LinqBuilder for input argument of an expression
        /// </summary>
        /// <param name="o"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static LinqBuilder<T> Subject<T, TResult>(this Expression<Func<T, TResult>> o)
        {
            return LinqBuilder<T>.Get();
        }
    }
}
