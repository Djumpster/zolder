// Copyright 2019 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	/// <summary>
	/// Event meant to pause a single sound.
	/// </summary>
	public class PauseAudioEvent : BaseAudioEvent
	{
		public PauseAudioEvent(AudioBankEntry audioBankEntry, GameObject context = null) : base(audioBankEntry, context)
		{
		}
	}
}