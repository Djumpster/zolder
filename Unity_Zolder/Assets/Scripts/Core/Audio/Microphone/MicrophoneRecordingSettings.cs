// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;

using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	/// <summary>
	/// Global microphone recording settings.
	/// </summary>
	[Serializable]
	public class MicrophoneRecordingSettings : ScriptableObject
	{
		[Range(0f, 0.1f), SerializeField] public float SilenceThreshold = 0.002f;
		[Range(0f, 10f), SerializeField] public float SilenceDuration = 3f;

		[Header("Recording Quality")]
		[SerializeField] public int SampleRate = 22050;
		[SerializeField] public int BufferSizeSecs = 1;
		[SerializeField] public int BitsPerSample = 16;
		[SerializeField] public int SampleSegments = 50;
	}
}
