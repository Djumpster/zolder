// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using Talespin.Core.Foundation.Events;
using UnityEngine;

namespace Talespin.Core.Foundation.Animations
{
	/// <summary>
	/// Wraps around AnimationEvent so the IEvent interface can be added and it can be used with IEventDispatcher.
	/// </summary>
	public class AnimationEventWrapper : IEvent
	{
		public Animator animator { get; private set; }
		public AnimationEvent AnimationEvent { get; }

		public AnimationState animationState { get { return AnimationEvent.animationState; } }

		public AnimatorClipInfo animatorClipInfo { get { return AnimationEvent.animatorClipInfo; } }

		public AnimatorStateInfo animatorStateInfo { get { return AnimationEvent.animatorStateInfo; } }

		[Obsolete("Use stringParameter instead")]
		public string data
		{
			get { return AnimationEvent.data; }
			set { AnimationEvent.data = value; }
		}

		public float floatParameter
		{
			get { return AnimationEvent.floatParameter; }
			set { AnimationEvent.floatParameter = value; }
		}

		public string functionName
		{
			get { return AnimationEvent.functionName; }
			set { AnimationEvent.functionName = value; }
		}

		public int intParameter
		{
			get { return AnimationEvent.intParameter; }
			set { AnimationEvent.intParameter = value; }
		}

		public bool isFiredByAnimator { get { return AnimationEvent.isFiredByAnimator; } }

		public bool isFiredByLegacy { get { return AnimationEvent.isFiredByLegacy; } }

		public SendMessageOptions messageOptions
		{
			get { return AnimationEvent.messageOptions; }
			set { AnimationEvent.messageOptions = value; }
		}

		public UnityEngine.Object objectReferenceParameter
		{
			get { return AnimationEvent.objectReferenceParameter; }
			set { AnimationEvent.objectReferenceParameter = value; }
		}

		public string stringParameter
		{
			get { return AnimationEvent.stringParameter; }
			set { AnimationEvent.stringParameter = value; }
		}

		public float time
		{
			get { return AnimationEvent.time; }
			set { AnimationEvent.time = value; }
		}

		public AnimationEventWrapper(AnimationEvent animationEvent, Animator animator)
		{
			AnimationEvent = animationEvent;
			this.animator = animator;
		}

		public AnimationEventWrapper()
		{
			AnimationEvent = new AnimationEvent();
		}
	}
}
