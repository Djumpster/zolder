// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using Talespin.Core.Foundation.Events;
using Talespin.Core.Foundation.Injection;
using UnityEngine;

namespace Talespin.Core.Foundation.Session
{
	/// <summary>
	/// A class that represents the a Gameplay session for the player. This is useful for any game
	/// that has a number of separate levels. Each level playthrough counts as a single session. 
	/// When linked in properly in the flow, the session should start as soon as a level is loading
	/// and ends whenever the player exits the level, either by quitting, winning or losing the level.
	/// </summary>
	public class SessionService : IScopedDependency
	{
		public event LevelHandler OnLevelStartingEvent = delegate { };
		public event LevelHandler OnLevelStartedEvent = delegate { };
		public event LevelHandler OnLevelStoppingEvent = delegate { };
		public event LevelHandler OnLevelStoppedEvent = delegate { };

		/// <summary>
		/// Get the time spend inside of the current session excluding
		/// any time that was spend paused.
		/// </summary>
		/// <value>
		/// A positive floating point value with the amount of seconds
		/// that have passed since the session became active, with the
		/// amount of time spend in a pause state deducted. This will
		/// give an accurate "active" play time.
		/// </value>
		public float TimePlayed
		{
			get
			{
				float endTime = IsSessionRunning ? Time.realtimeSinceStartup : sessionEndTime;

				if (pauseStartTime != -1)
				{
					totalPauseTime = Time.realtimeSinceStartup - pauseStartTime;
					pauseStartTime = Time.realtimeSinceStartup;
				}

				return endTime - sessionStartTime - totalPauseTime;
			}
		}

		public bool IsSessionRunning { private set; get; }

		private readonly GlobalEvents globalEvents;
		private readonly ILevelSelector selector;
		private readonly ILevelLoader loader;
		private readonly ILevelActivator activator;

		private float sessionStartTime;
		private float sessionEndTime;

		private float pauseStartTime;
		private float totalPauseTime;

		public SessionService(GlobalEvents globalEvents, ILevelSelector selector, ILevelLoader loader, ILevelActivator activator)
		{
			this.globalEvents = globalEvents;
			this.selector = selector;
			this.loader = loader;
			this.activator = activator;
		}

		public void Start()
		{
			ILevelData level = selector.CurrentLevel;

			if (level == null)
			{
				throw new NullReferenceException("No level selected");
			}
			
			void OnLevelLoaded(ILevelData _)
			{
				activator.ActivateLevel(level, OnLevelStarted);
			}

			void OnLevelStarted(ILevelData _)
			{
				IsSessionRunning = true;
				sessionStartTime = Time.realtimeSinceStartup;
				sessionEndTime = 0;
				pauseStartTime = -1;
				totalPauseTime = 0;

				OnLevelStartedEvent.Invoke(level);
			}

			globalEvents.Subscribe<PauseEvent>(OnPauseEvent);
			globalEvents.Subscribe<ResumeEvent>(OnResumeEvent);

			OnLevelStartingEvent.Invoke(level);
			loader.Load(level, OnLevelLoaded);
		}

		public void Stop()
		{
			sessionEndTime = Time.realtimeSinceStartup;

			ILevelData level = null;

			void OnLevelStopped(ILevelData l)
			{
				level = l;
				loader.CleanUp(OnLevelUnloaded);
			}

			void OnLevelUnloaded()
			{
				IsSessionRunning = false;
				OnLevelStoppedEvent.Invoke(level);
			}

			globalEvents.Unsubscribe<PauseEvent>(OnPauseEvent);
			globalEvents.Unsubscribe<ResumeEvent>(OnResumeEvent);

			OnLevelStoppingEvent.Invoke(level);
			activator.StopActiveLevel(OnLevelStopped);
		}

		private void OnPauseEvent(PauseEvent evt)
		{
			pauseStartTime = Time.realtimeSinceStartup;
		}

		private void OnResumeEvent(ResumeEvent evt)
		{
			totalPauseTime += Time.realtimeSinceStartup - pauseStartTime;
			pauseStartTime = -1;
		}
	}
}
