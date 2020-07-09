// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Talespin.Core.Foundation.Extensions
{
	/// <summary>
	/// Collection of utility functions missing in Linq.
	/// NOTE: uses Linq internally for some functions such as Unique.
	/// </summary>
	public static class EnumerableExtensions
	{
		// TODO: create breakable versions
		/// <summary>
		/// Declarative version of foreach that takes an iteratee (including key support) for processing each element.
		/// </summary>
		/// 
		/// <typeparam name="T">Type of the source.</typeparam>
		/// 
		/// <param name="source">The source to iterate.</param>
		/// <param name="iteratee">The iteratee to apply to each element.</param>

		public static void Each<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, Action<TValue, TKey> iteratee)
		{
			foreach (KeyValuePair<TKey, TValue> item in source)
			{
				iteratee(item.Value, item.Key);
			}
		}

		/// <summary>
		/// Declarative version of foreach that takes an iteratee (including count support) for processing each element.
		/// </summary>
		/// 
		/// <typeparam name="T">Type of the source.</typeparam>
		/// 
		/// <param name="source">The source to iterate.</param>
		/// <param name="iteratee">The iteratee to apply to each element.</param>

		public static void Each<T>(this IEnumerable<T> source, Action<T, int> iteratee)
		{
			using (var enumerator = source.GetEnumerator())
			{
				for (int count = 0; enumerator.MoveNext(); count++)
				{
					iteratee(enumerator.Current, count);
				}
			}
		}

		/// <summary>
		/// Declarative version of foreach that takes an iteratee for processing each element.
		/// </summary>
		/// 
		/// <typeparam name="T">Type of the source.</typeparam>
		/// 
		/// <param name="source">The source to iterate.</param>
		/// <param name="iteratee">The iteratee to apply to each element.</param>

		public static void Each<T>(this IEnumerable<T> source, Action<T> iteratee)
		{
			foreach (T item in source)
			{
				iteratee(item);
			}
		}

		/// <summary>
		/// Returns true if an enumerable is empty.
		/// </summary>
		/// 
		/// <typeparam name="T">Type of the source</typeparam>
		/// 
		/// <param name="source">Source to check for emptiness</param>
		/// 
		/// <returns>
		/// Boolean indicating if the enumerable is empty.
		/// </returns>

		public static bool IsEmpty<T>(this IEnumerable<T> source)
		{
			return !source.Any();
		}

		/// <summary>
		/// Returns true if an enumerable is null or empty.
		/// </summary>
		/// 
		/// <typeparam name="T">Type of the source</typeparam>
		/// 
		/// <param name="source">Source to check for null or emptiness</param>
		/// 
		/// <returns>
		/// Boolean indicating if the enumerable is null/empty.
		/// </returns>

		public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
		{
			return source == null || !source.Any();
		}

		/// <summary>
		/// This method is like Max except it will return the actual element instead of the value returned by the iteratee.
		/// </summary>
		/// 
		/// <typeparam name="T">The type of the source.</typeparam>
		/// 
		/// <param name="source">The source to get the max element from.</param>
		/// <param name="iteratee">Iteratee that generates the criterion (float) for ranking.</param>
		/// 
		/// <returns>
		/// The maximum element according to the criterion given by the iteratee.
		/// </returns>

		public static T MaxBy<T>(this IEnumerable<T> source, Func<T, float> iteratee)
		{
			return source.Aggregate((max, elm) => (iteratee(elm) > iteratee(max)) ? elm : max);
		}

		/// <summary>
		/// Returns a new enumerable containing only unique values.
		/// </summary>
		/// 
		/// <typeparam name="T">Type of the source</typeparam>
		/// 
		/// <param name="source">The source to make unique</param>
		/// 
		/// <returns>
		/// New enumerable containing only unique values
		/// </returns>

		public static IEnumerable<T> Unique<T>(this IEnumerable<T> source)
		{
			return source.Union(new T[] { });
		}

		/// <summary>
		/// Returns a new enumerable containing only unique values specified by a lambda expression.
		/// </summary>
		///
		/// <typeparam name="T">Type of the source.</typeparam>
		/// 
		/// <param name="source">The source to make unique.</param>
		/// <param name="propertySelector">A function defining the property on which uniqueness should be based.</param>
		/// 
		/// <returns>
		/// New enumerable containing only unique values as defined by iteratee.
		/// </returns>

		public static IEnumerable<TProp> Unique<T, TProp>(this IEnumerable<T> source, Func<T, TProp> propertySelector)
		{
			return source.Select(propertySelector).Unique();
		}

		/// <summary>
		/// This function is like Unique but will maintains the first of the original unique elements.
		/// This effectively takes the first original element after grouping the elements based on propertySelector.
		/// </summary>
		///
		/// <typeparam name="T">Type of the source.</typeparam>
		/// 
		/// <param name="source">The source to make unique.</param>
		/// <param name="propertySelector">A function defining the property on which uniqueness should be based.</param>
		/// 
		/// <returns>
		/// New enumerable containing only unique values as defined by iteratee.
		/// </returns>

		public static IEnumerable<T> UniqueBy<T, TProp>(this IEnumerable<T> source, Func<T, TProp> propertySelector)
		{
			return source.GroupBy(propertySelector).Select((entityGroup) => entityGroup.First());
		}

		// ALIAS AddToEnd -> Append
		public static IEnumerable<TSource> AddToEnd<TSource>(this IEnumerable<TSource> source, TSource element) => source.Append(element);
		// ALIAS AddToBeginning -> Prepend
		public static IEnumerable<TSource> AddToBeginning<TSource>(this IEnumerable<TSource> source, TSource element) => source.Prepend(element);

		// ALIAS Find -> FirstOrDefault
		public static TSource Find<TSource>(this IEnumerable<TSource> source) => source.FirstOrDefault();
		public static TSource Find<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) => source.FirstOrDefault(predicate);

		// ALIAS Map -> Select
		public static IEnumerable<TResult> Map<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> selector) => source.Select(selector);
		public static IEnumerable<TResult> Map<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector) => source.Select(selector);

		// ALIAS Reduce -> Aggregate
		public static TSource Reduce<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource> func) => source.Aggregate(func);
		public static TAccumulate Reduce<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func) => source.Aggregate(seed, func);
		public static TResult Reduce<TSource, TAccumulate, TResult>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector) => source.Aggregate(seed, func, resultSelector);
	}
}