// Copyright 2019 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	/// <summary>
	/// Event meant to let the audio jump to a specific timestamp.
	/// </summary>
	public class SeekAudioEvent : BaseAudioEvent
	{
		public SeekAudioEvent(AudioBankEntry audioBankEntry, float seekTime, GameObject context = null) : base(audioBankEntry, context)
		{
		}
	}
}