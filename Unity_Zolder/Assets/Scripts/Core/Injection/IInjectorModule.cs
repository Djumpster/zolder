// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;

namespace Talespin.Core.Foundation.Injection
{
	public interface IInjectorModule : IDisposable
	{
		void Inject(object o);
		void Remove(object o);
		string Log();
		IInjectorModule Clone();
	}
}