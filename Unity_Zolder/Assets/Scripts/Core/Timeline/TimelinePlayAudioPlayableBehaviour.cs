// Copyright 2020 Talespin, LLC. All Rights Reserved.

using UnityEngine;
using UnityEngine.Playables;

namespace Talespin.Core.Foundation.Timeline
{
	/// <summary>
	/// Plays the passed <see cref="AudioClip"/> on the tracks <see cref="AudioSource"/>
	/// </summary>
	public class TimelinePlayAudioPlayableBehaviour : PlayableBehaviour
	{
		private AudioClip audioClip;
		private bool execute = false;

		/// <summary>
		/// Initialized this behaviour with the needed <see cref="AudioClip"/>
		/// </summary>
		/// <param name="audioClip"></param>
		public void Initialize(AudioClip audioClip)
		{
			this.audioClip = audioClip;
		}

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			base.ProcessFrame(playable, info, playerData);

			AudioSource audioSource = playerData as AudioSource;

			if (!audioSource.isPlaying && execute)
			{
				execute = false;

				audioSource.clip = audioClip;
				audioSource.Play();
			}
		}

		public override void OnBehaviourPlay(Playable playable, FrameData info)
		{
			base.OnBehaviourPlay(playable, info);

			if (!Application.isPlaying)
			{
				return;
			}

			execute = true;
		}
	}
}
