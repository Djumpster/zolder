// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Talespin.Core.Foundation.AssetHandling;
using Talespin.Core.Foundation.Logging;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Talespin.Core.Foundation.Extensions
{
	public static partial class StringExtensions
	{
		public static TEnum ToEnum<TEnum>(this string strEnumValue, TEnum defaultValue)
		{
			if (!Enum.IsDefined(typeof(TEnum), strEnumValue))
			{
				return defaultValue;
			}

			return (TEnum)Enum.Parse(typeof(TEnum), strEnumValue);
		}

		public static string MakeValidIdentifier(this string s)
		{
			string pattern = @"[^a-z0-9_]";
			string pattern2 = @"^[0-9]+";
			Regex rex = new Regex(pattern, RegexOptions.IgnoreCase);
			Regex rex2 = new Regex(pattern2, RegexOptions.IgnoreCase);
			s = s.Replace(' ', '_');
			s = rex2.Replace(s, "");
			return rex.Replace(s, "");
		}

		private static Dictionary<string, string> lowerCaseStrings = new Dictionary<string, string>();
		private static Dictionary<string, string> upperCaseStrings = new Dictionary<string, string>();

		public static string ToCachedLower(this string orig)
		{
			string ret;
			if (orig == null)
			{
				LogUtil.Error(LogTags.SYSTEM, "StringExtensions", "Null parameter to lower!");
				return "";
			}
			if (!lowerCaseStrings.TryGetValue(orig, out ret))
			{
				ret = orig.ToLower();
				lowerCaseStrings.Add(orig, ret);
			}
			return ret;
		}

		public static string ToCachedUpper(this string orig)
		{
			string ret;
			if (orig == null)
			{
				LogUtil.Error(LogTags.SYSTEM, "StringExtensions", "Null parameter to upper!");
				return "";
			}
			if (!upperCaseStrings.TryGetValue(orig, out ret))
			{
				ret = orig.ToUpper();
				upperCaseStrings.Add(orig, ret);
			}
			return ret;
		}

		public static string ClampIfOver(this string orig, int maxChars)
		{
			if (orig == null)
			{
				return orig;
			}

			if (orig.Length > maxChars)
			{
				return orig.Remove(maxChars);
			}
			return orig;
		}

		[Obsolete("Use LoadGuid<T>(string) instead")]
		public static T LoadGUID<T>(this string orig) where T : Object
		{
			return LoadGuid<T>(orig);
		}

		public static T LoadGuid<T>(this string orig) where T : Object
		{
			if (string.IsNullOrEmpty(orig))
			{
				return null;
			}

			T asset = LoadGuidIfAvailable<T>(orig);

			if (asset != null)
			{
				return asset;
			}

			throw new InvalidOperationException("Trying to load \"" + orig + "\" but it is not a resource asset!");
		}

		public static ResourceRequest LoadGuidAsync(this string orig)
		{
			return GuidDatabaseManager.Instance.MapGuidToObjectAsync(orig);
		}

		[Obsolete("Use LoadGuidIfAvailable<T>(string) instead")]
		public static T LoadGUIDIfAvailable<T>(this string orig) where T : Object
		{
			return LoadGuidIfAvailable<T>(orig);
		}

		public static T LoadGuidIfAvailable<T>(this string orig) where T : Object
		{
			if (string.IsNullOrEmpty(orig))
			{
				return null;
			}

			try
			{
				return GuidDatabaseManager.Instance.MapGuidToObject<T>(orig);
			}
			catch { }

			return null;
		}

#if UNITY_EDITOR
		public static T LoadOriginalAssetByGuidIfAvailable<T>(this string orig) where T : Object
		{
			if (string.IsNullOrEmpty(orig))
			{
				return null;
			}

			try
			{
				string path = AssetDatabase.GUIDToAssetPath(orig);
				return AssetDatabase.LoadAssetAtPath<T>(path);
			}
			catch { }

			return null;
		}
#endif

		public static T Load<T>(this string orig) where T : Object
		{
			T asset = LoadIfAvailable<T>(orig);

			if (asset != null)
			{
				return asset;
			}

			throw new InvalidOperationException("Trying to load \"" + orig + "\" but it is not a resource asset!");
		}

		public static T LoadIfAvailable<T>(this string orig) where T : Object
		{
			return Resources.Load<T>(orig);
		}

		public static Type LoadType(this string orig)
		{
			GuidDatabaseManager guidDatabaseManager = GuidDatabaseManager.Instance;
			return guidDatabaseManager.MapGuidToType(orig);
		}

		public static Type[] LoadTypes(this string orig)
		{
			GuidDatabaseManager guidDatabaseManager = GuidDatabaseManager.Instance;
			return guidDatabaseManager.MapGuidToTypes(orig);
		}

		public static bool IsOfType(this string orig, Type type)
		{
			return orig.LoadType() == type;
		}

		public static Color HexToColor(this string hex)
		{
			hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
			hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
			byte a = 255;//assume fully visible unless specified in hex
			byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
			byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
			//Only use alpha if the string has enough characters
			if (hex.Length == 8)
			{
				a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
			}
			return new Color32(r, g, b, a);
		}

		/// <summary>
		/// This method postfixes a number to a string, to make it unique, similar to how Unity adds a postfix to
		/// keep cloned GameObjects unique. So upon cloning, it turns 'GameObject' into 'GameObject (1)', and so forth.
		/// </summary>
		/// <param name="originalName">The string it's attempting to make unique</param>
		/// <param name="blacklistedStrings">A list of string which are already taken.</param>
		/// <returns></returns>
		public static string MakeUnique(this string originalName, params string[] blacklistedStrings)
		{
			const int NUM_RENAME_ATTEMPTS = 1000;

			if (!blacklistedStrings.Contains(originalName))
			{
				return originalName;
			}
			else
			{
				// \([^\d]*(\d+)[^\d]*\)
				// Find the last any number between parentheses at the end of a string ($)
				// Leading space is optional (\s?)
				var regex = new Regex(@"\s?\((\d+)\)$");
				Match match = regex.Match(originalName);
				int initialNumber = 1;

				if (match.Success)
				{
					string number = Regex.Match(match.Value, "[0-9]+")?.Value;
					if (!int.TryParse(number, out initialNumber))
					{
						LogUtil.Error(LogTags.SYSTEM, "StringExtensions", "Could not parse '" + number + "' to int.");
					}

					// Strip optional space, parantheses and digit 
					originalName = originalName.Remove(match.Index, match.Length);
				}

				// Make rename attempts based on the generated data.
				for (int i = initialNumber; i < NUM_RENAME_ATTEMPTS; i++)
				{
					string newName = originalName + " (" + i + ")";

					if (!blacklistedStrings.Contains(newName))
					{
						return newName;
					}
				}
			}

			throw new Exception("Could not assign a unique string.");
		}
	}
}
