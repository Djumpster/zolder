// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Extensions
{
	public static class FloatExtensions
	{
		public static string ToTimeString(this float timeInSeconds)
		{
			System.TimeSpan span = System.TimeSpan.FromSeconds(timeInSeconds);
			return string.Format("{0}:{1}", span.Minutes.ToString("00"), span.Seconds.ToString("00"));
		}

		/// <summary>
		/// Determines if the value is between the specified min max. Including min and max values given.
		/// </summary>
		/// <returns><see langword="true" /> if is in between the specified min max including min and max; otherwise, <see langword="false" />.</returns>
		/// <param name="i">The value</param>
		/// <param name="min">Minimum.</param>
		/// <param name="max">Maximum.</param>
		public static bool IsInBetween(this float i, float min, float max)
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
		public static bool IsBetween(this float i, float min, float max)
		{
			if (i > min && i < max)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Normalized the given value between 0 and 1 of the min and max value
		/// </summary>
		/// <param name="i">the raw value</param>
		/// <param name="min">Minimum</param>
		/// <param name="max">Maximum</param>
		/// <returns></returns>
		public static float Normalize(this float i, float min, float max, bool clamp01 = true)
		{
			float res = (i - min) / (max - min);
			return clamp01 ? Mathf.Clamp01(res) : res;
		}
	}
}
