// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;
using UnityEngine.Playables;

namespace Talespin.Core.Foundation.Timeline
{
	/// <summary>
	/// Contains information for changing the timescale through TimeLine
	/// </summary>
	public class TimelineTimescaleAsset : PlayableAsset, ITimelineAssetDataSetter
	{
		[SerializeField] private float timeScale = 1f;

		public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
		{
			ScriptPlayable<TimelineTimescalePlayableBehaviour> playable = ScriptPlayable<TimelineTimescalePlayableBehaviour>.Create(graph);

			TimelineTimescalePlayableBehaviour behaviour = playable.GetBehaviour();

			behaviour.Initialize(timeScale);

			return playable;
		}

		/// <summary>
		/// Sets the needed data, casts it to <see cref="float"/> that is needed by the <see cref="PlayableBehaviour"/>
		/// </summary>
		/// <param name="data">a <see cref="float"/> object</param>
		public void SetData(object data)
		{
			timeScale = (float)data;
		}
	}
}
