// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Events
{
	/// <summary>
	/// Lets you add and remove callbacks. Callbacks are grouped by a key.
	/// </summary>
	/// <typeparam name="Key">A key to group callbacks by for invocation.</typeparam>
	/// <typeparam name="Callback">The callback to be performed.</typeparam>
	public interface ICallbackRegistry<Key, Callback>
	{
		int GetCallbacksAmountForKey(Key key);

		bool ContainsKey(Key key);

		void AddCallback(Key key, Callback callback);

		void RemoveCallback(Key key, Callback callback, bool removeDuplicates = true);

		void RemoveKey(Key key);
	}
}