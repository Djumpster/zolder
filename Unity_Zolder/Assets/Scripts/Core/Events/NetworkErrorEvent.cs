// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Events
{
	/// <summary>
	/// Event invoked when a networked service experiences a network error.
	/// </summary>
	public class NetworkErrorEvent : IEvent
	{
		public readonly string CallerName;
		public readonly long StatusCode;
		public readonly string Message;

		public NetworkErrorEvent(string callerName, long statusCode, string message)
		{
			CallerName = callerName;
			StatusCode = statusCode;
			Message = message;
		}
	}
}