// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Injection;

namespace Talespin.Core.Foundation.Services
{
	public class ConsoleViewerFactory : IDependencyLocator<ConsoleViewer>
	{
		public ConsoleViewer Construct(IDependencyInjector serviceLocator)
		{
			return new ConsoleViewer(serviceLocator.Get<ICallbackService>());
		}
	}
}