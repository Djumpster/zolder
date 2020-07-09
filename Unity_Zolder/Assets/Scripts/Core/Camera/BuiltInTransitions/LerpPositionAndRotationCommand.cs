// Copyright 2020 Talespin Reality Inc. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Cameras.Transition
{
	/// <summary>
	/// An <see cref="ICameraTransitionCommand"/> that can be executed by an <see cref="ICameraController"/>.
	/// Lerps the camera rig to a given position and makes the camera face a given facing.
	/// </summary>
	public class LerpPositionAndRotationCommand : ICameraTransitionCommand
	{
		/// <inheritdoc/>
		public bool HasBeenCompleted => progress == 1;

		/// <inheritdoc/>
		public bool HasBeenStarted => cameraController != null;

		private readonly float durationSeconds;

		private ICameraController cameraController;

		private Vector3 startPosition;
		private Quaternion startRotation;

		private Vector3 targetPosition;
		private Quaternion targetRotation;

		private float progress;

		public LerpPositionAndRotationCommand(Vector3 targetPosition, Quaternion targetRotation, float durationSeconds)
		{
			this.targetPosition = targetPosition;
			this.targetRotation = targetRotation;
			this.durationSeconds = durationSeconds;
		}

		/// <inheritdoc/>
		public void Initialize(ICameraController cameraController)
		{
			this.cameraController = cameraController;

			startPosition = cameraController.Head.position;
			startRotation = cameraController.Head.rotation;
		}

		/// <inheritdoc/>
		public void Execute(float deltaTimeSeconds)
		{
			if (HasBeenCompleted)
			{
				return;
			}

			progress += deltaTimeSeconds / durationSeconds;
			progress = Mathf.Clamp01(progress);

			Vector3 lerpedPosition = Vector3.Lerp(startPosition, targetPosition, progress);
			Quaternion lerpedCameraRotation = Quaternion.Lerp(startRotation, targetRotation, progress);

			cameraController.Move(lerpedPosition, lerpedCameraRotation, false);
		}
	}
}
