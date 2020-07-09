// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Injection
{
	/// <summary>
	/// A type that can start and stop and therefore has a "scope".
	/// </summary>
	public interface IScopedDependency
	{
		void Start();
		void Stop();
	}
}