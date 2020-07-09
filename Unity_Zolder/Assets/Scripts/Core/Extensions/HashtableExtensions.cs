// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections;
using System.Text.RegularExpressions;
using Talespin.Core.Foundation.Logging;
using UnityEngine;

namespace Talespin.Core.Foundation.Extensions
{
	public static class HashtableExtensions
	{
		public static int GetInt(this Hashtable hashtable, string id, int defaultVal)
		{
			object val = hashtable.Get(id, defaultVal);
			if (val is float)
			{
				return (int)(float)val;
			}
			return (int)val;
		}

		public static float GetFloat(this Hashtable hashtable, string id, float defaultVal)
		{
			return (float)hashtable.Get(id, defaultVal);
		}

		public static bool GetBool(this Hashtable hashtable, string id, bool defaultVal)
		{
			object val = hashtable.Get(id, defaultVal);
			if (val is string)
			{
				return bool.Parse((string)val);
			}

			return (bool)val;
		}

		public static Hashtable GetTable(this Hashtable hashtable, string id)
		{
			return (Hashtable)hashtable.Get(id, new Hashtable());
		}

		public static Hashtable GetTable(this Hashtable hashtable, string id, Hashtable defaultVal)
		{
			return (Hashtable)hashtable.Get(id, defaultVal);
		}

		public static ArrayList GetArray(this Hashtable hashtable, string id)
		{
			return (ArrayList)hashtable.Get(id, new ArrayList());
		}

		public static ArrayList GetArray(this Hashtable hashtable, string id, ArrayList defaultVal)
		{
			return (ArrayList)hashtable.Get(id, defaultVal);
		}

		public static object Get(this Hashtable hashtable, string id, object defaultVal)
		{
			if (hashtable == null || !hashtable.ContainsKey(id))
			{
				LogUtil.Log(LogTags.SYSTEM, "HashtableExtensions", string.Format("No entry found with key: '{0}', using default: '{1}'", id, defaultVal.ToString()));
				return defaultVal;
			}
			return hashtable[id];
		}

		/// <summary>
		/// Gets a value from a table by finding the closest integer match. 
		/// This can for example be used to get the value that closest matches the current difficulty:
		/// 	"runnerSpeedMapping":
		/// 	{
		///			"difficulty_0": 15,
		///			"difficulty_1": 17,
		///			"difficulty_5": 25,
		/// 	}
		/// So if you ask for the speed at difficulty '2' it will return '17'
		/// </summary>
		public static object GetMapped(this Hashtable table, string keyPrefix, int keyVal, object defaultVal)
		{
			return table.GetMapped(new Regex(string.Format("^{0}_([0-9]+)$", keyPrefix), RegexOptions.IgnoreCase), keyVal, defaultVal);
		}

		public static object GetMapped(this Hashtable table, Regex namePattern, int keyVal, object defaultVal)
		{
			int bestDifference = int.MaxValue;
			object bestValue = defaultVal;
			foreach (DictionaryEntry entry in table)
			{
				Match match = namePattern.Match(entry.Key.ToString());
				if (match != null && match.Success)
				{
					int entryVal = int.Parse(match.Groups[1].Value);
					int difference = Mathf.Abs(keyVal - entryVal);
					if (difference < bestDifference)
					{
						bestDifference = difference;
						bestValue = entry.Value;
					}
				}
			}
			return bestValue;
		}
	}
}
