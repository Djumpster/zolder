// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Linq;
using UnityEngine;

namespace Talespin.Core.Foundation.Filter
{
	public static partial class ComparisonMetrics
	{
		public static double OverlapCoefficient(this string source, string target)
		{
			return (Convert.ToDouble(source.Intersect(target).Count())) / Convert.ToDouble(Mathf.Min(source.Length, target.Length));
		}
	}
}
