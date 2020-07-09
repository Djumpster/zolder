// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine.Timeline;

namespace Talespin.Core.Foundation.Audio
{
	[TrackColor(1f, 1f, 0.082f)]
	[TrackClipType(typeof(PlayAudioTrackPlayableAsset))]
	public class PlayAudioTrackTrack : TrackAsset
	{
#if !UNITY_2018_1_OR_NEWER
		protected override Playable CreatePlayable(PlayableGraph graph, GameObject go, TimelineClip clip)
		{
			ScriptPlayable<PlayAudioTrackPlayableBehaviour> playable = (ScriptPlayable<PlayAudioTrackPlayableBehaviour>)base.CreatePlayable(graph, go, clip);

			// PlayAudioTrackPlayableBehaviour behaviour = playable.GetBehaviour();

			return playable;
		}
#endif
	}
}