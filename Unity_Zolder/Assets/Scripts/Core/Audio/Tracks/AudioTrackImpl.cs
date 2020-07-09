// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	/// <summary>
	/// This class is a basic AudioImpl which contains basic functionality to play tracks. In most cases this is either
	/// an ambience or music track. Whenever a new track is played, it can fade out the one currently playing. The
	/// ScriptableObject is included in Utils, so by creating one via AudioService can be done out of the box.
	/// The context is very important, because that allows you to create separate channels for, for instance, ambient
	/// or music.
	/// </summary>
	public class AudioTrackImpl : AudioBehaviourBase
	{
		public const string AUDIO_TRACK_IMPL_ASSET_NAME = "AudioTrackImpl";

		public override bool ReadyForCleanup { get { return false; } }
		public AudioPlayer CurrentPlayer { get; private set; }
		public AudioAsset CurrentAudioAsset { get; private set; }

		public void Play(AudioAsset audioAsset, float delay = 0f, float fadeDuration = 0f)
		{
			PlayCommand playCommand = new PlayCommand(audioAsset);

			// If we're already playing this asset, nothing will happen and we will continue playing it
			if (CurrentPlayer != null && CurrentPlayer.CurrentPlayCommand != null && CurrentPlayer.CurrentPlayCommand.Clip == playCommand.Clip)
			{
				return;
			}

			// Override the volume envelope's delay and attack
			DAHDSR volumeEnvelope = playCommand.VolumeEnvelope;
			volumeEnvelope.Delay = delay;
			volumeEnvelope.Attack = fadeDuration;
			playCommand.VolumeEnvelope = volumeEnvelope;

			// Try to find a free channel and play the new asset
			AudioPlayer player = channeledAudio.TryPlay(playCommand);

			if (player != null)
			{
				CurrentAudioAsset = audioAsset;

				// In some cases, like when the previously played asset was finished playing with non-looping assets,
				// oldPlayer and player will refer to the same AudioPlayer. Only if that's not the case will we fade out
				// the current asset.
				if (CurrentPlayer != null && player != CurrentPlayer && CurrentPlayer.IsPlaying)
				{
					FadeOutAndStopCurrent(fadeDuration, delay);
				}

				//				Debug.LogFormat("<color=green>Play {0} {1}</color>", fadeDuration, audioAsset.name);

				CurrentPlayer = player;
			}
		}

		public Coroutine Stop(float delay, float fadeDuration)
		{
			if (CurrentPlayer == null)
			{
				return null;
			}

			return FadeOutAndStopCurrent(fadeDuration, delay);
		}

		public void Pause()
		{
			if (CurrentPlayer != null)
			{
				CurrentPlayer.Pause();
			}
		}

		public void Unpause()
		{
			if (CurrentPlayer != null)
			{
				CurrentPlayer.Unpause();
			}
		}

		private Coroutine FadeOutAndStopCurrent(float fadeDuration, float delay)
		{
			//			Debug.LogFormat("<color=red>Fadeout {0} \tasset: {1}</color>", fadeDuration, player.CurrentAsset.name);

			if (CurrentAudioAsset == null)
			{
				return null;
			}

			StopCommand stopCommand = new StopCommand(CurrentAudioAsset);
			DAHDSR volumeEnvelope = stopCommand.VolumeEnvelope;
			volumeEnvelope.Release = fadeDuration;
			stopCommand.VolumeEnvelope = volumeEnvelope;

			Coroutine stopRoutine = CurrentPlayer.Stop(stopCommand, delay);
			CurrentPlayer = null;

			return stopRoutine;
		}
	}
}