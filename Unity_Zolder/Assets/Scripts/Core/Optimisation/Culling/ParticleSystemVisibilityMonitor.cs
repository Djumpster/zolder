// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Optimisation
{
	/// <summary>
	/// Keeps track of whether an object with a ParticleSystem on it is visible or not.
	/// </summary>
	public class ParticleSystemVisibilityMonitor : MonoBehaviour
	{
		public bool IsVisible { get; private set; }
		public ParticleSystem ParticleSystem { get; private set; }

		protected void Awake()
		{
			ParticleSystem = GetComponent<ParticleSystem>();
			IsVisible = true;
		}

		protected void OnBecameVisible()
		{
			IsVisible = true;
		}

		protected void OnBecameInvisible()
		{
			IsVisible = false;
		}
	}
}
