// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using UnityEngine;

namespace Talespin.Core.Foundation.Extensions
{
	public static class DateTimeExtensions
	{
		public static readonly DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		private static DateTime utcNowCached;
		private static float cacheFrame;

		public static DateTime UTCNowCached
		{
			get
			{
				int frameCount = Time.frameCount;
				if (cacheFrame != frameCount)
				{
					cacheFrame = frameCount;
					utcNowCached = DateTime.UtcNow;
				}
				return utcNowCached;
			}
		}

		public static DateTime UnixTimeStampToDateTime(this int unixTimeStamp)
		{
			return UnixTimeStampToDateTime((double)unixTimeStamp);
		}

		public static DateTime UnixTimeStampToDateTime(this double unixTimeStamp)
		{
			// Unix timestamp is seconds past epoch
			DateTime dtDateTime = EPOCH;
			dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
			return dtDateTime;
		}

		public static double DateTimeToUnixTimeStamp(this System.DateTime time)
		{
			TimeSpan span = (time.ToUniversalTime() - EPOCH);
			return span.TotalSeconds;
		}
	}
}
