// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Events
{
	public class EventBridge : System.IDisposable
	{
		public readonly IEventDispatcher SourceDispatcher, DestinationDispatcher;
		public EventBridge(IEventDispatcher from, IEventDispatcher to)
		{
			SourceDispatcher = from;
			DestinationDispatcher = to;

			SourceDispatcher.SubscribeToAnyEvent(ReceiveEvent);
		}

		public void Dispose()
		{
			SourceDispatcher.UnsubscribeToAnyEvent(ReceiveEvent);
		}

		void ReceiveEvent(object evt)
		{
			DestinationDispatcher.Invoke(evt.GetType(), evt);
		}
	}
}