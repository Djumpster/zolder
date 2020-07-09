// Copyright 2019 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation
{
	public static class LevelTools
	{
		#region Metering

		/// <summary>
		/// Checks if the volume reaches beyond the given threshold. All values are normalized [0..1] values.
		/// </summary>
		/// <param name="audioData">The audio data</param>
		/// <param name="threshold">The threshold, in normalized values</param>
		/// <returns></returns>
		public static bool DoesAudioGoAboveThreshold(float[] audioData, float threshold)
		{
			float peak = FindMinMaxVolume(audioData).y;

			return peak > threshold;
		}

		/// <summary>
		/// Runs through audioData to see whether the delta between the quietest sample and the loudest sample
		/// fluctuates more than minimalDelta
		/// </summary>
		/// <param name="audioData">The audio data</param>
		/// <param name="minimalDelta">The threshold at which it's considered enough fluctuation</param>
		/// <param name="sampleSizeTime">The length in time of each sample window</param>
		/// <param name="sampleRate">The audio clip's sample rate</param>
		/// <returns>If the audio fluctuated above the threshold</returns>
		public static bool DoesAudioFluctuate(float[] audioData, float minimalDelta, float sampleSizeTime, int sampleRate)
		{
			//NormalizeAudio(audioData);

			Vector2 minMaxRms = FindMinMaxRms(audioData, sampleSizeTime, sampleRate);
			float delta = minMaxRms.y - minMaxRms.x;

			Debug.Log("Quietest part at RMS: " + minMaxRms.x + " the loudest part at RMS: " + minMaxRms.y + ", delta: " + delta + " = <b>" + (delta > minimalDelta ? "SPEECH" : "SILENCE") + "</b>");

			return delta > minimalDelta;
		}

		/// <summary>
		/// Goes through audioData and makes samples the length of sampleSizeTime. It then returns the quietest and the
		/// loudest RMS values.
		/// </summary>
		/// <param name="audioData">The audio data</param>
		/// <param name="sampleSizeTime">The length in time of each sample window</param>
		/// <param name="sampleRate">The audio clip's sample rate</param>
		/// <returns>A Vector2 containing the min RMS (x) and the max RMS (y)</returns>
		public static Vector2 FindMinMaxRms(float[] audioData, float sampleSizeTime, int sampleRate)
		{
			float averageFloor = float.MaxValue;
			float averageCeil = 0;
			int blockSize = Mathf.Max((int)(sampleSizeTime * sampleRate), 1);

			for (int i = 0; i < audioData.Length; i += blockSize)
			{
				int from = i;
				int to = Mathf.Min(i + blockSize, audioData.Length);
				float rmsThisBlock = GetRootMeanSquare(audioData, from, to);

				if (rmsThisBlock < averageFloor)
				{
					averageFloor = rmsThisBlock;
				}

				if (rmsThisBlock > averageCeil)
				{
					averageCeil = rmsThisBlock;
				}
			}

			return new Vector2(averageFloor, averageCeil);
		}

		/// <summary>
		/// Returns the true lowest and highest peak values.
		/// </summary>
		/// <param name="audioData">The audio data</param>
		/// <returns>A Vector2 containing the min (x) and the max (y)</returns>
		public static Vector2 FindMinMaxVolume(float[] audioData)
		{
			float min = float.MaxValue;
			float max = 0f;

			for (int i = 0; i < audioData.Length; i++)
			{
				float absSample = Mathf.Abs(audioData[i]);

				if (absSample > max)
				{
					max = absSample;
				}

				if (absSample < min)
				{
					min = absSample;
				}
			}

			return new Vector2(min, max);
		}

		/// <summary>
		/// Calculate the RMS value of a sample in audioData from 'from' (inclusive) to 'to' (exclusive).
		/// </summary>
		/// <param name="audioData">The audio data</param>
		/// <param name="from">index to check from (inclusive)</param>
		/// <param name="to">index to check to (exclusive)</param>
		/// <returns>The RMS value for the given sample</returns>
		public static float GetRootMeanSquare(float[] audioData, int from, int to)
		{
			float average = 0;

			for (int i = from; i < to; i++)
			{
				average += Mathf.Pow(audioData[i], 2);
			}

			return Mathf.Sqrt(average / (to - from));
		}

		#endregion

		#region Modification

		/// <summary>
		/// Amplifies the audio by a given factor.
		/// </summary>
		/// <param name="audioData">The audio data</param>
		/// <param name="amplificationFactor">Factor to amplify the audio data with</param>
		public static void Amplify(float[] audioData, float amplificationFactor)
		{
			for (int i = 0; i < audioData.Length; i++)
			{
				float before = audioData[i];
				audioData[i] = Mathf.Clamp(audioData[i] * amplificationFactor, -1, 1);
			}
		}

		/// <summary>
		/// Normalizes the audioData's audio to a peak value
		/// </summary>
		/// <param name="audioData">The audio data</param>
		/// <param name="normalizeTo">The new peak value</param>
		public static void Normalize(float[] audioData, float normalizeTo = 1f)
		{
			float maxValue = FindMinMaxVolume(audioData).y;
			float factor = normalizeTo / maxValue;

			for (int i = 0; i < audioData.Length; i++)
			{
				audioData[i] *= factor;
			}
		}

		#endregion
	}
}