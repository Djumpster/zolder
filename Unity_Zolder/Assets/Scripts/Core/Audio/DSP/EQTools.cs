// Copyright 2019 Talespin, LLC. All Rights Reserved.

using NAudio.Dsp;

namespace Talespin.Core.Foundation.Audio
{
	/// <summary>
	/// Static class to provide equalizer like functionality in different forms.
	/// </summary>
	public static class EQTools
	{
		public static void Lowpass(float[] audioData, int sampleRate, float cutoffFrequency, float q, float poles)
		{
			BiQuadFilter filter;

			for (int j = 0; j < poles; j++)
			{
				filter = BiQuadFilter.LowPassFilter(sampleRate, cutoffFrequency, q);

				for (int i = 0; i < audioData.Length; i++)
				{
					audioData[i] = filter.Transform(audioData[i]);
				}
			}
		}
	}
}