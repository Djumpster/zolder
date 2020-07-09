// Copyright 2020 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Injection;
using UnityEngine;

namespace Talespin.Core.Foundation.Cameras
{
	/// <summary>
	/// The camera service is the main camera controller and can be used
	/// to retrieve various information from the camera setup, or perform
	/// translation and rotation.
	/// <para>
	/// This service uses the <see cref="ICameraProvider"/> implementation
	/// to retrieve a camera setup. By default the <see cref="CameraRigProvider"/> is
	/// used for this, however it is possible to create a custom camera provider
	/// by creating a factory.
	/// </para>
	/// </summary>
	/// <seealso cref="ICameraController"/>
	/// <seealso cref="ICameraProvider"/>
	public class CameraService : IScopedDependency, ICameraController
	{
		/// <inheritdoc/>
		public event CameraTransitionStartedHandler CameraTransitionStartedEvent = delegate { };

		/// <inheritdoc/>
		public event CameraTransitionEndedHandler CameraTransitionEndedEvent = delegate { };

		/// <inheritdoc/>
		public Camera MainCamera => controller?.MainCamera;

		/// <inheritdoc/>
		public Transform Rig => controller?.Rig;

		/// <inheritdoc/>
		public Transform Head => controller?.Head;

		/// <inheritdoc/>
		public Transform Ears => controller?.Ears;

		/// <inheritdoc/>
		public Transform LeftEye => controller?.LeftEye;

		/// <inheritdoc/>
		public Transform RightEye => controller?.RightEye;

		/// <inheritdoc/>
		public Transform LeftHand => controller?.LeftHand;

		/// <inheritdoc/>
		public Transform RightHand => controller?.RightHand;

		/// <inheritdoc/>
		public Vector3 RotationOffset
		{
			set
			{
				if (controller != null)
				{
					controller.RotationOffset = value;
				}
			}
			get => controller != null ? controller.RotationOffset : Vector3.zero;
		}

		public bool IsTransitioning => controller != null && controller.IsTransitioning;

		private readonly ICameraProvider provider;

		private ICameraController controller;

		public CameraService(ICameraProvider provider)
		{
			this.provider = provider;
		}

		public void Start()
		{
			controller = provider.Provide();
			controller.CameraTransitionStartedEvent += OnCameraTransitionStartedEvent;
			controller.CameraTransitionEndedEvent += OnCameraTransitionEndedEvent;
		}

		public void Stop()
		{
			controller.CameraTransitionStartedEvent -= OnCameraTransitionStartedEvent;
			controller.CameraTransitionEndedEvent -= OnCameraTransitionEndedEvent;
			controller.InterruptTransition();
			controller = null;
		}

		/// <inheritdoc/>
		public void ApplyCameraRotationOffset() => controller?.ApplyCameraRotationOffset();

		/// <inheritdoc/>
		public void ReApplyLastPositionAndRotation() => controller?.ReApplyLastPositionAndRotation();

		/// <inheritdoc/>
		public void Move(Vector3 position, Quaternion rotation, bool includeRotationOffset = true) => controller?.Move(position, rotation, includeRotationOffset);

		/// <inheritdoc/>
		public void Transition(ICameraTransitionCommand command, CameraTransitionEndedHandler onCompleteCallback = null) => controller?.Transition(command, onCompleteCallback);

		/// <inheritdoc/>
		public void InterruptTransition() => controller?.InterruptTransition();

		private void OnCameraTransitionStartedEvent(ICameraTransitionCommand command, ICameraController controller)
		{
			CameraTransitionStartedEvent.Invoke(command, controller);
		}

		private void OnCameraTransitionEndedEvent(ICameraTransitionCommand command, ICameraController controller, bool wasCompleted)
		{
			CameraTransitionEndedEvent.Invoke(command, controller, wasCompleted);
		}
	}
}
