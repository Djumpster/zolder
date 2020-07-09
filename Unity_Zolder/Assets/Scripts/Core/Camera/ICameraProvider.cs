// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Cameras
{
	/// <summary>
	/// <para>
	/// Used to provide ready-to-go data for the <see cref="CameraService"/>.
	/// </para>
	/// <para>
	/// While the exact setup can change depending on platform, the 2 GameObject structure
	/// implemented by both OVR and SteamVR seems to work across all platforms with no to
	/// minimal changes required.
	/// 
	/// A sample camera hierarchy would look like this:
	/// <code>
	/// - Main Camera Rig
	///		- Main Camera
	///	</code>
	///	
	/// On platforms where having a rig isn't a necessity, the Main Camera GameObject's
	/// local position can simply be set to <c>{ 0, 0, 0 }</c>.
	/// </para>
	/// </summary>
	/// <seealso cref="CameraRigProvider"/>
	/// <seealso cref="CameraService"/>
	public interface ICameraProvider
	{
		/// <summary>
		/// Construct a camera controller.
		/// </summary>
		/// <returns>A camera controller to populate the <see cref="CameraService"/> with</returns>
		ICameraController Provide();
	}
}
