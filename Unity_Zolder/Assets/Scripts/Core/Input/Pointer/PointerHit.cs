// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.InputHandling
{
	/// <summary>
	/// Contains all data related to a pointer hit.
	/// 
	/// Note that not everything has a value, depending 
	/// on the specific pointer implementation.
	/// </summary>
	public struct PointerHit
	{
		public bool IsValid => GameObject != null && Normal != Vector3.zero;

		public GameObject GameObject;
		public Vector3 Point;
		public Vector3 Normal;
		public float Distance;
	}
}
