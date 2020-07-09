// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Logging;

namespace Talespin.Core.Foundation.Extensions
{
	public static class EnumExtenions
	{
		public static bool HasFlags(this System.Enum orig, System.Enum flag)
		{
			if (orig.GetType() != flag.GetType())
			{
				LogUtil.Error(LogTags.SYSTEM, "EnumExtenions", "Incompatible enum types provided to HasFlags!");
			}
			long o = ((System.IConvertible)orig).ToInt64(null);
			long f = ((System.IConvertible)flag).ToInt64(null);
			return ((o & f) == f);
		}

		public static T CycleEnum<T>(this System.Enum orig, int inc = 1)
		{
			if (orig.GetType() != typeof(T))
			{
				LogUtil.Error(LogTags.SYSTEM, "EnumExtenions", "Incompatible enum types provided to CycleEnum!");
			}
			System.Array vals = System.Enum.GetValues(orig.GetType());
			int cur = System.Array.IndexOf(vals, orig);
			cur = CyclicAdd(cur, inc, vals.Length);
			return (T)vals.GetValue(cur);
		}

		private static int CyclicAdd(int val, int sum, int max)
		{
			val += sum;
			val %= max;
			if (val < 0)
			{
				val = max + val;
			}
			return val;
		}
	}
}
