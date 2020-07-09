// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Animations
{
	/// <summary>
	/// Makes the state machine wait for X frames so you can prevent frame stutters from skipping your animations.
	/// This can happen for example when spawning prefabs that cause frame stutters.
	/// NOTE: Depends on the "show" and "waitingXFrames" parameters to be used in the state machine.
	/// </summary>
	public class WaitXFrames : StateMachineBehaviour
	{
		[SerializeField] private int framesToWait = 1;
		[SerializeField] private string parameterName = "waitingXFrames";

		private bool needsWaiting;
		private bool isWaiting = false;
		private int waitUntillFrame = 0;

		private int _waitHash = -1;
		private int waitHash
		{
			get
			{
				if (_waitHash == -1)
				{
					_waitHash = Animator.StringToHash(parameterName);
				}
				return _waitHash;
			}
		}

		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			needsWaiting = animator.GetBool(waitHash);

			if (needsWaiting)
			{
				isWaiting = true;

				waitUntillFrame = Time.frameCount + framesToWait;
			}
		}

		override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (needsWaiting)
			{
				if (isWaiting && Time.frameCount >= waitUntillFrame)
				{
					isWaiting = false;
					animator.SetBool(waitHash, false);
				}
			}
		}
	}
}