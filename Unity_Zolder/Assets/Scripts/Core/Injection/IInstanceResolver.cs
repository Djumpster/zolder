// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;

namespace Talespin.Core.Foundation.Injection
{
	/// <summary>
	/// Resolves an instance for the configured type / id pair.
	/// </summary>
	public interface IInstanceResolver : IDisposable
	{
		Type Type { get; }

		string Identifier { get; }

		bool WasResolved { get; }

		object Instance { get; }

		object Resolve(IDependencyInjector injector);

		IInstanceResolver Clone();
	}
}