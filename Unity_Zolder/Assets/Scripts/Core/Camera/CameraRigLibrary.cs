// Copyright 2020 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Extensions;
using UnityEngine;

namespace Talespin.Core.Foundation.Cameras
{
	/// <summary>
	/// The camera rig library is used as the configuration object
	/// when using the default <see cref="CameraRigProvider"/>.
	/// <para>
	/// This contains a list of all available camera rig types and
	/// their associated prefabs. Note that the prefabs must contain
	/// an implementation of <see cref="ICameraController"/>.
	/// </para>
	/// </summary>
	/// <remarks>
	/// This library is not used when creating a custom implementation of
	/// <see cref="ICameraProvider"/>. Though it can be configured to
	/// work with custom implementations as well.
	/// </remarks>
	public class CameraRigLibrary : ScriptableObject
	{
		[SerializeField] private CameraRigConfiguration[] configurations;

		/// <summary>
		/// Get the configuration with the specified type.
		/// </summary>
		/// <param name="rigType">The type of the camera rig to retrieve</param>
		/// <returns>The prefab of the camera rig, or <see langword="null"/>
		/// if it hasn't been configured</returns>
		public CameraRigConfiguration GetConfiguration(CameraRigType rigType)
		{
			foreach (CameraRigConfiguration configuration in configurations)
			{
				if (configuration.RigType == rigType)
				{
					return configuration;
				}
			}

			Debug.LogWarning($"Could not find TrackedRigConfiguration for rig type: {rigType}");
			return null;
		}
	}
}
