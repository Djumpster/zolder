// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Extensions
{
	public static class BoundsExtensions
	{
		public static Bounds TransformFromBoundsSpace(Bounds orig, Bounds bbx)
		{
			Bounds ret = new Bounds(Vector3.zero, Vector3.one);
			ret.SetMinMax(orig.min.TransformFromBoundsSpace(bbx), orig.max.TransformFromBoundsSpace(bbx));
			return ret;
		}

		public static Bounds TransformToBoundsSpace(this Bounds orig, Bounds bbx)
		{
			Bounds ret = new Bounds(Vector3.zero, Vector3.one);
			ret.SetMinMax(orig.min.TransformToBoundsSpace(bbx), orig.max.TransformToBoundsSpace(bbx));
			return ret;
		}

		public static Rect ScreenRect(this Bounds b, Transform trans, Camera cam)
		{
			Vector3 v = cam.WorldToScreenPoint(trans.TransformPoint(new Vector3(b.min.x, b.min.y, b.min.z)));
			Rect r = new Rect(v.x, v.y, 0, 0);
			r = r.Encapsulate(cam.WorldToScreenPoint(trans.TransformPoint(new Vector3(b.max.x, b.min.y, b.min.z))));
			r = r.Encapsulate(cam.WorldToScreenPoint(trans.TransformPoint(new Vector3(b.min.x, b.max.y, b.min.z))));
			r = r.Encapsulate(cam.WorldToScreenPoint(trans.TransformPoint(new Vector3(b.max.x, b.max.y, b.min.z))));
			r = r.Encapsulate(cam.WorldToScreenPoint(trans.TransformPoint(new Vector3(b.min.x, b.min.y, b.max.z))));
			r = r.Encapsulate(cam.WorldToScreenPoint(trans.TransformPoint(new Vector3(b.max.x, b.min.y, b.max.z))));
			r = r.Encapsulate(cam.WorldToScreenPoint(trans.TransformPoint(new Vector3(b.min.x, b.max.y, b.max.z))));
			r = r.Encapsulate(cam.WorldToScreenPoint(trans.TransformPoint(new Vector3(b.max.x, b.max.y, b.max.z))));
			return r;
		}

		/// <summary>
		/// Gets the vertices of the Bounds.
		/// </summary>
		/// <returns>The vertices.</returns>
		public static Vector3[] GetVertices(this Bounds b)
		{
			var vertices = new Vector3[8];

			Vector3 min = b.min;
			Vector3 max = b.max;

			vertices[0] = min;
			vertices[1] = new Vector3(min.x, min.y, max.z);
			vertices[2] = new Vector3(max.x, min.y, max.z);
			vertices[3] = new Vector3(max.x, min.y, min.z);
			vertices[4] = new Vector3(max.x, max.y, min.z);
			vertices[5] = new Vector3(min.x, max.y, min.z);
			vertices[6] = new Vector3(min.x, max.y, max.z);
			vertices[7] = max;

			return vertices;
		}

		public static void DrawInDebug(this Bounds b, Color color = default(Color), float time = 10f)
		{
			Vector3[] vertices = b.GetVertices();
			Debug.DrawLine(vertices[0], vertices[1], color, time);
			Debug.DrawLine(vertices[1], vertices[2], color, time);
			Debug.DrawLine(vertices[2], vertices[3], color, time);
			Debug.DrawLine(vertices[3], vertices[4], color, time);

			Debug.DrawLine(vertices[4], vertices[5], color, time);
			Debug.DrawLine(vertices[5], vertices[6], color, time);
			Debug.DrawLine(vertices[6], vertices[7], color, time);
			Debug.DrawLine(vertices[7], vertices[4], color, time);

			Debug.DrawLine(vertices[7], vertices[2], color, time);
			Debug.DrawLine(vertices[6], vertices[1], color, time);
			Debug.DrawLine(vertices[5], vertices[0], color, time);
			Debug.DrawLine(vertices[3], vertices[0], color, time);
		}
	}
}
