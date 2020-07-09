// Copyright 2019 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	public static class StereoTools
	{
		public static float[] ConvertToMono(float[] stereoAudioData)
		{
			// NOTE audioClip.samples returns the amount of samples per channel, so this is the same value
			// regardless of whether we're dealing with mono or stereo data. When using GetData() it will return
			// <channels> times the data.
			int numSamples = stereoAudioData.Length / 2;

			Debug.LogFormat("<color=red>num samples: {0}, data length: {1}</color>", numSamples, stereoAudioData.Length);

			float[] monoAudioData = new float[numSamples];

			Debug.Log("Original data length: " + stereoAudioData.Length + " samples with 2 channels. Mono'ed this is: " + monoAudioData.Length + " samples.");

			for (int i = 0; i < monoAudioData.Length; i++)
			{
				monoAudioData[i] = stereoAudioData[i * 2];
			}

			return monoAudioData;
		}

		public static float[] ConvertToMono(AudioClip audioClip)
		{
			float[] monoAudioData;

			if (audioClip.channels == 1)
			{
				monoAudioData = new float[audioClip.samples];
				audioClip.GetData(monoAudioData, 0);
			}
			else if (audioClip.channels == 2)
			{
				float[] stereoAudioData = new float[audioClip.samples * 2];
				audioClip.GetData(stereoAudioData, 0);
				monoAudioData = ConvertToMono(stereoAudioData);
			}
			else
			{
				Debug.LogError("No more than 2 channels supported.");
				return null;
			}

			return monoAudioData;
		}

		public static float[] ConvertToStereo(float[] monoAudioData)
		{
			int numSamples = monoAudioData.Length * 2;
			float[] stereoAudioData = new float[numSamples];

			for (int i = 0; i < monoAudioData.Length; i++)
			{
				stereoAudioData[i * 2] = stereoAudioData[i * 2 + 1] = monoAudioData[i];
			}

			return stereoAudioData;
		}

		public static float[] ConvertToStereo(AudioClip audioClip)
		{
			float[] stereoAudioData;

			if (audioClip.channels == 1)
			{
				float[] monoAudioData = new float[audioClip.samples];
				audioClip.GetData(monoAudioData, 0);
				stereoAudioData = ConvertToStereo(monoAudioData);

			}
			else if (audioClip.channels == 2)
			{
				stereoAudioData = new float[audioClip.samples * 2];
				audioClip.GetData(stereoAudioData, 0);
			}
			else
			{
				Debug.LogError("No more than 2 channels supported.");
				return null;
			}

			return stereoAudioData;
		}
	}
}