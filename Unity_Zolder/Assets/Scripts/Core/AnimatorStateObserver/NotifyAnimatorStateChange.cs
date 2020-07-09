// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Attributes;
using UnityEngine;

namespace Talespin.Core.Foundation.AnimatorStateObserver
{
	/// <summary>
	/// Attach this script to an AnimationState in mecanim to make it send the specified identifier to the CharacterStateInfo MonoBehaviour.
	/// The identifier will serve as an abstraction of the state Mecanim is in ('Attacking' rather than a specific attack animation).
	/// </summary>
	public class NotifyAnimatorStateChange : StateMachineBehaviour
	{
		[SerializeField, ConstantTag(typeof(string), typeof(AnimatorStateTag))]
		private string biomechanicState = AnimatorStateTag.IDLE;
		private AnimatorStateObserver biomechanicStateHandler;

		/// <summary>
		/// Called at the start of a transition to this state.
		/// (Note: StateMachineBhaviour is a built-in Unity class! This will most likely never be called explicitly from our proprietary code)
		/// </summary>
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (biomechanicStateHandler == null)
			{
				biomechanicStateHandler = animator.GetComponent<AnimatorStateObserver>();
			}
			if (biomechanicStateHandler != null)
			{
				biomechanicStateHandler.SignalHeartbeat(biomechanicState, stateInfo.shortNameHash);
			}
		}

		/// <summary>
		/// OnStateExit is highly unreliable. We use a heartbeat system through Update to track states instead.
		/// Called by Unity.
		/// </summary>
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (biomechanicStateHandler == null)
			{
				biomechanicStateHandler = animator.GetComponent<AnimatorStateObserver>();
			}
			if (biomechanicStateHandler != null)
			{
				biomechanicStateHandler.SignalHeartbeat(biomechanicState, stateInfo.shortNameHash);
			}
		}
	}
}
