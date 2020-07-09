// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Extensions
{
	public static class Vector4Extensions
	{
		public static Vector4 NormalizeAsVector3(this Vector4 orig)
		{
			Vector3 v = orig;
			v.Normalize();
			return new Vector4(v.x, v.y, v.z, orig.w);
		}
	}
}
