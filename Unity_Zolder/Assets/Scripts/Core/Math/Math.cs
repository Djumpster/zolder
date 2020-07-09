// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Extensions;
using Talespin.Core.Swizzle;
using UnityEngine;

namespace Talespin.Core.Foundation.Maths
{
	public static class Math
	{
		public static int GetDigit(int number, int digit)
		{
			int num = number % (int)Mathf.Pow(10, digit + 1);
			return num / (int)Mathf.Pow(10, digit);
		}

		public static bool HasDigit(int number, int digit)
		{
			return (number == 0 && digit == 0) || (number / (int)Mathf.Pow(10, digit) > 0);
		}

		public static float FullAngle(Vector3 a, Vector3 b)
		{
			return FullAngle(a, b, Vector3.up);
		}

		public static float InverseQuadratic(float val)
		{
			val *= 2f;
			val -= 1f;
			return 1f - (val * val);
		}

		public static float BlockWave(float t)
		{
			return t % 1f < .5f ? 1f : 0f;
		}

		public static float SquareWave(float t)
		{
			return ((t % 1f) < .5f) ? -1f : 1f;
		}

		public static Rect ShrinkRect(Rect r, float amount)
		{
			return new Rect(r.x + amount, r.y + amount, r.width - amount * 2f, r.height - amount * 2f);
		}

		public static Vector3 FlipY(Vector3 v)
		{
			return new Vector3(v.x, -v.y, v.z);
		}

		public static float FullAngle(Vector3 a, Vector3 b, Vector3 reference)
		{
			a = a.normalized;
			b = b.normalized;
			float angle = Vector3.Angle(a, b);
			float dot = Vector3.Dot(b, Vector3.Cross(a, reference));
			if (dot > 0)
			{
				return 360f - angle;
			}
			else
			{
				return angle;
			}
		}

		public static float SignedAngle(Vector3 a, Vector3 b)
		{
			return SignedAngle(a, b, Vector3.up);
		}

		public static float SignedAngle(Vector3 a, Vector3 b, Vector3 reference)
		{
			float angle = Math.FullAngle(a, b, reference);
			if (angle > 180f)
			{
				return angle - 360f;
			}
			else
			{
				return angle;
			}
		}

		public static Vector3 RotateVectorAroundPoint(Vector3 vector, Vector3 pivot, Quaternion rotation)
		{
			return rotation * (vector - pivot) + pivot;
		}

		public static Vector2 Center(Rect r)
		{
			return new Vector2(r.x + r.width * .5f, r.y + r.height * .5f);
		}

		public static Rect SetCenter(Rect r, Vector2 center)
		{
			Vector2 oldCenter = Center(r),
					diff = center - oldCenter;
			return MoveRect(r, diff);
		}

		public static Rect MoveRect(Rect rect, Vector2 amount)
		{
			return MoveRect(rect, amount.x, amount.y);
		}
		public static Rect MoveRect(Rect rect, float x, float y)
		{
			return new Rect(rect.x + x, rect.y + y, rect.width, rect.height);
		}

		public static Rect ScaleRect(Rect rect, Vector2 scale)
		{
			return ScaleRect(rect, scale, new Vector2(.5f, .5f));
		}

		public static Rect ScaleRect(Rect rect, Vector2 scale, Vector2 center)
		{
			return GrowRect(rect, rect.width * (scale.x - 1), rect.height * (scale.y - 1), center);
		}


		public static Rect ScaleRect(Rect rect, float scale)
		{
			return ScaleRect(rect, scale, new Vector2(.5f, .5f));
		}

		public static Rect ScaleRect(Rect rect, float scale, Vector2 center)
		{
			return GrowRect(rect, rect.width * (scale - 1), rect.height * (scale - 1), center);
		}

		public static Rect GrowRect(Rect rect, float x, float y)
		{
			return GrowRect(rect, x, y, new Vector2(.5f, .5f));
		}

		public static Rect GrowRect(Rect rect, float x, float y, Vector2 center)
		{
			rect.width += x;
			rect.height += y;
			rect.x -= center.x * x;
			rect.y -= center.y * y;
			return rect;
		}

		public static Vector3 FlattenY(Vector3 v)
		{
			return new Vector3(v.x, 0f, v.z);
		}
		public static Vector3 FlattenX(Vector3 v)
		{
			return new Vector3(0f, v.y, v.z);
		}
		public static Vector3 FlattenZ(Vector3 v)
		{
			return v.xy();
		}

		public static Vector3 Average(Vector3 a, Vector3 b)
		{
			Vector3 avg = new Vector3((a.x + b.x) / 2f, (a.y + b.y) / 2f, (a.z + b.z) / 2f);
			return avg;
		}

		// Not sure if this makes sense.....	
		public static Vector3 Average(params Vector3[] vectors)
		{
			Vector3 averages = Vector3.zero;

			for (int i = 0; i < vectors.Length; i++)
			{
				averages += vectors[i];
			}
			float ax = averages.x / vectors.Length;
			float ay = averages.y / vectors.Length;
			float az = averages.z / vectors.Length;

			return new Vector3(ax, ay, az);
		}

		public static Vector2 ToYPlane(Vector3 v)
		{
			return new Vector2(v.x, v.z);
		}

		public static Rect Lerp(Rect from, Rect to, float frac)
		{
			Rect ret = new Rect();
			frac = Mathf.Clamp(frac, 0f, 1f);
			ret.x = from.x + (to.x - from.x) * frac;
			ret.y = from.y + (to.y - from.y) * frac;
			ret.width = from.width + (to.width - from.width) * frac;
			ret.height = from.height + (to.height - from.height) * frac;
			return ret;
		}

		public static Vector3 Lerp(Vector3 from, Vector3 to, float frac)
		{
			return from + (to - from) * frac;
		}

		public static float Lerp(float from, float to, float frac)
		{
			return from + (to - from) * frac;
		}

		public static Vector3 TriLerp(Vector3 a, Vector3 b, Vector3 c, float t)
		{
			t = Mathf.Clamp01(t);
			if (t < .5f)
			{
				return Vector3.Lerp(a, b, t * 2f);
			}

			return Vector3.Lerp(b, c, (t - .5f) * 2f);
		}

		public static Vector3 TriSlerp(Vector3 a, Vector3 b, Vector3 c, float t)
		{
			t = Mathf.Clamp01(t);
			var fracAB = Vector3.Angle(a, b);
			var fracBC = Vector3.Angle(b, c);
			var total = fracAB + fracBC;
			fracAB /= total;
			fracBC /= total;
			if (t < fracAB)
			{
				return Vector3.Slerp(a, b, t / fracAB);
			}

			return Vector3.Slerp(b, c, (t - fracAB) / fracBC);
		}

		public static Bounds GetEmptyBox()
		{
			Bounds b = new Bounds();
			b.SetMinMax(new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity), new Vector3(Mathf.NegativeInfinity, Mathf.NegativeInfinity, Mathf.NegativeInfinity));
			return b;
		}

		public static Vector2 OrthoProjectPoint(Vector3 projectionNormal, Vector3 up, Vector3 point)
		{
			Vector3 right;
			projectionNormal.ComputeOrthonormalBasis(out up, out right);
			Vector2 ret = Vector2.zero;
			Vector3 pr = Vector3.Project(point, right),
					pu = Vector3.Project(point, up);
			ret.x = pr.magnitude * Mathf.Sign(Vector3.Dot(pr, right));
			ret.y = pu.magnitude * Mathf.Sign(Vector3.Dot(pu, up));
			return ret;
		}

		// Algorithm source: http://geomalgorithms.com/a07-_distance.html
		public static float LineLineDistance(Vector3 l1p1, Vector3 l1p2, Vector3 l2p1, Vector3 l2p2)
		{
			Vector3 u = l1p2 - l1p1;
			Vector3 v = l2p2 - l2p1;
			Vector3 w = l1p1 - l2p1;
			float a = Vector3.Dot(u, u); // always >= 0
			float b = Vector3.Dot(u, v);
			float c = Vector3.Dot(v, v);         // always >= 0
			float d = Vector3.Dot(u, w);
			float e = Vector3.Dot(v, w);
			float D = a * c - b * b;        // always >= 0
			float sc, tc;

			// compute the line parameters of the two closest points
			if (D < Mathf.Epsilon)
			{ // the lines are almost parallel
				sc = 0f;
				tc = (b > c ? d / b : e / c); // use the largest denominator
			}
			else
			{
				sc = (b * e - c * d) / D;
				tc = (a * e - b * d) / D;
			}

			// get the difference of the two closest points
			Vector3 dP = w + (sc * u) - (tc * v);

			return Mathf.Sqrt(Vector3.Dot(dP, dP));   // return the closest distance
		}

		// Algorithm source: http://geomalgorithms.com/a07-_distance.html
		public static float SegmentSegmentDistance(Vector3 l1p1, Vector3 l1p2, Vector3 l2p1, Vector3 l2p2)
		{
			Vector3 u = l1p2 - l1p1;
			Vector3 v = l2p2 - l2p1;
			Vector3 w = l1p1 - l2p1;
			float a = Vector3.Dot(u, u);         // always >= 0
			float b = Vector3.Dot(u, v);
			float c = Vector3.Dot(v, v);         // always >= 0
			float d = Vector3.Dot(u, w);
			float e = Vector3.Dot(v, w);
			float D = a * c - b * b;        // always >= 0
			float sc, sN, sD = D;       // sc = sN / sD, default sD = D >= 0
			float tc, tN, tD = D;       // tc = tN / tD, default tD = D >= 0

			// compute the line parameters of the two closest points
			if (D < Mathf.Epsilon) // the lines are almost parallel
			{
				sN = 0.0f;         // force using point P0 on segment S1
				sD = 1.0f;         // to prevent possible division by 0.0 later
				tN = e;
				tD = c;
			}
			else // get the closest points on the infinite lines
			{
				sN = (b * e - c * d);
				tN = (a * e - b * d);
				if (sN < 0.0f) // sc < 0 => the s=0 edge is visible
				{
					sN = 0.0f;
					tN = e;
					tD = c;
				}
				else if (sN > sD) // sc > 1  => the s=1 edge is visible
				{
					sN = sD;
					tN = e + b;
					tD = c;
				}
			}

			if (tN < 0.0f) // tc < 0 => the t=0 edge is visible
			{
				tN = 0.0f;
				// recompute sc for this edge
				if (-d < 0.0f)
				{
					sN = 0.0f;
				}
				else if (-d > a)
				{
					sN = sD;
				}
				else
				{
					sN = -d;
					sD = a;
				}
			}
			else if (tN > tD)
			{      // tc > 1  => the t=1 edge is visible
				tN = tD;
				// recompute sc for this edge
				if ((-d + b) < 0f)
				{
					sN = 0f;
				}
				else if ((-d + b) > a)
				{
					sN = sD;
				}
				else
				{
					sN = (-d + b);
					sD = a;
				}
			}
			// finally do the division to get sc and tc
			sc = (Mathf.Abs(sN) < Mathf.Epsilon ? 0f : sN / sD);
			tc = (Mathf.Abs(tN) < Mathf.Epsilon ? 0f : tN / tD);

			// get the difference of the two closest points
			Vector3 dP = w + (sc * u) - (tc * v);  // =  S1(sc) - S2(tc)

			return Mathf.Sqrt(Vector3.Dot(dP, dP));   // return the closest distance
		}

		public static float Wrap(float value, float min, float max)
		{
			if (value < min)
			{
				float r = min - value;
				float span = max - min;
				float rw = r % span;
				value = max - rw;
			}
			else if (value >= max)
			{
				float r = value - max;
				float span = max - min;
				float rw = r % span;
				value = min + rw;
			}
			return value;
		}
		public static int Wrap(int value, int min, int max)
		{
			if (value < min)
			{
				int r = min - value;
				int span = max - min;
				int rw = r % span;
				value = max - rw;
			}
			else if (value >= max)
			{
				int r = value - max;
				int span = max - min;
				int rw = r % span;
				value = min + rw;
			}
			return value;
		}

		public static Vector3 MakeRelativeTo(Transform t, Vector3 v)
		{
			return MakeRelativeTo(v, t.right, t.up, t.forward);
		}

		public static Vector3 MakeRelativeTo(Vector3 v, Vector3 right, Vector3 up, Vector3 forward)
		{
			return (right * v.x) + (up * v.y) + (forward * v.z);
		}

		public static int Ceiling(double value)
		{
			return Mathf.CeilToInt((float)value);
		}

		public static double Abs(double value)
		{
			if (value < 0)
			{
				return -value;
			}

			return value;
		}

		public static int Sign(int value)
		{
			if (value < 0)
			{
				return -1;
			}

			if (value > 0)
			{
				return 1;
			}

			return 0;
		}

		public static bool ApproximatelyEqual(Vector3 a, Vector3 b)
		{
			return Mathf.Abs(a.x - b.x) < .01f && Mathf.Abs(a.y - b.y) < .01f && Mathf.Abs(a.z - b.z) < .01f;
		}

		public static bool ApproximatelyEqual(Quaternion a, Quaternion b)
		{
			return Mathf.Abs(a.x - b.x) < .01f && Mathf.Abs(a.y - b.y) < .01f && Mathf.Abs(a.z - b.z) < .01f && Mathf.Abs(a.w - b.w) < .01f;
		}

		public static bool ApproximatelyEqual(Quaternion a, Quaternion b, float threshhold)
		{
			return Mathf.Abs(a.x - b.x) < threshhold && Mathf.Abs(a.y - b.y) < threshhold && Mathf.Abs(a.z - b.z) < threshhold && Mathf.Abs(a.w - b.w) < threshhold;
		}

		public static bool ApproximatelyEqual(Vector3 a, Vector3 b, float threshhold)
		{
			return Mathf.Abs(a.x - b.x) < threshhold && Mathf.Abs(a.y - b.y) < threshhold && Mathf.Abs(a.z - b.z) < threshhold;
		}

		public static bool ApproximatelyEqual(float a, float b, float threshhold)
		{
			return Mathf.Abs(a - b) < threshhold;
		}

		public static float CalculatePitch(Vector3 inputVector)
		{
			float tilt = Vector3.Dot(Vector3.up, inputVector);
			float angle = Vector3.Angle(inputVector.xoz().normalized, inputVector);
			angle *= Mathf.Sign(tilt);
			return angle;
		}

		public static float CalculateYaw(Vector3 inputVector, Vector3? yawZero)
		{
			yawZero = yawZero ?? Vector3.forward;
			return SignedAngle(yawZero.Value, inputVector, Vector3.up);
		}

		#region LineIntersection
		public static Vector2 FindIntersect(Vector2 p1, Vector2 d1, Vector2 p2, Vector2 d2)
		{
			float ua = ((d2.x - d1.x) * (d1.y - p2.y) - (d2.y - p2.y) * (p1.x - p2.x)) / ((d2.y - p2.y) * (d1.x - p1.x) - (d2.x - p2.x) * (d1.y - p1.y));

			return p1 + (ua * (d1 - p1));
		}
		#endregion

		#region interception
		// AN OPTIMIZED VERSION OF THE FORMULA FOUND ON http://www.unifycommunity.com/wiki/index.php?title=Calculating_Lead_For_Projectiles


		//first-order intercept using absolute target position
		public static Vector3 FirstOrderIntercept(Vector3 shooterPosition, Vector3 shooterVelocity, float shotSpeed, Vector3 targetPosition, Vector3 targetVelocity)
		{
			float time;
			return FirstOrderIntercept(shooterPosition, shooterVelocity, shotSpeed, targetPosition, targetVelocity, out time);
		}
		public static Vector3 FirstOrderIntercept(Vector3 shooterPosition, Vector3 shooterVelocity, float shotSpeed, Vector3 targetPosition, Vector3 targetVelocity, out float time)
		{
			Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
			time = FirstOrderInterceptTime(shotSpeed, targetPosition - shooterPosition, targetRelativeVelocity);
			return targetPosition + time * (targetRelativeVelocity);
		}

		//first-order intercept using relative target position
		private static float FirstOrderInterceptTime(float shotSpeed, Vector3 targetRelativePosition, Vector3 targetRelativeVelocity)
		{
			float velocitySquared = targetRelativeVelocity.sqrMagnitude;
			if (velocitySquared < 0.001f)
			{
				return 0f;
			}

			float a = velocitySquared - shotSpeed * shotSpeed;

			//handle similar velocities
			if (Mathf.Abs(a) < 0.001f)
			{
				float t = -targetRelativePosition.sqrMagnitude / (2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition));
				return Mathf.Max(t, 0f);    //don't shoot back in time
			}

			float aa = 2f * a,
					b = 2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition),
					c = targetRelativePosition.sqrMagnitude,
					det = b * b - 2f * aa * c;

			if (det > 0f)
			{
				//determinant > 0; two intercept paths (most common)
				float detSqr = Mathf.Sqrt(det),
						t1 = (-b + detSqr) / aa,
						t2 = (-b - detSqr) / aa;
				if (t1 > 0f)
				{
					return (t2 > 0f)
						? Mathf.Min(t1, t2)     //both are positive
						: t1;                   //only t1 is positive
				}
				else
				{
					return Mathf.Max(t2, 0f);   //don't shoot back in time
				}
			}
			else
			{
				return (det < 0f)
					? 0f                        //determinant < 0; no intercept path
					: Mathf.Max(-b / aa, 0f);   //determinant = 0; one intercept path, pretty much never happens
			}
		}
		#endregion

		#region unit conversions
		public static float FeetToCentimeters(float feet)
		{
			return feet * 30.48f;
		}

		public static float FeetToInches(float feet)
		{
			return feet * 12;
		}

		public static float InchesToCentimeters(float inches)
		{
			return inches * 2.54f;
		}

		public static float CentimetersToFeet(float centimeters)
		{
			return centimeters * 0.0328084f;
		}

		public static float CentimetersToInches(float centimeters)
		{
			return centimeters * 0.393701f;
		}

		/// <summary>
		/// Example:  180 => 5'10
		/// </summary>
		public static string CentimetersToFeetAndInchesFloored(float centimeters)
		{
			int ft = Mathf.FloorToInt(CentimetersToFeet(centimeters));
			int inches = Mathf.FloorToInt(CentimetersToInches(centimeters) - FeetToInches(ft));
			return ft.ToString() + "'" + inches.ToString();
		}

		/// <summary>
		/// Example:  5'10 => 177.8
		/// </summary>
		public static bool FeetAndInchesToCentimeters(string feetAndInches, out float centimeters)
		{
			centimeters = -1;
			string[] values = feetAndInches.Split('\'');
			if (values.Length != 2)
			{
				return false;
			}
			else
			{
				int ft = 0;
				int inches = 0;
				bool success = int.TryParse(values[0], out ft) && int.TryParse(values[1], out inches);
				if (success)
				{
					centimeters = FeetToCentimeters(ft) + InchesToCentimeters(inches);
				}
				return success;
			}
		}
		#endregion
	}
}