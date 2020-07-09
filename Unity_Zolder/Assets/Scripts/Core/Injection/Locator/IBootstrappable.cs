// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Injection
{
	/// <summary>
	/// Implementing this interface will make the dependency available for bootstrapping.
	/// Add the service to the "Bootstrapped Dependencies" dropdown in any state machine.
	/// </summary>
	public interface IBootstrappable
	{
	}
}
