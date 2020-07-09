// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Talespin.Core.Foundation.Extensions
{
	public static class ArrayExtensions
	{
		public static bool HasIndex<T>(this T[] orig, int index)
		{
			return (index >= 0) && (index < orig.Length);
		}

		/// <summary>
		/// Randomizes the array.
		/// Fisher-Yates Shuffle: Generic Method - http://www.dotnetperls.com/shuffle
		/// </summary>
		/// <param name="orig">Original.</param>
		public static void Randomize<T>(this T[] orig)
		{
			System.Random random = new System.Random();

			List<KeyValuePair<int, T>> list = new List<KeyValuePair<int, T>>();

			// Add all values from array
			// Add new random int each time
			foreach (T val in orig)
			{
				list.Add(new KeyValuePair<int, T>(random.Next(), val));
			}

			// Sort the list by the random number
			IOrderedEnumerable<KeyValuePair<int, T>> sorted = from item in list
															  orderby item.Key
															  select item;

			// Copy values to array
			int index = 0;
			foreach (KeyValuePair<int, T> pair in sorted)
			{
				orig[index] = pair.Value;
				index++;
			}
		}

		public static string AllToString<T>(this T[] orig, bool eachOnNewLine = true)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < orig.Length; i++)
			{
				T value = orig[i];
				if (eachOnNewLine)
				{
					stringBuilder.AppendLine(value.ToString());
				}
				else
				{
					stringBuilder.Append(value.ToString());
					if (i < orig.Length - 1)
					{
						stringBuilder.Append(", ");
					}
				}
			}
			return stringBuilder.ToString();
		}

		public static T Random<T>(this T[] orig)
		{
			if (orig.Length == 0)
			{
				throw new System.IndexOutOfRangeException("Array index is out of range: array is empty");
			}

			//int i = Mathf.FloorToInt(UnityEngine.Random.value * orig.Length);
			int i = UnityEngine.Random.Range(0, orig.Length);
			return orig[i];
		}

		public static T Random<T>(this List<T> orig)
		{
			if (orig.Count == 0)
			{
				throw new System.IndexOutOfRangeException("List index is out of range: List is empty");
			}

			int i = UnityEngine.Random.Range(0, orig.Count);
			return orig[i];
		}

		public static T Random<T>(this IEnumerable<T> orig)
		{
			int count = orig.Count();
			if (count == 0)
			{
				throw new System.IndexOutOfRangeException("IEnumerable index is out of range: IEnumerable is empty");
			}

			int i = UnityEngine.Random.Range(0, count);
			return orig.ElementAt(i);
		}

		public static T Random<T>(this T[] orig, System.Random random)
		{
			if (orig.Length == 0)
			{
				throw new System.IndexOutOfRangeException("Array index is out of range: array is empty");
			}

			//int i = Mathf.FloorToInt(random.NextFloat() * orig.Length);
			int i = UnityEngine.Random.Range(0, orig.Length);
			return orig[i];
		}

		public static void Shuffle<T>(this IList<T> list, System.Random rnd)
		{
			T temp;
			for (int i = 0; i < list.Count; i++)
			{
				int j = rnd.Next(i, list.Count);
				temp = list[i];
				list[i] = list[j];
				list[j] = temp;
			}
		}

		public static void Shuffle<T>(this T[] array, System.Random rnd)
		{
			T temp;
			for (int i = 0; i < array.Length; i++)
			{
				int j = rnd.Next(i, array.Length);
				temp = array[i];
				array[i] = array[j];
				array[j] = temp;
			}
		}

		public static bool Contains<T>(this T[] orig, T value)
		{
			return System.Array.IndexOf(orig, value) >= 0;
		}

		public static bool Contains(this IEnumerable<string> orig, string value)
		{
			foreach (string element in orig)
			{
				if (element == value)
				{
					return true;
				}
			}

			return false;
		}

		public static int IndexOf<T>(this T[] orig, T value)
		{
			return System.Array.IndexOf(orig, value);
		}

		public static T First<T>(this T[] orig)
		{
			return orig[0];
		}

		public static T Last<T>(this T[] orig)
		{
			int i = orig.Length - 1;
			return orig[i];
		}

		public static T[] Add<T>(this T[] orig, T newElement)
		{
			int length = orig.Length;
			Array.Resize(ref orig, length + 1);
			orig[length] = newElement;
			return orig;
		}
	}
}
