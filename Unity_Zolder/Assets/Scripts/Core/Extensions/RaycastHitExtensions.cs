// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Extensions
{
	public static class RaycastHitExtensions
	{
		public static Mesh GetMesh(this RaycastHit orig)
		{
			MeshCollider mc = orig.collider as MeshCollider;
			return mc?.sharedMesh;
		}
	}
}
