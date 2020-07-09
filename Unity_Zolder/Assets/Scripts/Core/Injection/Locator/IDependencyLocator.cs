// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Injection
{
	/// <summary>
	/// Implementing classes will be picked up by <see cref="BaseDependencyLocator"/> instances who
	/// will use them to resolve dependencies.
	/// </summary>
	/// <typeparam name="T">The type this factory will provide.</typeparam>
	public interface IDependencyLocator<out T> : Internal.IDependencyLocator
	{
		T Construct(IDependencyInjector serviceLocator);
	}

	namespace Internal
	{
		public interface IDependencyLocator
		{
		}
	}
}
