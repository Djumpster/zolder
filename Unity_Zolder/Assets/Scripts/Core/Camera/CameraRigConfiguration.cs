// Copyright 2020 Talespin, LLC. All Rights Reserved.

using System;
using Talespin.Core.Foundation.AssetHandling;
using Talespin.Core.Foundation.Extensions;
using UnityEngine;

namespace Talespin.Core.Foundation.Cameras
{
	/// <summary>
	/// The serialized configuration for a camera rig.
	/// </summary>
	[Serializable]
	public class CameraRigConfiguration
	{
		/// <summary>
		/// The type of the camera rig.
		/// </summary>
		public CameraRigType RigType => rigType;

		/// <summary>
		/// The assigned prefab.
		/// This will throw an exception if the prefab cannot be loaded.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the prefab cannot be loaded</exception>
		public GameObject RigPrefab => rigPrefab.LoadGuid<GameObject>();

		[SerializeField] private CameraRigType rigType;
		[SerializeField, GuidResource(typeof(GameObject))] private string rigPrefab;
	}
}
