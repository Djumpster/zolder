// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Talespin.Core.Foundation.Extensions
{
	public static class DictionaryExtensions
	{
		public static void Ensure<TKey, TValue>(this Dictionary<TKey, TValue> orig, TKey key) where TValue : ICollection, new()
		{
			if (!orig.ContainsKey(key))
			{
				orig.Add(key, new TValue());
			}
		}

		public static void Ensure<TKey, TValue>(this Dictionary<TKey, TValue> orig, TKey key, TValue defaultValue)
		{
			if (!orig.ContainsKey(key))
			{
				orig.Add(key, defaultValue);
			}
		}

		public static TValue GetOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> orig, TKey key, TValue defaultValue)
		{
			TValue result;
			if (!orig.TryGetValue(key, out result))
			{
				result = defaultValue;
				orig.Add(key, result);
			}
			return result;
		}

		public static KeyValuePair<TKey, TValue>[] ToArray<TKey, TValue>(this Dictionary<TKey, TValue> orig)
		{
			KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[orig.Count];
			int counter = 0;
			foreach (KeyValuePair<TKey, TValue> kvp in orig)
			{
				array[counter] = kvp;
				counter++;
			}

			return array;
		}

		public static bool SetOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> orig, TKey key, TValue @value)
		{
			if (!orig.ContainsKey(key))
			{
				orig.Add(key, @value);
				return true;
			}
			else
			{
				orig[key] = @value;
				return false;
			}
		}

		public static T GetValueIgnoreCase<T>(this Dictionary<string, T> orig, string key)
		{
			return GetValueIgnoreCase(orig, key, default(T));
		}

		public static T GetValueIgnoreCase<T>(this Dictionary<string, T> orig, string key, T defaultValue)
		{
			foreach (KeyValuePair<string, T> entry in orig)
			{
				if (entry.Key.Equals(key, System.StringComparison.InvariantCultureIgnoreCase))
				{
					return entry.Value;
				}
			}

			return defaultValue;
		}

		/// <summary>
		/// Creates a string with each kvp on a new line as "key : value ".
		/// </summary>
		public static string ToStringKVP<TKey, TValue>(this Dictionary<TKey, TValue> orig)
		{
			if (orig == null)
			{
				return "null";
			}

			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<TKey, TValue> kvp in orig)
			{
				stringBuilder.AppendLine(string.Format("{0} : {1}", kvp.Key, kvp.Value));
			}

			return stringBuilder.ToString();
		}
	}
}
