// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;
using UnityEngine.Playables;

namespace Talespin.Core.Foundation.Audio
{
	public class PlayAudioTrackPlayableBehaviour : PlayableBehaviour
	{
		private string contextIdentifier;
		private AudioAsset asset;
		private float fadeDuration;
		private bool waitForQuantization;
		private AudioTrackControls audioTrackControls;

		public void Initialize(string contextIdentifier, AudioAsset asset, float fadeDuration, bool waitForQuantization)
		{
			this.contextIdentifier = contextIdentifier;
			this.asset = asset;
			this.fadeDuration = fadeDuration;
			this.waitForQuantization = waitForQuantization;
		}

		public override void OnBehaviourPlay(Playable playable, FrameData info)
		{
			if (!Application.isPlaying)
			{
				return;
			}
			audioTrackControls = new AudioTrackControls(contextIdentifier);
			audioTrackControls.Play(asset, fadeDuration, waitForQuantization);
		}
	}
}