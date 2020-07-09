// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Filter;
using UnityEngine;
using UnityEngine.Playables;

namespace Talespin.Core.Foundation.Timeline
{
	/// <summary>
	/// Contains information for invoking an IEvent on GlobalEvents using Timeline.
	/// </summary>
	[System.Serializable]
	public class TimelineGlobalEventsAsset : PlayableAsset, ITimelineAssetDataSetter
	{
		[SerializeField, TypeFilter(typeof(Events.IEvent))] private string globalEvent;

		public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
		{
			ScriptPlayable<TimelineGlobalEventsPlayableBehaviour> playable = ScriptPlayable<TimelineGlobalEventsPlayableBehaviour>.Create(graph);

			TimelineGlobalEventsPlayableBehaviour behaviour = playable.GetBehaviour();

			behaviour.Initialize(globalEvent);

			return playable;
		}

		/// <summary>
		/// Sets the needed data, casts data to a string that is needed by the <see cref="PlayableBehaviour"/>
		/// </summary>
		/// <param name="data"></param>
		public void SetData(object data)
		{
			globalEvent = data as string;
		}
	}
}
