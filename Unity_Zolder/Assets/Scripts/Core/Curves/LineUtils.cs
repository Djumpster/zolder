// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Swizzle;
using UnityEngine;

namespace Talespin.Core.Foundation.Curves
{
	public static class LineUtils
	{
		public struct Line
		{
			public readonly Vector3 origin;
			public readonly Vector3 destination;

			public Vector3 GetCenter()
			{
				return origin + GetDirection() * (GetLength() / 2);
			}

			public float GetLength()
			{
				return Vector3.Distance(origin, destination);
			}

			public float GetSqrLength()
			{
				return Vector3.SqrMagnitude(origin - destination);
			}

			public Vector3 GetDirection()
			{
				return (destination - origin).normalized;
			}

			public Line Inverse()
			{
				return new Line(destination, origin);
			}

			public Line(Vector3 origin, Vector3 destination)
			{
				this.origin = origin;
				this.destination = destination;
			}

			public Vector3 ClosestPoint(Vector3 v, out float sqrDistance)
			{
				float time = -Vector3.Dot(origin - v, destination - origin) / (destination - origin).sqrMagnitude;
				Vector3 point = origin + (destination - origin) * Mathf.Clamp01(time);
				sqrDistance = (point - v).sqrMagnitude;
				return point;
			}

			public float SquareDistanceTo(Vector3 point)
			{
				float sqrDist;
				ClosestPoint(point, out sqrDist);
				return sqrDist;
			}

			public Line? Project(Line l, bool onlyAllowSameDirection = false)
			{
				if (onlyAllowSameDirection && Vector3.Dot(GetDirection(), l.GetDirection()) < 0)
				{
					return null;
				}
				float timeA = -Vector3.Dot(origin - l.origin, destination - origin) / (destination - origin).sqrMagnitude;
				float timeB = -Vector3.Dot(origin - l.destination, destination - origin) / (destination - origin).sqrMagnitude;
				if ((timeA < 0f && timeB < 0f) || (timeA > 1f && timeB > 1f))
				{
					return null;
				}
				Vector3 pointA = origin + (destination - origin) * Mathf.Clamp01(timeA);
				Vector3 pointB = origin + (destination - origin) * Mathf.Clamp01(timeB);

				Line ret = new Line(pointA, pointB);
				return ret;
			}
		}

		public static Vector2? FindIntersection(this Line lineA, Line lineB)
		{
			float d = (lineA.origin.x - lineA.destination.x) * (lineB.origin.y - lineB.destination.y) - (lineA.origin.y - lineA.destination.y) * (lineB.origin.x - lineB.destination.x);
			if (d == 0)
			{
				return null;
			}
			float x = ((lineB.origin.x - lineB.destination.x) * (lineA.origin.x * lineA.destination.y - lineA.origin.y * lineA.destination.x) -
						 (lineA.origin.x - lineA.destination.x) * (lineB.origin.x * lineB.destination.y - lineB.origin.y * lineB.destination.x)) / d;

			float y = ((lineB.origin.y - lineB.destination.y) * (lineA.origin.x * lineA.destination.y - lineA.origin.y * lineA.destination.x) -
						 (lineA.origin.y - lineA.destination.y) * (lineB.origin.x * lineB.destination.y - lineB.origin.y * lineB.destination.x)) / d;

			Vector2 intersectPoint = new Vector2(x, y);
			float sqrToCenterA = Vector2.SqrMagnitude(intersectPoint - lineA.GetCenter().xy()),
					halfLengthA = lineA.GetLength() / 2;
			if (sqrToCenterA > halfLengthA * halfLengthA)
			{
				return null;
			}

			float sqrToCenterB = Vector2.SqrMagnitude(intersectPoint - lineB.GetCenter().xy()),
					halfLengthB = lineB.GetLength() / 2;
			if (sqrToCenterB > halfLengthB * halfLengthB)
			{
				return null;
			}

			return intersectPoint;
		}

		public static Line ClampIntersection(this Line line, Line clampAgainst)
		{
			Vector2? intersection = FindIntersection(line, clampAgainst);
			if (intersection == null)
			{
				return line;
			}

			return new Line(line.origin, intersection.Value);
		}

		static Vector2 MinimumIntersects(Vector2 org, params Vector2?[] vecs)
		{
			float min = Mathf.Infinity;
			Vector2 ret = org;
			foreach (Vector2? v in vecs)
			{
				if (v != null)
				{
					float dist = (v.Value - org).sqrMagnitude;
					if (dist < min)
					{
						min = dist;
						ret = v.Value;
					}
				}
			}
			return ret;
		}

		public static Line? ClampInsideRect(this Line line, Rect rect)
		{
			Plane a = new Plane(new Vector2(1, 0), new Vector2(rect.xMin, rect.center.y));
			Plane b = new Plane(new Vector2(-1, 0), new Vector2(rect.xMax, rect.center.y));
			Plane c = new Plane(new Vector2(0, 1), new Vector2(rect.center.x, rect.yMin));
			Plane d = new Plane(new Vector2(0, -1), new Vector2(rect.center.x, rect.yMax));

			if (!a.GetSide(line.origin) && !a.GetSide(line.destination))
			{
				return null;
			}

			if (!b.GetSide(line.origin) && !b.GetSide(line.destination))
			{
				return null;
			}

			if (!c.GetSide(line.origin) && !c.GetSide(line.destination))
			{
				return null;
			}

			if (!d.GetSide(line.origin) && !d.GetSide(line.destination))
			{
				return null;
			}

			if (!rect.Contains(line.origin))
			{
				Vector2? intersectA = line.FindIntersection(new Line(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y)));
				Vector2? intersectB = line.FindIntersection(new Line(new Vector2(rect.x + rect.width, rect.y), new Vector2(rect.x + rect.width, rect.y + rect.height)));
				Vector2? intersectC = line.FindIntersection(new Line(new Vector2(rect.x + rect.width, rect.y + rect.height), new Vector2(rect.x, rect.y + rect.height)));
				Vector2? intersectD = line.FindIntersection(new Line(new Vector2(rect.x, rect.y + rect.height), new Vector2(rect.x, rect.y)));
				line = new Line(MinimumIntersects(line.origin, intersectA, intersectB, intersectC, intersectD), line.destination);
				line = new Line(line.origin + (line.destination - line.origin).normalized, line.destination);
			}

			line = line.ClampIntersection(new Line(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y)));
			line = line.ClampIntersection(new Line(new Vector2(rect.x + rect.width, rect.y), new Vector2(rect.x + rect.width, rect.y + rect.height)));
			line = line.ClampIntersection(new Line(new Vector2(rect.x + rect.width, rect.y + rect.height), new Vector2(rect.x, rect.y + rect.height)));
			line = line.ClampIntersection(new Line(new Vector2(rect.x, rect.y + rect.height), new Vector2(rect.x, rect.y)));
			return line;
		}
	}
}
