// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Linq;

namespace Talespin.Core.Foundation.Filter
{
	public static partial class ComparisonMetrics
	{
		public static double TanimotoCoefficient(this string source, string target)
		{
			double Na = source.Length;
			double Nb = target.Length;
			double Nc = source.Intersect(target).Count();

			return Nc / (Na + Nb - Nc);
		}
	}
}
