// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;

namespace Talespin.Core.Foundation.Events
{
	/// <summary>
	/// Generic class to facilitate event sending.
	/// By design, only supports subscribing to specific events. You cannot subscribe to a base event class and receive
	/// any events that inherit from that base type. 
	///
	/// Updated by Tom Feb 26, 2014: 
	/// Changed entire implementation to use Invoke instead of that nasty Dynamic Invoke
	/// this will improve performance and greatly improve readability of the call-stack when exceptions happen in invoked methods.
	///
	/// Updated by Tom Mar 22, 2017: 
	/// This class is now unit tested. Make sure to run the tests if you make any changes.
	/// </summary>
	public class EventDispatcher : IEventDispatcher
	{
		// EventDispatchers have a name for debugging purposes
		public string Name { get; private set; }

		// the main mapping from event type to callback
		private readonly Dictionary<Type, Action<object>> eventCallbackMap = new Dictionary<Type, Action<object>>();

		// lookup required for being able to unsubscribe our anonymous delegate
		private readonly Dictionary<object, Action<object>> anonymousDelegateLookup = new Dictionary<object, Action<object>>();

		// list of "SubscribeToAny" delegates
		private readonly List<Action<object>> subscribeToAnyList = new List<Action<object>>();

		public EventDispatcher(string dispatcherName)
		{
			Name = dispatcherName;
		}

		public void SubscribeToAnyEvent(Action<object> handler)
		{
			subscribeToAnyList.Add(handler);
		}

		public void UnsubscribeToAnyEvent(Action<object> handler)
		{
			subscribeToAnyList.Remove(handler);
		}

		public void SubscribeToType(Type type, Action handler)
		{
			if (!eventCallbackMap.ContainsKey(type))
			{
				eventCallbackMap.Add(type, delegate { });
			}
			// we need an anonymous delegate to disguise our Action<T> as an Action<object> (see Invoke(...))
			Action<object> func = obj => handler();
			eventCallbackMap[type] += func;
			// we need to store the anonymous delegate in a lookup, to be able to unsubscribe it later 
			if (anonymousDelegateLookup.ContainsKey(handler))
			{
				anonymousDelegateLookup[handler] = func;
			}
			else
			{
				anonymousDelegateLookup.Add(handler, func);
			}
		}

		public void UnsubscribeToType(Type type, Action handler)
		{
			if (eventCallbackMap.ContainsKey(type) && anonymousDelegateLookup.ContainsKey(handler))
			{
				eventCallbackMap[type] -= anonymousDelegateLookup[handler];
				anonymousDelegateLookup.Remove(handler);
				if (eventCallbackMap[type].GetInvocationList().Length == 1)
				{
					eventCallbackMap.Remove(type);
				}
			}
		}

		public void Subscribe<T>(Action<T> handler) where T : IEvent
		{
			Type type = typeof(T);
			if (!eventCallbackMap.ContainsKey(type))
			{
				eventCallbackMap.Add(type, delegate { });
			}
			// we need an anonymous delegate to disguise our Action<T> as an Action<object> (see Invoke(...))
			Action<object> func = obj => handler((T)obj);
			eventCallbackMap[type] += func;
			// we need to store the anonymous delegate in a lookup, to be able to unsubscribe it later 
			if (anonymousDelegateLookup.ContainsKey(handler))
			{
				anonymousDelegateLookup[handler] = func;
			}
			else
			{
				anonymousDelegateLookup.Add(handler, func);
			}
		}

		public void Unsubscribe<T>(Action<T> handler) where T : IEvent
		{
			Type type = typeof(T);
			if (eventCallbackMap.ContainsKey(type) && anonymousDelegateLookup.ContainsKey(handler))
			{
				eventCallbackMap[type] -= anonymousDelegateLookup[handler];
				anonymousDelegateLookup.Remove(handler);
				if (eventCallbackMap[type].GetInvocationList().Length == 1)
				{
					eventCallbackMap.Remove(type);
				}
			}
		}

		public void Invoke<T>(T eventObject) where T : IEvent
		{
			Type type = eventObject.GetType();
			Invoke(type, eventObject);
		}

		public void Invoke(Type type, object eventObject)
		{
			Action<object> handler;

			if (!eventCallbackMap.TryGetValue(type, out handler))
			{
				// put debug message here
			}
			else
			if (handler == null)
			{
				// put debug message here
			}
			else
			{
				// Our anonymous delegate can be invoked!
				// If we didn't use this delegate we would have had to use DynamicInvoke, 
				// which is slow and creates a hideous call stack
				handler.Invoke(eventObject);
			}

			// Also invoke on all objects in de subscribe to all list
			for (int i = 0; i < subscribeToAnyList.Count; i++)
			{
				Action<object> hndl = subscribeToAnyList[i];
				hndl.Invoke(eventObject);
			}
		}

		public void Dispose()
		{
			eventCallbackMap.Clear();
			anonymousDelegateLookup.Clear();
			subscribeToAnyList.Clear();
		}
	}
}