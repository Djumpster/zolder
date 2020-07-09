// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Extensions
{
	public static class RectExtensions
	{
		public static Rect ScaleToFit(this Rect rect, float targetAspect)
		{
			float currentAspect = rect.width / rect.height;
			if (currentAspect > targetAspect)
			{
				float aspectFrac = targetAspect / currentAspect;
				return new Rect(rect.xMin + rect.width * (1f - aspectFrac) * .5f, rect.yMin, aspectFrac * rect.width, rect.height);
			}
			else
			{
				float aspectFrac = currentAspect / targetAspect;
				return new Rect(rect.xMin, rect.yMin + rect.height * (1f - aspectFrac) * .5f, rect.width, aspectFrac * rect.height);
			}
		}

		public static Rect? Union(this Rect orig, params Rect[] rects)
		{
			foreach (Rect r in rects)
			{
				if (r.xMin < orig.xMax && r.xMax > orig.xMin && r.yMin < orig.yMax && r.yMax > orig.yMin)
				{
					if (r.xMin > orig.xMin && r.xMin < orig.xMax) { orig.xMin = r.xMin; }
					if (r.xMax > orig.xMin && r.xMax < orig.xMax) { orig.xMax = r.xMax; }
					if (r.yMin > orig.yMin && r.yMin < orig.yMax) { orig.yMin = r.yMin; }
					if (r.yMax > orig.yMin && r.yMax < orig.yMax) { orig.yMax = r.yMax; }
				}
				else
				{
					return null;
				}
			}
			return orig;
		}

		public static Rect? Intersect(this Rect orig, Rect otherRect)
		{
			if (!orig.Overlaps(otherRect))
			{
				return null;
			}

			float xMin = Mathf.Max(orig.xMin, otherRect.xMin);
			float xMax = Mathf.Min(orig.xMax, otherRect.xMax);
			float yMin = Mathf.Max(orig.yMin, otherRect.yMin);
			float yMax = Mathf.Min(orig.yMax, otherRect.yMax);

			Rect intersect = new Rect();
			intersect.xMin = xMin;
			intersect.xMax = xMax;
			intersect.yMin = yMin;
			intersect.yMax = yMax;

			return intersect;
		}

		public static Rect SetCenter(this Rect org, Vector2 center)
		{
			Vector2 currentCenter = org.center,
			toNewCenter = center - currentCenter;
			return new Rect(org.x + toNewCenter.x, org.y + toNewCenter.y, org.width, org.height);
		}

		public static Vector2 Clamp(this Rect orig, Vector2 pos)
		{
			pos.x = Mathf.Clamp(pos.x, orig.xMin, orig.xMax);
			pos.y = Mathf.Clamp(pos.y, orig.yMin, orig.yMax);
			return pos;
		}

		public static Vector2 Coord(this Rect orig, float x, float y)
		{
			return new Vector2((x - orig.x) / ((orig.x + orig.width) - orig.x), (y - orig.y) / ((orig.y + orig.height) - orig.y));
		}

		public static Vector2 Coord(this Rect orig, Vector2 screenPos)
		{
			return orig.Coord(screenPos.x, screenPos.y);
		}

		public static Rect Scale(this Rect org, float multiplier)
		{
			return org.Grow(new Vector2(org.width * (multiplier - 1), org.height * (multiplier - 1)));
		}

		public static Rect Shrink(this Rect org, float amount)
		{
			return org.Shrink(Vector2.one * amount);
		}

		public static Rect Shrink(this Rect org, Vector2 amount)
		{
			return org.Grow(new Vector2(-amount.x, -amount.y));
		}

		public static Rect Grow(this Rect org, Vector2 amount)
		{
			return new Rect(org.x - amount.x * .5f, org.y - amount.y * .5f, org.width + amount.x, org.height + amount.y);
		}

		public static Rect Fit(this Rect orig, Rect container)
		{
			float factor = Mathf.Min(Mathf.Abs(container.width / orig.width), Mathf.Abs(container.height / orig.height));
			orig.width *= factor * Mathf.Sign(container.width);
			orig.height *= factor * Mathf.Sign(container.height);
			orig.center = container.center;
			return orig;
		}

		public static Rect TransformToBoundsSpace(this Rect orig, Rect bbx)
		{
			Vector2 min = new Vector2(orig.xMin, orig.yMin);
			Vector2 max = new Vector2(orig.xMax, orig.yMax);
			min = min.TransformToBoundsSpace(bbx);
			max = max.TransformToBoundsSpace(bbx);
			return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
		}

		public static Rect TransformFromBoundsSpace(this Rect orig, Rect bbx)
		{
			Vector2 min = new Vector2(orig.xMin, orig.yMin);
			Vector2 max = new Vector2(orig.xMax, orig.yMax);
			min = min.TransformFromBoundsSpace(bbx);
			max = max.TransformFromBoundsSpace(bbx);
			return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
		}

		public static Rect Encapsulate(this Rect orig, Rect b)
		{
			Rect r = new Rect
			{
				x = Mathf.Min(orig.x, b.x),
				y = Mathf.Min(orig.y, b.y)
			};
			float x = Mathf.Max(orig.x + orig.width, b.x + b.width);
			float y = Mathf.Max(orig.y + orig.height, b.y + b.height);
			r.width = x - r.x;
			r.height = y - r.y;
			orig.x = r.x;
			orig.y = r.y;
			orig.width = r.width;
			orig.height = r.height;
			return orig;
		}

		public static Rect Encapsulate(this Rect orig, Vector2 point)
		{
			if (orig.xMin > point.x)
			{
				orig.xMin = point.x;
			}

			if (orig.xMax < point.x)
			{
				orig.xMax = point.x;
			}

			if (orig.yMin > point.y)
			{
				orig.yMin = point.y;
			}

			if (orig.yMax < point.y)
			{
				orig.yMax = point.y;
			}

			return orig;
		}

		public static Rect Round(this Rect orig)
		{
			orig.x = Mathf.Round(orig.x);
			orig.y = Mathf.Round(orig.y);
			orig.width = Mathf.Round(orig.width);
			orig.height = Mathf.Round(orig.height);
			return orig;
		}

		public static Vector2 Min(this Rect orig)
		{
			return new Vector2(orig.xMin, orig.yMin);
		}
		public static Vector2 Max(this Rect orig)
		{
			return new Vector2(orig.xMax, orig.yMax);
		}
		public static Vector2 Size(this Rect orig)
		{
			return new Vector2(orig.width, orig.height);
		}

		public static Rect Align(this Rect orig, Rect container, TextAnchor alignment)
		{
			// align X
			if (alignment == TextAnchor.UpperLeft || alignment == TextAnchor.MiddleLeft || alignment == TextAnchor.LowerLeft)
			{
				orig.x = container.x;
			}
			else if (alignment == TextAnchor.UpperCenter || alignment == TextAnchor.MiddleCenter || alignment == TextAnchor.LowerCenter)
			{
				orig.x = container.x + (container.width - orig.width) / 2f;
			}
			else
			{
				orig.x = container.xMax - orig.width;
			}

			// align Y
			if (alignment == TextAnchor.UpperLeft || alignment == TextAnchor.UpperCenter || alignment == TextAnchor.UpperRight)
			{
				orig.y = container.y;
			}
			else if (alignment == TextAnchor.MiddleLeft || alignment == TextAnchor.MiddleCenter || alignment == TextAnchor.MiddleRight)
			{
				orig.y = container.y + (container.height - orig.height) / 2f;
			}
			else
			{
				orig.y = container.yMax - orig.height;
			}
			return orig;
		}

		public static float Aspect(this Rect r)
		{
			return (r.width / r.height);
		}

		public static (Vector2, Vector2, Vector2, Vector2) GetCorners(this Rect orig)
		{
			return (
				new Vector2(orig.xMin, orig.yMin),
				new Vector2(orig.xMax, orig.yMin),
				new Vector2(orig.xMax, orig.yMax),
				new Vector2(orig.xMin, orig.yMax)
				);
		}

		#region [ line intersection ]
		/// <summary>
		/// checks if a line intersects with this rectangle
		/// </summary>
		/// <returns>Returns true if the given line intersects with this rectangle or if the rectangle contains the given line</returns>
		public static bool DoesLineSegmentIntersect(this Rect orig, Vector2 lineStartPoint, Vector2 lineEndPoint)
		{
			//started or ended inside rect
			if (orig.Contains(lineStartPoint))
			{
				return true;
			}

			if (orig.Contains(lineEndPoint))
			{
				return true;
			}

			//corners
			float xMin = Mathf.Min(lineStartPoint.x, lineEndPoint.x);
			float xMax = Mathf.Max(lineStartPoint.x, lineEndPoint.x);
			float yMin = Mathf.Min(lineStartPoint.y, lineEndPoint.y);
			float yMax = Mathf.Max(lineStartPoint.y, lineEndPoint.y);

			//completely outside of rect
			if (xMin > orig.xMax)
			{
				return false;
			}

			if (xMax < orig.xMin)
			{
				return false;
			}

			if (yMin > orig.yMax)
			{
				return false;
			}

			if (yMax < orig.yMin)
			{
				return false;
			}

			//lines
			Line2D left = new Line2D(orig.xMin, orig.yMin, orig.xMin, orig.yMax);
			Line2D top = new Line2D(orig.xMin, orig.yMin, orig.xMax, orig.yMin);
			Line2D right = new Line2D(orig.xMax, orig.yMin, orig.xMax, orig.yMax);
			Line2D bottom = new Line2D(orig.xMin, orig.yMax, orig.xMax, orig.yMax);
			Line2D line = new Line2D(lineStartPoint, lineEndPoint);

			//any intersecting
			if (DoLinesIntersect(line, left)
				|| DoLinesIntersect(line, top)
				|| DoLinesIntersect(line, right)
				|| DoLinesIntersect(line, bottom))
			{
				return true;
			}

			return false;
		}

		private static bool DoLinesIntersect(Line2D line1, Line2D line2)
		{
			return CrossProduct(line1.StartPoint, line1.EndPoint, line2.StartPoint) !=
				   CrossProduct(line1.StartPoint, line1.EndPoint, line2.EndPoint) ||
				   CrossProduct(line2.StartPoint, line2.EndPoint, line1.StartPoint) !=
				   CrossProduct(line2.StartPoint, line2.EndPoint, line1.EndPoint);
		}

		private static double CrossProduct(Vector2 p1, Vector2 p2, Vector2 p3)
		{
			return (p2.x - p1.x) * (p3.y - p1.y) - (p3.x - p1.x) * (p2.y - p1.y);
		}

		private struct Line2D
		{
			public Vector2 StartPoint { get; private set; }
			public Vector2 EndPoint { get; private set; }

			public Line2D(Vector2 startPoint, Vector2 endPoint)
			{
				StartPoint = startPoint;
				EndPoint = endPoint;
			}
			public Line2D(float xMin, float yMin, float xMax, float yMax)
			{
				StartPoint = new Vector2(xMin, yMin);
				EndPoint = new Vector2(xMax, yMax);
			}
		}
		#endregion
	}
}
