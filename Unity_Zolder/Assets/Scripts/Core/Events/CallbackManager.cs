// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using Talespin.Core.Foundation.Logging;

namespace Talespin.Core.Foundation.Events
{
	/// <summary>
	/// Lets you add, remove and invoke callbacks. Callbacks are grouped by a key.
	/// Useful if you have a method with a callback parameter that can be called multiple times before invoking the
	/// callbacks, in that case you can use this to queue the callbacks.
	/// You can also use this instead of an event if you only want to listen if the event has a specific
	/// parameter value in which case you can use that specific parameter value as the Key for this class.
	/// 
	/// You can expose this object as ICallbackRegistry<Key, Callback> to allow adding and removing but not invoking.
	/// </summary>
	/// <typeparam name="Key">A key to group callbacks by for invocation.</typeparam>
	/// <typeparam name="Callback">The callback to be performed.</typeparam>
	public class CallbackManager<Key, Callback> : ICallbackRegistry<Key, Callback>, IDisposable
	{
		private Dictionary<Key, List<Callback>> callbacks = new Dictionary<Key, List<Callback>>();

		private readonly bool allowDuplicateCallbacks;

		public CallbackManager(bool allowDuplicateCallbacks = false)
		{
			this.allowDuplicateCallbacks = allowDuplicateCallbacks;
		}

		public int GetCallbacksAmountForKey(Key key)
		{
			return callbacks.ContainsKey(key) ? callbacks[key].Count : 0;
		}

		public bool ContainsKey(Key key)
		{
			return callbacks.ContainsKey(key);
		}

		public void AddCallback(Key key, Callback callback)
		{
			if (!callbacks.ContainsKey(key))
			{
				callbacks.Add(key, new List<Callback>() { callback });
			}
			else if (!allowDuplicateCallbacks && callbacks[key].Contains(callback))
			{
				LogUtil.Warning(LogTags.SYSTEM, this, "Attempted to assign a duplicate callback which was not allowed," +
					" ignoring.");
			}
			else
			{
				callbacks[key].Add(callback);
			}
		}

		public void RemoveCallback(Key key, Callback callback, bool removeDuplicates = true)
		{
			if (callbacks == null || !callbacks.ContainsKey(key))
			{
				return;
			}

			List<Callback> callbackList = callbacks[key];
			while (callbackList.Contains(callback))
			{
				callbackList.Remove(callback);
				if (!removeDuplicates)
				{
					break;
				}
			}

			if (callbackList.Count == 0)
			{
				callbacks.Remove(key);
			}
		}

		public void InvokeForKey(Key key, Action<Callback> callbackPerformer, bool removeCallbacks)
		{
			if (!callbacks.ContainsKey(key))
			{
				return;
			}

			List<Callback> callbacksCopy = callbacks[key];
			if (removeCallbacks)
			{
				// do this to ensure any new subscribes handled in a callback do not get invoked while processing
				// the callbacks here.
				callbacksCopy = new List<Callback>(callbacks[key]);
				callbacks.Remove(key);
			}

			for (int i = 0; i < callbacksCopy.Count; i++)
			{
				Callback callback = callbacksCopy[i];
				callbackPerformer(callback);
			}
		}

		public void RemoveKey(Key key)
		{
			callbacks.Remove(key);
		}

		public void Dispose()
		{
			callbacks = null;
		}

		public void Clear()
		{
			callbacks.Clear();
		}
	}
}
