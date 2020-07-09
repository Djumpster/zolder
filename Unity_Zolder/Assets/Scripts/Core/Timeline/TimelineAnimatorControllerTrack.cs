// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Talespin.Core.Foundation.Timeline
{
	/// <summary>
	/// A track for on the Timeline to manipulate animator parameters.
	/// </summary>
	[TrackColor(0.31f, 1f, 0.55f)]
#if !UNITY_2018_2_OR_NEWER
	[TrackMediaType(TimelineAsset.MediaType.Script)]
#endif
	[TrackClipType(typeof(TimelineAnimatorControllerAsset))]
	[TrackBindingType(typeof(Animator))]
	[System.Serializable]
	public class TimelineAnimatorControllerTrack : TrackAsset
	{
		public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
		{
			return ScriptPlayable<TimelineAnimatorControllerMixerBehaviour>.Create(graph, inputCount);
		}
	}
}