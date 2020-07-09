// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Talespin.Core.Foundation.Maths;

namespace Talespin.Core.Foundation.Extensions
{
	public static class SortingExtensions
	{
		public delegate int Compare<T>(T A, T B);

		public static IEnumerable<T> Sort<T>(this IEnumerable<T> that, Sorting.Compare<T> compare)
		{
			return new SortedIEnumerable<T>(that, compare);
		}

		private class SortedIEnumerable<T> : IEnumerable<T>
		{
			private IEnumerable<T> source;
			private readonly Sorting.Compare<T> compare;

			public SortedIEnumerable(IEnumerable<T> source, Sorting.Compare<T> compare)
			{
				this.source = source;
				this.compare = compare;
			}

			public IEnumerator<T> GetEnumerator()
			{
				T[] data = source.ToArray();
				data.QuickSort(compare);
				return ((IEnumerable<T>)data).GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		#region String
		public static IEnumerable<string> AlphabetSort(this IEnumerable<string> that)
		{
			return that.AlphabetSort(s => s);
		}

		public static IEnumerable<T> AlphabetSort<T>(this IEnumerable<T> that, Func<T, string> valueSelector)
		{
			return that.Sort((a, b) => valueSelector(a).CompareTo(valueSelector(b)));
		}
		#endregion
	}
}
