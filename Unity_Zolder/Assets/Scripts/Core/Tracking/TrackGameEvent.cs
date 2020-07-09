// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using Talespin.Core.Foundation.Events;

namespace Talespin.Core.Foundation.Tracking
{
	/// <summary>
	/// Listens to game events and updates the tracker based on changes.
	/// </summary>
	public abstract class TrackGameEvent<T> : ITrackGameEvent where T : IEvent
	{
		/// <summary>
		/// Is the event currently being listened to and the changes tracked?
		/// </summary>
		/// <value><see langword="true" /> if is listening; otherwise, <see langword="false" />.</value>
		public bool isListening { get; protected set; }

		protected GlobalEvents globalEvents;

		protected StatLoggerService statLogger;

		protected TrackGameEvent(GlobalEvents globalEvents, StatLoggerService statLogger)
		{
			if (globalEvents == null)
			{
				throw new NullReferenceException("globalEvents cannot be null.");
			}
			if (statLogger == null)
			{
				throw new NullReferenceException("statTracker cannot be null.");
			}

			this.globalEvents = globalEvents;
			this.statLogger = statLogger;

			StartListening();
		}

		/// <summary>
		/// Starts listening to the global events. On by default.
		/// </summary>
		public virtual void StartListening()
		{
			if (isListening)
			{
				return;
			}

			isListening = true;

			globalEvents.Subscribe<T>(ProcessEvent);
		}

		/// <summary>
		/// Stop listenting to the global events.
		/// </summary>
		public virtual void StopListening()
		{
			if (!isListening)
			{
				return;
			}

			isListening = false;

			globalEvents.Unsubscribe<T>(ProcessEvent);
		}

		public virtual void Destroy()
		{
			StopListening();

			globalEvents = null;
			statLogger = null;
		}

		protected abstract void ProcessEvent(T trackedEvent);
	}
}
