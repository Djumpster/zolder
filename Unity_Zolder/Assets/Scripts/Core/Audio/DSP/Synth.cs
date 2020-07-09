// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	public class Synth
	{
		private double phase = 0;

		public float Sine(double frequency, double gain, int outputSampleRate)
		{
			double samplingFrequency = outputSampleRate;
			double increment = frequency * 2 * System.Math.PI / samplingFrequency;
			phase = phase + increment;
			float sample = (float)(gain * System.Math.Sin(phase));

			if (phase > 2 * System.Math.PI)
			{
				phase = 0;
			}

			return sample;
		}

		public void Sine(float[] outputData, int channels, double frequency, double gain, int outputSampleRate)
		{
			double samplingFrequency = outputSampleRate;
			double increment = frequency * 2 * System.Math.PI / samplingFrequency;

			for (int i = 0; i < outputData.Length; i = i + channels)
			{
				phase = phase + increment;

				if (phase > 2 * System.Math.PI) // Resetting triggers glitchy behaviour, phase = phase % (2 * System.Math.PI) caused some odd clicking behaviour
				{
					phase = 0;
				}

				outputData[i] = (float)(gain * Mathf.Sin((float)phase));

				if (channels == 2)
				{
					outputData[i + 1] = outputData[i];
				}


			}
		}

		public void Sine(float[] outputData, int channels, double[] frequencies, float[] volumes, int outputSampleRate)
		{
			bool getFrequency = true;
			double increment = 0;

			for (int i = 0; i < outputData.Length; i = i + channels)
			{
				double samplingFrequency = outputSampleRate;

				if (getFrequency)
				{
					increment = frequencies[i / 2] * 2 * System.Math.PI / samplingFrequency;
				}

				phase = phase + increment;
				outputData[i] = (float)(volumes[i / 2] * System.Math.Sin(phase));

				if (channels == 2)
				{
					outputData[i + 1] = outputData[i];
				}

				if (phase > 2 * System.Math.PI)
				{
					phase = 0;
				}
			}
		}
	}
}