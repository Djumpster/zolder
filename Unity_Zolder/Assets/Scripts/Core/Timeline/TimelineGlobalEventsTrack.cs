// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine.Timeline;

namespace Talespin.Core.Foundation.Timeline
{
	/// <summary>
	/// A track for on the Timeline to dispatch IEvents on the GlobalEvents bus.
	/// </summary>
	[TrackColor(0.31f, 1f, 0.55f)]
	[TrackClipType(typeof(TimelineGlobalEventsAsset))]
	[System.Serializable]
	public class TimelineGlobalEventsTrack : TrackAsset
	{
#if !UNITY_2018_1_OR_NEWER
		protected override Playable CreatePlayable(PlayableGraph graph, GameObject go, TimelineClip clip)
		{
			ScriptPlayable<TimelineGlobalEventsPlayableBehaviour> playable = (ScriptPlayable<TimelineGlobalEventsPlayableBehaviour>)base.CreatePlayable(graph, go, clip);

			TimelineGlobalEventsPlayableBehaviour behaviour = playable.GetBehaviour();
			behaviour.Clip = clip;

			return playable;
		}
#endif
	}
}