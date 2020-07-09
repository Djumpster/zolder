// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	public class SimpleAudioPlayer : AudioPlayer
	{
		[SerializeField] private AudioAsset asset;

		protected void OnEnable()
		{
			Play(new PlayCommand(asset));
		}

		protected void OnDisable()
		{
			Stop();
		}
	}
}