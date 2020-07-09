// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace Talespin.Core.Foundation.Serialization
{
	/// <summary>
	/// Static helper methods relating to SerializedClass instances.
	/// </summary>
	public static class SerializedClassHelper
	{
		/// <summary>
		/// Finds out if a specific serialized class is overriden.
		/// </summary>
		/// <returns>If the current serialized class is overridden in the list, it 
		/// will return the override. Otherwise, it will return the current.</returns>
		/// <param name = "overrides">The list of overrides that may contain an override
		/// for the current serialized class.</param>
		/// <param name = "current">The current class that we want to override.</param>
		/// <typeparam name = "T">The type of the serialized class.</typeparam>
		public static T FindOverride<T>(IEnumerable<T> overrides, T current) where T : IRuntimeSerializedClass
		{
			if (overrides != null)
			{
				foreach (T targetOverride in overrides)
				{
					if (targetOverride.Identifier == current.Identifier)
					{
						return targetOverride;
					}
				}
			}
			return current;
		}

		/// <summary>
		/// Prettify the name of a type. If the type contains a <see cref="TypeNameAttribute"/> it will
		/// use the name specified. If no such attribute is present the following will happen:
		/// <para>
		/// 1. "Action" and "Rule" suffixes will be removed from the type name, essentially converting
		/// <c>ScopedDependencyAction</c> into <c>ScopedDependency</c>.
		/// </para>
		/// <para>
		/// 2. Spaces will be added when an uppercase character is encountered, thus
		/// <c>ScopedDependency</c> becomes <c>Scoped Dependency</c>.
		/// </para>
		/// </summary>
		/// <param name="type">The type to prettify the name of</param>
		/// <param name="preserveAcronyms">If set to <see langword="true" />, the behaviour of step 2
		/// will slightly change. If multiple uppercase characters are found in succession, it will
		/// not seperate those into seperate words beyond the first occurence. For instance <c>SetURL</c>
		/// will become <c>Set URL</c> instead of <c>Set U R L</c>.</param>
		/// <returns>A user-friendly string with the name of the type.</returns>
		/// <seealso cref="TypeNameAttribute"/>
		public static string PrettifyTypeName(Type type, bool preserveAcronyms)
		{
			if (type.IsDefined(typeof(TypeNameAttribute), false))
			{
				TypeNameAttribute attribute = type.GetCustomAttributes(typeof(TypeNameAttribute), false)[0] as TypeNameAttribute;
				return attribute.Name;
			}

			return PrettifyString(type.Name, preserveAcronyms);
		}

		/// <summary>
		/// Prettify a name. This essentially runs the following processes on the input string:
		/// <para>
		/// 1. "Action" and "Rule" suffixes will be removed from the name, essentially converting
		/// <c>ScopedDependencyAction</c> into <c>ScopedDependency</c>.
		/// </para>
		/// <para>
		/// 2. Spaces will be added when an uppercase character is encountered, thus
		/// <c>ScopedDependency</c> becomes <c>Scoped Dependency</c>.
		/// </para>
		/// </summary>
		/// <param name="text">The name to prettify</param>
		/// <param name="preserveAcronyms">If set to <see langword="true" />, the behaviour of step 2
		/// will slightly change. If multiple uppercase characters are found in succession, it will
		/// not seperate those into seperate words beyond the first occurence. For instance <c>SetURL</c>
		/// will become <c>Set URL</c> instead of <c>Set U R L</c>.</param>
		/// <returns>A user-friendly representation of the input string.</returns>
		/// <seealso cref="TypeNameAttribute"/>
		public static string PrettifyString(string text, bool preserveAcronyms)
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				return string.Empty;
			}

			if (text.EndsWith("Action"))
			{
				text = text.Substring(0, text.Length - 6);
			}
			else if (text.EndsWith("Rule"))
			{
				text = text.Substring(0, text.Length - 4);
			}

			StringBuilder newText = new StringBuilder(text.Length * 2);
			newText.Append(text[0].ToString().ToUpper());

			for (int i = 1; i < text.Length; i++)
			{
				if (char.IsUpper(text[i]))
				{
					if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) || (preserveAcronyms && char.IsUpper(text[i - 1]) && i < text.Length - 1 && !char.IsUpper(text[i + 1])))
					{
						newText.Append(' ');
					}
				}

				newText.Append(text[i]);
			}

			return newText.ToString();
		}
	}
}
