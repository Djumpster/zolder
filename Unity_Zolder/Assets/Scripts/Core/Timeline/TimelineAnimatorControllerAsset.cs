// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using Talespin.Core.Foundation.Animations;
using UnityEngine;
using UnityEngine.Playables;

namespace Talespin.Core.Foundation.Timeline
{
	/// <summary>
	/// Contains information for manipulating the parameters of an AnimatorController.
	/// </summary>
	[Serializable]
	public class TimelineAnimatorControllerAsset : PlayableAsset, ITimelineAssetDataSetter
	{
		[SerializeField] private AnimatorParameterContainer animatorParameterContainer;

		public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
		{
			ScriptPlayable<TimelineAnimatorControllerBehaviour> playable = ScriptPlayable<TimelineAnimatorControllerBehaviour>.Create(graph);
			TimelineAnimatorControllerBehaviour behaviour = playable.GetBehaviour();
			behaviour.SetAnimationParameterContainer(animatorParameterContainer);

			return playable;
		}

		/// <summary>
		/// Sets the needed data, casts it to <see cref="AnimatorParameterContainer"/> that is needed by the <see cref="PlayableBehaviour"/>
		/// </summary>
		/// <param name="data">an <see cref="AnimatorParameterContainer"/> object</param>
		public void SetData(object data)
		{
			animatorParameterContainer = data as AnimatorParameterContainer;
		}
	}
}
