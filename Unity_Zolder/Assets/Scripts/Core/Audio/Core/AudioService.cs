// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Talespin.Core.Foundation.Logging;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace Talespin.Core.Foundation.Audio
{
	/// <summary>
	/// The AudioService is the main entrypoint to control audio through <see cref="AudioBanks"/>.
	/// It instantiates an <see cref="BaseAudioBankBehaviour"/> as configured in each <see cref="AudioBank"/>, and 
	/// audio control methods find the associated behaviour to invoke the event upon.
	/// </summary>
	public class AudioService : System.IDisposable
	{
		private readonly AudioBanks[] audioBanks;
		private readonly GameObject audioRoot;
		private readonly AudioBankParameters parameters;
		private readonly Dictionary<AudioBank, BaseAudioBankBehaviour> audioBankMap;

		public AudioService(AudioBanks[] audioBanks)
		{
			this.audioBanks = audioBanks;

			audioBankMap = new Dictionary<AudioBank, BaseAudioBankBehaviour>();
			audioRoot = new GameObject("Audio");
			parameters = audioRoot.AddComponent<AudioBankParameters>();
			UnityObject.DontDestroyOnLoad(audioRoot);

			InitializeAudioBankBehaviours();
		}

		public void Dispose()
		{
			UnityObject.Destroy(audioRoot);
		}

		/// <summary>
		/// Starts playing audio with the given ID within the given context.
		/// </summary>
		/// <param name="audioID">The audio ID as declared in an <see cref="AudioBank"/>.</param>
		/// <param name="context">Optional. The GameObject which forms the context for this event. If provided, the 
		/// player will switch to 3D mode and follow the context. If none gets provided, the audio is played inside 
		/// the global domain. </param>
		/// <param name="OnAudioClipFinishedAction">Optional. An action that will be executed when the audio clip has
		/// finished playing.</param>
		public AudioBankBehaviour.PlayInfo Play(string audioID, GameObject context = null, Action onAudioClipFinishedAction = null)
		{
			(bool success, _, AudioBankEntry audioBankEntry, BaseAudioBankBehaviour audioBankBehaviour) = GetEventDependencies(audioID);
			if (!success)
			{
				return null;
			}

			audioBankBehaviour.HandleEvent(new PlayAudioEvent(audioBankEntry, context), out AudioBankBehaviour.PlayInfo playInfo, onAudioClipFinishedAction);
			return playInfo;
		}

		/// <summary>
		/// Stops playing the first encountered (non chronological) audio with the given ID within the given context.
		/// </summary>
		/// <param name="audioID">The audio ID as declared in an <see cref="AudioBank"/>.</param>
		/// <param name="context">Optional. The GameObject which forms the context for this event.</param>
		public void Stop(string audioID, GameObject context = null)
		{
			(bool success, _, AudioBankEntry audioBankEntry, BaseAudioBankBehaviour audioBankBehaviour) = GetEventDependencies(audioID);
			if (!success)
			{
				return;
			}

			audioBankBehaviour.HandleEvent(new StopAudioEvent(audioBankEntry, context), out AudioBankBehaviour.PlayInfo playInfo);
		}

		/// <summary>
		/// Stops playing all audio with the given ID within the given context.
		/// </summary>
		/// <param name="audioID">The audio ID as declared in an <see cref="AudioBank"/>.</param>
		/// <param name="context">Optional. The GameObject which forms the context for this event.</param>
		public void StopAll(string audioID, GameObject context = null)
		{
			(bool found, _, AudioBankEntry audioBankEntry, BaseAudioBankBehaviour audioBankBehaviour) = GetEventDependencies(audioID);
			if (!found)
			{
				return;
			}

			audioBankBehaviour.HandleEvent(new StopAllAudioEvent(audioBankEntry, context), out AudioBankBehaviour.PlayInfo playInfo);
		}

		/// <summary>
		/// Pauses all audio.
		/// </summary>
		/// <param name="pause">The audio ID as declared in an <see cref="AudioBank"/>.</param>
		public void PauseService(bool pause = true)
		{
			audioBankMap.Values.ToList().ForEach(audioBankBehaviour => audioBankBehaviour.PauseBehaviour(pause));
		}

		/*public void SetParameter(string parameterID, float value, GameObject context = null)
		{
            parameters.SetParameter(parameterID, value, context);
		}

		public float GetParameter(string parameterID, GameObject context = null)
		{
            float? value = parameters.GetParameter(parameterID, context);

            if (value == null)
            {
			    throw new KeyNotFoundException("Did not find parameter with key '" + parameterID + "' in the global parameter list.");
            }

            return value.Value;
		}*/

		/// <summary>
		/// Initializes the behaviour on each <see cref="AudioBank"/> as declared in <see cref="AudioBanks"/>.
		/// </summary>
		private void InitializeAudioBankBehaviours()
		{
			foreach (AudioBank audioBank in audioBanks.SelectMany(audioBanks => audioBanks.Banks))
			{
				GameObject audioBankGameObject = new GameObject("AudioBank: " + audioBank.AudioBankID);
				audioBankGameObject.transform.SetParent(audioRoot.transform);

				AudioBankBehaviour audioBankBehaviour = audioBankGameObject.AddComponent(audioBank.BehaviourType) as AudioBankBehaviour;
				audioBankBehaviour.Init(audioBank.NumberOfChannels);

				audioBankMap.Add(audioBank, audioBankBehaviour);
			}
		}

		/// <summary>
		/// Returns relevant information for a given audioID.
		/// </summary>
		/// <param name="audioID">The audio ID as declared in an <see cref="AudioBank"/>.</param>
		/// <returns>(bool found, <see cref="AudioBank"/>, <see cref="AudioBankEntry"/>, <see cref="BaseAudioBankBehaviour"/>)</returns>
		private (bool, AudioBank, AudioBankEntry, BaseAudioBankBehaviour) GetEventDependencies(string audioID)
		{
			(AudioBank audioBank, AudioBankEntry audioBankEntry) = GetAudioBankEntry(audioID);
			if (audioBank == null || audioBankEntry == null)
			{
				LogUtil.Error(LogTags.AUDIO, this, "No config found for audio event '" + audioID + "'.");
				return (false, null, null, null);
			}

			if (!audioBankMap.TryGetValue(audioBank, out BaseAudioBankBehaviour audioBankBehaviour))
			{
				LogUtil.Error(LogTags.AUDIO, this, "No behaviour found for audio event '" + audioID + "'.");
				return (false, null, null, null);
			}

			return (true, audioBank, audioBankEntry, audioBankBehaviour);
		}

		/// <summary>
		/// Returns the <see cref="AudioBank"/> and <see cref="AudioBankEntry"/> associated with the audioID.
		/// </summary>
		/// <param name="audioID">The audioID which we are interested in.</param>
		/// <returns>A tuple containing the related <see cref="AudioBank"/> and <see cref="AudioBankEntry"/>.</returns>
		public (AudioBank, AudioBankEntry) GetAudioBankEntry(string audioID)
		{
			foreach (AudioBanks audioBanksEntry in audioBanks)
			{
				(AudioBank audioBank, AudioBankEntry audioBankEntry) = audioBanksEntry.GetAudioBankEntry(audioID);

				if (audioBankEntry != null)
				{
					return (audioBank, audioBankEntry);
				}
			}

			return (null, null);
		}
	}
}
