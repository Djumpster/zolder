// Copyright 2020 Talespin, LLC. All Rights Reserved.

using UnityEngine;
using UnityEngine.Timeline;

namespace Talespin.Core.Foundation.Timeline
{
	/// <summary>
	/// A track for the Unity timeline that allows playback of audio on a specific <see cref="AudioSource"/> 
	/// </summary>
	[TrackColor(0.7f, 0.5f, 0f)]
	[TrackClipType(typeof(TimelinePlayAudioAsset))]
	[TrackBindingType(typeof(AudioSource))]
	public class TimelinePlayAudioTrack : TrackAsset
	{
	}
}
