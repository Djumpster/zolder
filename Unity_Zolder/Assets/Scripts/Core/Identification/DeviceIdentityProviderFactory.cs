// Copyright 2020 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Injection;
using UnityEngine;

namespace Talespin.Core.Foundation.Identification
{
	/// <summary>
	/// A factory for the various device identity providers. Needs to be updated
	/// when a new device identity provider has been added.
	/// </summary>
	public class DeviceIdentityProviderFactory : IDependencyLocator<IDeviceIdentityProvider>
	{
		public IDeviceIdentityProvider Construct(IDependencyInjector serviceLocator)
		{
			if (Application.isEditor)
			{
				return new EditorDeviceIdentityProvider();
			}

			if (Application.platform == RuntimePlatform.Android)
			{
				return new AndroidDeviceIdentityProvider();
			}

			if (Application.platform == RuntimePlatform.WindowsPlayer)
			{
				return new WindowsPlayerDeviceIdentityProvider();
			}

			Debug.LogError($"No identification provider for {Application.platform} specified");
			return new EmptyDeviceIdentityProvider();
		}
	}
}
