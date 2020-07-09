// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Injection;
using Talespin.Core.Foundation.Services;
using Talespin.Core.Foundation.Storage;

namespace Talespin.Core.Foundation.Tracking
{
	public class TimePlayedTrackerServiceFactory : IDependencyLocator<TimePlayedTrackerService>
	{
		public TimePlayedTrackerService Construct(IDependencyInjector serviceLocator)
		{
			return new TimePlayedTrackerService(
				serviceLocator.Get<LocalDataManager>(),
				serviceLocator.Get<LocalDataManager>().GetDataPacket("time", true),
				serviceLocator.Get<CoroutineService>());
		}
	}
}
