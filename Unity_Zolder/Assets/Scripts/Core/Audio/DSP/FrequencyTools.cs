// Copyright 2019 Talespin, LLC. All Rights Reserved.


namespace Talespin.Core.Foundation.Audio
{
	/// <summary>
	/// Static class to provide frequency specific information on audio.
	/// </summary>
	public static class FrequencyTools
	{
		/// <summary>
		/// This method returns an array with the same length as the input audio data. Its result is a frequency map
		/// from sample to frequency. These are ledged on a 'per half phase' basis. So everytime the signal crosses
		/// zero, the frequency for that fluctuation from start of it the previous zero crossing is mapped out.
		/// </summary>
		/// <param name="inputDataMono">The input data to be analyzed, in mono.</param>
		/// <param name="sampleRate">The sample rate of the input audio.</param>
		/// <param name="lowpass">Whether lowpassing should be applied, this helps when trying to find the fundamental.</param>
		/// <param name="cutoffFrequency">If lowpassing, where the cutoff takes place.</param>
		/// <param name="q">If lowpassing, the amount of Q to apply.</param>
		/// <param name="poles">If lowpassing, the amount of lowpass iterations, which is useful for steeper curves.</param>
		/// <param name="outputDataMono">The output data, as processed by this method, for debugging purporses.</param>
		/// <returns></returns>
		public static double[] GetFrequencies(float[] inputDataMono, int sampleRate, bool lowpass, float cutoffFrequency, float q, int poles, out float[] outputDataMono)
		{
			outputDataMono = new float[inputDataMono.Length];
			inputDataMono.CopyTo(outputDataMono, 0);

			// Lowpass if required
			if (lowpass)
			{
				EQTools.Lowpass(outputDataMono, sampleRate, cutoffFrequency, q, poles);
			}

			double[] frequencies = new double[outputDataMono.Length];
			bool positive = outputDataMono[0] >= 0;
			double currentFrequency = 0;
			double ledgedFrequency = 0;
			int samplesThisPhase = 0;

			//Debug.LogFormat("<color=yellow>clip sample rate: {0}, num samples: {1}</color>", sampleRate, outputDataMono.Length);
			//Debug.Log(string.Join(",\n", audioData));

			for (int i = 0; i < outputDataMono.Length; i++)
			{
				samplesThisPhase++;

				// We need to know whether we crossed zero, and whether we are stable, so we don't ledge over periods 
				// where there is zero fluctuation.
				bool crossedZero = (outputDataMono[i] >= 0 && !positive) || (outputDataMono[i] < 0 && positive);
				bool stableOnZero = i > 0 && outputDataMono[i - 1] == 0 && outputDataMono[i] == 0;

				// We crossed zero and measure how many samples are in between, to calculate the frequency based on
				// the sample rate.
				if (crossedZero)
				{
					positive = outputDataMono[i] >= 0;

					int zeroCrossingDelta = samplesThisPhase;
					double zeroCrossingRate = ((double)zeroCrossingDelta * 2) / sampleRate;
					currentFrequency = 1 / zeroCrossingRate;

					// If we are lowpassing, we only want to mark this as a relevant zero crossing if this frequency
					// stays below the lowpass level. In general the filter should ensure us that no higher frequencies
					// are being passed on, but this features as a cheap trap for those that slip through.
					bool isRelevantFrequency = !lowpass || (lowpass && currentFrequency < cutoffFrequency);
					if (isRelevantFrequency)
					{
						//float time = i / samplesPerSecond;
						//Debug.LogFormat("<color=red>Crossed zero at {3}, delta: {0}, frequency: {1}, time: {2}</color>", zeroCrossingDelta, currentFrequency, time, i);
						ledgedFrequency = currentFrequency;
						samplesThisPhase = 0;
					}
				}
				else if (stableOnZero)
				{
					ledgedFrequency = 0;
				}

				frequencies[i] = ledgedFrequency;
			}

			return frequencies;
		}
	}
}