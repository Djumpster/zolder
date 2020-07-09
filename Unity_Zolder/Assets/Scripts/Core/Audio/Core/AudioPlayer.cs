// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections;
using Talespin.Core.Foundation.Extensions;
using Talespin.Core.Foundation.Injection;
using Talespin.Core.Foundation.Logging;
using Talespin.Core.Foundation.Pooling;
using Talespin.Core.Foundation.Services;
using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	public class AudioPlayer : MonoBehaviour, IPoolableObject
	{
		protected struct FadeInfo
		{
			public float TargetValue;
			public float Duration;

			public FadeInfo(float targetValue, float duration)
			{
				TargetValue = targetValue;
				Duration = duration;
			}
		}

		private static PrefabObjectPool<AudioPlayer> _audioPlayerPool;
		private static PrefabObjectPool<AudioPlayer> AudioPlayerPool
		{
			get
			{
				if (_audioPlayerPool == null)
				{
					_audioPlayerPool = new PrefabObjectPool<AudioPlayer>("Audio/AudioPlayer",
						GlobalDependencyLocator.Instance.Get<UnityCallbackService>(), 50, "AudioPlayer");
					_audioPlayerPool.FillPool(10);
				}

				return _audioPlayerPool;
			}
		}

		private static PrefabObjectPool<AudioPlayer> _positionedAudioPlayerPool;
		private static PrefabObjectPool<AudioPlayer> PositionedAudioPlayerPool
		{
			get
			{
				if (_positionedAudioPlayerPool == null)
				{
					_positionedAudioPlayerPool = new PrefabObjectPool<AudioPlayer>("Audio/PositionedAudioPlayer",
						GlobalDependencyLocator.Instance.Get<UnityCallbackService>(), 50, "PositionedAudioPlayer");
					_positionedAudioPlayerPool.FillPool(10);
				}

				return _positionedAudioPlayerPool;
			}
		}

		public AudioSource Source
		{
			get
			{
				if (_source == null && gameObject != null) // gameObject-check prevents it from recreating AudioSources while closing the game due to blasts on OnDisable of MonoBehaviours for instance.
				{
					_source = gameObject.RequireComponent<AudioSource>();
					_source.playOnAwake = false;
					_source.Stop(); //The Stop() needs to be there, because OnAudioFilterRead operates as a synthesizer if no clip is added, therefore putting isPlaying on true
				}

				return _source;
			}
		}

		public IPlayCommand CurrentPlayCommand { get; private set; }
		public IStopCommand CurrentStopCommand { get; private set; }
		public bool IsPlaying { get { return this && Source.isPlaying && !isPaused || inDelayPhase; } }
		public bool IsPaused { get { return isPaused; } }
		public bool FinishedPlaying { get { return Source.loop == false && currentTime < previousTime && !isPaused; } }
		public float Time { get { return Source.time; } set { Source.time = value; } }
		public int TimeSamples { get { return Source.timeSamples; } set { Source.timeSamples = value; } }
		public float Length { get { return Source.clip != null ? Source.clip.length : 0; } }
		public int Samples { get { return Source.clip != null ? Source.clip.samples : 0; } }
		public float PlayerVolume { get { return playerVolume; } set { playerVolume = value; } }

		[Header("Runtime debug values")]
		[SerializeField] protected bool isPlaying;
		[SerializeField, Range(0, 1)] private float playerVolume = 1f; // This is the player's internal volume, used in case you want to manually change the volume at runtime
		[SerializeField, Range(0, 1)] private float dahdsrVolume = 1f; // This value is specifically following the volume envelope
		[SerializeField, Range(0, 1)] private float finalVolume = 1f; // This is the final volume, calculated by multiplying the above + the asset's volume

		private bool isPaused;
		private bool inDelayPhase;
		private AudioSource _source;
		private double startTime;
		private Coroutine playRoutine;
		private Coroutine stopRoutine;
		private float currentTime = 0.0f;
		private float previousTime = 0.0f;

		#region IPoolableObject implementation

		public bool IsInPool { get; private set; } = true;

		public event System.Action<IPoolableObject> ReturnToPoolHandler;

		public void Reset()
		{
			StopAllCoroutines();
			Stop();
		}

		public void BecomeActive()
		{
			IsInPool = false;
			gameObject.SetActive(true);
		}

		public void BecomeInactive()
		{
			Stop();
			gameObject.SetActive(false);
		}

		public void ReturnToPool()
		{
			IsInPool = true;
			ReturnToPoolHandler(this);
		}

		public void DestroyForever()
		{
			Destroy(gameObject);
		}

		#endregion

		public static AudioPlayer Blast(IPlayCommand playCommand)
		{
			if (playCommand == null)
			{
				LogUtil.Warning(LogTags.AUDIO, "AudioPlayer", "Missing PlayCommand!");
				return null;
			}
			else
			{
				AudioPlayer audioPlayer;
				if (playCommand.FollowTransform != null)
				{
					audioPlayer = PositionedAudioPlayerPool.GetPoolableObject();

					// This is a terrible hack for Blast(), due to tune restrictions, we're keeping this for MR2
					AudioSource source = audioPlayer.gameObject.GetComponent<AudioSource>();
					source.spatialBlend = 1.0f;
					source.rolloffMode = AudioRolloffMode.Linear;
					source.minDistance = 30f;
					source.maxDistance = 100f;
				}
				else
				{
					audioPlayer = AudioPlayerPool.GetPoolableObject();
				}

				audioPlayer.gameObject.name = "SoundBlast";

				audioPlayer.Play(playCommand);
				audioPlayer.WaitAndDestroy(audioPlayer.Source.clip);

				return audioPlayer;
			}
		}

		public void Play(IPlayCommand playCommand)
		{
			CurrentPlayCommand = playCommand;

			if (CurrentPlayCommand.Clip == null)
			{
				LogUtil.Warning(LogTags.AUDIO, this, "No audioClip provided!");
				return;
			}

			Source.timeSamples = 0; // Essential fix, if time was set in a previous play on this AudioPlayer, a bug will occur where it keeps this time. Reference: oilslick fire in Geostorm.
			Source.clip = CurrentPlayCommand.Clip;
			Source.outputAudioMixerGroup = CurrentPlayCommand.AudioMixerGroup;
			Source.pitch = CurrentPlayCommand.Pitch;
			Source.dopplerLevel = CurrentPlayCommand.DopplerLevel;
			Source.loop = CurrentPlayCommand.Loop;
			Source.ignoreListenerPause = CurrentPlayCommand.IgnoreListenerPause;

			CurrentStopCommand = null;

			PlayerVolume = 1f;

			StopPlayAndStopCoroutines();
			playRoutine = StartCoroutine(DelayedPlayRoutine(playCommand.VolumeEnvelope.Delay));
		}

		public void Pause()
		{
			Source.Pause();
			isPaused = true;
		}

		public void Unpause()
		{
			Source.UnPause();
			isPaused = false;
		}

		public Coroutine Stop(float releaseDelay = 0f)
		{
			if (CurrentPlayCommand == null)
			{
				return null;
			}

			return Stop(new StopCommand(CurrentPlayCommand.VolumeEnvelope), releaseDelay);
		}

		public Coroutine Stop(IStopCommand customStopCommand, float releaseDelay = 0f) // Might want to pull delay to DAHDSR
		{
			if (!gameObject.activeInHierarchy)
			{
				return null;
			}

			inDelayPhase = false; // Make sure we reset this, if we Stop() within the delay phase

			DAHDSR volumeEnvelope = customStopCommand.VolumeEnvelope;
			volumeEnvelope.ReleaseStartTime = (float)(AudioSettings.dspTime - startTime + releaseDelay);
			customStopCommand.VolumeEnvelope = volumeEnvelope;

			CurrentStopCommand = customStopCommand;

			StopPlayAndStopCoroutines();
			stopRoutine = StartCoroutine(StopRoutine(customStopCommand.VolumeEnvelope.Release, releaseDelay));
			return stopRoutine;
		}

		public void GetSpectrumData(float[] samples, int channel, FFTWindow window)
		{
			Source.GetSpectrumData(samples, channel, window);
		}

		protected void OnAudioFilterRead(float[] data, int channels)
		{
			float currentPlayCommandVolume = CurrentPlayCommand != null ? CurrentPlayCommand.Volume : 1f;
			float relativeTime = (float)(AudioSettings.dspTime - startTime);
			dahdsrVolume = GetDAHDSRVolume(relativeTime);

			//Debug.Log("relativeTime: " + relativeTime + "\tdahdsrVolume: " + dahdsrVolume);

			finalVolume = playerVolume * dahdsrVolume * currentPlayCommandVolume;

			for (int i = 0; i < data.Length; i++)
			{
				data[i] *= finalVolume;
			}
		}

		protected virtual void Update()
		{
#if UNITY_EDITOR
			isPlaying = IsPlaying;
#endif

			if (CurrentPlayCommand != null && CurrentPlayCommand.FollowTransform != null)
			{
				Source.spatialBlend = 1f;
				transform.position = CurrentPlayCommand.FollowTransform.position;
				transform.rotation = CurrentPlayCommand.FollowTransform.rotation;
			}
			else
			{
				Source.spatialBlend = 0f;
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
			}

			previousTime = currentTime;
			currentTime = Time;
		}

		private IEnumerator DelayedPlayRoutine(double delay)
		{
			inDelayPhase = false;

			startTime = AudioSettings.dspTime;

			if (delay > 0f)
			{
				Source.PlayScheduled(AudioSettings.dspTime + delay);
				inDelayPhase = true;
				yield return new WaitForSeconds((float)CurrentPlayCommand.VolumeEnvelope.Delay);
				inDelayPhase = false;
			}
			else
			{
				Source.Play();
			}

			//startTime = AudioSettings.dspTime - delay; // Now our startTime is not effected by any updates

		}

		private IEnumerator StopRoutine(double release, double delay = 0f)
		{
			if (delay > 0f)
			{
				Source.SetScheduledEndTime(AudioSettings.dspTime + release + delay);
			}

			yield return new WaitForSeconds((float)(delay + release));

			if (delay <= 0f)
			{
				Source.Stop();
			}

			CurrentPlayCommand = null;
			CurrentStopCommand = null;
		}

		private void StopPlayAndStopCoroutines()
		{
			if (playRoutine != null)
			{
				StopCoroutine(playRoutine);
			}

			if (stopRoutine != null)
			{
				StopCoroutine(stopRoutine);
			}
		}

		private void WaitAndDestroy(AudioClip clip)
		{
			StartCoroutine(_WaitAndDestroy(clip));
		}

		private IEnumerator _WaitAndDestroy(AudioClip clip)
		{
			while ((IsPlaying || AudioListener.pause) && Source.clip == clip)
			{
				yield return null;
			}
			ReturnToPool();
		}

		private float GetDAHDSRVolume(float time)
		{
			if (CurrentStopCommand != null)
			{
				return CurrentStopCommand.VolumeEnvelope.Evaluate(time);
			}

			if (CurrentPlayCommand != null)
			{
				return CurrentPlayCommand.VolumeEnvelope.Evaluate(time);
			}

			return 1f;
		}
	}
}
