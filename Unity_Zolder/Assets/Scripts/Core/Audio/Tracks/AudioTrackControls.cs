// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Injection;
using Talespin.Core.Foundation.Logging;
using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	public class AudioTrackControls
	{
		private string contextIdentifier;
		private AudioService audioService;
		private MetronomeService metronomeService;

		public AudioTrackControls(string contextIdentifier)
		{
			this.contextIdentifier = contextIdentifier;

			audioService = GlobalDependencyLocator.Instance.Get<AudioService>();
			metronomeService = GlobalDependencyLocator.Instance.Get<MetronomeService>();
		}

		public void Play(AudioAsset asset, float fadeDuration = 0f, bool waitForQuantization = false)
		{
			AudioTrackImpl audioTrackImpl = null;// audioService.GetOrCreate(AudioTrackImpl.AUDIO_TRACK_IMPL_ASSET_NAME, contextIdentifier) as AudioTrackImpl;

			if (audioTrackImpl == null)
			{
				LogUtil.Error(LogTags.AUDIO, this, "Could not find AudioTrackImpl.");
				return;
			}

			float delay = waitForQuantization ? (float)(metronomeService.NextQuantizationDspTime - AudioSettings.dspTime) : 0f;

			if (asset != null)
			{
				audioTrackImpl.Play(asset, delay, fadeDuration);
				if (asset.AudioConfiguration.MetronomeSettings.Quantization != Quantizations.None && contextIdentifier == "music")
				{
					metronomeService.StartOrQueueMetronome(asset.AudioConfiguration.MetronomeSettings);
				}
			}
			else
			{
				audioTrackImpl.Stop(delay, fadeDuration);

				if (contextIdentifier == "music")
				{
					metronomeService.StopMetronome();
				}
			}
		}
	}
}