// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Injection;
using UnityEngine;
using UnityEngine.Audio;

namespace Talespin.Core.Foundation.Audio
{
	public class AudioMixerServiceFactory : IDependencyLocator<AudioMixerService>
	{
		public const string MAIN_MIXER_PATH = AudioMixerService.MIXERS_PATH + "Main";

		public AudioMixerService Construct(IDependencyInjector serviceLocator)
		{
			AudioMixer mainMixer = Resources.Load<AudioMixer>(MAIN_MIXER_PATH);

			return new AudioMixerService(mainMixer);
		}
	}
}