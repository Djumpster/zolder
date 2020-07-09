// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Extensions
{
	public static class Vector2Extensions
	{
		public static Vector2 TransformToBoundsSpace(this Vector2 orig, Rect bbx)
		{
			return new Vector2((orig.x - bbx.xMin) / bbx.width, (orig.y - bbx.yMin) / bbx.height);
		}

		public static Vector2 TransformFromBoundsSpace(this Vector2 orig, Rect bbx)
		{
			return new Vector2((orig.x * bbx.width + bbx.xMin), (orig.y * bbx.height + bbx.yMin));
		}

		public static bool IsNormalizable(this Vector2 orig)
		{
			bool isNull = (orig.x == 0f && orig.y == 0f);
			bool isNaN = (float.IsNaN(orig.x) || float.IsNaN(orig.y));
			return !isNull && !isNaN;
		}

		public static Vector3 ToVector3(this Vector2 orig, float z = 0f)
		{
			return new Vector3(orig.x, orig.y, z);
		}

		public static Vector2 InvertScreenY(this Vector2 orig)
		{
			orig.y = Screen.height - orig.y;
			return orig;
		}

		public static Vector2 SafeNormalize(this Vector2 orig)
		{
			return SafeNormalize(orig, Vector2.zero);
		}

		public static Vector2 SafeNormalize(this Vector2 orig, Vector2 returnOnError)
		{
			return orig.IsNormalizable() ? orig.normalized : returnOnError;
		}

		public static Vector2 FromAngle(this Vector2 orig, float radians)
		{
			orig.x = Mathf.Cos(radians);
			orig.y = Mathf.Sin(radians);
			return orig;
		}

		public static Vector2 Abs(this Vector2 orig)
		{
			return new Vector2(Mathf.Abs(orig.x), Mathf.Abs(orig.y));
		}
	}
}

#region SWIZZLE
namespace Talespin.Core.Swizzle
{
	public static class Vector2SwizzleExtensions
	{
		public static Vector2 xy(this Vector2 orig)
		{
			return new Vector2(orig.x, orig.y);
		}
		public static Vector2 xx(this Vector2 orig)
		{
			return new Vector2(orig.x, orig.x);
		}

		public static Vector2 xo(this Vector2 orig)
		{
			return new Vector2(orig.x, 0f);
		}
		public static Vector2 oy(this Vector2 orig)
		{
			return new Vector2(0f, orig.y);
		}

		public static Vector2 yo(this Vector2 orig)
		{
			return new Vector2(orig.y, 0f);
		}
		public static Vector2 ox(this Vector2 orig)
		{
			return new Vector2(0f, orig.x);
		}

		public static Vector2 yy(this Vector2 orig)
		{
			return new Vector2(orig.y, orig.y);
		}
		public static Vector2 yx(this Vector2 orig)
		{
			return new Vector2(orig.y, orig.x);
		}

		public static Vector3 xxx(this Vector2 orig)
		{
			return new Vector3(orig.x, orig.x, orig.x);
		}
		public static Vector3 xyy(this Vector2 orig)
		{
			return new Vector3(orig.x, orig.y, orig.y);
		}
		public static Vector3 xxy(this Vector2 orig)
		{
			return new Vector3(orig.x, orig.x, orig.y);
		}
		public static Vector3 xyx(this Vector2 orig)
		{
			return new Vector3(orig.x, orig.y, orig.x);
		}

		public static Vector3 yyy(this Vector2 orig)
		{
			return new Vector3(orig.y, orig.y, orig.y);
		}
		public static Vector3 yxx(this Vector2 orig)
		{
			return new Vector3(orig.y, orig.x, orig.x);
		}
		public static Vector3 yxy(this Vector2 orig)
		{
			return new Vector3(orig.y, orig.x, orig.y);
		}
		public static Vector3 yyx(this Vector2 orig)
		{
			return new Vector3(orig.y, orig.y, orig.x);
		}

		public static Vector3 xyo(this Vector2 orig)
		{
			return new Vector3(orig.x, orig.y, 0f);
		}
	}
}
#endregion