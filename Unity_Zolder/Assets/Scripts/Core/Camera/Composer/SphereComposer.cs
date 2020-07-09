// Copyright 2019 Talespin, LLC. All Rights Reserved.

using System.Collections;
using Talespin.Core.Foundation.TimeKeeping;
using UnityEngine;

namespace Talespin.Core.Foundation.Cameras.Composer
{
	/// <summary>
	/// Fades the FadeSphere using a vignette effect
	/// </summary>
	public class SphereComposer : BaseComposer
	{
		private const string FADE_PARAMETER_NAME = "_Fade";

		[SerializeField] private MeshRenderer meshRenderer;
		[SerializeField] private MeshCollider meshCollider;

		protected void LateUpdate()
		{
			meshRenderer.enabled = IsComposerVisible;
			meshCollider.enabled = IsComposerVisible;
		}

		/// <inheritdoc/>
		protected override IEnumerator Transition(float duration, float target)
		{
			if (duration > 0)
			{
				Timer timer = new Timer(duration);
				float startAlpha = meshRenderer.material.GetFloat(FADE_PARAMETER_NAME);

				while (timer)
				{
					float alpha = Mathf.Lerp(startAlpha, target, timer.progress);
					meshRenderer.material.SetFloat(FADE_PARAMETER_NAME, alpha);

					yield return null;
				}
			}

			meshRenderer.material.SetFloat(FADE_PARAMETER_NAME, target);
		}
	}
}
