// Copyright 2020 Talespin Reality Labs Inc. All Rights Reserved.

using System;
using System.Collections;
using UnityEngine;

namespace Talespin.Core.Foundation.Cameras.Composer
{
	/// <summary>
	/// The base camera composer implementation. This implements most of the
	/// composer functionality and allows for a custom implementation of the
	/// visualization.
	/// </summary>
	public abstract class BaseComposer : MonoBehaviour, ICameraComposer
	{
		private const float TRANSITION_SAFE_DURATION = 0.25f;

		/// <inheritdoc/>
		public event Action ComposerStartingEvent = delegate { };

		/// <inheritdoc/>
		public event Action ComposerStartedEvent = delegate { };

		/// <inheritdoc/>
		public event Action ComposerEndingEvent = delegate { };

		/// <inheritdoc/>
		public event Action ComposerEndedEvent = delegate { };

		/// <inheritdoc/>
		public CameraComposerState State { private set; get; }

		protected bool IsComposerVisible => State != CameraComposerState.Ended;

		[SerializeField] private float transitionInDuration = 2;
		[SerializeField] private float transitionOutDuration = 2;

		private bool isTransitioning;

		protected virtual void Awake()
		{
			HideComposerImmediately();
		}

		/// <inheritdoc/>
		public Coroutine ShowComposer(float duration = -1)
		{
			duration = duration > 0 ? duration : transitionInDuration;
			return StartCoroutine(DoShowComposer(duration));
		}

		/// <inheritdoc/>
		public Coroutine HideComposer(float duration = -1)
		{
			duration = duration > 0 ? duration : transitionOutDuration;
			return StartCoroutine(DoHideComposer(duration));
		}

		/// <inheritdoc/>
		public virtual void ShowComposerImmediately()
		{
			StopAllCoroutines();
			ShowComposer(0);
		}

		/// <inheritdoc/>
		public virtual void HideComposerImmediately()
		{
			StopAllCoroutines();
			HideComposer(0);
		}

		/// <summary>
		/// Perform the transition of the composer's visual.
		/// </summary>
		/// <param name="duration">The duration of the transition in seconds</param>
		/// <param name="target">The target "alpha" of the transition, within the range of 0 to 1.</param>
		protected abstract IEnumerator Transition(float duration, float target);

		private IEnumerator DoShowComposer(float duration)
		{
			while (isTransitioning)
			{
				yield return null;
			}

			if (State == CameraComposerState.Ended)
			{
				isTransitioning = true;

				State = CameraComposerState.Starting;
				ComposerStartingEvent.Invoke();

				yield return Transition(duration, 1);

				if (duration > 0)
				{
					yield return new WaitForSeconds(TRANSITION_SAFE_DURATION);
				}

				State = CameraComposerState.Started;
				ComposerStartedEvent.Invoke();

				isTransitioning = false;
			}
		}

		private IEnumerator DoHideComposer(float duration)
		{
			while (isTransitioning)
			{
				yield return null;
			}

			if (State == CameraComposerState.Started)
			{
				isTransitioning = true;

				State = CameraComposerState.Ending;
				ComposerEndingEvent.Invoke();

				if (duration > 0)
				{
					yield return new WaitForSeconds(TRANSITION_SAFE_DURATION);
				}

				yield return Transition(duration, 0);

				State = CameraComposerState.Ended;
				ComposerEndedEvent.Invoke();

				isTransitioning = false;
			}
		}
	}
}
