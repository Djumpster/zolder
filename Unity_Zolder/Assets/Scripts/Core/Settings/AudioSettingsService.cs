// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections;
using Talespin.Core.Foundation.Audio;
using Talespin.Core.Foundation.Services;
using UnityEngine;

namespace Talespin.Core.Foundation.Settings
{
	public class AudioSettingsService : SettingsService<AudioSettingsService.AudioMode>
	{
		public enum AudioMode : int
		{
			OFF = 0,
			ON = 1,
			SFX_ONLY = 2,
		}

		private AudioMixerService audioMixerService;
		private ICoroutineService coroutineService;
		private object applyWaitContext = new object();

		public AudioSettingsService(AudioMixerService audioMixerService, ICoroutineService coroutineService) : base(false)
		{
			this.audioMixerService = audioMixerService;
			this.coroutineService = coroutineService;

			ApplyMode((AudioMode)mode);
		}

		protected override string Identifier
		{
			get
			{
				return "set_audio";
			}
		}

		protected override AudioMode DefaultValue
		{
			get
			{
				return AudioMode.ON;
			}
		}

		protected override void ApplyMode(AudioMode mode)
		{
			coroutineService.StopContext(applyWaitContext);
			applyWaitContext = new object();
			coroutineService.StartCoroutine(RealApplyMode(mode), applyWaitContext, GetType().Name + ".RealApplyMode");
		}

		private IEnumerator RealApplyMode(AudioMode mode)
		{
			yield return new WaitForEndOfFrame();
			switch (mode)
			{
				case AudioMode.ON:
					audioMixerService.UIVolume = 1f;
					audioMixerService.SoundEffectVolume = 1f;
					audioMixerService.MusicVolume = 1f;
					break;

				case AudioMode.OFF:
					audioMixerService.UIVolume = 0f;
					audioMixerService.SoundEffectVolume = 0f;
					audioMixerService.MusicVolume = 0f;
					break;

				case AudioMode.SFX_ONLY:
					audioMixerService.UIVolume = 1f;
					audioMixerService.SoundEffectVolume = 1f;
					audioMixerService.MusicVolume = 0f;
					break;
			}
			coroutineService.StopContext(applyWaitContext);
		}
	}
}