// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Events
{
	/// <summary>
	/// Service for subscribing and listening to global messages
	/// </summary>
	public class GlobalEvents : EventDispatcher
	{
		public GlobalEvents(string dispatcherName) : base(dispatcherName)
		{
		}
	}
}