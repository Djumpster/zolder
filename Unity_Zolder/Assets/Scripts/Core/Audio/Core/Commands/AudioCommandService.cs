// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using Talespin.Core.Foundation.Extensions;
using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	public class AudioCommandService
	{
		private Dictionary<AudioConfiguration, int> lastRequestedClips = new Dictionary<AudioConfiguration, int>();

		public AudioClip GetRandomClip(AudioConfiguration audioConfiguration)
		{
			if (audioConfiguration.Clips.Length == 0)
			{
				return null;
			}
			else if (audioConfiguration.Clips.Length == 1)
			{
				return audioConfiguration.Clips[0];
			}

			int i;

			if (audioConfiguration.PreventRepetition)
			{
				List<AudioClip> validClips;
				int lastRequestedIndex = GetLastRequestedIndex(audioConfiguration);

				validClips = new List<AudioClip>(audioConfiguration.Clips);

				if (lastRequestedIndex != -1)
				{
					validClips.RemoveAt(lastRequestedIndex);
				}

				i = Random.Range(0, validClips.Count);

				lastRequestedClips.SetOrCreate(audioConfiguration, audioConfiguration.Clips.IndexOf(validClips[i]));

				return validClips[i];
			}
			else
			{
				i = Random.Range(0, audioConfiguration.Clips.Length);
				return audioConfiguration.Clips[i];
			}
		}

		private int GetLastRequestedIndex(AudioConfiguration audioConfiguration)
		{
			int lastRequestedIndex;
			if (!lastRequestedClips.TryGetValue(audioConfiguration, out lastRequestedIndex))
			{
				lastRequestedIndex = -1;
			}

			return lastRequestedIndex;
		}
	}
}