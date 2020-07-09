// Copyright 2019 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Misc
{
	public delegate void UpdateCallback(float deltaTime);

	/// <summary>
	/// Interface that allows classes to subscribe to a specific <see cref="UpdateMode"/> which is
	/// useful for when you want to configure the correct update method in the editor.
	/// </summary>
	public interface IUpdater
	{
		void Subscribe(UpdateMode updateMode, UpdateCallback updateCallback, bool unscaled = false);
		void Unsubscribe(UpdateCallback updateCallback);
	}
}