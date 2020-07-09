// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	[Serializable]
	public struct DAHDSR
	{
		[SerializeField] public double Delay;
		[SerializeField] public double Attack;
		[SerializeField] public double Hold;
		[SerializeField] public double Decay;
		[SerializeField, Range(0, 1)] public float Sustain;
		[SerializeField] public double Release;

		[NonSerialized] public double? ReleaseStartTime;
		private double attackStartTime;
		private double holdStartTime;
		private double decacyStartTime;
		private double sustainStartTime;

		private bool calculatedStartTimes;

		// NOTE: the default values are only set if you call this specific constructor and NOT the empty constructor C# generates, therefore 'new DAHDSR();' will yield an envelope with sustain 0
		public DAHDSR(double delay = 0f, double attack = 0f, double hold = 0f, double decay = 0f, float sustain = 1f, double release = 0f)
		{
			Delay = delay;
			Attack = attack;
			Hold = hold;
			Decay = decay;
			Sustain = sustain;
			Release = release;

			attackStartTime = 0f;
			holdStartTime = 0f;
			decacyStartTime = 0f;
			sustainStartTime = 0f;
			ReleaseStartTime = null;

			calculatedStartTimes = false;
		}

		public float Evaluate(float time)
		{
			if (!calculatedStartTimes)
			{
				CalculateStartTimes();
			}

			if (ReleaseStartTime != null) // Release
			{
				// Debug.LogFormat("<color=lightblue>Release: {0}, {1}</color>", time, ReleaseStartTime);
				float t = Mathf.InverseLerp((float)ReleaseStartTime.Value, (float)ReleaseStartTime.Value + (float)Release, time);
				return Mathf.Lerp(Sustain, 0f, t);
			}

			if (time < attackStartTime) // Delay
			{
				// Debug.LogFormat("<color=red>Delay: {0}</color>", time);

				// Determine at what volume we initiate at. If this is wrong, it results in the first bit being 
				// chopped of if we initiate at 0.
				// Is attack set? We start at 0.
				// Else, are decay or hold set? We start at 1.
				// If none are set, we start at sustain.

				return Attack > 0 ? 0 : Decay > 0 || Hold > 0 ? 1 : Sustain;
			}

			if (time < holdStartTime) // Attack
			{
				// Debug.LogFormat("<color=orange>Attack: {0}</color>", time);
				float t = Mathf.InverseLerp((float)attackStartTime, (float)holdStartTime, time);
				return Mathf.Lerp(0, 1, t);
			}

			if (time < decacyStartTime) // Hold
			{
				// Debug.LogFormat("<color=maroon>Hold: {0}</color>", time);
				return 1f;
			}

			if (time < sustainStartTime) // Decay
			{
				// Debug.LogFormat("<color=yellow>Decay: {0}</color>", time);
				float t = Mathf.InverseLerp((float)decacyStartTime, (float)sustainStartTime, time);
				return Mathf.Lerp(1, Sustain, t);
			}

			// Debug.LogFormat("<color=green>Sustain: {0}</color>", time);
			return Sustain;
		}

		private void CalculateStartTimes()
		{
			attackStartTime = Delay;
			holdStartTime = attackStartTime + Attack;
			decacyStartTime = holdStartTime + Hold;
			sustainStartTime = decacyStartTime + Decay;

			calculatedStartTimes = true;
		}
	}
}