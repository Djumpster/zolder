// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine.Timeline;

namespace Talespin.Core.Foundation.Timeline
{
	[TrackColor(0.31f, 1f, 0.55f)]
	[TrackClipType(typeof(TimelineAnimationEventsAsset))]
	[System.Serializable]
	public class TimelineAnimationEventsTrack : TrackAsset
	{
#if !UNITY_2018_1_OR_NEWER
		protected override Playable CreatePlayable(PlayableGraph graph, GameObject go, TimelineClip clip)
		{
			ScriptPlayable<TimelineAnimationEventsPlayableBehaviour> playable = (ScriptPlayable<TimelineAnimationEventsPlayableBehaviour>)base.CreatePlayable(graph, go, clip);

			TimelineAnimationEventsPlayableBehaviour behaviour = playable.GetBehaviour();
			behaviour.Clip = clip;

			return playable;
		}
#endif
	}
}