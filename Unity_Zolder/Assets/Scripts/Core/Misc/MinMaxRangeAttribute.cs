// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Misc
{
	public class MinMaxRangeAttribute : PropertyAttribute
	{
		public float MinLimit { get; }

		public float MaxLimit { get; }

		public bool HardLimit { get; }

		public MinMaxRangeAttribute(float minLimit, float maxLimit, bool hardLimit = true)
		{
			MinLimit = minLimit;
			MaxLimit = maxLimit;
			HardLimit = hardLimit;
		}
	}
}