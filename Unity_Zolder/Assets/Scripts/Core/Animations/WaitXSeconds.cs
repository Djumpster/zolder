// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Animations
{
	public class WaitXSeconds : StateMachineBehaviour
	{
		private static readonly int ANIM_WAIT = Animator.StringToHash("waitingXSeconds");

		[SerializeField] private float secondsToWait = 1;

		private float waitUntilTime;

		private bool needsWaiting;
		private bool isWaiting;

		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			needsWaiting = animator.GetBool(ANIM_WAIT);

			if (needsWaiting)
			{
				isWaiting = true;
				waitUntilTime = Time.time + secondsToWait;
			}
		}

		override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (needsWaiting)
			{
				if (isWaiting && Time.time >= waitUntilTime)
				{
					isWaiting = false;
					animator.SetBool(ANIM_WAIT, false);
				}
			}
		}
	}
}