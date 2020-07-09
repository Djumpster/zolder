// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Curves
{
	// This class is a curve implementation using Bezier spline mathematics.
	// A Bezier curve is a commonly used construct that represents a curved line 
	// by defining origin and destination points as well as the tangents of the 
	// curve at these two points.Due to the way Beziers work, they can never 
	// have more than two bends in it, which means you usually need more than 
	// one curve to represent a path in a scene.
	//
	// Check the ICurve interface for extensive documentation on the key public functions
	public class Bezier : CurveSegment
	{
		private const float LENGHT_PRECISION = 2000f;

		[SerializeField, HideInInspector] private Vector3 origin;
		public override Vector3 Origin { get { return origin; } }

		[SerializeField, HideInInspector] private Vector3 controlPoint1;
		public Vector3 ControlPoint1 { get { return controlPoint1; } }

		[SerializeField, HideInInspector] private Vector3 destination;
		public override Vector3 Destination { get { return destination; } }

		[SerializeField, HideInInspector] private Vector3 controlPoint2;
		public Vector3 ControlPoint2 { get { return controlPoint2; } }

		[SerializeField, HideInInspector] private float length;
		public override float Length { get { return length; } }

		// These values are cashed in the inspector, to speed up the bezier calculations
		[SerializeField, HideInInspector] private Vector3 fp1;
		[SerializeField, HideInInspector] private Vector3 fp2;
		[SerializeField, HideInInspector] private Vector3 q0;
		[SerializeField, HideInInspector] private Vector3 q1;
		[SerializeField, HideInInspector] private Vector3 q2;

		private Bezier() { } //Not ment to be created using the contructor. (because its an scriptable object)

		public static Bezier Create(Vector3 origin, Vector3 destination)
		{
			Vector3 toDest = destination - origin;
			return Create(origin, origin + toDest * .25f, origin + toDest * .75f, destination);
		}

		public static Bezier Create(Vector3 origin, Vector3 controlPoint1, Vector3 controlPoint2, Vector3 destination)
		{
			Bezier inst = CreateInstance<Bezier>();
			inst.SetData(origin, controlPoint1, controlPoint2, destination);
			return inst;
		}

		public override void SetData(Vector3 origin, Vector3 destination)
		{
			Vector3 cp1Offset = ControlPoint1 - this.Origin,
					cp2Offset = ControlPoint2 - this.Destination;

			SetData(origin, origin + cp1Offset, destination + cp2Offset, destination);
		}

		public void SetData(Vector3 origin, Vector3 controlPoint1, Vector3 controlPoint2, Vector3 destination)
		{
			this.origin = origin;
			this.destination = destination;
			this.controlPoint1 = controlPoint1;
			this.controlPoint2 = controlPoint2;
			PrecalculateData();
			length = IntegrateSpeed(0f, 1f, LENGHT_PRECISION);
		}

		private void PrecalculateData()
		{
			fp1 = ControlPoint1 * 3.0f;
			fp2 = ControlPoint2 * 3.0f;
			q0 = (ControlPoint1 - Origin) * 3.0f;
			q1 = (ControlPoint2 - ControlPoint1) * 6.0f;
			q2 = (Destination - ControlPoint2) * 3.0f;
		}

		// evaluation of a 3D Bezier
		// returns the position of the curve at a given time
		public override Vector3 Evaluate(float time)
		{
			float invT = 1.0f - time;
			return invT * invT * invT * Origin +
					time * invT * invT * fp1 +
					time * time * invT * fp2 +
					time * time * time * Destination;
		}

		// Mathematical: the derivative of this bezier
		// Conceptual: the speed of the bezier at a given time
		public override Vector3 Derivative(float time)
		{
			float invT = 1.0f - time;
			return invT * invT * q0 +
					time * invT * q1 +
					time * time * q2;
		}

		// integrate over the speed of the curve
		public float IntegrateSpeed(float start, float end, float accuracy)
		{
			float step = (end - start) / accuracy;
			float integral = 0.0f;
			for (float currentTime = start; currentTime < end; currentTime += step)
			{
				integral += Derivative(currentTime).magnitude * step;
			}

			return integral;
		}

		// Reparamterize the 'time' parameter of this function.
		// interpolating input 's' from 0 to 1 will yield a time t that
		// interpolates over the bezier curve with fixed speed.
		// Typical use: 
		// bezier.Evaluate(bezier.Reparameterize(t));
		public override float Reparameterize(float s, int iterations)
		{
			s *= Length;
			float t = 0f;           // initial condition 
			float h = s / iterations;   // step size 
			for (int i = 0; i < iterations; i++)
			{
				// The divisions here might be a problem if the divisors are nearly zero. 
				float k1 = h / Derivative(t).magnitude;
				float k2 = h / Derivative(t + k1 * .5f).magnitude;
				float k3 = h / Derivative(t + k2 * .5f).magnitude;
				float k4 = h / Derivative(t + k3).magnitude;
				t += (k1 + 2f * (k2 + k3) + k4) / 6f;
			}
			return t;
		}

		public override CurveSearchDataset ClosestPointOnCurve(Vector3 point, int iterations)
		{
			return this.BinarySearch(position => (point - position).sqrMagnitude, iterations);
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
			for (int i = 1; i < iterations + 1; i++)
			{
				b.Encapsulate(Evaluate(i / ((float)iterations + 1)));
			}

			return b;
		}

		public override void DrawGizmos(int density, Color? color = null)
		{
			Color prevColor = Gizmos.color;
			Gizmos.color = color ?? Color.blue;
			float d = density;
			for (float i = 0; i < d; i++)
			{
				Gizmos.DrawLine(Evaluate(i / d), Evaluate((i + 1f) / d));
			}
			Gizmos.color = Color.red;
			Gizmos.DrawLine(Origin, ControlPoint1);
			Gizmos.DrawLine(Destination, ControlPoint2);
			Gizmos.color = prevColor;
		}
	}
}