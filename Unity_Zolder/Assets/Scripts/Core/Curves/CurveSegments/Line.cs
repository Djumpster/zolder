// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Curves
{
	// The Line is an implementation of the ICurve interface that represents a 
	// straight Line. It represents a line starting at an origin point, going 
	// straight to a destination point. The Line implementation is significantly 
	// faster than a Bezier implementation and is therefore often used to replace 
	// Bezier segments that are approximately straight.
	//
	// Check the ICurve interface for extensive documentation on the key public functions
	public class Line : CurveSegment
	{
		[SerializeField, HideInInspector] private Vector3 origin;
		public override Vector3 Origin { get { return origin; } }

		[SerializeField, HideInInspector] private Vector3 destination;
		public override Vector3 Destination { get { return destination; } }

		[SerializeField, HideInInspector] private float length;
		public override float Length { get { return length; } }

		private Line() { } //Not ment to be created using the contructor. (because its an scriptable object)

		public static Line Create(Vector3 origin, Vector3 destination)
		{
			Line inst = CreateInstance<Line>();
			inst.SetData(origin, destination);
			return inst;
		}

		public override void SetData(Vector3 origin, Vector3 destination)
		{
			this.origin = origin;
			this.destination = destination;
			length = (destination - origin).magnitude;
		}

		public override Vector3 Evaluate(float time)
		{
			return (Origin + (Destination - Origin) * time);
		}

		public override Vector3 Derivative(float time)
		{
			return (Destination - Origin);
		}

		public override float Reparameterize(float time, int iterations)
		{
			return time;
		}

		public override CurveSearchDataset ClosestPointOnCurve(Vector3 point, int iterations)
		{
			float curveTime = -Vector3.Dot(Origin - point, Destination - Origin) / (Length * Length);
			curveTime = Mathf.Clamp01(curveTime);
			Vector3 curvePoint = Origin + (Destination - Origin) * curveTime;
			return new CurveSearchDataset(curveTime, curvePoint, Vector3.SqrMagnitude(curvePoint - point));
		}

		public override CurveSearchDataset ClosestPointOnCurve(LineUtils.Line line, int iterations)
		{
			return this.BinarySearch(line.SquareDistanceTo, iterations);
		}

		public override Bounds Boundingbox(int iterations)
		{
			Bounds b = Maths.Math.GetEmptyBox();
			b.Encapsulate(Origin);
			b.Encapsulate(Destination);
			return b;
		}

		public override void DrawGizmos(int density, Color? color = null)
		{
			Color prevColor = Gizmos.color;
			Gizmos.color = color ?? new Color(0f, 1f, 0f, 1f);
			Gizmos.DrawLine(Origin, Destination);
			Gizmos.color = prevColor;
		}
	}
}