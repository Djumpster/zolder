// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Talespin.Core.Foundation.Audio
{
	public class AudioAssetPlayableBehaviour : PlayableBehaviour
	{
		public bool OverrideVolumeEnvelope { get { return Clip != null && (Clip.easeInDuration > 0 || Clip.easeOutDuration > 0); } }

		public TimelineClip Clip
		{
			get
			{
				return _clip;
			}
			set
			{
				_clip = value;
				_clip.displayName = asset.name;
			}
		}

		private AudioAsset asset;
		private AudioPlayer player;
		private TimelineClip _clip;

		public void Initialize(AudioAsset asset)
		{
			this.asset = asset;
		}

		public override void OnBehaviourPlay(Playable playable, FrameData info)
		{
			if (!asset || !Application.isPlaying)
			{
				return;
			}

			AudioConfiguration audioConfiguration;

			if (OverrideVolumeEnvelope)
			{
				audioConfiguration = new AudioConfiguration(asset.AudioConfiguration)
				{
					VolumeEnvelope = new DAHDSR() // reset the envelope
				};
			}
			else
			{
				audioConfiguration = asset.AudioConfiguration;
			}

			PlayCommand playCommand = new PlayCommand(audioConfiguration);

			player = AudioPlayer.Blast(playCommand);
		}

		public override void OnBehaviourPause(Playable playable, FrameData info)
		{
			if (!player || !Application.isPlaying)
			{
				return;
			}

			player.Stop();
			player = null;
		}

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			if (OverrideVolumeEnvelope && player)
			{
				player.PlayerVolume = info.effectiveWeight;
			}
		}
	}
}