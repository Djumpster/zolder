// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Talespin.Core.Foundation.Curves
{
	// The CompoundCurve class is a curve implementation that connects a list of 
	// CurveSegments (Bezier.cs and Line.cs both implement CurveSegment) into a 
	// longer sequence, which allows for an arbitrarily curved path through the 
	// scene.
	//
	// Check the ICurve interface for extensive documentation on the key public functions
	public class CompoundCurve : ScriptableObject, ICurve
	{
		[Serializable]
		private class CurveSegmentData
		{
			public CurveSegment curve;
			public float startDistance;

			public CurveSegmentData(CurveSegment c, float d)
			{
				curve = c;
				startDistance = d;
			}

			public float ToInnerTime(float outerDist)
			{
				return (outerDist - startDistance) / curve.Length;
			}

			public float ToOuterDistance(float innerT)
			{
				return startDistance + innerT * curve.Length;
			}
		}

		public float MaxDistance
		{
			get
			{
				return maxDistance;
			}
		}
		[SerializeField] private float maxDistance;
		[SerializeField] private List<CurveSegmentData> curves = new List<CurveSegmentData>();
		public bool HasCurves { get { return curves.Count > 0; } }

		[SerializeField] private float length;
		public float Length
		{
			get { return length; }
		}

		public Vector3 Origin
		{
			get
			{
				if (curves.Count == 0)
				{
					throw new Exception("[CompoundCurve] No curves set.");
				}
				return curves[0].curve.Origin;
			}
		}

		public Vector3 Destination
		{
			get
			{
				if (curves.Count == 0)
				{
					throw new Exception("[CompoundCurve] No curves set.");
				}
				return curves[curves.Count - 1].curve.Destination;
			}
		}


		public float AverageLength
		{
			get
			{
				return length / curves.Count;
			}
		}

		private CompoundCurve() { } //Not ment to be created using the contructor. (because it's a scriptable object)

		public static CompoundCurve Create()
		{
			return CreateInstance<CompoundCurve>();
		}

		public static CompoundCurve Create(IEnumerable<CurveSegment> input)
		{
			CompoundCurve inst = CreateInstance<CompoundCurve>();
			foreach (CurveSegment c in input)
			{
				inst.Add(c);
			}

			return inst;
		}

		public IEnumerable<CurveSegment> GetCurves()
		{
			foreach (CurveSegmentData cs in curves)
			{
				yield return cs.curve;
			}
		}

		public void Add(CurveSegment c)
		{
			curves.Add(new CurveSegmentData(c, Length));
			if (c != null)
			{
				length += c.Length;
			}
		}

		public void InsertBezier(float atDistance)
		{
			CurveSegmentData curveData = FindCurve(atDistance);
			float percentualLenth = curveData.curve.Length * .1f;
			Vector3 controlDirectionAtNewPoint = Derivative(atDistance / Length).normalized * percentualLenth;
			Vector3 controlDirectionAtEndPoint = curveData.curve.Derivative(1f).normalized * percentualLenth;
			Vector3 newPoint = Evaluate(atDistance / Length),
			endPoint = curveData.curve.Destination;

			if (curveData.curve is Bezier bezier)
			{
				Vector3 controlPoint1 = bezier.ControlPoint1;
				bezier.SetData(curveData.curve.Origin, controlPoint1, newPoint - controlDirectionAtNewPoint, newPoint);
			}
			else
			{
				curveData.curve.SetData(curveData.curve.Origin, newPoint);
			}
			var newBezier = Bezier.Create(newPoint, newPoint + controlDirectionAtNewPoint, endPoint - controlDirectionAtEndPoint, endPoint);

			curves.Insert(curves.IndexOf(curveData) + 1, new CurveSegmentData(newBezier, 0f));
			UpdateLengths();
		}

		public void InsertLine(float atDistance)
		{
			Insert((beginPos, endPos) => Line.Create(beginPos, endPos), atDistance);
		}

		private void Insert(Func<Vector3, Vector3, CurveSegment> createCurve, float atDistance)
		{
			CurveSegmentData curveData = FindCurve(atDistance);
			Vector3 controlDir2 = Derivative(atDistance / Length);
			Vector3 beginPoint = Evaluate(atDistance / Length),
			endPoint = curveData.curve.Destination;
			if (curveData.curve is Bezier bezier)
			{
				Vector3 controlPoint1 = bezier.ControlPoint1;
				bezier.SetData(curveData.curve.Origin, controlPoint1, beginPoint - controlDir2, beginPoint);
			}
			else
			{
				curveData.curve.SetData(curveData.curve.Origin, beginPoint);
			}
			curves.Insert(curves.IndexOf(curveData) + 1, new CurveSegmentData(createCurve(beginPoint, endPoint), 0f));
			UpdateLengths();
		}

		public void DeleteLast()
		{
			if (curves.Count <= 0)
			{
				throw new Exception("Unable to delete a curve when there are no segments compound curve");
			}

			CurveSegmentData data = curves[curves.Count - 1];
			length -= data.curve.Length;
			DestroyImmediate(data.curve, true);
			curves.RemoveAt(curves.Count - 1);
		}

		public void DeleteControlPoint(float atDistance)
		{
			if (curves.Count <= 0)
			{
				throw new Exception("Unable to delete a controlpoint when there are no segments in the compound curve");
			}

			CurveSegmentData closest = null;
			float closestDistance = Mathf.Infinity;
			int closestIndex = -1;
			for (int i = 0; i < curves.Count; i++)
			{
				float cDist = Mathf.Abs(curves[i].startDistance - atDistance);
				if (cDist < closestDistance)
				{
					closest = curves[i];
					closestDistance = cDist;
					closestIndex = i;
				}
			}
			bool isBeginOfCurve = closestIndex == 0,
			isEndOfCurve = (closestIndex == (curves.Count - 1)) && closestDistance > (closest.curve.Length * .5f);
			if (!isBeginOfCurve && !isEndOfCurve)
			{
				CurveSegmentData prevData = curves[closestIndex - 1];
				Vector3 beginPoint = closest.curve.Destination;
				prevData.curve.SetData(prevData.curve.Origin, beginPoint);
			}
			curves.RemoveAt(closestIndex);
			DestroyImmediate(closest.curve, true);
			UpdateLengths();
		}

		public void DeleteControlPoint(CurveSegment lastSegment, CurveSegment segment)
		{
			if (curves.Count <= 0)
			{
				throw new Exception("Unable to delete a controlpoint when there are no segments in the compound curve");
			}
			int closestIndex = -1;
			if (segment == null)
			{
				closestIndex = curves.Count - 1;
			}
			for (int i = 0; i < curves.Count; i++)
			{
				if (curves[i].curve == segment)
				{
					closestIndex = i;
				}
			}
			bool isBeginOfCurve = closestIndex == 0;
			bool isEndOfCurve = closestIndex == (curves.Count - 1);
			if (!isBeginOfCurve && !isEndOfCurve)
			{
				CurveSegmentData prevData = curves[closestIndex - 1];
				Vector3 beginPoint = curves[closestIndex].curve.Destination;
				prevData.curve.SetData(prevData.curve.Origin, beginPoint);
			}
			DestroyImmediate(curves[closestIndex].curve, true);
			curves.RemoveAt(closestIndex);
			UpdateLengths();
		}

		public void UpdateLengths()
		{
			length = 0f;
			foreach (CurveSegmentData segData in curves)
			{
				segData.startDistance = Length;
				length += segData.curve.Length;
			}
		}

		public CurveSegment GetSegment(float distance)
		{
			return FindCurve(distance).curve;
		}

		private CurveSegmentData FindCurve(float distance)
		{
			if (curves.Count <= 0)
			{
				return null;
			}

			for (int i = 0; i < curves.Count - 1; i++)
			{
				if ((distance >= curves[i].startDistance || i == 0) && distance < curves[i + 1].startDistance)
				{
					return curves[i];
				}
			}

			return curves[curves.Count - 1];
		}

		public Vector3 Evaluate(float time)
		{
			if (curves.Count <= 0)
			{
				throw new Exception("Unable to evaluate the curve while there are no segments in the compound curve");
			}

			time = Mathf.Clamp01(time) * Length;
			CurveSegmentData c = FindCurve(time);
			return c.curve.Evaluate(c.ToInnerTime(time));
		}

		public Vector3 Derivative(float time)
		{
			if (curves.Count <= 0)
			{
				throw new Exception("Unable to get the derivative while there are no segments in the compound curve");
			}

			time = Mathf.Clamp01(time) * Length;
			CurveSegmentData c = FindCurve(time);
			return c.curve.Derivative(c.ToInnerTime(time));
		}

		public float Reparameterize(float time, int iterations = 10)
		{
			if (curves.Count <= 0)
			{
				throw new Exception("Unable to reparamaterize while there are no segments in the compound curve");
			}

			time = Mathf.Clamp01(time) * Length;
			CurveSegmentData c = FindCurve(time);
			float it = c.curve.Reparameterize(c.ToInnerTime(time), iterations);
			return (c.startDistance + (it * c.curve.Length)) / Length;
		}

		public CurveSearchDataset ClosestPointOnCurve(Vector3 point, int iterations)
		{
			if (curves.Count <= 0)
			{
				throw new Exception("Unable to get the closestpoint while there are no segments in the compound curve");
			}

			return this.BinarySearch(position => (point - position).sqrMagnitude, iterations);
		}

		public CurveSearchDataset ClosestPointOnCurve(LineUtils.Line line, int iterations)
		{
			if (curves.Count <= 0)
			{
				throw new Exception("Unable to get the closestpoint while there are no segments in the compound curve");
			}
			var minData = new CurveSearchDataset();
			var minCurve = new CurveSegmentData(null, 0f);
			for (int i = 0; i < curves.Count; i++)
			{
				CurveSearchDataset data = curves[i].curve.BinarySearch(position => line.SquareDistanceTo(position), iterations);
				if (i == 0 || data.squareDistance < minData.squareDistance)
				{
					minData = data;
					minCurve = curves[i];
				}
			}
			minData.curveTime = minCurve.ToOuterDistance(minData.curveTime) / length;
			return minData;
		}

		public void ReplaceStraightBeziersByLines(float approx = .001f)
		{
			foreach (CurveSegmentData cs in curves)
			{
				if (cs.curve is Bezier b)
				{
					Vector3 org = b.ControlPoint1 - b.Origin;
					float orgMag = org.magnitude;
					org /= orgMag;
					Vector3 dst = b.Destination - b.ControlPoint2;
					float dstMag = dst.magnitude;
					dst /= dstMag;
					Vector3 od = (b.Destination - b.Origin).normalized;

					bool hasOrg = orgMag > 0f;
					bool hasDst = dstMag > 0f;
					bool line = true;

					if (hasOrg && !hasDst)
					{
						line = Vector3.Dot(org, od) > 1f - approx;
					}
					else if (hasDst && !hasOrg)
					{
						line = Vector3.Dot(org, dst) > 1f - approx;
					}
					else if (hasDst && hasOrg)
					{
						line = Vector3.Dot(org, dst) > 1f - approx && Vector3.Dot(org, od) > 1f - approx && Vector3.Dot(org, dst) > 1f - approx;
					}

					if (line)
					{
						cs.curve = Line.Create(b.Origin, b.Destination);
						DestroyImmediate(b, true);
					}
				}
			}

			//Recalculate the start times because its probably incorrect due to bezier length inprecsions with straight lines.
			length = 0f;
			foreach (CurveSegmentData cs in curves)
			{
				cs.startDistance = Length;
				length += cs.curve.Length;
			}
		}

		public void ReplaceLinesByBeziers()
		{
			foreach (CurveSegmentData cs in curves)
			{
				if (cs.curve is Line l)
				{
					Vector3 toEnd = l.Destination - l.Origin,
					bOrg = l.Origin,
					bCp1 = l.Origin + toEnd * .25f,
					bCp2 = l.Destination - toEnd * .25f,
					bDest = l.Destination;

					cs.curve = Bezier.Create(bOrg, bCp1, bCp2, bDest);
					DestroyImmediate(l, true);
				}
			}
		}

		public void DrawGizmos(int density, Color? color = null)
		{
			foreach (CurveSegmentData cs in curves)
			{
				cs.curve.DrawGizmos(density, color);
			}
		}

		public Bounds Boundingbox(int iterations)
		{
			Bounds b = Maths.Math.GetEmptyBox();
			foreach (CurveSegmentData c in curves)
			{
				b.Encapsulate(c.curve.Boundingbox(iterations));
			}

			return b;
		}
	}
}