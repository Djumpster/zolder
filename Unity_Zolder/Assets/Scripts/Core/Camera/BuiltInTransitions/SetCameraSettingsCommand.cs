// Copyright 2020 Talespin Reality Inc. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Cameras.Transition
{
	/// <summary>
	/// An <see cref="ICameraTransitionCommand"/> that can be executed by an <see cref="ICameraController"/>.
	/// Copies the settings from a target camera to the controller. Linearly interpolates values where applicable and possible.
	/// </summary>
	public class SetCameraSettingsCommand : ICameraTransitionCommand
	{
		/// <inheritdoc/>
		public bool HasBeenCompleted => progress == 1;

		/// <inheritdoc/>
		public bool HasBeenStarted => cameraController != null;

		private readonly float durationSeconds;

		private ICameraController cameraController;

		private CameraSettings targetCameraSettings;
		private Vector3 startPosition;
		private Quaternion startRotation;

		private CameraSettings startCameraSettings;
		private Vector3 targetPosition;
		private Quaternion targetRotation;

		private float progress;

		public SetCameraSettingsCommand(CameraSettings targetCameraSettings, Vector3 targetPosition, Quaternion targetRotation, float durationSeconds)
		{
			this.targetCameraSettings = targetCameraSettings;
			this.targetPosition = targetPosition;
			this.targetRotation = targetRotation;
			this.durationSeconds = durationSeconds;
		}

		/// <inheritdoc/>
		public void Initialize(ICameraController cameraController)
		{
			this.cameraController = cameraController;

			if (targetCameraSettings is OrthographicCameraSettings)
			{
				startCameraSettings = new OrthographicCameraSettings(cameraController.MainCamera);
				cameraController.MainCamera.orthographic = true;
			}
			else if (targetCameraSettings is PerspectiveCameraSettings)
			{
				startCameraSettings = new PerspectiveCameraSettings(cameraController.MainCamera);
				cameraController.MainCamera.orthographic = false;
			}

			cameraController.MainCamera.clearFlags = targetCameraSettings.CameraClearFlags;
			cameraController.MainCamera.cullingMask = targetCameraSettings.CullingMask;

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

			cameraController.MainCamera.backgroundColor = Color.Lerp(startCameraSettings.BackgroundColor, targetCameraSettings.BackgroundColor, progress);
			cameraController.MainCamera.nearClipPlane = Mathf.Lerp(startCameraSettings.NearClipPlane, targetCameraSettings.NearClipPlane, progress);
			cameraController.MainCamera.farClipPlane = Mathf.Lerp(startCameraSettings.FarClipPlane, targetCameraSettings.FarClipPlane, progress);
			cameraController.MainCamera.depth = Mathf.Lerp(startCameraSettings.Depth, targetCameraSettings.Depth, progress);

			if (cameraController.MainCamera.orthographic)
			{
				float startSize = (startCameraSettings as OrthographicCameraSettings).OrthographicSize;
				float targetSize = (targetCameraSettings as OrthographicCameraSettings).OrthographicSize;
				cameraController.MainCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, progress);
			}
			else
			{
				float startFoV = (startCameraSettings as PerspectiveCameraSettings).FieldOfView;
				float targetFoV = (targetCameraSettings as PerspectiveCameraSettings).FieldOfView;
				cameraController.MainCamera.fieldOfView = Mathf.Lerp(startFoV, targetFoV, progress);
			}

			Vector3 lerpedPosition = Vector3.Lerp(startPosition, targetPosition, progress);
			Quaternion lerpedCameraRotation = Quaternion.Lerp(startRotation, targetRotation, progress);

			cameraController.Move(lerpedPosition, lerpedCameraRotation, false);
		}
	}
}
