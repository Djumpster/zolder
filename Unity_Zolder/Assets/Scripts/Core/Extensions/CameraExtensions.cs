// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Maths;
using UnityEngine;

namespace Talespin.Core.Foundation.Extensions
{
	public static class CameraExtensions
	{
		public struct ObjectLocator
		{
			public readonly Vector2 ScreenPoint;
			public readonly bool IsOffScreen;
			public readonly bool IsBehind;
			public readonly float AngleWithCameraUp;

			public ObjectLocator(Vector2 screenPoint, bool isOffScreen, bool isBehind, float angle)
			{
				ScreenPoint = screenPoint;
				IsOffScreen = isOffScreen;
				AngleWithCameraUp = angle;
				IsBehind = isBehind;
			}
		}

		private static Vector2 FindClosestPointOnScreenEdge(Vector2 screenSize, Vector2 screenPoint)
		{
			Vector2 center = screenSize * .5f;
			Vector2 dir = screenPoint - center;
			float vLeft = (0f - center.x) / dir.x;
			float vRight = (screenSize.x - center.x) / dir.x;
			float vTop = (0f - center.y) / dir.y;
			float vBottom = (screenSize.y - center.y) / dir.y;

			if (vLeft >= 0f && (vLeft < vTop || vTop < 0f) && (vLeft < vBottom || vBottom < 0f))
			{
				return center + dir * vLeft;
			}
			else if (vRight >= 0f && (vRight < vTop || vTop < 0f) && (vRight < vBottom || vBottom < 0f))
			{
				return center + dir * vRight;
			}
			else if (vTop >= 0f && (vTop < vLeft || vLeft < 0f) && (vTop < vRight || vRight < 0f))
			{
				return center + dir * vTop;
			}
			else if (vBottom >= 0f && (vBottom < vLeft || vLeft < 0f) && (vBottom < vRight || vRight < 0f))
			{
				return center + dir * vBottom;
			}
			else
			{
				return Vector2.zero;
			}
		}

		public static ObjectLocator WorldToScreenClampedPosition(this Camera orig, Vector3 worldPosition)
		{
			return WorldToScreenClampedPosition(orig, worldPosition, new Rect(0, 0, 1, 1));
		}

		public static ObjectLocator WorldToScreenClampedPosition(this Camera orig, Vector3 worldPosition, Rect safeZone)
		{
			Vector2 screenSize = new Vector2(orig.pixelWidth, orig.pixelHeight);
			Vector2 center = screenSize * .5f;
			Vector3 screenPoint = orig.WorldToScreenPoint(worldPosition);
			Vector2 point = screenPoint;
			Rect screen = new Rect(0, 0, orig.pixelWidth, orig.pixelHeight);
			bool isBehind = screenPoint.z < 0f;
			bool isOffScreen = !screen.Contains(point) || isBehind;
			if (isBehind)
			{
				// invert x to prevent the location from jumping to the back of the screen
				point.x = -point.x;
				point.y -= center.y;
				// make sure the point is never exactly in the center
				if (Mathf.Approximately(point.y, center.y))
				{
					point.y += 1f;
				}

				point = center + (point - center) * screenSize.sqrMagnitude;
				point = screen.Clamp(point);
			}
			if (isOffScreen)
			{
				point = FindClosestPointOnScreenEdge(new Vector2(screen.width, screen.height), point);
				point = screen.Clamp(point);
			}
			float angle = Math.SignedAngle(Vector2.up, center - point, Vector3.forward);
			return new ObjectLocator(point, isOffScreen, isBehind, angle);
		}
	}
}
