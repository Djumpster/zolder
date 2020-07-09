// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Curves
{
	// AN abstract class for curves to allow different curve implementations to 
	// be used combined in a CompoundCurve.
	public abstract class CurveSegment : ScriptableObject, ICurve
	{
		public abstract Vector3 Origin { get; }
		public abstract Vector3 Destination { get; }
		public abstract float Length { get; }

		public abstract void SetData(Vector3 origin, Vector3 destination);

		public abstract Vector3 Evaluate(float time);
		public abstract Vector3 Derivative(float time);
		public abstract float Reparameterize(float time, int iterations);

		public abstract CurveSearchDataset ClosestPointOnCurve(Vector3 point, int iterations);
		public abstract CurveSearchDataset ClosestPointOnCurve(LineUtils.Line line, int iterations);

		public abstract Bounds Boundingbox(int iterations);
		public abstract void DrawGizmos(int density, Color? color);
	}
}