// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;

namespace Talespin.Core.Foundation.Events
{
	/// <summary>
	/// Generic interface for event dispatching.
	/// </summary>
	public interface IEventDispatcher : IDisposable
	{
		/// <summary>
		/// Channels have a name, primarily for debugging purposes.
		/// </summary>
		/// <value>The name of this dispatcher.</value>
		string Name { get; }

		/// <summary>
		/// Subscribes an action to any event that is invoked on this dispatcher.
		/// </summary>
		/// <param name="handler">The function that is called whenever an event is invoked on this dispatcher. 
		/// The object parameter is the event being fired.</param>
		void SubscribeToAnyEvent(Action<object> handler);
		/// <summary>
		/// Unsubscribes an action from all events that are invoked on this dispatcher.
		/// </summary>
		/// <param name="handler">The function that is removed as a listener (should be the same as the one that was subscribed!).</param>	
		void UnsubscribeToAnyEvent(Action<object> handler);

		/// <summary>
		/// Subscribes an action to a specific event that is invoked on this dispatcher.
		/// </summary>
		/// <param name="handler">The function that is called whenever the specific event is invoked on this dispatcher. 
		/// The action parameter of type T is the event being fired.</param>
		void Subscribe<T>(Action<T> handler) where T : IEvent;
		/// <summary>
		/// Unsubscribes an action from  a specific event that is invoked on this dispatcher.
		/// </summary>
		/// <param name="handler">The function that is removed as a listener (should be the same as the one that was subscribed!).</param>	
		void Unsubscribe<T>(Action<T> handler) where T : IEvent;

		/// <summary>
		/// Subscribes an action to an event of a specific type that is invoked on this dispatcher.
		/// </summary>
		/// <param name = "type">The type of the event to subscribe to.</param>
		/// <param name="handler">The function that is called whenever the specific event is invoked on this dispatcher. 
		/// The action parameter of type T is the event being fired.</param>
		void SubscribeToType(Type type, Action handler);
		/// <summary>
		/// Unsubscribes an action from an event of a specific type that is invoked on this dispatcher.
		/// </summary>
		/// <param name = "type">The type of the event to unsubscribe from.</param>
		/// <param name="handler">The function that is removed as a listener (should be the same as the one that was subscribed!).</param>	
		void UnsubscribeToType(Type type, Action handler);

		/// <summary>
		/// Invoke an event on this dispatcher.
		/// </summary>
		/// <param name = "eventObject">The instance of event that will be dispatched to all listeners.</param>
		void Invoke<T>(T eventObject) where T : IEvent;
		/// <summary>
		/// Invoke an event on this dispatcher. This variant is useful for classes that do not have the type as
		/// a generic parameter.
		/// </summary>
		/// <param name = "type">The type of the event to invoke.</param>
		/// <param name = "eventObject">The instance of event that will be dispatched to all listeners.</param>
		void Invoke(Type type, object eventObject);
	}
}