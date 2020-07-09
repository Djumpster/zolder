// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Animations;
using Talespin.Core.Foundation.Events;
using Talespin.Core.Foundation.Injection;
using Talespin.Core.Foundation.Logging;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Talespin.Core.Foundation.Timeline
{
	/// <summary>
	/// Invokes an AnimationEventWrapper when a clip starts.
	/// </summary>
	[System.Serializable]
	public class TimelineAnimationEventsPlayableBehaviour : PlayableBehaviour
	{
		public TimelineClip Clip
		{
			get
			{
				return clip;
			}
			set
			{
				clip = value;
				clip.displayName = "Animation Event";
			}
		}

		private TimelineClip clip;
		private AnimationEventParameterContainer animationEventParameters;

		/// <summary>
		/// Initializes the behaviour with the passed <see cref="AnimationEventParameterContainer"/>
		/// </summary>
		/// <param name="animationEventParameters">The parameters to process</param>
		public void Initialize(AnimationEventParameterContainer animationEventParameters)
		{
			if (!Application.isPlaying)
			{
				return;
			}

			this.animationEventParameters = animationEventParameters;
		}

		public override void OnBehaviourPlay(Playable playable, FrameData info)
		{
			if (!Application.isPlaying)
			{
				return;
			}

			if (animationEventParameters == null)
			{
				LogUtil.Warning(LogTags.ANIMATION, this, "Can't invoke timeline event because a check returns null");
				return;
			}

			GlobalDependencyLocator.Instance.Get<GlobalEvents>().Invoke(CreateAnimationEventWrapper());
		}

		private AnimationEventWrapper CreateAnimationEventWrapper()
		{
			AnimationEventWrapper wrapper = new AnimationEventWrapper
			{
				floatParameter = animationEventParameters.FloatParameter,
				intParameter = animationEventParameters.IntParameter,
				stringParameter = animationEventParameters.StringParameter,
				objectReferenceParameter = animationEventParameters.ObjectParameter
			};

			return wrapper;
		}
	}
}
