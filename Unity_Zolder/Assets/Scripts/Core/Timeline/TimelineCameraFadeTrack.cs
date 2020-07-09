// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine.Timeline;

namespace Talespin.Core.Foundation.Timeline
{
	/// <summary>
	/// A track for on the Timeline to perform a camera fade to a specific color.
	/// </summary>
	[TrackColor(0.31f, 1f, 0.55f)]
	[TrackClipType(typeof(TimelineCameraFadeAsset))]
	[System.Serializable]
	public class TimelineCameraFadeTrack : TrackAsset
	{
#if !UNITY_2018_1_OR_NEWER
		protected override Playable CreatePlayable(PlayableGraph graph, GameObject go, TimelineClip clip)
		{
			ScriptPlayable<TimelineCameraFadePlayableBehaviour> playable = (ScriptPlayable<TimelineCameraFadePlayableBehaviour>)base.CreatePlayable(graph, go, clip);

			TimelineCameraFadePlayableBehaviour behaviour = playable.GetBehaviour();
			behaviour.Clip = clip;

			return playable;
		}
#endif
	}
}