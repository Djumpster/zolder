// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Misc
{
	[System.Serializable]
	public class MinMaxRange
	{
		public float Min => min;
		public float Max => max;
		public float Value => inMinMaxMode ? Random.Range(min, max) : min;

		[SerializeField] private float min;
		[SerializeField] private float max;
		[SerializeField] private bool inMinMaxMode;

		public MinMaxRange(float defaultMin, float defaultMax, bool defaultInMinMaxMode = true)
		{
			min = defaultMin;
			max = defaultMax;
			inMinMaxMode = defaultInMinMaxMode;
		}

		public static implicit operator float(MinMaxRange minMaxRange) => minMaxRange.Value;

		public override string ToString() => Value.ToString();
	}
}
