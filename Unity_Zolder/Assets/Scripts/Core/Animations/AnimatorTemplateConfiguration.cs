// Copyright 2019 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Animations
{
	/// <summary>
	///	Applies the <see cref="AnimatorConfiguration"/> to the specified <see cref="Animator"/>
	/// </summary>
	public class AnimatorTemplateConfiguration : MonoBehaviour
	{
		[SerializeField] private AnimatorConfiguration animatorConfiguration;
		[SerializeField] private Animator targetAnimator;
		[SerializeField] private float intensity = 1f;

		private float appliedIntensity = 0;

		protected void Awake()
		{
			Apply(intensity);
		}

		protected void OnEnable()
		{
			Apply(intensity);
		}

		private void Apply(float intensity)
		{
			if (targetAnimator == null)
			{
				targetAnimator = gameObject.GetComponent<Animator>();
			}

			appliedIntensity = intensity;
			animatorConfiguration.Apply(targetAnimator, intensity);
		}

#if UNITY_EDITOR

		protected void Update()
		{
			if (appliedIntensity != intensity)
			{
				Apply(intensity);
			}
		}

#endif
	}
}