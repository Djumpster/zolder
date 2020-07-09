// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Extensions
{
	public static class PlaneExtensions
	{
		public static Vector3 ProjectPoint(this Plane plane, Vector3 point)
		{
			float u = -(point.x * plane.normal.x + point.y * plane.normal.y + point.z * plane.normal.z + plane.distance) /
				(plane.normal.x * plane.normal.x + plane.normal.y * plane.normal.y + plane.normal.z * plane.normal.z);
			return point + plane.normal * u;
		}

		public static Vector3 ProjectDirection(this Plane p, Vector3 direction)
		{
			return direction - Vector3.Project(direction, p.normal);
		}
	}
}
