// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Extensions
{
	public static class QuaternionExtensions
	{
		public static void GetYawPitchRoll(this Quaternion q, out float yaw, out float pitch, out float roll)
		{
			int A1 = 1; // Axis_Y = 1
			int A2 = 0; // Axis_X = 0
			int A3 = 2; //  Axis_Z = 2
			float D = 1.0f; // Rotate_CCW = 1, Rotate_CW  = -1 
			float S = 1.0f; //Handed_R = 1, Handed_L = -1

			float[] Q = new float[3] { q.x, q.y, q.z };  //Quaternion components x,y,z

			float ww = q.w * q.w;
			float Q11 = Q[A1] * Q[A1];
			float Q22 = Q[A2] * Q[A2];
			float Q33 = Q[A3] * Q[A3];

			float psign = -1.0f;
			// Determine whether even permutation
			if (((A1 + 1) % 3 == A2) && ((A2 + 1) % 3 == A3))
			{
				psign = 1;
			}

			float s2 = psign * 2.0f * (psign * q.w * Q[A2] + Q[A1] * Q[A3]);

			float singularityRadius = 1e-7f;
			if (s2 < -1.0f + singularityRadius)
			{ // South pole singularity
				yaw = 0.0f;
				pitch = -S * D * (Mathf.PI * 0.5f);
				roll = S * D * (Mathf.Atan2(2.0f * (psign * Q[A1] * Q[A2] + q.w * Q[A3]), ww + Q22 - Q11 - Q33));
			}
			else if (s2 > 1.0f - singularityRadius)
			{  // North pole singularity
				yaw = 0.0f;
				pitch = S * D * (Mathf.PI * 0.5f);
				roll = S * D * (Mathf.Atan2(2.0f * (psign * Q[A1] * Q[A2] + q.w * Q[A3]), ww + Q22 - Q11 - Q33));
			}
			else
			{
				yaw = -S * D * (Mathf.Atan2(-2.0f * (q.w * Q[A1] - psign * Q[A2] * Q[A3]), ww + Q33 - Q11 - Q22));
				pitch = S * D * (Mathf.Asin(s2));
				roll = S * D * (Mathf.Atan2(2.0f * (q.w * Q[A3] - psign * Q[A1] * Q[A2]), ww + Q11 - Q22 - Q33));
			}
		}
	}
}
