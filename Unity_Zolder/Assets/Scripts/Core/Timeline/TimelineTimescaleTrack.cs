// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine.Timeline;

namespace Talespin.Core.Foundation.Timeline
{
	[TrackColor(1f, 0.2f, 0.1f)]
	[TrackClipType(typeof(TimelineTimescaleAsset))]
	/// <summary>
	/// Timeline track for timescaling.
	/// </summary>
	public class TimelineTimescaleTrack : TrackAsset
	{
#if !UNITY_2018_1_OR_NEWER
		protected override Playable CreatePlayable(PlayableGraph graph, GameObject go, TimelineClip clip)
		{
			ScriptPlayable<TimelineTimescalePlayableBehaviour> playable = (ScriptPlayable<TimelineTimescalePlayableBehaviour>)base.CreatePlayable(graph, go, clip);

			TimelineTimescalePlayableBehaviour behaviour = playable.GetBehaviour();
			behaviour.Clip = clip;

			return playable;
		}
#endif
	}
}