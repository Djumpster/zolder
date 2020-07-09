// Copyright 2019 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections;
using Talespin.Core.Foundation.Services;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
using UnityEngine;
using UnityEngine.Assertions;

namespace Talespin.Core.Foundation.Animations
{
	/// <summary>
	/// A configuration of animator parameters that can be applied to an animator controller.
	/// </summary>
	[Serializable]
	public class AnimatorConfiguration
	{
		[Serializable]
		public class AnimatorParameter
		{
			public string ParameterName => parameterName;
			[SerializeField] private string parameterName;
		}

		[Serializable]
		public class FloatAnimatorParameter : AnimatorParameter
		{
			public float Min => min;
			[SerializeField, Tooltip("Inclusive")] private float min;

			public float Max => max;
			[SerializeField, Tooltip("Inclusive")] private float max;
		}

		[Serializable]
		public class IntAnimatorParameter : AnimatorParameter
		{
			public int Min => min;
			[SerializeField, Tooltip("Inclusive")] private int min;

			public int Max => max;
			[SerializeField, Tooltip("Inclusive")] private int max;
		}

		[Serializable]
		public class BoolAnimatorParameter : AnimatorParameter
		{
			public bool Value => value;
			[SerializeField] private bool value;
		}

		[Serializable]
		public class TriggerAnimatorParameter : AnimatorParameter
		{
			public bool Value => value;
			[SerializeField] private bool value;

			public bool ReaplyOnChangedIntensity => reapplyOnChangedIntensity;
			[SerializeField, Tooltip("Should this trigger be reapplied if the same animator configuration is applied" +
				" but with a different intensity?")]
			private bool reapplyOnChangedIntensity = false;
		}

		[SerializeField] private FloatAnimatorParameter[] floatParameters;
		[SerializeField] private IntAnimatorParameter[] intParameters;
		[SerializeField] private BoolAnimatorParameter[] boolParameters;
		[SerializeField] private TriggerAnimatorParameter[] triggerParameters;

#if UNITY_EDITOR
		[Tooltip("The animator used as an example while configuring this. The configuration can still be applied to other animators.")]
		[SerializeField] private AnimatorController templateAnimator;

		[Tooltip("If not empty then only parameters that contain the paramaterFilter text are included for selection.")]
		[SerializeField] private string parameterFilter;
#endif

		public void ApplyLerped(Animator animator, float intensity, ICoroutineService coroutineService, float time,
			bool isReapplication = false)
		{
			Assert.IsTrue(intensity >= 0f && intensity <= 1f, "Intensity must be a value between 0 and 1.");

			SetTriggersAndBooleans(animator, isReapplication);
			if (time == 0)
			{
				ApplyAnimatorConfiguration(animator, intensity);
			}
			else
			{
				coroutineService.StopContext(this);
				coroutineService.StartCoroutine(ApplyLerped(animator, time, intensity), this);
			}
		}

		public void ApplyCurved(Animator animator, float intensity, ICoroutineService coroutineService, AnimationCurve curve,
			bool isReapplication = false)
		{
			Assert.IsTrue(intensity >= 0f && intensity <= 1f, "Intensity must be a value between 0 and 1.");

			SetTriggersAndBooleans(animator, isReapplication);

			coroutineService.StopContext(this);
			coroutineService.StartCoroutine(ApplyCurved(animator, curve, intensity), this);
		}

		/// <summary>
		/// Applies the animator configuration to the given animator.
		/// </summary>
		/// <param name="animator">The animator to apply the configured parameters on.</param>
		/// <param name="intensity">The intensity to apply the configured parameters with. This only affects floats and ints which have a min / max range.</param>
		/// <param name="isReapplication">Is this effect being already being applied and being reapplied with a different intensity?</param>
		public void Apply(Animator animator, float intensity, bool isReapplication = false)
		{
			Assert.IsTrue(intensity >= 0f && intensity <= 1f, "Intensity must be a value between 0 and 1.");

			SetTriggersAndBooleans(animator, isReapplication);
			ApplyAnimatorConfiguration(animator, intensity);
		}

		public void SetTriggersAndBooleans(Animator animator, bool isReapplication = false)
		{
			foreach (BoolAnimatorParameter boolParameter in boolParameters)
			{
				animator.SetBool(boolParameter.ParameterName, boolParameter.Value);
			}

			foreach (TriggerAnimatorParameter triggerParameter in triggerParameters)
			{
				if (isReapplication && !triggerParameter.ReaplyOnChangedIntensity)
				{
					continue;
				}

				if (triggerParameter.Value)
				{
					animator.SetTrigger(triggerParameter.ParameterName);
				}
				else
				{
					animator.ResetTrigger(triggerParameter.ParameterName);
				}
			}
		}

		public void Stop(ICoroutineService coroutineService)
		{
			coroutineService.StopContext(this);
		}

		private IEnumerator ApplyLerped(Animator animator, float duration, float intensity)
		{
			float[] floatStartValues = new float[floatParameters.Length];
			int[] intStartValues = new int[intParameters.Length];

			for (int i = 0; i < floatParameters.Length; i++)
			{
				FloatAnimatorParameter floatParameter = floatParameters[i];
				floatStartValues[i] = animator.GetFloat(floatParameter.ParameterName);
			}
			for (int i = 0; i < intParameters.Length; i++)
			{
				IntAnimatorParameter intParameter = intParameters[i];
				intStartValues[i] = animator.GetInteger(intParameter.ParameterName);
			}

			float timeToWait = duration;
			while (timeToWait > 0)
			{
				float progress = 1f - timeToWait / duration;
				ApplyAnimatorConfiguration(animator, intensity * progress);

				for (int i = 0; i < floatParameters.Length; i++)
				{
					FloatAnimatorParameter floatParameter = floatParameters[i];
					float target = Mathf.Lerp(floatParameter.Min, floatParameter.Max, intensity);
					animator.SetFloat(floatParameter.ParameterName, Mathf.Lerp(floatStartValues[i], target, progress));
				}
				for (int i = 0; i < intParameters.Length; i++)
				{
					IntAnimatorParameter intParameter = intParameters[i];
					float target = Mathf.RoundToInt(Mathf.Lerp(intParameter.Min, intParameter.Max, intensity));
					animator.SetInteger(intParameter.ParameterName, (int)Mathf.Lerp(intStartValues[i], target, progress));
				}

				yield return new WaitForSeconds(0.033f);
				timeToWait -= 0.033f;
			}

			ApplyAnimatorConfiguration(animator, intensity);
		}

		private IEnumerator ApplyCurved(Animator animator, AnimationCurve transitionCurve, float intensity)
		{
			float[] floatStartValues = new float[floatParameters.Length];
			int[] intStartValues = new int[intParameters.Length];

			for (int i = 0; i < floatParameters.Length; i++)
			{
				FloatAnimatorParameter floatParameter = floatParameters[i];
				floatStartValues[i] = animator.GetFloat(floatParameter.ParameterName);
			}
			for (int i = 0; i < intParameters.Length; i++)
			{
				IntAnimatorParameter intParameter = intParameters[i];
				intStartValues[i] = animator.GetInteger(intParameter.ParameterName);
			}

			float start = transitionCurve.keys[0].time;
			float end = transitionCurve.keys[transitionCurve.keys.Length - 1].time;

			float current = start;
			while (current < end)
			{
				float progress = transitionCurve.Evaluate(current);
				ApplyAnimatorConfiguration(animator, intensity * progress);

				for (int i = 0; i < floatParameters.Length; i++)
				{
					FloatAnimatorParameter floatParameter = floatParameters[i];
					float target = Mathf.Lerp(floatParameter.Min, floatParameter.Max, intensity);
					animator.SetFloat(floatParameter.ParameterName, Mathf.Lerp(floatStartValues[i], target, progress));
				}
				for (int i = 0; i < intParameters.Length; i++)
				{
					IntAnimatorParameter intParameter = intParameters[i];
					float target = Mathf.RoundToInt(Mathf.Lerp(intParameter.Min, intParameter.Max, intensity));
					animator.SetInteger(intParameter.ParameterName, (int)Mathf.Lerp(intStartValues[i], target, progress));
				}

				yield return new WaitForSeconds(0.033f);
				current += 0.033f;
			}

			ApplyAnimatorConfiguration(animator, intensity);
		}

		private void ApplyAnimatorConfiguration(Animator animator, float intensity)
		{
			foreach (FloatAnimatorParameter floatParameter in floatParameters)
			{
				animator.SetFloat(floatParameter.ParameterName, Mathf.Lerp(floatParameter.Min, floatParameter.Max, intensity));
			}
			foreach (IntAnimatorParameter intParameter in intParameters)
			{
				animator.SetInteger(intParameter.ParameterName, Mathf.RoundToInt(Mathf.Lerp(intParameter.Min, intParameter.Max, intensity)));
			}
		}
	}
}