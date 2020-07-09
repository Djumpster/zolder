// Copyright 2019 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	public class AudioBankBehaviour : BaseAudioBankBehaviour
	{
		[System.Serializable]
		public class PlayInfo
		{
			public string AudioID;
			public AudioPlayer Player;
			public GameObject Context;

			public PlayInfo(AudioPlayer player, string audioID, GameObject context)
			{
				Player = player;
				AudioID = audioID;
				Context = context;
			}
		}

		public override bool ReadyForCleanup => false;

		[Header("Debug:")]
		[SerializeField] private List<PlayInfo> players;

		private Dictionary<PlayInfo, Action> audioFinishedActions;
		private List<KeyValuePair<PlayInfo, Action>> finishedAudioClipsKvp;

		public virtual void Init(int numChannels)
		{
			InitBase(null, numChannels);

			players = new List<PlayInfo>();
			audioFinishedActions = new Dictionary<PlayInfo, Action>();
			finishedAudioClipsKvp = new List<KeyValuePair<PlayInfo, Action>>();

			foreach (AudioPlayer player in channeledAudio.AudioPlayers)
			{
				players.Add(new PlayInfo(player, string.Empty, null));
			}
		}

		public void Update()
		{
			InvokeAndRemoveFinishedAudioClips();
		}

		public override void HandleEvent(BaseAudioEvent audioEvent, out PlayInfo usedAudioPlayer, Action onAudioClipFinishedAction = null)
		{
			usedAudioPlayer = null;
			if (audioEvent is PlayAudioEvent playAudioEvent)
			{
				usedAudioPlayer = Play(playAudioEvent, onAudioClipFinishedAction);
			}
			else if (audioEvent is StopAudioEvent stopAudioEvent)
			{
				usedAudioPlayer = Stop(stopAudioEvent);
			}
			else if (audioEvent is StopAllAudioEvent stopAllAudioEvent)
			{
				StopAll(stopAllAudioEvent);
			}
			else if (audioEvent is PauseAudioEvent pauseAudioEvent)
			{
				usedAudioPlayer = Pause(pauseAudioEvent);
			}
			else if (audioEvent is PauseAllAudioEvent pauseAllAudioEvent)
			{
				PauseAll(pauseAllAudioEvent);
			}
			else if (audioEvent is SeekAudioEvent seekAudioEvent)
			{
				usedAudioPlayer = Seek(seekAudioEvent);
			}
		}

		public override void PauseBehaviour(bool pause = true)
		{
			foreach (PlayInfo playInfo in players)
			{
				if (pause)
				{
					playInfo.Player.Pause();
				}
				else
				{
					playInfo.Player.Unpause();
				}
			}
		}

		private void InvokeAndRemoveFinishedAudioClips()
		{
			foreach (var kvp in audioFinishedActions)
			{
				if (!kvp.Key.Player.IsPlaying && !kvp.Key.Player.IsPaused)
				{
					kvp.Value.Invoke();
					finishedAudioClipsKvp.Add(kvp);
				}
			}

			for (int i = 0; i < finishedAudioClipsKvp.Count; i++)
			{
				audioFinishedActions.Remove(finishedAudioClipsKvp[i].Key);
			}
			finishedAudioClipsKvp.Clear();
		}

		private PlayInfo Play(PlayAudioEvent playAudioEvent, Action onAudioClipFinishedAction = null)
		{
			PlayCommand playCommand = new PlayCommand(playAudioEvent.AudioBankEntry.AudioConfiguration);

			// TODO probably want follow/3D as an optional flag in the config
			bool follows = playAudioEvent.Context != null;
			if (follows)
			{
				playCommand.FollowTransform = playAudioEvent.Context.transform;
			}

			AudioPlayer player = channeledAudio.TryPlay(playCommand);

			if (player != null)
			{
				PlayInfo playInfo = players.First(info => info.Player == player);
				playInfo.AudioID = playAudioEvent.AudioBankEntry.AudioID;
				playInfo.Context = playAudioEvent.Context;
				if (onAudioClipFinishedAction != null)
				{
					audioFinishedActions.Add(playInfo, onAudioClipFinishedAction);
				}
				return playInfo;
			}
			return null;
		}

		private PlayInfo Stop(StopAudioEvent stopAudioEvent)
		{
			PlayInfo playInfo = players.FirstOrDefault(i => i.AudioID == stopAudioEvent.AudioBankEntry.AudioID &&
												i.Context == stopAudioEvent.Context);

			if (playInfo == null)
			{
				return null;
			}

			Stop(new PlayInfo[] { playInfo }, stopAudioEvent.AudioBankEntry.AudioConfiguration);
			return playInfo;
		}

		private void Stop(IEnumerable<PlayInfo> playInfos, AudioConfiguration audioConfiguration)
		{
			foreach (PlayInfo playInfo in playInfos)
			{
				StopCommand stopCommand = new StopCommand(audioConfiguration);
				if (playInfo.Player != null)
				{
					playInfo.Player.Stop(stopCommand);

					playInfo.AudioID = string.Empty;
					playInfo.Context = null;
				}
			}
		}

		private void StopAll(StopAllAudioEvent stopAllAudioEvent)
		{
			IEnumerable<PlayInfo> playInfos = players.Where(i => i.AudioID == stopAllAudioEvent.AudioBankEntry.AudioID &&
															i.Context == stopAllAudioEvent.Context);

			Stop(playInfos, stopAllAudioEvent.AudioBankEntry.AudioConfiguration);
		}

		private PlayInfo Pause(PauseAudioEvent pauseAudioEvent)
		{
			throw new NotImplementedException();
		}

		private void PauseAll(PauseAllAudioEvent pauseAllAudioEvent)
		{
			throw new NotImplementedException();
		}

		private PlayInfo Seek(SeekAudioEvent seekAudioEvent)
		{
			throw new NotImplementedException();
		}
	}
}
