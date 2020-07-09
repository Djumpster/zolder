// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Injection;
using Talespin.Core.Foundation.TimeKeeping;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Talespin.Core.Foundation.Timeline
{
	/// <summary>
	/// Changes the time scale of the game through a timeline clip.
	/// </summary>
	public class TimelineTimescalePlayableBehaviour : PlayableBehaviour
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
				clip.displayName = "Timescale";
			}
		}

		private TimelineClip clip;
		private float timeScale;
		private TimeManagerService timeManagerService;

		/// <summary>
		/// Initializes the behaviour with all the needed data and systems/services
		/// </summary>
		/// <param name="timeScale">the timescale to set</param>
		public void Initialize(float timeScale)
		{
			if (!Application.isPlaying)
			{
				return;
			}

			this.timeScale = timeScale;
			timeManagerService = GlobalDependencyLocator.Instance.Get<TimeManagerService>();
		}

		public override void OnBehaviourPause(Playable playable, FrameData info)
		{
			if (!Application.isPlaying)
			{
				return;
			}

			timeManagerService.RemoveTimeModifier(this);
		}

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			if (!Application.isPlaying)
			{
				return;
			}

			float scale = Mathf.Lerp(Time.timeScale, timeScale, info.effectiveWeight);
			timeManagerService.AddTimeModifier(this, scale);
		}
	}
}
