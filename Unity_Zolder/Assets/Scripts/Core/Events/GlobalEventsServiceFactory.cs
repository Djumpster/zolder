// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Injection;

namespace Talespin.Core.Foundation.Events
{
	public class GlobalEventsServiceFactory : IDependencyLocator<GlobalEvents>
	{
		public GlobalEvents Construct(IDependencyInjector serviceLocator)
		{
			return new GlobalEvents("GlobalEvents");
		}
	}
}