// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Misc
{
	/// <summary>
	/// Simple interface to implement the Command pattern.
	/// A command is an encapsulated operation that can be performed when required.
	/// It's dependencies should be injected in the constructor.
	/// </summary>
	public interface ICommand
	{
		void Execute();

		void Destroy();
	}
}
