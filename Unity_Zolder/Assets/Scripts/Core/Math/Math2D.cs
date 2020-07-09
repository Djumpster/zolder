// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Maths
{
	public static class Math2D
	{
		#region Cicle
		public struct Circle
		{
			public readonly Vector2 center;
			public readonly float radius;

			public float Circumferance
			{
				get { return Mathf.PI * 2f * radius; }
			}

			public Circle(Vector2 center, float radius)
			{
				this.center = center;
				this.radius = radius;
			}
		}

		public static Vector2 PointOnCircle(Circle circle, float point)
		{
			return circle.center + Vector2.up.Rotate(point * 360f) * circle.radius;
		}

		public static Vector2 NormalOfCircle(Circle circle, float point)
		{
			return Vector2.up.Rotate(point * 360f);
		}

		public static Vector2 TangentOfCircle(Circle circle, float point)
		{
			return NormalOfCircle(circle, point).Right();
		}
		#endregion

		#region Ellipse
		public struct Ellipse
		{
			public readonly Vector2 center;
			public readonly float radiusA,
									radiusB,
									angle;

			public float Circumferance
			{
				get { return Mathf.PI * 2f * Mathf.Sqrt((radiusA * radiusA + radiusB * radiusB) / 2f); }
			}

			public Ellipse(Vector2 center, float radiusA, float radiusB, float angle = 0f)
			{
				this.center = center;
				this.radiusA = radiusA;
				this.radiusB = radiusB;
				this.angle = angle;
			}

			public Ellipse SetCenter(Vector2 center)
			{
				return new Ellipse(center, radiusA, radiusB, angle);
			}
		}

		public static Vector2 PointOnEllipse(Ellipse ellipse, float point)
		{
			point %= 1f;
			float radA = point * Mathf.PI * 2f,
					sinA = (float)System.Math.Sin(radA),
					cosA = (float)System.Math.Cos(radA),
					radB = -ellipse.angle * Mathf.PI * 2f,
					sinB = (float)System.Math.Sin(radB),
					cosB = (float)System.Math.Cos(radB),
					x = ellipse.center.x + (ellipse.radiusA * cosA * cosB - ellipse.radiusB * sinA * sinB),
					y = ellipse.center.y + (ellipse.radiusA * cosA * sinB + ellipse.radiusB * sinA * cosB);
			return new Vector2(x, y);
		}

		public static Vector2 NormalOfEllipse(Ellipse ellipse, float point)
		{
			float rad = (point % 1f) * Mathf.PI * 2f;
			float y = ellipse.radiusA * Mathf.Sin(rad);
			float x = ellipse.radiusB * Mathf.Cos(rad);
			return new Vector2(x, y).normalized.Rotate(-ellipse.angle * 360f);
		}

		public static Vector2 TangentOfEllipse(Ellipse ellipse, float point)
		{
			return NormalOfEllipse(ellipse, point).Right();
		}
		#endregion

		public static float GetSignedAngle(Vector2 a, Vector2 b)
		{
			float fullAngle = GetFullAngle(a, b);
			return (fullAngle > 180f) ? (fullAngle - 360f) : fullAngle;
		}

		public static float GetFullAngle(Vector2 a, Vector2 b)
		{
			//			a = a.normalized;
			//			b = b.normalized;
			float angle = Vector2.Angle(a, b);
			Vector2 reference = a.Right(); //A vector perpendicular to vector a.
			return (Vector2.Dot(b, reference) > 0f) ? (360f - angle) : angle;
		}

		public static Vector2 Project(this Vector2 vector, Vector2 onNormal)
		{
			float num = Vector2.Dot(onNormal, onNormal);
			if (num < 1.401298E-45f)
			{
				return Vector2.zero;
			}

			return onNormal * Vector2.Dot(vector, onNormal) / num;
		}

		public static Vector2 Rotate(this Vector2 vector, float angle)
		{
			float rad = angle * Mathf.Deg2Rad,
					cs = (float)System.Math.Cos(rad),
					sn = (float)System.Math.Sin(rad);
			return new Vector2(vector.x * cs - vector.y * sn, vector.x * sn + vector.y * cs);
		}

		public static Vector2 Right(this Vector2 vector)
		{
			return new Vector2(vector.y, -vector.x);
		}
	}
}