// Copyright 2019 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections;
using UnityEngine;

namespace Talespin.Core.Foundation.SpawnPoints
{
	/// <summary>
	/// Manages the position and rotation  of a transform. In the editor this will
	/// update in real-time.
	/// </summary>
	public class SpawnPointHelper : MonoBehaviour
	{
		public Func<Vector3, Vector3> OverridePositionFunc { set; get; }
		public Func<Quaternion, Quaternion> OverrideRotationFunc { set; get; }

		private Transform origin;
		private Transform spawnPoint;

		private bool managePosition;
		private bool manageRotation;

		public void Initialize(Transform origin, Transform spawnPoint, bool managePosition, bool manageRotation)
		{
			this.origin = origin;
			this.spawnPoint = spawnPoint;
			this.managePosition = managePosition;
			this.manageRotation = manageRotation;
		}

		protected IEnumerator Start()
		{
			UpdateTransform();

			// Hack to fix the collider(s) not moving?
			Collider[] colliders = GetComponentsInChildren<Collider>();
			bool[] wasEnabled = new bool[colliders.Length];

			for (int i = 0; i < colliders.Length; i++)
			{
				wasEnabled[i] = colliders[i].enabled;
				colliders[i].enabled = false;
			}

			yield return null;

			for (int i = 0; i < colliders.Length; i++)
			{
				colliders[i].enabled = wasEnabled[i];
			}
		}

#if UNITY_EDITOR
		protected void LateUpdate()
		{
			UpdateTransform();
		}
#endif

		private void UpdateTransform()
		{
			if (!origin || !spawnPoint)
			{
				Destroy(this);
				return;
			}

			if (managePosition)
			{
				if (OverridePositionFunc != null)
				{
					origin.position = OverridePositionFunc.Invoke(spawnPoint.position);
				}
				else
				{
					origin.position = spawnPoint.position;
				}
			}

			if (manageRotation)
			{
				if (OverrideRotationFunc != null)
				{
					origin.rotation = OverrideRotationFunc.Invoke(spawnPoint.rotation);
				}
				else
				{
					origin.rotation = spawnPoint.rotation;
				}
			}
		}
	}
}
