// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	/// <summary>
	/// Represents an activatable microphone device.
	/// </summary>
	public interface IActivateable
	{
		bool IsRecording { get; }

		AudioClip Start(bool loop = true);
		AudioClip Stop();
	}
}