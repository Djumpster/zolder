// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Logging;
using UnityEngine;
using UnityEngine.Audio;

namespace Talespin.Core.Foundation.Audio
{
	public class AudioMixerService
	{
		public const string MIXERS_PATH = "Audio/Mixers/";

		public const string PARAMETER_VOLUME_MASTER = "VolumeMaster";
		public const string PARAMETER_VOLUME_UI = "VolumeUI";
		public const string PARAMETER_VOLUME_SOUND_EFFECTS = "VolumeSoundEffects";
		public const string PARAMETER_VOLUME_MUSIC = "VolumeMusic";
		public const string PARAMETER_VOLUME_PAUSE_BUS = "VolumePauseBus";
		private const float MIN_VOLUME = -80;
		private const float MAX_VOLUME = 0;

		public AudioMixerService(AudioMixer mainMixer)
		{
			MainMixer = mainMixer;
		}

		public AudioMixer MainMixer { get; }

		public float MasterVolume
		{
			get
			{
				if (MainMixer == null)
				{
					return 0;
				}

				float value;
				MainMixer.GetFloat(PARAMETER_VOLUME_MASTER, out value);
				return Convert(value);
			}
			set
			{
				if (MainMixer == null)
				{
					return;
				}

				MainMixer.SetFloat(PARAMETER_VOLUME_MASTER, Mathf.Lerp(MIN_VOLUME, MAX_VOLUME, value));
			}
		}

		public float MusicVolume
		{
			get
			{
				if (MainMixer == null)
				{
					return 0;
				}

				float value;
				MainMixer.GetFloat(PARAMETER_VOLUME_MUSIC, out value);
				return Convert(value);
			}
			set
			{
				if (MainMixer == null)
				{
					return;
				}

				MainMixer.SetFloat(PARAMETER_VOLUME_MUSIC, Mathf.Lerp(MIN_VOLUME, MAX_VOLUME, value));
			}
		}

		public float SoundEffectVolume
		{
			get
			{
				if (MainMixer == null)
				{
					return 0;
				}

				float value;
				MainMixer.GetFloat(PARAMETER_VOLUME_SOUND_EFFECTS, out value);
				return Convert(value);
			}
			set
			{
				if (MainMixer == null)
				{
					return;
				}

				MainMixer.SetFloat(PARAMETER_VOLUME_SOUND_EFFECTS, Mathf.Lerp(MIN_VOLUME, MAX_VOLUME, value));
			}
		}

		public float UIVolume
		{
			get
			{
				if (MainMixer == null)
				{
					return 0;
				}

				float value;
				MainMixer.GetFloat(PARAMETER_VOLUME_UI, out value);
				return Convert(value);
			}
			set
			{
				if (MainMixer == null)
				{
					return;
				}

				MainMixer.SetFloat(PARAMETER_VOLUME_UI, Mathf.Lerp(MIN_VOLUME, MAX_VOLUME, value));
			}
		}

		public float PauseBusVolume
		{
			get
			{
				if (MainMixer == null)
				{
					return 0;
				}

				float value;
				MainMixer.GetFloat(PARAMETER_VOLUME_PAUSE_BUS, out value);
				return Convert(value);
			}
			set
			{
				if (MainMixer == null)
				{
					return;
				}

				MainMixer.SetFloat(PARAMETER_VOLUME_PAUSE_BUS, Mathf.Lerp(MIN_VOLUME, MAX_VOLUME, value));
			}
		}

		private float Convert(float value)
		{
			value -= MIN_VOLUME;
			const float delta = MAX_VOLUME - MIN_VOLUME;
			return value / delta;
		}

		private AudioMixerGroup SearchGroup(string subPath)
		{
			if (MainMixer == null)
			{
				return null;
			}

			AudioMixerGroup[] groups = MainMixer.FindMatchingGroups(subPath);

			if (groups.Length > 1)
			{
				LogUtil.Error(LogTags.AUDIO, this, "Found multiple cases of subPath '" + subPath + "'. Return null.");
			}

			return groups.Length == 1 ? groups[0] : null;
		}
	}
}