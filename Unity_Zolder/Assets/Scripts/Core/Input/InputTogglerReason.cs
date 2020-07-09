// Copyright 2019 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.InputHandling
{
	/// <summary>
	/// Provides a means for distinguishing between different types input toggles.
	/// Some UI elements require this reason to perform different actions based on the input toggle reasoning.
	/// </summary>
	public enum InputTogglerReason
	{
		None = 0,
		LayerTransition = 1,
		StateTransition = 2,
		ObjectRemoved = 3
	}
}