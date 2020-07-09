// Copyright 2019 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Events;
using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	/// <summary>
	/// Abstract base class for an event which can be handled by an <see cref="AudioBankBehaviour"/>.
	/// </summary>
	public abstract class BaseAudioEvent : IEvent
	{
		public readonly AudioBankEntry AudioBankEntry;
		public readonly GameObject Context;

		public BaseAudioEvent(AudioBankEntry audioBankEntry, GameObject context = null)
		{
			AudioBankEntry = audioBankEntry;
			Context = context;
		}
	}
}