// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Curves
{
	// THe CurveEditor class is a MonoBehaviour that can be placed on a GameObject 
	// in the scene in order to create and edit a curve in the transform space 
	// of that GameObject. The CurveEditorInspector script is the actual 
	// implementation of the editing tool and is pretty outdated and needs 
	// modification for improved UX.
	public class CurveEditor : MonoBehaviour, ICurve
	{
		private Transform cachedTransform;
		private Transform CachedTransform
		{
			get
			{
				if (cachedTransform == null)
				{
					cachedTransform = transform;
				}

				return cachedTransform;
			}
		}

		public enum EditorMode
		{
			Bezier,
			Line
		}

		// public to avoid clunky editor code
		public CompoundCurve Curve;
		public LayerMask SnapToSurface;
		public EditorMode CurrentMode;

		public void ReplaceStraightBeziersByLines()
		{
			if (Curve != null)
			{
				Curve.ReplaceStraightBeziersByLines();
			}
		}

		public void ReplaceLinesByBeziers()
		{
			if (Curve != null)
			{
				Curve.ReplaceLinesByBeziers();
			}
		}

		private void OnDrawGizmos()
		{
			DrawGizmos(50);
		}

		public Vector3 Origin
		{
			get
			{
				if (Curve == null)
				{
					throw new System.Exception("Unable to get the origin when no curve is set");
				}

				return CachedTransform.position + CachedTransform.TransformDirection(Curve.Origin); // NOTE - Using TransformDirection to eliminate scale influence
			}
		}

		public Vector3 Destination
		{
			get
			{
				if (Curve == null)
				{
					throw new System.Exception("Unable to get the destination when no curve is set");
				}

				return CachedTransform.position + CachedTransform.TransformDirection(Curve.Destination); // NOTE - Using TransformDirection to eliminate scale influence
			}
		}

		public float Length
		{
			get
			{
				if (Curve == null)
				{
					throw new System.Exception("Unable to get the length when no curve is set");
				}

				return Curve.Length;
			}
		}

		public Vector3 Evaluate(float time)
		{
			if (Curve == null)
			{
				throw new System.Exception("Unable to evaluate when no curve is set");
			}

			return CachedTransform.position + CachedTransform.TransformDirection(Curve.Evaluate(time));
		}

		public Vector3 Derivative(float time)
		{
			if (Curve == null)
			{
				throw new System.Exception("Unable to get the derivative when no curve is set");
			}

			return CachedTransform.TransformDirection(Curve.Derivative(time));
		}

		public float Reparameterize(float time, int iterations)
		{
			if (Curve == null)
			{
				throw new System.Exception("Unable to reparamaterize when no curve is set");
			}

			return Curve.Reparameterize(time, iterations);
		}

		public CurveSearchDataset ClosestPointOnCurve(Vector3 point, int iterations)
		{
			if (Curve == null)
			{
				throw new System.Exception("Unable to get the closest point when no curve is set");
			}

			Vector3 localPoint = CachedTransform.InverseTransformPoint(point);
			CurveSearchDataset localResult = Curve.ClosestPointOnCurve(localPoint, iterations);
			return localResult;
		}

		public CurveSearchDataset ClosestPointOnCurve(LineUtils.Line line, int iterations)
		{
			if (Curve == null)
			{
				throw new System.Exception("Unable to get the closest point when no curve is set");
			}

			var localLine = new LineUtils.Line(CachedTransform.InverseTransformPoint(line.origin), CachedTransform.InverseTransformPoint(line.destination));
			CurveSearchDataset localResult = Curve.ClosestPointOnCurve(localLine, iterations);
			return localResult;
		}

		public Bounds Boundingbox(int iterations)
		{
			Bounds b = Maths.Math.GetEmptyBox();
			if (Curve == null)
			{
				return b;
			}

			b.Encapsulate(Origin);
			b.Encapsulate(Destination);
			for (int i = 1; i < iterations + 1; i++)
			{
				b.Encapsulate(Evaluate(i / ((float)iterations + 1)));
			}
			return b;
		}

		public void DrawGizmos(int density, Color? color = null)
		{
			if (Curve)
			{
				Matrix4x4 m = Gizmos.matrix;
				Gizmos.matrix = CachedTransform.localToWorldMatrix * m;
				Curve.DrawGizmos(density, color);
				Gizmos.matrix = m;
			}
		}

		public float AverageLength()
		{
			return Curve.AverageLength;
		}
	}
}