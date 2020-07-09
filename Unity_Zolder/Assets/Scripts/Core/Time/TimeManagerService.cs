// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Talespin.Core.Foundation.TimeKeeping
{
	public class TimeManagerService : IDisposable
	{
		public event Action<float> TimeScaleUpdatedEvent = delegate { };

		public bool IsPaused => Time.timeScale == 0;

		private readonly Dictionary<object, float> timeModifiers;
		private readonly float baseFixedDeltaTime;

		public TimeManagerService()
		{
			timeModifiers = new Dictionary<object, float>();
			baseFixedDeltaTime = Time.fixedDeltaTime;
		}

		public void Dispose()
		{
			Time.timeScale = 1;
			Time.fixedDeltaTime = baseFixedDeltaTime;

			TimeScaleUpdatedEvent.Invoke(1);
		}

		/// <summary>
		/// Pauses the game. Any object/behaviour that needs to pause the game can call this method.
		/// </summary>
		/// <param name="context">The object requesting the game to pause.</param>
		public void AddPauseTimeModifier(object context) => AddTimeModifier(context, 0);

		/// <summary>
		/// Any script wishing to adjust the global time scale can call this function.
		/// </summary>
		/// <param name="context">The object requesting the time adjustment.</param>
		/// <param name="timeScale">the desired time scale fraction.</param>
		public void AddTimeModifier(object context, float timeScale)
		{
			timeModifiers[context] = timeScale;
			AdjustTimeScale();
		}

		/// <summary>
		/// Any script that is no longer interested in modifying the global time scale needs to unregister 
		/// by calling this method.
		/// </summary>
		/// <param name="context">The object that is no longer interested in modifying the time scale</param>
		public void RemoveTimeModifier(object context)
		{
			if (timeModifiers.Remove(context))
			{
				AdjustTimeScale();
			}
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder("[TimeManager] modifiers:\n");

			foreach (KeyValuePair<object, float> entry in timeModifiers)
			{
				builder.Append(entry.Key);
				builder.Append(": ");
				builder.Append(entry.Value);
				builder.Append("\n");
			}

			return builder.ToString();
		}

		private void AdjustTimeScale()
		{
			float timeScale = 1f;

			// first determine the highest timeScale
			foreach (KeyValuePair<object, float> kvp in timeModifiers)
			{
				timeScale = Mathf.Max(kvp.Value, timeScale);
			}

			// Then pick the lowest
			foreach (KeyValuePair<object, float> kvp in timeModifiers)
			{
				timeScale = Mathf.Min(kvp.Value, timeScale);
			}

			Time.timeScale = timeScale;
			Time.fixedDeltaTime = baseFixedDeltaTime * timeScale;

			TimeScaleUpdatedEvent.Invoke(timeScale);
		}
	}
}
