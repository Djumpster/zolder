// Copyright 2020 Talespin, LLC. All Rights Reserved.

using UnityEngine;
using UnityEngine.Playables;

namespace Talespin.Core.Foundation.Timeline
{
	/// <summary>
	/// This asset contains the needed data, an <see cref="AudioClip"/>, the <see cref="TimelinePlayAudioPlayableBehaviour"/> needs to play back
	/// </summary>
	public class TimelinePlayAudioAsset : PlayableAsset, ITimelineAssetDataSetter
	{
		[SerializeField] private AudioClip audioClip;

		public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
		{
			ScriptPlayable<TimelinePlayAudioPlayableBehaviour> playable = ScriptPlayable<TimelinePlayAudioPlayableBehaviour>.Create(graph);

			TimelinePlayAudioPlayableBehaviour behaviour = playable.GetBehaviour();
			behaviour.Initialize(audioClip);

			return playable;
		}

		/// <summary>
		/// Sets the needed data, casts it to <see cref="AudioClip"/> that is needed by the <see cref="PlayableBehaviour"/>
		/// </summary>
		/// <param name="data">an <see cref="AudioClip"/> object</param>
		public void SetData(object data)
		{
			audioClip = data as AudioClip;
		}
	}
}
