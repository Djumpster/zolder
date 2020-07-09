// Copyright 2020 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Cameras
{
	/// <summary>
	/// Interface to allow for customization of the tracked rig being spawned.
	/// If this interface is not implemented it will use a default behavior.
	/// </summary>
	/// <see cref="CameraRigProvider"/>
	/// <see cref="CameraRigProvider.GetRigTypeBuiltIn"/>
	public interface ICameraRigTypeProvider
	{
		/// <summary>
		/// Get the desired tracked rig type to spawn.
		/// </summary>
		/// <returns>The rig type to spawn</returns>
		CameraRigType GetRigType();
	}
}
