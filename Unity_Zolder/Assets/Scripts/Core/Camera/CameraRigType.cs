// Copyright 2020 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Cameras
{
	/// <summary>
	/// All available camera rig types supported by the
	/// Talespin Core library.
	/// </summary>
	public enum CameraRigType
	{
		/// <summary>
		/// A default value of None
		/// </summary>
		None = 0,

		/// <summary>
		/// The Oculus Rift (includes the Rift S)
		/// </summary>
		OculusRift = 1,

		/// <summary>
		/// The Oculus Quest
		/// </summary>
		OculusQuest = 2,

		/// <summary>
		/// The Oculus GO
		/// </summary>
		OculusGo = 3,

		/// <summary>
		/// The Samsung GearVR
		/// </summary>
		GearVR = 4,

		/// <summary>
		/// The HTC Vive (includes the Vive Pro)
		/// </summary>
		HtcVive = 5,

		/// <summary>
		/// A mobile platform without VR capabilities,
		/// i.e. Android or iOS.
		/// </summary>
		Mobile = 6,

		/// <summary>
		/// A desktop platform without VR capabilities,
		/// i.e. Windows or MacOS.
		/// </summary>
		Desktop = 7
	}
}
