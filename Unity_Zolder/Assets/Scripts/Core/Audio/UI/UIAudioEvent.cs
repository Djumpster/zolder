// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Events;

namespace Talespin.Core.Foundation.Audio
{
	public class UIAudioEvent : IEvent
	{
		/// <summary>
		/// UIEvent class.
		/// </summary>	
		public string AudioType { get; private set; }
		public float Duration { get; private set; }
		public object Context { get; private set; }

		public UIAudioEvent(string audioType, float duration = 0f, object context = null)
		{
			AudioType = audioType;
			Context = context;
			Duration = duration;
		}
	}
}