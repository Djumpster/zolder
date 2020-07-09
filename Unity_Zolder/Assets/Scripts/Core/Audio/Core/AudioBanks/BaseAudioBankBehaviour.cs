// Copyright 2019 Talespin, LLC. All Rights Reserved.

using System;
using static Talespin.Core.Foundation.Audio.AudioBankBehaviour;

namespace Talespin.Core.Foundation.Audio
{
	/// <summary>
	/// This class forms the base for all implementations which are compatible with the <see cref="AudioService"/>. They can be found
	/// in the <see cref="AudioBank"/> dropdown, and will be passed an event.
	/// </summary>
	public abstract class BaseAudioBankBehaviour : AudioBehaviourBase
	{
		public abstract void HandleEvent(BaseAudioEvent audioEvent, out PlayInfo usedAudioPlayer, Action onAudioClipFinishedAction = null);

		public abstract void PauseBehaviour(bool pause = true);
	}
}
