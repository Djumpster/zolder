// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	public class AudioMixerMacroMapping : ScriptableObject
	{
		[System.Serializable]
		public class ParameterMapping
		{
			public string Parameter { get { return mixerParameter; } }

			[SerializeField] private string mixerParameter;
			[SerializeField] private Vector2 minMaxRange;
			[SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

			public float GetParameterValue(float t)
			{
				t = curve.Evaluate(t);
				return Mathf.Lerp(minMaxRange.x, minMaxRange.y, t);
			}
		}

		[System.Serializable]
		public class Macro
		{
			public string ID { get { return id; } }
			public float DefaultWeight { get { return defaultWeight; } } // I cannot InverseLerp this from the mixer because multiple parameters might be at conflicting values
			public ParameterMapping[] Mappings { get { return mappings; } }

			[SerializeField] private string id;
			[SerializeField, Range(0, 1)] private float defaultWeight;
			[SerializeField] private ParameterMapping[] mappings;
		}

		public Macro[] Macros { get { return macros; } }

		[SerializeField] private Macro[] macros;
	}
}