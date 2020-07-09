// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Injection;
using Talespin.Core.Foundation.Storage;

namespace Talespin.Core.Foundation.TimeKeeping
{
	public class CurrentTimeServiceFactory : IDependencyLocator<CurrentTimeService>
	{
		public CurrentTimeService Construct(IDependencyInjector serviceLocator)
		{
			LocalDataManager localDataManager = serviceLocator.Get<LocalDataManager>();
			DataPacket dataPacket = localDataManager.GetDataPacket("timeOffsetData", true);

			return new CurrentTimeService(localDataManager, dataPacket);
		}
	}
}
