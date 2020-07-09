// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections;
using Talespin.Core.Foundation.TimeKeeping;
using UnityEngine;

namespace Talespin.Core.Foundation.Cameras
{
	public class CameraOccluder : MonoBehaviour
	{
		private const float FADE_TIME = 1.3f;
		private const float MIN_ALPHA = 0.3f;


		[SerializeField] Renderer fadableRenderer;
		[SerializeField] Material[] fadeMaterial;

		private Material[] originalMaterial;
		private float fadeVal = 1f;

		private void Start()
		{
			originalMaterial = fadableRenderer.sharedMaterials;
		}

		public void FadeOut()
		{
			StopAllCoroutines();
			StartCoroutine("DoFadeOut");

		}

		public void FadeIn()
		{
			StopAllCoroutines();
			StartCoroutine("DoFadeIn");
		}

		void SwitchToFadableMaterial()
		{
			fadableRenderer.materials = fadeMaterial;
		}

		void RevertMaterial()
		{
			fadableRenderer.sharedMaterials = originalMaterial;
		}


		float fadeOutProgress = 0f;
		float fadeInProgress = 0f;


		IEnumerator DoFadeOut()
		{
			SwitchToFadableMaterial();
			Timer t = new Timer(FADE_TIME * (1f - fadeOutProgress));

			while (t)
			{
				fadeVal = Mathf.Lerp(fadeVal, MIN_ALPHA, Mathf.Pow(t.progress, 2));
				for (int i = 0; i < fadableRenderer.materials.Length; i++)
				{
					fadableRenderer.materials[i].SetFloat("_AlphaFade", fadeVal);
				}
				fadeOutProgress = t.progress;
				fadeInProgress = 1f - t.progress;


				yield return null;
			}
			fadeVal = MIN_ALPHA;

		}


		IEnumerator DoFadeIn()
		{
			SwitchToFadableMaterial();
			Timer t = new Timer(FADE_TIME * (1f - fadeInProgress));
			while (t)
			{
				fadeVal = Mathf.Lerp(fadeVal, 1f, Mathf.Pow(t.progress, 2));
				for (int i = 0; i < fadableRenderer.materials.Length; i++)
				{
					fadableRenderer.materials[i].SetFloat("_AlphaFade", fadeVal);
				}
				fadeOutProgress = 1f - t.progress;
				fadeInProgress = t.progress;


				yield return null;
			}
			fadeVal = 1f;
			RevertMaterial();
		}
	}
}