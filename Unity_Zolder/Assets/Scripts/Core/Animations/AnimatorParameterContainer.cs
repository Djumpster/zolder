// Copyright 2020 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Talespin.Core.Foundation.Animations
{
	/// <summary>
	/// A simple data container for animator parameters that can be used to set data in code or through the editor.
	/// </summary>
	[Serializable]
	public class AnimatorParameterContainer
	{
		[Serializable]
		public class AnimatorParameter
		{
			public string ParameterName => parameterName;

			[SerializeField] private string parameterName;

			public AnimatorParameter(string parameterName)
			{
				this.parameterName = parameterName;
			}
		}

		[Serializable]
		public class FloatAnimatorParameter : AnimatorParameter
		{
			public float Value => value;

			[SerializeField] private float value;

			public FloatAnimatorParameter(string parameterName, float value) : base(parameterName)
			{
				this.value = value;
			}
		}

		[Serializable]
		public class IntAnimatorParameter : AnimatorParameter
		{
			public int Value => value;

			[SerializeField] private int value;

			public IntAnimatorParameter(string parameterName, int value) : base(parameterName)
			{
				this.value = value;
			}
		}

		[Serializable]
		public class BoolAnimatorParameter : AnimatorParameter
		{
			public bool Value => value;

			[SerializeField] private bool value;

			public BoolAnimatorParameter(string parameterName, bool value) : base(parameterName)
			{
				this.value = value;
			}
		}

		[Serializable]
		public class TriggerAnimatorParameter : AnimatorParameter
		{
			public TriggerAnimatorParameter(string parameterName) : base(parameterName)
			{
			}
		}

		public List<FloatAnimatorParameter> FloatParameters => floatParameters;
		public List<IntAnimatorParameter> IntParameters => intParameters;
		public List<BoolAnimatorParameter> BoolParameters => boolParameters;
		public List<TriggerAnimatorParameter> TriggerParameters => triggerParameters;

		[SerializeField] private List<FloatAnimatorParameter> floatParameters = new List<FloatAnimatorParameter>();
		[SerializeField] private List<IntAnimatorParameter> intParameters = new List<IntAnimatorParameter>();
		[SerializeField] private List<BoolAnimatorParameter> boolParameters = new List<BoolAnimatorParameter>();
		[SerializeField] private List<TriggerAnimatorParameter> triggerParameters = new List<TriggerAnimatorParameter>();
	}
}
