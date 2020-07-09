// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Injection;

namespace Talespin.Core.Foundation.Services
{
	public class MainThreadServiceFactory : IDependencyLocator<MainThreadService>
	{
		public MainThreadService Construct(IDependencyInjector serviceLocator)
		{
			return new MainThreadService(serviceLocator.Get<UnityCallbackService>());
		}
	}
}
