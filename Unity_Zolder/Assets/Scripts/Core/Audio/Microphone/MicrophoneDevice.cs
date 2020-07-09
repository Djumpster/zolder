// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using Talespin.Core.Foundation.Logging;
using Talespin.Core.Foundation.Services;
using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	/// <summary>
	/// Class for using the microphone
	/// </summary>
	public class MicrophoneDevice : IActivateable, IAudioRecordable, IDisposable
	{
		private const int SEGMENT_HISTORY_BUFFER_COUNT = 32;

		public delegate void AudioSegmentReadHandler(float[] samples, float audioLevel, int segmentHistoryCount);

		public event AudioSegmentReadHandler AudioSegmentReadEvent = delegate { };
		public event Action DeviceDisconnectedEvent = delegate { };
		public event Action SilenceDetectedEvent = delegate { };

		/// <summary>
		/// The audio level of the current sample segment.
		/// This is the absolute max without any further processing. So NOT the actual dB.
		/// </summary>
		public float AudioLevel { get; private set; } = 0f;
		public bool IsSilent { get; private set; } = false;
		public bool IsRecording => Microphone.IsRecording(DeviceName);

		public int Position => Microphone.GetPosition(DeviceName);

		public string DeviceName { get; private set; }

		private readonly ICallbackService callbackService;
		private readonly int frequency;
		private readonly int lengthSec;
		private readonly int sampleCount;
		private readonly float silenceThreshold;
		private readonly float silenceDuration;

		private AudioClip recordingClip;
		private long silenceStartTick = -1;
		private long ticksCache;

		private int curMicIndex;
		private float[] segmentBuffer;
		private List<float[]> segmentBuffers;
		private int curSegmentBufferIndex;

		public MicrophoneDevice(string deviceName, int frequency, int lengthSec, int sampleCount, float silenceThreshold, float silenceDuration,
						  ICallbackService callbackService)
		{
			this.DeviceName = deviceName;
			this.frequency = frequency;
			this.lengthSec = lengthSec;
			this.sampleCount = sampleCount;
			this.silenceThreshold = silenceThreshold;
			this.silenceDuration = silenceDuration;
			this.callbackService = callbackService;
		}

		/// <summary>
		/// Disposes the AudioSource GameObject, is called when the Microphone is disconnected
		/// </summary>
		public void Dispose()
		{
			callbackService.UpdateEvent -= Update;
			DeviceDisconnectedEvent.Invoke();
		}

		/// <summary>
		/// Starts the microphone so it can be used, creates a gameObject for the AudioSource
		/// </summary>
		public AudioClip Start(bool loop = true)
		{
			if (IsRecording)
			{
				return recordingClip;
			}

			silenceStartTick = -1;

			recordingClip = Microphone.Start(DeviceName, loop, lengthSec, frequency);
			InitSegmentBuffers(recordingClip);
			callbackService.UpdateEvent -= Update;
			callbackService.UpdateEvent += Update;

			Debug.Log("Started microphone recording.");

			return recordingClip;
		}

		/// <summary>
		/// Stops the microphone and hides the AudioSource GameObject.
		/// </summary>
		/// <returns>The full recorded clip.</returns>
		public AudioClip Stop()
		{
			if (!IsRecording)
			{
				return recordingClip;
			}

			Microphone.End(DeviceName);

			silenceStartTick = -1;
			AudioLevel = 0f;

			callbackService.UpdateEvent -= Update;

			Debug.Log("Stopped microphone recording.");

			return recordingClip;
		}

		/// <summary>
		/// Restarts this microphone when a new one has been connected or disconnected. Is necessary for ensuring the microphone continues to work.
		/// </summary>
		public void OnNewDeviceConnected()
		{
			if (IsRecording)
			{
				Start();
			}
		}

		private void InitSegmentBuffers(AudioClip clip)
		{
			curSegmentBufferIndex = 0;
			segmentBuffers = new List<float[]>(SEGMENT_HISTORY_BUFFER_COUNT);
			for (int i = 0; i < SEGMENT_HISTORY_BUFFER_COUNT; i++)
			{
				segmentBuffers.Add(new float[clip.samples / sampleCount]);
			}
			segmentBuffer = segmentBuffers[curSegmentBufferIndex];
			curSegmentBufferIndex++;
		}

		private void Update()
		{
			if (!IsRecording)
			{
				return;
			}

			int microphonePosition = Microphone.GetPosition(DeviceName);
			int sampleSegmentSize = recordingClip.samples / sampleCount;
			ticksCache = DateTime.Now.Ticks;
			while (curMicIndex + sampleSegmentSize < microphonePosition || curMicIndex > microphonePosition)
			{
				if (recordingClip.GetData(segmentBuffer, curMicIndex))
				{
					AudioLevel = GetAudioLevel(segmentBuffer);
					IsSilent = IsInputSilent(AudioLevel);

					AudioSegmentReadEvent.Invoke(segmentBuffer, AudioLevel, SEGMENT_HISTORY_BUFFER_COUNT);

					segmentBuffer = segmentBuffers[curSegmentBufferIndex];
					curSegmentBufferIndex++;
					if (curSegmentBufferIndex >= SEGMENT_HISTORY_BUFFER_COUNT)
					{
						curSegmentBufferIndex = 0;
					}

					curMicIndex += sampleSegmentSize;
					if (curMicIndex >= recordingClip.samples)
					{
						curMicIndex = 0;
					}
				}
			}
		}

		private float GetAudioLevel(float[] samples)
		{
			float result = 0;

			foreach (float sample in samples)
			{
				float absSample = Mathf.Abs(sample);

				if (absSample > result)
				{
					result = absSample;
				}
			}

			return result;
		}

		/// <summary>
		/// Detects if the microphone is silent based on the silence duration and silence threshold.
		/// Invokes the SilenceDetectedEvent when true.
		/// </summary>
		/// <returns>Whether the indicating if the microphone is silent or not.</returns>
		private bool IsInputSilent(float audioLevel)
		{
			if (!IsRecording)
			{
				return true;
			}

			if (audioLevel <= silenceThreshold)
			{
				if (silenceStartTick == -1)
				{
					silenceStartTick = ticksCache;
				}

				TimeSpan duration = TimeSpan.FromTicks(ticksCache - silenceStartTick);

				if (duration.TotalSeconds >= silenceDuration)
				{
					SilenceDetectedEvent.Invoke();
					return true;
				}
			}
			else
			{
				silenceStartTick = -1;
			}

			return false;
		}
	}
}
