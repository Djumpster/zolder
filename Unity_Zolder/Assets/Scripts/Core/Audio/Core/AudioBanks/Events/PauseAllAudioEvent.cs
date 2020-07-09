// Copyright 2019 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	/// <summary>
	/// Event meant to pause all audio with a specific ID.
	/// </summary>
	public class PauseAllAudioEvent : BaseAudioEvent
	{
		public PauseAllAudioEvent(AudioBankEntry audioBankEntry, GameObject context = null) : base(audioBankEntry, context)
		{
		}
	}
}