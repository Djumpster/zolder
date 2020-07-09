// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using UnityEngine;

namespace Talespin.Core.Foundation.Extensions
{
	public static class MeshFilterExtensions
	{
		public struct MeshHit
		{
			public readonly Vector3 position;
			public readonly MeshFilter filter;
			public readonly float distance;
			public MeshHit(Vector3 position, MeshFilter filter, float distance)
			{
				this.position = position;
				this.filter = filter;
				this.distance = distance;
			}
		}

		public static Rect ScreenRect(this MeshFilter orig, Camera cam)
		{
			Bounds b = orig.mesh.bounds;
			Transform trans = orig.transform;
			return b.ScreenRect(trans, cam);
		}

		public static bool RayCast(this MeshFilter orig, Ray worldRay, out MeshHit hit)
		{
			hit = new MeshHit();
			if (orig.sharedMesh == null)
			{
				return false;
			}

			Transform t = orig.transform;
			Bounds b = orig.sharedMesh.bounds;

			Ray local = new Ray
			(
				t.InverseTransformPoint(worldRay.origin),
				t.InverseTransformDirection(worldRay.direction)
			);

			if (b.IntersectRay(local))
			{
				hit = new MeshHit(Vector3.zero, orig, (local.origin - b.center).magnitude);
				return true;
			}
			return false;
		}

		public static bool MultiCast(this IEnumerable<MeshFilter> orig, Ray worldRay, out MeshHit hit)
		{
			hit = new MeshHit(Vector3.zero, null, Mathf.Infinity);
			bool ret = false;
			foreach (MeshFilter mf in orig)
			{
				MeshHit h;
				if (mf.RayCast(worldRay, out h))
				{
					ret = true;
					if (h.distance < hit.distance || hit.filter == null)
					{
						hit = h;
					}
				}
			}
			return ret;
		}
	}
}
