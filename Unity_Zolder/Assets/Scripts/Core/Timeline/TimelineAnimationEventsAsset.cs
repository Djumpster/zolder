// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Animations;
using UnityEngine;
using UnityEngine.Playables;

namespace Talespin.Core.Foundation.Timeline
{
	/// <summary>
	/// Contains information for invoking an AnimationEventWrapper.
	/// </summary>
	[System.Serializable]
	public class TimelineAnimationEventsAsset : PlayableAsset, ITimelineAssetDataSetter
	{
		[SerializeField] private AnimationEventParameterContainer animationEventParameters;

		public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
		{
			ScriptPlayable<TimelineAnimationEventsPlayableBehaviour> playable = ScriptPlayable<TimelineAnimationEventsPlayableBehaviour>.Create(graph);

			TimelineAnimationEventsPlayableBehaviour behaviour = playable.GetBehaviour();

			behaviour.Initialize(animationEventParameters);

			return playable;
		}

		/// <summary>
		/// Sets the needed data, casts it to <see cref="AnimationEventParameterContainer"/> that is needed by the <see cref="PlayableBehaviour"/>
		/// </summary>
		/// <param name="data">an <see cref="AnimationEventParameterContainer"/> object</param>
		public void SetData(object data)
		{
			animationEventParameters = data as AnimationEventParameterContainer;
		}
	}
}
