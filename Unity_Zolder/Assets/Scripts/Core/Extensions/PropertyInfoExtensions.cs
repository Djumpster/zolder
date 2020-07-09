// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Reflection;

namespace Talespin.Core.Foundation.Extensions
{
	public static class PropertyInfoExtensions
	{
		public static bool HasAttribute<T>(this PropertyInfo property) where T : System.Attribute
		{
			return property.GetCustomAttributes(typeof(T), true).Length > 0;
		}

		public static bool HasType<T>(this PropertyInfo property)
		{
			return property.PropertyType.IsAssignableFrom(typeof(T));
		}
	}
}
