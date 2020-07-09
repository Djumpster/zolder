// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Injection;
using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	public class AudioServiceFactory : IDependencyLocator<AudioService>
	{
		public AudioService Construct(IDependencyInjector serviceLocator)
		{
			return new AudioService(Resources.LoadAll<AudioBanks>("Audio"));
		}
	}
}
