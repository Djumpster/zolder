// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Injection;
using UnityEngine;
using UnityEngine.Audio;

namespace Talespin.Core.Foundation.Audio
{
	public class PlayCommand : IPlayCommand
	{
		public AudioClip Clip { get; set; }
		public AudioMixerGroup AudioMixerGroup { get; set; }
		public bool Loop { get; set; }
		public float Volume { get; set; }
		public float Pitch { get; set; }
		public float DopplerLevel { get; set; }
		public Transform FollowTransform { get; set; }
		public DAHDSR VolumeEnvelope { get; set; }
		public bool IgnoreListenerPause { get; set; }

		public PlayCommand() : this(new AudioConfiguration())
		{
		}

		public PlayCommand(AudioAsset audioAsset) : this(audioAsset.AudioConfiguration)
		{
		}

		public PlayCommand(AudioConfiguration audioConfiguration)
		{
			Clip = GlobalDependencyLocator.Instance.Get<AudioCommandService>().GetRandomClip(audioConfiguration);
			AudioMixerGroup = audioConfiguration.AudioMixerGroup;
			Loop = audioConfiguration.Loop;
			Volume = audioConfiguration.Volume;
			Pitch = audioConfiguration.Pitch;
			DopplerLevel = audioConfiguration.DopplerLevel;
			VolumeEnvelope = audioConfiguration.VolumeEnvelope;
			IgnoreListenerPause = audioConfiguration.IgnoreListenerPause;
		}
	}
}