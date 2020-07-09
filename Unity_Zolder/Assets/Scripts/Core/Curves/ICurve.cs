// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Curves
{
	public struct CurveSearchDataset
	{
		public float curveTime;
		public Vector3 curvePosition;
		public float squareDistance;

		public CurveSearchDataset(float curveTime, Vector3 curvePosition, float squareDistance)
		{
			this.curveTime = curveTime;
			this.curvePosition = curvePosition;
			this.squareDistance = squareDistance;
		}
	}

	public interface ICurve
	{
		Vector3 Origin { get; } // The start point of the curve
		Vector3 Destination { get; } // The end point of the curve
		float Length { get; } // The approximate distance to to travel from Origin to Destination along the curve 

		// the position of the curve at 'time', where time=0 is the start of the curve,
		// time=1 is the end of the curve. linear increases in time do *not* guarantee
		// equal distance moves (see Reparameterize(...)).
		Vector3 Evaluate(float time);

		// the derivative (tangent) of the curve at 'time', where time=0 is the start of the curve,
		// time=1 is the end of the curve. 
		Vector3 Derivative(float time);

		// reparameterize 'time' using a number of iterations (an integer between 1 and infinity), more iterations is slower but more precise.
		// reparameterization is used to get constant speed over the curve (i.e. if time increases with equal 
		// sized steps, distance will also increase with equal sized steps).
		//
		// example usage:
		//	ICurve c;
		//	for (float t = 0; t <= 1f; t += .1f)
		//	{
		//		// t increases with fixed steps, but that won't guarantee fixed speed on all curve types, so we reparameterize t 
		//		float fixedSpeedTime = c.Reparameterize(t, 10);
		//	
		//		// get the curve values using the result from the reparameterization	
		//		pos = c.Evaluate(fixedSpeedTime);
		//		tan = c.Derivative(fixedSpeedTime);
		//		// the 10 values for pos and tan in this loop now move in approximately equal length steps.
		//	}
		//
		// Never reparameterize the output of the Reparameterize function itself, as it would yield meaningless results
		float Reparameterize(float time, int iterations);

		// this function gets the closest point to 'point' on the curve.
		CurveSearchDataset ClosestPointOnCurve(Vector3 point, int iterations);
		// this function get the closest point to 'line' on the curve.
		CurveSearchDataset ClosestPointOnCurve(LineUtils.Line line, int iterations);

		// Find the enclosing axis-aligned boundingbox for this curve. iterations will determine
		// the precision of the result but with more expensive calculation  
		Bounds Boundingbox(int iterations);

		void DrawGizmos(int density, Color? color = null);
	}
}