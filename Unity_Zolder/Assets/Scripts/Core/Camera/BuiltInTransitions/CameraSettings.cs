// Copyright 2020 Talespin Reality Inc. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Cameras.Transition
{
	/// <summary>
	/// A container class for specific camera settings, differentiated between shared settings and specifics for perspective and orthographic projections.
	/// </summary>
	public class CameraSettings
	{
		/// <seealso cref="Camera.clearFlags"/>
		public CameraClearFlags CameraClearFlags { get; }

		/// <seealso cref="Camera.backgroundColor"/>
		public Color BackgroundColor { get; }

		/// <seealso cref="Camera.cullingMask"/>
		public int CullingMask { get; }

		/// <seealso cref="Camera.nearClipPlane"/>
		public float NearClipPlane { get; }

		/// <seealso cref="Camera.farClipPlane"/>
		public float FarClipPlane { get; }

		/// <seealso cref="Camera.depth"/>
		public float Depth { get; }

		public CameraSettings(Camera camera)
		{
			CameraClearFlags = camera.clearFlags;
			BackgroundColor = camera.backgroundColor;
			CullingMask = camera.cullingMask;
			NearClipPlane = camera.nearClipPlane;
			FarClipPlane = camera.farClipPlane;
			Depth = camera.depth;
		}
	}

	/// <summary>
	/// Extension for perspective camera settings.
	/// </summary>
	public class PerspectiveCameraSettings : CameraSettings
	{
		public float FieldOfView { get; }

		public PerspectiveCameraSettings(Camera camera) : base(camera)
		{
			FieldOfView = camera.fieldOfView;
		}
	}

	/// <summary>
	/// Extension for orthographic camera settings.
	/// </summary>
	public class OrthographicCameraSettings : CameraSettings
	{
		public float OrthographicSize { get; }

		public OrthographicCameraSettings(Camera camera) : base(camera)
		{
			OrthographicSize = camera.orthographicSize;
		}
	}
}
