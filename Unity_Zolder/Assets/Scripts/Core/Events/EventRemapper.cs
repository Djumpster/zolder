// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using Talespin.Core.Foundation.Reflection;

namespace Talespin.Core.Foundation.Events
{
	/// <summary>
	/// The EventRemapper is used specifically by layers in a state machine to remap events fired on
	/// that layer to lower layers and potentially map them to another event type.
	///
	/// There are probably not many other scenarios where this very specific class is useful.
	/// The ForwardFilteredEvents function is unit tested.
	/// </summary>
	public class EventRemapper : IEventDispatcher, IDisposable
	{
		public readonly IEventDispatcher SourceDispatcher;
		public readonly IEventDispatcher DestinationDispatcher;
		private Stack<object> eventStack = new Stack<object>();

		public string Name
		{
			get;
			private set;
		}

		public EventRemapper(string name, IEventDispatcher mapEventsTo)
		{
			Name = name;
			SourceDispatcher = new EventDispatcher(name);
			DestinationDispatcher = mapEventsTo;
		}

		public void SubscribeToAnyEvent(Action<object> handler)
		{
			SourceDispatcher.SubscribeToAnyEvent(handler);
		}

		public void UnsubscribeToAnyEvent(Action<object> handler)
		{
			SourceDispatcher.UnsubscribeToAnyEvent(handler);
		}

		public void SubscribeToType(Type type, Action handler)
		{
			SourceDispatcher.SubscribeToType(type, handler);
		}

		public void UnsubscribeToType(Type type, Action handler)
		{
			SourceDispatcher.UnsubscribeToType(type, handler);
		}

		public void Subscribe<T>(Action<T> handler) where T : IEvent
		{
			SourceDispatcher.Subscribe(handler);
		}

		public void Unsubscribe<T>(Action<T> handler) where T : IEvent
		{
			SourceDispatcher.Unsubscribe(handler);
		}

		public void Invoke<T>(T eventObject) where T : IEvent
		{
			eventStack.Push(eventObject);
			SourceDispatcher.Invoke(eventObject);
		}

		public void Invoke(Type type, object eventObject)
		{
			eventStack.Push(eventObject);
			SourceDispatcher.Invoke(type, eventObject);
		}

		public void ForwardFilteredEvents(TypeMap filter)
		{
			while (eventStack.Count > 0)
			{
				Type evtType = eventStack.Pop().GetType();
				Type newType = filter.Map(evtType);
				if (newType != null)
				{
					DestinationDispatcher.Invoke(newType, Reflect.Instantiate(newType.ToString()));
					eventStack.Clear();
				}
			}
		}

		public void Dispose()
		{
			SourceDispatcher.Dispose();
		}
	}
}