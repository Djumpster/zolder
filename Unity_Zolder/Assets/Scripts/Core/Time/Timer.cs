// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.TimeKeeping
{
	/// <summary>
	/// A struct that can be used for creating a quick timer that is used to measure the progress of time.
	/// It is typically used within animation code and as a struct is not thrashing memory when used.
	/// The struct is written to reduce the amount of code used in tracking time. Therefore, the timer starts
	/// immediately when creating the timer. 
	/// </summary>
	public struct Timer
	{
		public readonly float startTime;
		public readonly float duration;
		public readonly bool useRealTime;
		private float durationModifier;

		public Timer(float dur) : this(dur, false) { }
		public Timer(bool useRealTimeSinceStartup) : this(0f, useRealTimeSinceStartup) { }
		public Timer(float dur, float startOffset) : this(dur, startOffset, false) { }

		public Timer(float dur, bool useRealTimeSinceStartup)
		{
			useRealTime = useRealTimeSinceStartup;
			startTime = useRealTime ? Time.realtimeSinceStartup : Time.time;
			duration = dur;
			durationModifier = 0f;
		}

		public Timer(float dur, float startOffset, bool useRealTimeSinceStartup)
		{
			useRealTime = useRealTimeSinceStartup;
			startTime = (useRealTime ? Time.realtimeSinceStartup : Time.time) - startOffset;
			duration = dur;
			durationModifier = 0f;
		}

		private float currentTime
		{
			get
			{
				return useRealTime ? Time.realtimeSinceStartup : Time.time;
			}
		}

		public float time
		{
			get
			{
				return currentTime - startTime;
			}
		}

		public float remaining
		{
			get
			{
				return (duration + durationModifier) - time;
			}
		}

		public float progress
		{
			get
			{
				return Mathf.Clamp01((duration + durationModifier) == 0 ? 1f : time / (duration + durationModifier));
			}
		}

		public bool expired
		{
			get
			{
				return currentTime >= startTime + (duration + durationModifier);
			}
		}

		public void SubtractDuration(float time)
		{
			durationModifier -= time;
		}
		public void AddDuration(float time)
		{
			durationModifier += time;
		}

		public static implicit operator bool(Timer m)
		{
			return !m.expired;
		}
	}
}
