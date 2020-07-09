// Copyright 2019 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	/// <summary>
	/// Event meant to play a sound.
	/// </summary>
	public class PlayAudioEvent : BaseAudioEvent
	{
		public PlayAudioEvent(AudioBankEntry audioBankEntry, GameObject context = null) : base(audioBankEntry, context)
		{
		}
	}
}