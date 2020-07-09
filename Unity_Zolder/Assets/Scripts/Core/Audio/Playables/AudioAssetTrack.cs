// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine.Timeline;

namespace Talespin.Core.Foundation.Audio
{
	[TrackColor(1f, 0.384f, 0.082f)]
	[TrackClipType(typeof(AudioAssetPlayableAsset))]
	public class AudioAssetTrack : TrackAsset
	{
		// public override Playable CreateTrackMixer(UnityEngine.Playables.PlayableGraph graph, GameObject go, int inputCount)
		// {
		// 	Debug.LogFormat("<color=yellow>CreateTrackMixer</color>");
		// 	return ScriptPlayable<AudioAssetPlayableBehaviour>.Create(graph, inputCount);
		// }

#if !UNITY_2018_1_OR_NEWER
		protected override Playable CreatePlayable(PlayableGraph graph, GameObject go, TimelineClip clip)
		{
			ScriptPlayable<AudioAssetPlayableBehaviour> playable = (ScriptPlayable<AudioAssetPlayableBehaviour>)base.CreatePlayable(graph, go, clip);

			AudioAssetPlayableBehaviour behaviour = playable.GetBehaviour();
			behaviour.Clip = clip;

			return playable;
		}
#endif
	}
}