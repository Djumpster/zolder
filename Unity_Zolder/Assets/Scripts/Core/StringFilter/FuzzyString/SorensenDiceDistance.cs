// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Linq;

namespace Talespin.Core.Foundation.Filter
{
	public static partial class ComparisonMetrics
	{
		public static double SorensenDiceDistance(this string source, string target)
		{
			return 1 - source.SorensenDiceIndex(target);
		}

		public static double SorensenDiceIndex(this string source, string target)
		{
			return (2 * Convert.ToDouble(source.Intersect(target).Count())) / (Convert.ToDouble(source.Length + target.Length));
		}
	}
}
