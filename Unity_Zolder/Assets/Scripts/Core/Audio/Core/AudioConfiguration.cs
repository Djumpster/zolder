// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Misc;
using UnityEngine;
using UnityEngine.Audio;

namespace Talespin.Core.Foundation.Audio
{
	[System.Serializable]
	public class AudioConfiguration
	{
		public enum Order { Random, Sequential, PingPong }

		public AudioClip[] Clips { get { return clips; } set { clips = value; } }
		public Order ClipOrder { get { return clipOrder; } set { clipOrder = value; } }
		public bool PreventRepetition { get { return preventRepetition; } set { preventRepetition = value; } }
		public AudioMixerGroup AudioMixerGroup { get { return audioMixerGroup; } set { audioMixerGroup = value; } }
		public bool Loop { get { return loop; } set { loop = value; } }
		public float Volume { get { return volume; } set { volume = new MinMaxRange(value, value, false); } }
		public MinMaxRange VolumeMinMax { get { return volume; } set { volume = value; } }
		public float Pitch { get { return pitch; } set { pitch = new MinMaxRange(value, value, false); } }
		public MinMaxRange PitchMinMax { get { return pitch; } set { pitch = value; } }
		public float DopplerLevel { get { return dopplerLevel; } set { dopplerLevel = value; } }
		public DAHDSR VolumeEnvelope { get { return volumeEnvelope; } set { volumeEnvelope = value; } }
		public MetronomeSettings MetronomeSettings { get { return metronomeSettings; } set { metronomeSettings = value; } }
		public bool IgnoreListenerPause { get { return ignoreListenerPause; } set { ignoreListenerPause = value; } }

		[SerializeField] private AudioClip[] clips;
		[SerializeField] private Order clipOrder;
		[SerializeField] private bool preventRepetition = true;
		[SerializeField] private AudioMixerGroup audioMixerGroup;
		[SerializeField] private bool loop;
		[SerializeField, MinMaxRange(0, 1, true)] private MinMaxRange volume = new MinMaxRange(0.5f, 0.5f, false);
		[SerializeField, MinMaxRange(-3, 3, true)] private MinMaxRange pitch = new MinMaxRange(1, 1, false);
		[SerializeField] private float dopplerLevel;
		[SerializeField] private DAHDSR volumeEnvelope;
		[SerializeField] private MetronomeSettings metronomeSettings;
		[SerializeField] private bool ignoreListenerPause = false;

		public AudioConfiguration()
		{
			// Correct default setup for DAHDSR when creating a new AudioAsset
			volumeEnvelope = new DAHDSR(0, 0, 0, 0, 1, 0);
			metronomeSettings = new MetronomeSettings();
		}

		public AudioConfiguration(AudioConfiguration audioConfiguration)
		{
			clips = audioConfiguration.clips;
			clipOrder = audioConfiguration.clipOrder;
			preventRepetition = audioConfiguration.preventRepetition;
			audioMixerGroup = audioConfiguration.audioMixerGroup;
			loop = audioConfiguration.loop;
			volume = audioConfiguration.volume;
			pitch = audioConfiguration.pitch;
			dopplerLevel = audioConfiguration.dopplerLevel;
			volumeEnvelope = audioConfiguration.volumeEnvelope;
			metronomeSettings = audioConfiguration.metronomeSettings;
			ignoreListenerPause = audioConfiguration.ignoreListenerPause;
		}
	}
}