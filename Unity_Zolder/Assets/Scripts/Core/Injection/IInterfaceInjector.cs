// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;

namespace Talespin.Core.Foundation.Injection
{
	public interface IInterfaceInjector : IInjectorModule
	{
		T GetModule<T>() where T : class, IInjectorModule;

		void AddModules(IEnumerable<IInjectorModule> modules);

		void AddModule(IInjectorModule module);

		void InsertModules(int index, IEnumerable<IInjectorModule> modules);

		void InsertModule(int index, IInjectorModule module);

		int RemoveModule(IInjectorModule module);
	}
}