// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using System.Linq;
using Talespin.Core.Foundation.Extensions;
using Talespin.Core.Foundation.Logging;
using Talespin.Core.Foundation.Maths;
using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	/// <summary>
	/// ChanneledAudio reserves a number of channels for playing audio. Each channel is an AudioPlayer instance.
	/// It provides control over multiple AudioPlayers, which can play in parallel, while still having a limit to the
	/// amount of players/channels.
	/// The intended implementing behaviour is that it's either extended as a component for custom audio Impl or added
	/// as a component in a more complex setup like AudioService/AudioImpl are doing.
	/// </summary>
	public class ChanneledAudio : MonoBehaviour
	{
		public bool IsPlaying { get { return AudioPlayers.Count(player => player.IsPlaying) != 0; } }
		public List<AudioPlayer> AudioPlayers { get; protected set; }

		[SerializeField] private int numChannels = 8;

		private int lastReturnedChannelIndex = 0;

		public virtual void Init()
		{
			Init(numChannels);
		}

		public virtual void Init(int numChannels)
		{
			this.numChannels = numChannels;

			AudioPlayers = CreateChannels(numChannels);
		}

		public virtual AudioPlayer TryPlay(IPlayCommand playCommand, bool forcePlay = false, AudioPlayer specificPlayer = null)
		{
			AudioPlayer usedPlayer;

			// In case no player is specified, we search for one. In both cases we check whether the players are free.
			if (specificPlayer == null)
			{
				usedPlayer = FindFreeChannel(forcePlay);

				if (usedPlayer == null)
				{
					LogUtil.Warning(LogTags.AUDIO, this, "[ChanneledAudio] Tried to play '" + playCommand.Clip +
						"' but all " + numChannels + " channels were taken.", this);
				}
			}
			else
			{
				// Only play if we are playing BUT forceplay is on (override), or when specificPlayer is not playing
				usedPlayer = forcePlay && specificPlayer.IsPlaying || !specificPlayer.IsPlaying ? specificPlayer : null;
			}

			// Only play if we found a player which is free
			if (usedPlayer != null)
			{
				usedPlayer.Play(playCommand);
			}

			return usedPlayer;
		}

		public List<AudioPlayer> CreateChannels(int numChannels, string domain = "")
		{
			List<AudioPlayer> audioPlayers = new List<AudioPlayer>();

			for (int i = 0; i < numChannels; i++)
			{
				GameObject channel = new GameObject();
				channel.name = (domain != "" ? "[" + domain + "] " : "") + "Channel " + i;
				channel.transform.parent = transform;
				channel.transform.Reset();

				AudioPlayer player = channel.AddComponent<AudioPlayer>();

				audioPlayers.Add(player);
			}

			return audioPlayers;
		}

		private AudioPlayer FindFreeChannel(bool forceFind)
		{
			for (int i = 0; i < AudioPlayers.Count; i++)
			{
				if (!AudioPlayers[i].IsPlaying)
				{
					lastReturnedChannelIndex = i;
					return AudioPlayers[i];
				}
			}

			if (forceFind)
			{
				int forcedPlayerIndex = Math.Wrap(lastReturnedChannelIndex + 1, 0, AudioPlayers.Count - 1);
				lastReturnedChannelIndex = forcedPlayerIndex;
				return AudioPlayers[forcedPlayerIndex];
			}

			return null;
		}
	}
}
