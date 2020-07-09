// Copyright 2018 Talespin, LLC. All Rights Reserved.

namespace Talespin.Core.Foundation.Audio
{
	/// <summary>
	/// Represents a microphone device capable of recording audio.
	/// </summary>
	public interface IAudioRecordable
	{
		/// <summary>
		/// The audio level of the current sample segment.
		/// This is the absolute max without any further processing. So NOT the actual dB.
		/// </summary>
		float AudioLevel { get; }

		bool IsRecording { get; }
		bool IsSilent { get; }
	}
}
