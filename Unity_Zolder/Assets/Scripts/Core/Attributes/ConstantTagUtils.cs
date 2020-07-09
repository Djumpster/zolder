// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using Talespin.Core.Foundation.Reflection;
#if UNITY_EDITOR
#endif

namespace Talespin.Core.Foundation.Attributes
{
	/// <summary>
	/// <para>
	/// Common utilities for finding constant tags.
	/// </para>
	/// </summary>
	public static class ConstantTagUtils
	{
		/// <summary>
		/// Find all tags for the specified attribute.
		/// </summary>
		/// <typeparam name="T">The type to cast the tags to</typeparam>
		/// <param name="attribute">The attribute</param>
		/// <param name="availableTagStrings">A list that will contain string representations of the tag values</param>
		/// <returns>A list containing all available tags</returns>
		public static List<T> GetAvailableTags<T>(ConstantTagAttribute attribute, List<string> availableTagStrings)
		{
			return GetAvailableTags<T>(attribute.ConstType, attribute.Types, availableTagStrings);
		}

		/// <summary>
		/// Find all tags for the type and its derived types.
		/// </summary>
		/// <typeparam name="T">The type to cast the tags to</typeparam>
		/// <param name="constType">The constant type</param>
		/// <param name="types">The types to look search in</param>
		/// <param name="availableTagStrings">A list that will contain string representations of the tag values</param>
		/// <returns>A list containing all available tags</returns>
		public static List<T> GetAvailableTags<T>(Type constType, Type[] types, List<string> availableTagStrings)
		{
			List<T> availableTags = new List<T>();

			foreach (Type type in types)
			{
				availableTags.AddRange(GetAvailableTagsExcludingSubTypes<T>(constType, type, availableTagStrings));

				foreach (Type subType in Reflect.AllTypesFrom(type))
				{
					availableTags.AddRange(GetAvailableTagsExcludingSubTypes<T>(constType, subType, availableTagStrings));
				}
			}

			return availableTags;
		}

		/// <summary>
		/// Find all tags for the type and its derived types.
		/// </summary>
		/// <typeparam name="T">The type to cast the tags to</typeparam>
		/// <param name="constType">The constant type</param>
		/// <param name="type">The type to look search in</param>
		/// <param name="availableTagStrings">A list that will contain string representations of the tag values</param>
		/// <returns>A list containing all available tags</returns>
		public static List<T> GetAvailableTags<T>(Type constType, Type type, List<string> availableTagStrings)
		{
			List<T> availableTags = new List<T>();

			availableTags.AddRange(GetAvailableTagsExcludingSubTypes<T>(constType, type, availableTagStrings));

			foreach (Type subType in Reflect.AllTypesFrom(type))
			{
				availableTags.AddRange(GetAvailableTagsExcludingSubTypes<T>(constType, subType, availableTagStrings));
			}

			return availableTags;
		}

		private static List<T> GetAvailableTagsExcludingSubTypes<T>(Type constType, Type type, List<string> availableTagStrings)
		{
			List<T> availableTags = new List<T>();
			IEnumerable<FieldInfo> fieldInfos = Reflect.GetFieldInfos(type);

			foreach (FieldInfo fieldInfo in fieldInfos)
			{
				if (fieldInfo.FieldType == constType && fieldInfo.IsLiteral)
				{
					T tag = (T)fieldInfo.GetRawConstantValue();
					if (!availableTags.Contains(tag))
					{
						availableTags.Add(tag);
						availableTagStrings.Add(tag.ToString());
					}
				}
			}

			return availableTags;
		}
	}
}