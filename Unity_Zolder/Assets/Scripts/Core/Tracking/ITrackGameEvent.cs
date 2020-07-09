// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Tracking
{
	/// <summary>
	/// Listens to events and updates the stat logger based on changes.
	/// </summary>
	public interface ITrackGameEvent
	{
		/// <summary>
		/// Is the event currently being listened to and the changes tracked?
		/// </summary>
		/// <value><see langword="true" /> if is listening; otherwise, <see langword="false" />.</value>
		bool isListening { get; }

		/// <summary>
		/// Starts listening to the global events. On by default.
		/// </summary>
		void StartListening();

		/// <summary>
		/// Stop listenting to the global events.
		/// </summary>
		void StopListening();

		void Destroy();
	}
}
