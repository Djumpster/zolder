// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Reflection;
using System.Text;

namespace Talespin.Core.Foundation.Extensions
{
	public static class TypeExtensions
	{
		public static FieldInfo GetFieldRecursive(this Type type, string name, BindingFlags flags)
		{
			FieldInfo field = type.GetField(name, flags);
			return field ?? GetFieldRecursive(type.BaseType, name, flags);
		}

		public static Type FindTypeInAssemblies(this Type type, string className)
		{
			// Search main assembly
			type = Type.GetType(className + ", Assembly-CSharp");
			if (type == null)
			{
				// Search plugin assembly
				type = Type.GetType(className + ", Assembly-CSharp-firstpass");
			}
			return type;
		}

		public static bool HasAttribute<T>(this Type type) where T : Attribute
		{
			return type.GetAttribute<T>() != null;
		}

		public static T GetAttribute<T>(this Type type) where T : Attribute
		{
			return (T)Attribute.GetCustomAttribute(type, typeof(T), true);
		}

		/// <summary>
		/// Fixes Type.Name strings so the type names of generic types are also normally returned instead of being
		/// displayed as '1. So TaskService'1 will be displayed properly as TaskService<ITask>.
		/// </summary>
		/// <returns>The sanitized type name string.</returns>
		/// <param name="type">Type.</param>
		public static string GetSanitizedTypeNameString(this Type type)
		{
			if (type == null)
			{
				return "null";
			}
			else if (type.IsArray)
			{
				return type.GetElementType().GetSanitizedTypeNameString() + "[]";
			}
			else if (type.IsGenericType)
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(type.Name.Substring(0, type.Name.Length - 2));
				sb.Append("<");
				for (int index = 0; index < type.GetGenericArguments().Length; index++)
				{
					Type argument = type.GetGenericArguments()[index];
					if (index < type.GetGenericArguments().Length - 1)
					{
						sb.Append(argument.GetSanitizedTypeNameString() + ", ");
					}
					else
					{
						sb.Append(argument.GetSanitizedTypeNameString());
					}
				}
				sb.Append(">");
				return sb.ToString();
			}

			return type.Name;
		}
	}
}
