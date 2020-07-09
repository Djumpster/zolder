// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Injection;
using UnityEngine;

namespace Talespin.Core.Foundation.Services
{
	public class ICallbackServiceSelectorFactory : IDependencyLocator<ICallbackService>
	{
		public ICallbackService Construct(IDependencyInjector serviceLocator)
		{
			if (Application.isPlaying)
			{
				return serviceLocator.Get<UnityCallbackService>();
			}
			else
			{
				return new MockCallbackService();
			}
		}
	}
}