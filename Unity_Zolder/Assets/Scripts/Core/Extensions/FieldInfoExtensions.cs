// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Linq;
using System.Reflection;

namespace Talespin.Core.Foundation.Extensions
{
	public static class FieldInfoExtensions
	{
		public static bool HasAttribute<T>(this FieldInfo field) where T : System.Attribute
		{
			return field.GetCustomAttributes(typeof(T), true).Length > 0;
		}
		public static T GetAttribute<T>(this FieldInfo field) where T : System.Attribute
		{
			var list = field.GetCustomAttributes(typeof(T), true);
			return (T)list.FirstOrDefault();
		}

		public static bool HasTypeAttribute<T>(this FieldInfo field) where T : System.Attribute
		{
			return field.FieldType.GetCustomAttributes(typeof(T), true).Length > 0;
		}

		public static bool HasType<T>(this FieldInfo field)
		{
			return field.FieldType.IsAssignableFrom(typeof(T));
		}
	}
}
