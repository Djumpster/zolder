// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Injection;
using Talespin.Core.Foundation.Storage;

namespace Talespin.Core.Foundation.Tracking
{
	public class StatLoggerServiceFactory : IDependencyLocator<StatLoggerService>
	{
		public StatLoggerService Construct(IDependencyInjector serviceLocator)
		{
			LocalDataManager localDataManager = serviceLocator.Get<LocalDataManager>();
			DataPacket loggedStats = localDataManager.GetDataPacket("loggedStats", true);

			return new StatLoggerService(loggedStats, localDataManager);
		}
	}
}
