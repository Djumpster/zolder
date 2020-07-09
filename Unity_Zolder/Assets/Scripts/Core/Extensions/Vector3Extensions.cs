// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Extensions
{
	public static class Vector3Extensions
	{
		public static Vector3 TransformToBoundsSpace(this Vector3 orig, Bounds bbx)
		{
			return new Vector3((orig.x - bbx.min.x) / bbx.size.x, (orig.y - bbx.min.y) / bbx.size.y, (orig.z - bbx.min.z) / bbx.size.z);
		}

		public static Vector3 TransformFromBoundsSpace(this Vector3 orig, Bounds bbx)
		{
			return new Vector3((orig.x * bbx.size.x + bbx.min.x), (orig.y * bbx.size.y + bbx.min.y), (orig.z * bbx.size.z + bbx.min.z));
		}

		public static bool IsNormalizable(this Vector3 orig)
		{
			bool isNull = (orig.x == 0f && orig.y == 0f && orig.z == 0f);
			bool isNaN = (float.IsNaN(orig.x) || float.IsNaN(orig.y) || float.IsNaN(orig.z));
			return !isNull && !isNaN;
		}

		public static float Dot(this Vector3 orig, Vector3 rhs)
		{
			return Vector3.Dot(orig, rhs);
		}

		public static Vector3 FlattenX(this Vector3 orig)
		{
			return new Vector3(0f, orig.y, orig.z);
		}

		public static Vector3 FlattenY(this Vector3 orig)
		{
			return new Vector3(orig.x, 0f, orig.z);
		}

		public static Vector3 FlattenZ(this Vector3 orig)
		{
			return new Vector3(orig.x, orig.y, 0f);
		}

		public static Vector2 ToVector2(this Vector3 orig)
		{
			return new Vector2(orig.x, orig.y);
		}

		public static Vector3 SafeNormalize(this Vector3 orig)
		{
			return SafeNormalize(orig, Vector3.zero);
		}

		public static Vector3 Abs(this Vector3 orig)
		{
			return new Vector3(Mathf.Abs(orig.x), Mathf.Abs(orig.y), Mathf.Abs(orig.z));
		}

		public static Vector3 SafeNormalize(this Vector3 orig, Vector3 returnOnError)
		{
			return orig.IsNormalizable() ? orig.normalized : returnOnError;
		}

		// note: forward, up (and right) is just some vector that happens to match the Unity Vector3.up when forward is roughly in the x,z plane!
		// while the algorithm does try, there is no guarantee at all that up has a positive, major y-component
		public static void ComputeOrthonormalBasis(this Vector3 orig, out Vector3 up, out Vector3 right)
		{
			Vector3 helper = -Vector3.up;
			if (Mathf.Abs(orig.x) < 0.01f && Mathf.Abs(orig.z) < 0.01f)
			{
				helper = -Vector3.forward;
			}

			right = Vector3.Cross(orig, helper);
			up = Vector3.Cross(orig, right);
		}

		public static Vector3 MirrorOntoPlaneAsPoint(this Vector3 orig, Plane p)
		{
			Matrix4x4 m = new Matrix4x4();
			m.SetMirrorMatrix(p);
			return m.MultiplyPoint(orig);
		}

		public static Vector3 MirrorOntoPlaneAsDirection(this Vector3 orig, Plane p)
		{
			Matrix4x4 m = new Matrix4x4();
			m.SetMirrorMatrix(p);
			return m.MultiplyVector(orig);
		}

		public static Vector3 InvertScreenY(this Vector3 orig)
		{
			orig.y = Screen.height - orig.y;
			return orig;
		}

		public static Vector3 FromAngle(this Vector3 orig, float radians)
		{
			orig.x = Mathf.Cos(radians);
			orig.z = Mathf.Sin(radians);
			return orig;
		}

		public static Quaternion Rotation(this Vector3 orig)
		{
			return Quaternion.Euler(orig);
		}

		//returns -1 when to the left, 1 to the right, and 0 for forward/backward
		public static float AngleDirection(this Vector3 orig, Vector3 forward, Vector3 up)
		{
			Vector3 perp = Vector3.Cross(forward, orig);
			float dir = Vector3.Dot(perp, up);

			if (dir > 0.0f)
			{
				return 1.0f;
			}
			else if (dir < 0.0f)
			{
				return -1.0f;
			}
			else
			{
				return 0.0f;
			}
		}
	}
}

namespace Talespin.Core.Swizzle
{
	public static class Vector3SwizzleExtensions
	{
		public static Vector2 xy(this Vector3 orig)
		{
			return new Vector2(orig.x, orig.y);
		}
		public static Vector2 xz(this Vector3 orig)
		{
			return new Vector2(orig.x, orig.z);
		}
		public static Vector2 yz(this Vector3 orig)
		{
			return new Vector2(orig.y, orig.z);
		}
		public static Vector2 xx(this Vector3 orig)
		{
			return new Vector2(orig.x, orig.x);
		}
		public static Vector2 yy(this Vector3 orig)
		{
			return new Vector2(orig.y, orig.y);
		}
		public static Vector2 zz(this Vector3 orig)
		{
			return new Vector2(orig.z, orig.z);
		}

		public static Vector3 xxx(this Vector3 orig)
		{
			return new Vector3(orig.x, orig.x, orig.x);
		}
		public static Vector3 xxy(this Vector3 orig)
		{
			return new Vector3(orig.x, orig.x, orig.y);
		}
		public static Vector3 xyx(this Vector3 orig)
		{
			return new Vector3(orig.x, orig.y, orig.x);
		}
		public static Vector3 xyy(this Vector3 orig)
		{
			return new Vector3(orig.x, orig.y, orig.y);
		}
		public static Vector3 xxz(this Vector3 orig)
		{
			return new Vector3(orig.x, orig.x, orig.z);
		}
		public static Vector3 xzx(this Vector3 orig)
		{
			return new Vector3(orig.x, orig.z, orig.x);
		}
		public static Vector3 xzz(this Vector3 orig)
		{
			return new Vector3(orig.x, orig.z, orig.z);
		}
		public static Vector3 xyz(this Vector3 orig)
		{
			return new Vector3(orig.x, orig.y, orig.z);
		}
		public static Vector3 xzy(this Vector3 orig)
		{
			return new Vector3(orig.x, orig.z, orig.y);
		}

		public static Vector3 yxx(this Vector3 orig)
		{
			return new Vector3(orig.y, orig.x, orig.x);
		}
		public static Vector3 yxy(this Vector3 orig)
		{
			return new Vector3(orig.y, orig.x, orig.y);
		}
		public static Vector3 yyx(this Vector3 orig)
		{
			return new Vector3(orig.y, orig.y, orig.x);
		}
		public static Vector3 yyy(this Vector3 orig)
		{
			return new Vector3(orig.y, orig.y, orig.y);
		}
		public static Vector3 yxz(this Vector3 orig)
		{
			return new Vector3(orig.y, orig.x, orig.z);
		}
		public static Vector3 yzx(this Vector3 orig)
		{
			return new Vector3(orig.y, orig.z, orig.x);
		}
		public static Vector3 yzz(this Vector3 orig)
		{
			return new Vector3(orig.y, orig.z, orig.z);
		}
		public static Vector3 yyz(this Vector3 orig)
		{
			return new Vector3(orig.y, orig.y, orig.z);
		}
		public static Vector3 yzy(this Vector3 orig)
		{
			return new Vector3(orig.y, orig.z, orig.y);
		}

		public static Vector3 zxx(this Vector3 orig)
		{
			return new Vector3(orig.z, orig.x, orig.x);
		}
		public static Vector3 zxy(this Vector3 orig)
		{
			return new Vector3(orig.z, orig.x, orig.y);
		}
		public static Vector3 zyx(this Vector3 orig)
		{
			return new Vector3(orig.z, orig.y, orig.x);
		}
		public static Vector3 zyy(this Vector3 orig)
		{
			return new Vector3(orig.z, orig.y, orig.y);
		}
		public static Vector3 zxz(this Vector3 orig)
		{
			return new Vector3(orig.z, orig.x, orig.z);
		}
		public static Vector3 zzx(this Vector3 orig)
		{
			return new Vector3(orig.z, orig.z, orig.x);
		}
		public static Vector3 zzz(this Vector3 orig)
		{
			return new Vector3(orig.z, orig.z, orig.z);
		}
		public static Vector3 zyz(this Vector3 orig)
		{
			return new Vector3(orig.z, orig.y, orig.z);
		}
		public static Vector3 zzy(this Vector3 orig)
		{
			return new Vector3(orig.z, orig.z, orig.y);
		}

		public static Vector3 xoz(this Vector3 orig)
		{
			return new Vector3(orig.x, 0, orig.z);
		}

		public static Vector3 oyo(this Vector3 orig)
		{
			return new Vector3(0, orig.y, 0);
		}

		public static Vector3 oyz(this Vector3 orig)
		{
			return new Vector3(0, orig.y, orig.z);
		}

		public static Vector3 xyo(this Vector3 orig)
		{
			return new Vector3(orig.x, orig.y, 0);
		}

		public static Vector3 x0z(this Vector3 orig)
		{
			return new Vector3(orig.x, 0f, orig.z);
		}

		public static Vector3 x0y(this Vector3 orig)
		{
			return new Vector3(orig.x, 0f, orig.y);
		}

		public static Vector4 xyz1(this Vector3 orig)
		{
			return new Vector4(orig.x, orig.y, orig.z, 1);
		}

		public static Vector4 xyz0(this Vector3 orig)
		{
			return new Vector4(orig.x, orig.y, orig.z, 0);
		}
	}
}