// Copyright 2020 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Cameras
{
	/// <summary>
	/// A default and basic implementation of a <see cref="ICameraController"/>.
	/// <para>
	/// For most movement operations the <see cref="Move(Vector3)"/> and
	/// <see cref="Rotate(Quaternion, bool)"/> will suffice, however
	/// the <see cref="Transition(ICameraTransitionCommand, CameraTransitionEndedHandler)"/>
	/// function is also available for more complex camera operations.
	/// </para>
	/// </summary>
	public class CameraControllerBehaviour : MonoBehaviour, ICameraController
	{
		/// <inheritdoc/>
		public event CameraTransitionStartedHandler CameraTransitionStartedEvent = delegate { };

		/// <inheritdoc/>
		public event CameraTransitionEndedHandler CameraTransitionEndedEvent = delegate { };

		/// <inheritdoc/>
		public Camera MainCamera => mainCamera;

		/// <inheritdoc/>
		public Transform Rig => rig;

		/// <inheritdoc/>
		public Transform Head => head;

		/// <inheritdoc/>
		public Transform Ears => ears;

		/// <inheritdoc/>
		public Transform LeftEye => leftEye;

		/// <inheritdoc/>
		public Transform RightEye => rightEye;

		/// <inheritdoc/>
		public Transform LeftHand => leftHand;

		/// <inheritdoc/>
		public Transform RightHand => rightHand;

		/// <inheritdoc/>
		public Vector3 RotationOffset { set; get; }

		public bool IsTransitioning => currentTransitionCommand != null && !currentTransitionCommand.HasBeenCompleted;

		[SerializeField] private Camera mainCamera;

		[SerializeField] private Transform rig;
		[SerializeField] private Transform head;
		[SerializeField] private Transform ears;
		[SerializeField] private Transform leftEye;
		[SerializeField] private Transform rightEye;
		[SerializeField] private Transform leftHand;
		[SerializeField] private Transform rightHand;

		private ICameraTransitionCommand currentTransitionCommand;
		private CameraTransitionEndedHandler currentTransitionCallback;

		private Vector3 lastSetPosition;
		private Quaternion lastSetRotation;
		private bool lastIncludeRotationOffset;

		protected void Update()
		{
			if (!IsTransitioning)
			{
				return;
			}

			currentTransitionCommand.Execute(Time.deltaTime);

			if (currentTransitionCommand.HasBeenCompleted)
			{
				ICameraTransitionCommand completedCommand = currentTransitionCommand;
				CameraTransitionEndedHandler completedCommandCallback = currentTransitionCallback;

				currentTransitionCommand = null;
				currentTransitionCallback = null;

				completedCommandCallback?.Invoke(completedCommand, this, true);
				CameraTransitionEndedEvent.Invoke(completedCommand, this, true);
			}
		}

		/// <inheritdoc/>
		public void ApplyCameraRotationOffset()
		{
			lastIncludeRotationOffset = true;
			rig.rotation = ApplyRotationOffset(rig.rotation);
		}

		/// <inheritdoc/>
		public void ReApplyLastPositionAndRotation()
		{
			Move(lastSetPosition, lastSetRotation, lastIncludeRotationOffset);
		}

		/// <inheritdoc/>
		public void Move(Vector3 position, Quaternion rotation, bool includeRotationOffset = true)
		{
			lastSetPosition = position;
			lastSetRotation = rotation;
			lastIncludeRotationOffset = includeRotationOffset;

			rig.rotation = includeRotationOffset ? ApplyRotationOffset(rotation) : rotation;

			Vector3 newRigPosition = Vector3.zero;
			newRigPosition.x = rig.position.x + (position.x - head.position.x);
			newRigPosition.y = position.y;
			newRigPosition.z = rig.position.z + (position.z - head.position.z);

			rig.position = newRigPosition;
		}

		/// <inheritdoc/>
		public void Transition(ICameraTransitionCommand command, CameraTransitionEndedHandler onCompleteCallback = null)
		{
			ICameraTransitionCommand interrupedCommand = currentTransitionCommand;
			CameraTransitionEndedHandler interruptedCommandCallback = currentTransitionCallback;

			currentTransitionCommand = command;
			currentTransitionCallback = onCompleteCallback;

			currentTransitionCommand.Initialize(this);

			if (interrupedCommand != null && !interrupedCommand.HasBeenCompleted)
			{
				interruptedCommandCallback?.Invoke(interrupedCommand, this, false);
				CameraTransitionEndedEvent.Invoke(interrupedCommand, this, false);
			}

			CameraTransitionStartedEvent.Invoke(currentTransitionCommand, this);

			Update();
		}

		/// <inheritdoc/>
		public void InterruptTransition()
		{
			ICameraTransitionCommand interrupedCommand = currentTransitionCommand;
			CameraTransitionEndedHandler interruptedCommandCallback = currentTransitionCallback;

			currentTransitionCommand = null;
			currentTransitionCallback = null;

			if (interrupedCommand != null && !interrupedCommand.HasBeenCompleted)
			{
				interruptedCommandCallback?.Invoke(interrupedCommand, this, false);
				CameraTransitionEndedEvent.Invoke(interrupedCommand, this, false);
			}
		}

		private Quaternion ApplyRotationOffset(Quaternion rotation)
		{
			Vector3 offsetEuler = rotation.eulerAngles + RotationOffset;
			return Quaternion.Euler(offsetEuler);
		}
	}
}
