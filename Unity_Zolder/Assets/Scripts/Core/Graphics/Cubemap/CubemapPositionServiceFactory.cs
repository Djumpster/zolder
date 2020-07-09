// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Injection;
using UnityEngine;

namespace Talespin.Core.Foundation.Graphics
{
	public class CubemapPositionServiceFactory : IDependencyLocator<CubemapPositionService>
	{
		public CubemapPositionService Construct(IDependencyInjector serviceLocator)
		{
			return new CubemapPositionService(Resources.Load<CubemapPositionData>("CubemapPositionData"));
		}
	}
}
