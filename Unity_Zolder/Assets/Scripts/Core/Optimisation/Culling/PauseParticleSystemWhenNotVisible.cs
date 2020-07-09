// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using UnityEngine;

namespace Talespin.Core.Foundation.Optimisation
{
	/// <summary>
	/// Will update a given particle system only when it is enabled and visible and pause it otherwise.
	/// This prevents a performance hit when the ParticleSystem becomes visible again after a while,
	/// since it will try to fast-forward the simulation for the time passed.
	/// </summary>
	public class PauseParticleSystemWhenNotVisible : MonoBehaviour
	{
		#region members
		[SerializeField] private ParticleSystem targetParticleSystem;

		private List<ParticleSystemVisibilityMonitor> subEmittersVisibility = new List<ParticleSystemVisibilityMonitor>();
		#endregion

		#region Unity callbacks
		protected void Awake()
		{
			if (targetParticleSystem == null)
			{
				targetParticleSystem = GetComponent<ParticleSystem>();
			}
			if (targetParticleSystem != null)
			{
				for (int i = 0; i < targetParticleSystem.subEmitters.subEmittersCount; i++)
				{
					ParticleSystem subEmitter = targetParticleSystem.subEmitters.GetSubEmitterSystem(i);
					subEmittersVisibility.Add(subEmitter.gameObject.AddComponent<ParticleSystemVisibilityMonitor>());
				}
			}
		}

		protected void Update()
		{
			foreach (ParticleSystemVisibilityMonitor subEmitterVisibility in subEmittersVisibility)
			{
				if (subEmitterVisibility.gameObject.activeInHierarchy && subEmitterVisibility.IsVisible)
				{
					Resume();
				}
			}
		}

		protected void OnEnable()
		{
			Resume();
		}

		protected void OnDisable()
		{
			Pause();
		}

		protected void OnBecameVisible()
		{
			Resume();
		}

		protected void OnBecameInvisible()
		{
			foreach (ParticleSystemVisibilityMonitor subEmitterVisibility in subEmittersVisibility)
			{
				if (subEmitterVisibility.gameObject.activeInHierarchy &&
					subEmitterVisibility.ParticleSystem.isPlaying &&
					subEmitterVisibility.IsVisible)
				{
					return;
				}
			}

			Pause();
		}
		#endregion

		#region private methods
		private void Pause()
		{
			if (targetParticleSystem.isPlaying)
			{
				targetParticleSystem.Pause();
			}
		}

		private void Resume()
		{
			if (targetParticleSystem.isPaused)
			{
				targetParticleSystem.Play();
			}
		}
		#endregion
	}
}
