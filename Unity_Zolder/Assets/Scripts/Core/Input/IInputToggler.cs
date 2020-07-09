// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.InputHandling
{
	public interface IInputToggler
	{
		void EnableInput(InputTogglerReason reason);
		void DisableInput(InputTogglerReason reason);
	}
}