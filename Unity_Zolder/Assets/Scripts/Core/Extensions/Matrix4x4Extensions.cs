// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Extensions
{
	public static class Matrix4x4Extensions
	{
		public static Matrix4x4 SetPerspectiveOffCenter(this Matrix4x4 m, float left, float right, float bottom, float top, float near, float far)
		{
			float x = (2f * near) / (right - left);
			float y = (2f * near) / (top - bottom);
			float a = (right + left) / (right - left);
			float b = (top + bottom) / (top - bottom);
			float c = -(far + near) / (far - near);
			float d = -(2f * far * near) / (far - near);
			float e = -1f;

			m[0, 0] = x; m[0, 1] = 0; m[0, 2] = a; m[0, 3] = 0;
			m[1, 0] = 0; m[1, 1] = y; m[1, 2] = b; m[1, 3] = 0;
			m[2, 0] = 0; m[2, 1] = 0; m[2, 2] = c; m[2, 3] = d;
			m[3, 0] = 0; m[3, 1] = 0; m[3, 2] = e; m[3, 3] = 0;
			return m;
		}


		public static Matrix4x4 SetMirrorMatrix(this Matrix4x4 mat, Plane p)
		{
			// OpenGL matrix from: http://knol.google.com/k/mirroring-a-point-on-a-3d-plane#
			Vector3 n = p.normal.normalized;
			float k = -p.distance;
			mat.SetRow(0, new Vector4(1f - (2f * n.x * n.x), -2f * n.x * n.y, -2f * n.x * n.z, 2f * n.x * k));
			mat.SetRow(1, new Vector4(-2f * n.y * n.x, 1f - (2f * n.y * n.y), -2f * n.y * n.z, 2f * n.y * k));
			mat.SetRow(2, new Vector4(-2f * n.z * n.x, -2f * n.z * n.y, 1f - (2f * n.z * n.z), 2f * n.z * k));
			mat.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
			return mat;
		}

		public static Quaternion GetRotation(this Matrix4x4 matrix)
		{
			Quaternion q = new Quaternion
			{
				w = Mathf.Sqrt(Mathf.Max(0, 1 + matrix.m00 + matrix.m11 + matrix.m22)) / 2,
				x = Mathf.Sqrt(Mathf.Max(0, 1 + matrix.m00 - matrix.m11 - matrix.m22)) / 2,
				y = Mathf.Sqrt(Mathf.Max(0, 1 - matrix.m00 + matrix.m11 - matrix.m22)) / 2,
				z = Mathf.Sqrt(Mathf.Max(0, 1 - matrix.m00 - matrix.m11 + matrix.m22)) / 2
			};

			q.x = Mathf.Sign(q.x) > 0 ? Mathf.Abs(matrix.m21 - matrix.m12) : -Mathf.Abs(matrix.m21 - matrix.m12);
			q.y = Mathf.Sign(q.y) > 0 ? Mathf.Abs(matrix.m02 - matrix.m20) : -Mathf.Abs(matrix.m02 - matrix.m20);
			q.z = Mathf.Sign(q.z) > 0 ? Mathf.Abs(matrix.m10 - matrix.m01) : -Mathf.Abs(matrix.m10 - matrix.m01);
			return q;
		}

		public static Vector3 GetPosition(this Matrix4x4 matrix)
		{
			var x = matrix.m03;
			var y = matrix.m13;
			var z = matrix.m23;
			return new Vector3(x, y, z);
		}

		public static Vector3 GetScale(this Matrix4x4 m)
		{
			var x = Mathf.Sqrt(m.m00 * m.m00 + m.m01 * m.m01 + m.m02 * m.m02);
			var y = Mathf.Sqrt(m.m10 * m.m10 + m.m11 * m.m11 + m.m12 * m.m12);
			var z = Mathf.Sqrt(m.m20 * m.m20 + m.m21 * m.m21 + m.m22 * m.m22);
			return new Vector3(x, y, z);
		}
	}
}
