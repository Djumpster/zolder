// Copyright 2020 Talespin Reality Labs Inc. All Rights Reserved.

using System;
using Talespin.Core.Foundation.Injection;
using UnityEngine;

namespace Talespin.Core.Foundation.Cameras.Composer
{
	/// <summary>
	/// A default composer implementation designed to work with the default camera rig provider.
	/// <para>
	/// This will likely also work with custom <see cref="ICameraProvider"/> implementations though.
	/// It requires a <see cref="MonoBehaviour"/> within the hierarchy of the <see cref="ICameraController.Rig"/>
	/// that implements the <see cref="ICameraComposer"/> interface.
	/// </para>
	/// </summary>
	public class CameraRigComposerService : IScopedDependency, ICameraComposer
	{
		/// <inheritdoc/>
		public event Action ComposerStartingEvent = delegate { };

		/// <inheritdoc/>
		public event Action ComposerStartedEvent = delegate { };

		/// <inheritdoc/>
		public event Action ComposerEndingEvent = delegate { };

		/// <inheritdoc/>
		public event Action ComposerEndedEvent = delegate { };

		/// <inheritdoc/>
		public CameraComposerState State => handler != null ? handler.State : CameraComposerState.Ended;

		private readonly ICameraController cameraController;

		private ICameraComposer handler;

		public CameraRigComposerService(ICameraController cameraController)
		{
			this.cameraController = cameraController;
		}

		public void Start()
		{
			handler = cameraController.Rig.GetComponentInChildren<ICameraComposer>();

			if (handler != null)
			{
				handler.ComposerStartingEvent += OnComposerStartingEvent;
				handler.ComposerStartedEvent += OnComposerStartedEvent;
				handler.ComposerEndingEvent += OnComposerEndingEvent;
				handler.ComposerEndedEvent += OnComposerEndedEvent;
			}
		}

		public void Stop()
		{
			if (handler != null)
			{
				handler.ComposerStartingEvent -= OnComposerStartingEvent;
				handler.ComposerStartedEvent -= OnComposerStartedEvent;
				handler.ComposerEndingEvent -= OnComposerEndingEvent;
				handler.ComposerEndedEvent -= OnComposerEndedEvent;
			}

			handler = null;
		}

		/// <inheritdoc/>
		public Coroutine ShowComposer(float duration = -1) => handler.ShowComposer(duration);

		/// <inheritdoc/>
		public Coroutine HideComposer(float duration = -1) => handler?.HideComposer(duration);

		/// <inheritdoc/>
		public void ShowComposerImmediately() => handler?.ShowComposerImmediately();

		/// <inheritdoc/>
		public void HideComposerImmediately() => handler?.HideComposerImmediately();

		private void OnComposerStartingEvent() => ComposerStartingEvent.Invoke();

		private void OnComposerStartedEvent() => ComposerStartedEvent.Invoke();

		private void OnComposerEndingEvent() => ComposerEndingEvent.Invoke();

		private void OnComposerEndedEvent() => ComposerEndedEvent.Invoke();
	}
}
