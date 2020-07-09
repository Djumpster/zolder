// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Animations;
using UnityEngine;
using UnityEngine.Playables;

namespace Talespin.Core.Foundation.Timeline
{
	/// <summary>
	/// Manipulates the given animator parameter when the clip starts.
	/// </summary>
	[System.Serializable]
	public class TimelineAnimatorControllerBehaviour : PlayableBehaviour
	{
		public AnimatorParameterContainer AnimatorParameterContainer { get; private set; }

		private bool execute = false;

		public void SetAnimationParameterContainer(AnimatorParameterContainer animatorParameterContainer)
		{
			AnimatorParameterContainer = animatorParameterContainer;
		}

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			base.ProcessFrame(playable, info, playerData);

			Animator animator = playerData as Animator;

			if (execute)
			{
				execute = false;

				for (int i = 0; i < AnimatorParameterContainer.BoolParameters.Count; i++)
				{
					AnimatorParameterContainer.BoolAnimatorParameter boolParameter = AnimatorParameterContainer.BoolParameters[i];
					animator.SetBool(boolParameter.ParameterName, boolParameter.Value);
				}

				for (int i = 0; i < AnimatorParameterContainer.TriggerParameters.Count; i++)
				{
					AnimatorParameterContainer.TriggerAnimatorParameter triggerParameter = AnimatorParameterContainer.TriggerParameters[i];
					animator.SetTrigger(triggerParameter.ParameterName);
				}

				for (int i = 0; i < AnimatorParameterContainer.IntParameters.Count; i++)
				{
					AnimatorParameterContainer.IntAnimatorParameter intParameter = AnimatorParameterContainer.IntParameters[i];
					animator.SetInteger(intParameter.ParameterName, intParameter.Value);
				}
			}
		}

		public override void OnBehaviourPlay(Playable playable, FrameData info)
		{
			base.OnBehaviourPlay(playable, info);

			execute = true;
		}
	}
}
