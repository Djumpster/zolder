// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using Talespin.Core.Foundation.Logging;
using Talespin.Core.Foundation.Services;
using UnityEngine;

namespace Talespin.Core.Foundation.TimeKeeping
{
	/// <summary>
	/// A more expensive and elaborate version of Timer that is not a struct but a class.
	/// Be sure to Dispose() this timer when you're done with it or it might not get GC-ed.
	/// </summary>
	public class FancyTimer
	{
		#region events
		public delegate void TimerTickedHandler(FancyTimer timer, float totalTimePassed, float totalTimeRemaining, int ticksRemaining);
		/// <summary>
		/// Dispatched when a tick has finished.
		/// Also dispatched once the final tick finishes.
		/// </summary>
		public event TimerTickedHandler TimerTickedEvent;

		public event TimerCompletedHandler TimerCompletedEvent;
		public delegate void TimerCompletedHandler(FancyTimer timer);

		public event TimerStartedHandler TimerStartedEvent;
		public delegate void TimerStartedHandler(FancyTimer timer);

		#endregion

		#region properties
		public readonly float Duration;
		public readonly int Ticks;
		public float TotalDuration { get { return Duration * (Ticks); } }

		public float TotalTimePassed
		{
			get
			{
				if (!hasStarted)
				{
					return 0f;
				}
				return Mathf.Clamp(lastRealTimeSinceStartup - startTime - pausedTime, float.MinValue, TotalDuration);
			}
		}
		public float TickTimePassed { get { return TotalTimePassed % Duration; } }

		public float TotalTimeRemaining { get { return TotalDuration - TotalTimePassed; } }
		public float TickTimeRemaining { get { return Duration - TickTimePassed; } }

		/// <summary>
		/// Expressed in value 0f to 1f.
		/// </summary>
		/// <value>The percentage complete.</value>
		public float PercentageComplete { get { return TotalTimePassed / TotalDuration; } }

		public int CurrentTick { get { return Mathf.FloorToInt(TotalTimePassed / Duration); } }

		/// <summary>
		/// Includes the current repeat.
		/// </summary>
		/// <value>The repeats remaining.</value>
		public int TicksRemaining { get { return Ticks - CurrentTick; } }

		/// <summary>
		/// The time this timer started, based on Time.realtimeSinceStartup. 
		/// </summary>
		/// <value>The start time.</value>
		public float StartTime { get { return startTime; } }
		private float startTime;

		/// <summary>
		/// The time this repeat started, based on Time.realtimeSinceStartup.
		/// </summary>
		/// <value>The repeat start time.</value>
		public float TickStartTime { get { return tickStartTime; } }
		private float tickStartTime;

		/// <summary>
		/// Total time spent while the timer was paused.
		/// </summary>
		public float PausedTime
		{
			get
			{
				return pausedTime;
			}
		}
		private float pausedTime = 0f;

		public bool HasStarted { get { return hasStarted; } }
		private bool hasStarted = false;

		public bool HasExpired { get { return hasStarted && TotalTimePassed >= TotalDuration; } }

		public bool IsRunning { get { return !HasExpired && isRunning; } }
		private bool isRunning = false;

		public bool IsPaused { get { return hasStarted && !HasExpired && !IsRunning; } }

		#endregion

		#region members
		private ICallbackService unityCallbackService;

		private float lastRealTimeSinceStartup = 0f;

		private bool manualPause = false;
		#endregion

		#region constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="Talespin.Core.FancyTimer"/> class.
		/// </summary>
		/// <param name="duration">The duration in seconds.</param>
		/// <param name="ticks">The amount of times the timer should run for the given duration before completing.</param>
		/// <param name="unityCallbackService">unityCallbackService</param>
		public FancyTimer(float duration, int ticks, ICallbackService unityCallbackService)
		{
			if (duration <= 0)
			{
				throw new ArgumentException("duration must be > 0.");
			}
			if (ticks < 1)
			{
				throw new ArgumentException("ticks must be >= 1.");
			}
			if (unityCallbackService == null)
			{
				throw new ArgumentNullException("unityCallbackService cannot be null.");
			}

			Duration = duration;
			Ticks = ticks;
			this.unityCallbackService = unityCallbackService;

			this.unityCallbackService.UpdateEvent += OnUpdateEvent;
			this.unityCallbackService.ApplicationPauseEvent += OnApplicationPauseEvent;
			this.unityCallbackService.ApplicationFocusEvent += OnApplicationFocusEvent;
		}

		#endregion

		#region public methods
		/// <summary>
		/// Use this to Start the timer or to resume the timer after pausing.
		/// If the timer has expired you will need to Reset() it first.
		/// </summary>
		public void Start()
		{
			if (HasExpired)
			{
				LogUtil.Warning(LogTags.SYSTEM, this, "Attempting to Start while expired, please reset first.");
			}

			manualPause = false;

			if (!hasStarted)
			{
				startTime = Time.realtimeSinceStartup;
				tickStartTime = Time.realtimeSinceStartup;


				unityCallbackService.UpdateEvent -= OnUpdateEvent;
				unityCallbackService.ApplicationPauseEvent -= OnApplicationPauseEvent;
				unityCallbackService.ApplicationFocusEvent -= OnApplicationFocusEvent;

				unityCallbackService.UpdateEvent += OnUpdateEvent;
				unityCallbackService.ApplicationPauseEvent += OnApplicationPauseEvent;
				unityCallbackService.ApplicationFocusEvent += OnApplicationFocusEvent;
			}

			lastRealTimeSinceStartup = Time.realtimeSinceStartup;

			hasStarted = true;
			isRunning = true;

			DispatchTimerStarted();
		}

		public void Pause()
		{
			if (isRunning)
			{
				// add the time before this call was made and then pause.
				UpdateTime();

				isRunning = false;

				manualPause = true;
			}
		}

		public void Stop()
		{
			hasStarted = false;
			isRunning = false;
			manualPause = false;
			pausedTime = 0f;
			startTime = 0f;
			tickStartTime = 0f;

			unityCallbackService.UpdateEvent -= OnUpdateEvent;
			unityCallbackService.ApplicationPauseEvent -= OnApplicationPauseEvent;
			unityCallbackService.ApplicationFocusEvent -= OnApplicationFocusEvent;
		}

		/// <summary>
		/// Resets the timer. Does not Stop() or unpause the timer.
		/// </summary>
		public void Reset()
		{
			pausedTime = 0f;
			startTime = 0f;
			tickStartTime = 0f;
			manualPause = false;

			if (hasStarted)
			{
				startTime = Time.realtimeSinceStartup;
				tickStartTime = Time.realtimeSinceStartup;
				lastRealTimeSinceStartup = Time.realtimeSinceStartup;
			}
		}

		public void Dispose()
		{
			unityCallbackService.UpdateEvent -= OnUpdateEvent;
			unityCallbackService.ApplicationPauseEvent -= OnApplicationPauseEvent;
			unityCallbackService.ApplicationFocusEvent -= OnApplicationFocusEvent;

			unityCallbackService = null;
		}

		#endregion

		#region private methods
		private void PerformPause()
		{
			if (isRunning)
			{
				// add the time before this call was made and then pause.
				UpdateTime();

				isRunning = false;
			}
		}

		private void UpdateTime()
		{
			float timeSinceStartup = Time.realtimeSinceStartup;
			float timePassed = timeSinceStartup - lastRealTimeSinceStartup;
			bool isPaused = IsPaused;
			lastRealTimeSinceStartup = timeSinceStartup;

			if (isPaused)
			{
				pausedTime += timePassed;
			}
			else if (isRunning)
			{
				if (TickTimePassed < timePassed)
				{
					DispatchTimerTickedEvent();
				}
				if (HasExpired)
				{
					isRunning = false;
					DispatchTimerCompletedEvent();
				}
			}
		}

		private void DispatchTimerStarted()
		{
			TimerStartedEvent?.Invoke(this);
		}

		private void DispatchTimerTickedEvent()
		{
			TimerTickedEvent?.Invoke(this, TotalTimePassed, TotalTimeRemaining, TicksRemaining);
		}

		private void DispatchTimerCompletedEvent()
		{
			TimerCompletedEvent?.Invoke(this);
		}

		private void OnUpdateEvent()
		{
			if (isRunning)
			{
				UpdateTime();
			}
		}

		#endregion

		#region Unity callbacks
		private void OnApplicationPauseEvent(bool paused)
		{
			if (paused && !IsPaused && isRunning)
			{
				PerformPause();
			}
			else if (!paused && IsPaused && !manualPause)
			{
				UpdateTime();
				Start();
			}
		}

		private void OnApplicationFocusEvent(bool focus)
		{
			if (!focus && !IsPaused && isRunning)
			{
				PerformPause();
			}
			else if (focus && IsPaused && !manualPause)
			{
				UpdateTime();
				Start();
			}
		}

		#endregion

		#region operators
		public static implicit operator bool(FancyTimer m)
		{
			return !m.HasExpired;
		}

		#endregion
	}
}