// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
using System.Linq;
using Talespin.Core.Foundation.Attributes;
using Talespin.Core.Foundation.Events;
using Talespin.Core.Foundation.Injection;
using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	public class UIAudioImpl : AudioBehaviourBase
	{
		[System.Serializable]
		public class UIAudioSet
		{
			public AudioAsset AudioAsset { get { return audioAsset; } }
			public string AudioType { get { return audioType; } }
			public string StopLoopType { get { return stopLoopType; } }

			[SerializeField, ConstantTag(typeof(string), typeof(UIAudioTypeBase))] private string audioType = UIAudioTypes.BUTTON_GENERIC;
			[SerializeField, ConstantTag(typeof(string), typeof(UIAudioTypeBase))] private string stopLoopType = UIAudioTypes.BUTTON_GENERIC;
			[SerializeField] private AudioAsset audioAsset;
		}

		private enum Operators
		{
			Equals,
			NotEquals,
			GreaterThan,
			SmallerThan
		}

		private class ContextLoop
		{
			public object Context { get; private set; }
			public AudioPlayer Loop { get; private set; }

			public ContextLoop(AudioPlayer loop, object context)
			{
				Context = context;
				Loop = loop;
			}
		}

		public override bool ReadyForCleanup { get { return false; } }

		[SerializeField] private UIAudioSet[] uiAudioSets;

		private GlobalEvents globalEvents;
		private List<AudioPlayer> blasts;
		private Dictionary<string, List<ContextLoop>> loops;

		public void Init(object context, int numChannels)
		{
			InitBase(context, numChannels);

			blasts = new List<AudioPlayer>();
			loops = new Dictionary<string, List<ContextLoop>>();

			globalEvents = GlobalDependencyLocator.Instance.Get<GlobalEvents>();

			globalEvents.Subscribe<UIAudioEvent>(OnUIAudioEvent);
		}

		public override void OnCleanup()
		{
			base.OnCleanup();

			globalEvents.Unsubscribe<UIAudioEvent>(OnUIAudioEvent);
		}

		private void OnUIAudioEvent(UIAudioEvent e)
		{
			// Catch stop all
			if (e.AudioType == UIAudioTypes.STOP_ALL)
			{
				// Blasts
				foreach (AudioPlayer player in blasts)
				{
					if (player != null && player.IsPlaying)
					{
						player.Stop();
					}
				}

				blasts.Clear();

				// Loops
				foreach (KeyValuePair<string, List<ContextLoop>> kvp in loops)
				{
					foreach (ContextLoop contextLoop in kvp.Value)
					{
						if (contextLoop.Loop != null && contextLoop.Loop.IsPlaying)
						{
							contextLoop.Loop.Stop();
						}
					}
				}

				loops.Clear();

				return;
			}

			// Play or stop loop
			UIAudioSet uiAudioSet = uiAudioSets.FirstOrDefault(s => s.AudioType == e.AudioType || s.StopLoopType == e.AudioType);

			if (uiAudioSet != null)
			{
				bool isPlayEvent = e.AudioType == uiAudioSet.AudioType;

				if (isPlayEvent)
				{
					// We're playing
					PlayCommand playCommand = new PlayCommand(uiAudioSet.AudioAsset);
					if (e.Context != null && e.Context is Transform)
					{
						playCommand.FollowTransform = e.Context as Transform;
					}
					AudioPlayer audioPlayer = channeledAudio.TryPlay(playCommand);

					// Loop play?
					if (audioPlayer != null && playCommand.Loop)
					{
						if (!loops.ContainsKey(uiAudioSet.StopLoopType))
						{
							loops.Add(uiAudioSet.StopLoopType, new List<ContextLoop>());
						}
						loops[uiAudioSet.StopLoopType].Add(new ContextLoop(audioPlayer, e.Context));
					}
				}
				else
				{
					// We're stopping a loop
					if (loops.ContainsKey(uiAudioSet.StopLoopType))
					{
						for (int i = loops[uiAudioSet.StopLoopType].Count - 1; i >= 0; i--)
						{
							ContextLoop contextLoop = loops[uiAudioSet.StopLoopType][i];
							if (e.Context == null || e.Context == contextLoop.Context)
							{
								StopCommand stopCommand = new StopCommand(uiAudioSet.AudioAsset);
								contextLoop.Loop.Stop(stopCommand);
								loops[uiAudioSet.StopLoopType].RemoveAt(i);
							}
						}
						if (loops[uiAudioSet.StopLoopType].Count == 0)
						{
							loops.Remove(uiAudioSet.StopLoopType);
						}
					}
				}
			}
		}
	}
}