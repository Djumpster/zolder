// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;

namespace Talespin.Core.Foundation.Extensions
{
	public static class IntExtensions
	{
		public static T ToEnum<T>(this int param, T defaultValue)
		{
			Type info = typeof(T);
			if (info.IsEnum)
			{
				T result = (T)Enum.Parse(typeof(T), param.ToString(), true);
				return result;
			}

			return defaultValue;
		}

		/// <summary>
		/// Determines if the value is between the specified min max. Including min and max values given.
		/// </summary>
		/// <returns><see langword="true" /> if is in between the specified min max including min and max; otherwise, <see langword="false" />.</returns>
		/// <param name="i">The value</param>
		/// <param name="min">Minimum.</param>
		/// <param name="max">Maximum.</param>
		public static bool IsInBetween(this int i, int min, int max)
		{
			if (i >= min && i <= max)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Determines if the value is between the specified min and max.
		/// </summary>
		/// <returns><see langword="true" /> if the value is between the specified min and max; otherwise, <see langword="false" />.</returns>
		/// <param name="i">The value</param>
		/// <param name="min">Minimum.</param>
		/// <param name="max">Maximum.</param>
		public static bool IsBetween(this int i, int min, int max)
		{
			if (i > min && i < max)
			{
				return true;
			}

			return false;
		}
	}
}
