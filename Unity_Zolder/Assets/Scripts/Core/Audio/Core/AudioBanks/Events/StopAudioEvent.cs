// Copyright 2019 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	/// <summary>
	/// Event meant to stop a specific sound.
	/// </summary>
	public class StopAudioEvent : BaseAudioEvent
	{
		public StopAudioEvent(AudioBankEntry audioBankEntry, GameObject context = null) : base(audioBankEntry, context)
		{
		}
	}
}