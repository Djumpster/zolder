// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	public class AudioAsset : ScriptableObject
	{
		public AudioConfiguration AudioConfiguration { get { return audioConfiguration; } }

		[SerializeField] private AudioConfiguration audioConfiguration;

		public void SetClips(AudioClip[] clips)
		{
			audioConfiguration = new AudioConfiguration
			{
				Clips = clips
			};
		}
	}
}