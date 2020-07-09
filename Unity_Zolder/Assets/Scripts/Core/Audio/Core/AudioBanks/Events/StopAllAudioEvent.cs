// Copyright 2019 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	/// <summary>
	/// Event meant to stop all audio with a specific ID.
	/// </summary>
	public class StopAllAudioEvent : BaseAudioEvent
	{
		public StopAllAudioEvent(AudioBankEntry audioBankEntry, GameObject context = null) : base(audioBankEntry, context)
		{
		}
	}
}