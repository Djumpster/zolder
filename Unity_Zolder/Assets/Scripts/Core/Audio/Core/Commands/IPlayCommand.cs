// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;
using UnityEngine.Audio;

namespace Talespin.Core.Foundation.Audio
{
	public interface IPlayCommand : IAudioCommand
	{
		AudioClip Clip { get; }
		AudioMixerGroup AudioMixerGroup { get; }
		bool Loop { get; }
		float Volume { get; }
		float Pitch { get; }
		float DopplerLevel { get; }
		DAHDSR VolumeEnvelope { get; }
		Transform FollowTransform { get; }
		bool IgnoreListenerPause { get; }
	}
}