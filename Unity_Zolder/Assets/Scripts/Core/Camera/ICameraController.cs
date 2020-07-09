// Copyright 2020 Talespin, LLC. All Rights Reserved.

using UnityEngine;
using Talespin.Core.Foundation.Cameras.Transition;

namespace Talespin.Core.Foundation.Cameras
{
	/// <summary>
	/// Indicates that a camera transition has started.
	/// </summary>
	/// <param name="command">The command that is being performed</param>
	/// <param name="controller">The controller that is performing the transition</param>
	public delegate void CameraTransitionStartedHandler(ICameraTransitionCommand command, ICameraController controller);

	/// <summary>
	/// Indicates that a camera transition has ended.
	/// </summary>
	/// <param name="command">The command that has been performed</param>
	/// <param name="controller">The controller that has performed the transition</param>
	/// <param name="wasCompleted">Whether the transition was successfully completed</param>
	public delegate void CameraTransitionEndedHandler(ICameraTransitionCommand command, ICameraController controller, bool wasCompleted);

	/// <summary>
	/// The base camera controller data structure. Allows for access
	/// to various components such as the user's ears or hands.
	/// <para>
	/// Allows for movement and rotation of the camera with the
	/// <see cref="Move(Vector3)"/> function.
	/// </para>
	/// <para>
	/// Additionally this allows for a rotational offset to be configured
	/// which can be applied to all set-rotation commands. The rotational
	/// offset can be used to create a custom forward-face in your application.
	/// </para>
	/// <para>
	/// For more complex camera operations custom "transition" commands can be created
	/// and performed using the <see cref="Transition(ICameraTransitionCommand, CameraTransitionEndedHandler)"/>
	/// function.
	/// </para>
	/// </summary>
	/// <seealso cref="CameraControllerBehaviour"/>
	public interface ICameraController
	{
		/// <summary>
		/// Fired when a transition was is being started.
		/// </summary>
		event CameraTransitionStartedHandler CameraTransitionStartedEvent;

		/// <summary>
		/// Fired when the current transition has ended.
		/// </summary>
		event CameraTransitionEndedHandler CameraTransitionEndedEvent;

		/// <summary>
		/// A direct reference to the main camera.
		/// </summary>
		Camera MainCamera { get; }

		/// <summary>
		/// The root camera rig transform, in a virtual reality
		/// environment this is the only transform that can be
		/// reliably positioned.
		/// </summary>
		Transform Rig { get; }

		/// <summary>
		/// The transform of the user's head, in a virtual reality
		/// environment this will be controlled by the headset.
		/// <para>
		/// This transform usually contains the main camera, although there
		/// may also be cameras present on the <see cref="LeftEye"/> and <see cref="RightEye"/>.
		/// </para>
		/// </summary>
		/// <seealso cref="LeftEye"/>
		/// <seealso cref="RightEye"/>
		Transform Head { get; }

		/// <summary>
		/// The transform of the user's ears. Any audio listeners
		/// should be attached to this transform for realistic sound behavior.
		/// </summary>
		Transform Ears { get; }

		/// <summary>
		/// The transform of the user's left eye. In a virtual reality
		/// environment this is will generally be dynamically positioned
		/// using the IPD distance of the lenses during runtime.
		/// </summary>
		/// <seealso cref="Head"/>
		/// <seealso cref="RightEye"/>
		Transform LeftEye { get; }

		/// <summary>
		/// The transform of the user's right eye. In a virtual reality
		/// environment this is will generally be dynamically positioned
		/// using the IPD distance of the lenses during runtime.
		/// </summary>
		/// <seealso cref="Head"/>
		/// <seealso cref="LeftEye"/>
		Transform RightEye { get; }

		/// <summary>
		/// The transform of the user's left hand. In a virtual reality
		/// environment this will be the location of the left controller.
		/// <para>
		/// This may be <see langword="null"/> outside of a virtual reality
		/// environment.
		/// </para>
		/// </summary>
		/// <seealso cref="RightHand"/>
		Transform LeftHand { get; }

		/// <summary>
		/// The transform of the user's right hand. In a virtual reality
		/// environment this will be the location of the right controller.
		/// <para>
		/// This may be <see langword="null"/> outside of a virtual reality
		/// environment.
		/// </para>
		/// </summary>
		/// <seealso cref="LeftHand"/>
		Transform RightHand { get; }

		/// <summary>
		/// A three dimensional vector containing the current
		/// rotational offset, it is likely that only the <c>Y</c>
		/// component is used.
		/// </summary>
		Vector3 RotationOffset { get; set; }

		/// <summary>
		/// Whether the camera is currently performing a transition.
		/// </summary>
		bool IsTransitioning { get; }

		/// <summary>
		/// Apply the configured <see cref="RotationOffset"/> to the current transform.
		/// </summary>
		void ApplyCameraRotationOffset();

		/// <summary>
		/// Reposition the transform to match the last position and rotation configured
		/// by <see cref="Translate(Vector3, Quaternion, bool)"/>.
		/// </summary>
		/// <seealso cref="Translate(Vector3, Quaternion, bool)"/>
		void ReApplyLastPositionAndRotation();

		/// <summary>
		/// Immediately translates and rotates the camera to the specified position and rotation.
		/// <para>
		/// This function will keep the local position offset of the <see cref="Head"/> transform into
		/// account when performing transitions, it will ensure that the position of the <see cref="Head"/>
		/// is on the specified <paramref name="position"/> rather than the <see cref="Rig"/>.
		/// </para>
		/// <para>
		/// When the <paramref name="includeRotationOffset"/> parameter is set to <see langword="true"/>,
		/// this will automatically take the configured offset into account when rotating the camera.
		/// A rotation offset of <c>0,90,0</c> will add a 90 degree euler offset on the <c>Y</c> component
		/// of the input rotation.
		/// </para>
		/// </summary>
		/// <remarks>
		/// To implement smooth camera movement, simply call this function from a coroutine
		/// lerping the <paramref name="position"/> parameter.
		/// </remarks>
		/// <param name="position">The new camera position</param>
		/// <param name="rotation">The new camera rotation</param>
		/// <param name="includeRotationOffset">Whether the specified rotational offset should be applied</param>
		void Move(Vector3 position, Quaternion rotation, bool includeRotationOffset = true);

		/// <summary>
		/// Perform an arbitrary transition on the camera. Transition commands allow for more control
		/// over the camera's behavior besides translation and rotation. For example modifying camera
		/// properties, or creating custom smooth transitions.
		/// </summary>
		/// <param name="command">The command to perform</param>
		/// <param name="onCompleteCallback">The callback to fire when the command has ended</param>
		/// <seealso cref="SetCameraSettingsCommand"/>
		/// <seealso cref="LerpPositionAndRotationCommand"/>
		void Transition(ICameraTransitionCommand command, CameraTransitionEndedHandler onCompleteCallback = null);

		/// <summary>
		/// Interrupts the current transition command and immediately stops it. This will still fire
		/// the <see cref="CameraTransitionEndedEvent"/> event.
		/// </summary>
		void InterruptTransition();
	}
}
