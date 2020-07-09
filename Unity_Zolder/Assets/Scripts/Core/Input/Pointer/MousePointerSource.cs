// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Cameras;
using UnityEngine;

namespace Talespin.Core.Foundation.InputHandling.Pointer
{
	/// <summary>
	/// Casts rays from the transform to where the mouse is pointing.
	/// </summary>
	public class MousePointerSource : PointerSourceInfo
	{
		/// <inheritdoc/>
		public override Transform Transform
		{
			get
			{
				InitializeRaycastSourceIfNull();
				return raycastSource.transform;
			}
		}

		[SerializeField] private Camera cameraRaycaster;

		private ICameraController cameraController;
		private Camera internalCamera;
		private GameObject raycastSource;

		public void InjectDependencies(ICameraController cameraController)
		{
			this.cameraController = cameraController;
		}

		protected void OnEnable()
		{
			internalCamera = cameraRaycaster != null ? cameraRaycaster : cameraController.MainCamera;

			InitializeRaycastSourceIfNull();
		}

		protected void OnDestroy()
		{
			DestroyRaycastSource();
		}

		protected void Update()
		{
			raycastSource.transform.position = internalCamera.ScreenToWorldPoint(Input.mousePosition);
			raycastSource.transform.forward = internalCamera.transform.forward;
		}

		private void InitializeRaycastSourceIfNull()
		{
			if (raycastSource == null)
			{
				raycastSource = new GameObject("MouseRaycastSource");
				DontDestroyOnLoad(raycastSource);
			}
		}

		private void DestroyRaycastSource()
		{
			Destroy(raycastSource);
		}
	}
}
