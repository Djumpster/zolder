// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Injection;
using Talespin.Core.Foundation.Services;
using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	public class AudioMixerMacroServiceFactory : IDependencyLocator<AudioMixerMacroService>
	{
		public AudioMixerMacroService Construct(IDependencyInjector serviceLocator)
		{
			AudioMixerMacroMapping macroMapping = Resources.Load<AudioMixerMacroMapping>("Audio/AudioMixerMacroMapping");

			return new AudioMixerMacroService(macroMapping, serviceLocator.Get<AudioMixerService>(), serviceLocator.Get<UnityCallbackService>(), serviceLocator.Get<CoroutineService>());
		}
	}
}