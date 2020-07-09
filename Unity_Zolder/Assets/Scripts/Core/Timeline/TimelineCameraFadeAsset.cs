// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;
using UnityEngine.Playables;

namespace Talespin.Core.Foundation.Timeline
{
	/// <summary>
	/// Contains information for performing a camera fade to a color.
	/// </summary>
	[System.Serializable]
	public class TimelineCameraFadeAsset : PlayableAsset, ITimelineAssetDataSetter
	{
		[SerializeField] private TimelineCameraFadeParameters fadeParameters;

		public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
		{
			ScriptPlayable<TimelineCameraFadePlayableBehaviour> playable = ScriptPlayable<TimelineCameraFadePlayableBehaviour>.Create(graph);

			TimelineCameraFadePlayableBehaviour behaviour = playable.GetBehaviour();

			behaviour.Initialize(fadeParameters.FromColor, fadeParameters.ToColor, fadeParameters.AlsoFadeUI);

			return playable;
		}

		/// <summary>
		/// Sets the needed data, casts it to <see cref="TimelineCameraFadeParameters"/> that is needed by the <see cref="PlayableBehaviour"/>
		/// </summary>
		/// <param name="data">a <see cref="TimelineCameraFadeParameters"/> object</param>
		public void SetData(object data)
		{
			fadeParameters = data as TimelineCameraFadeParameters;
		}
	}
}
