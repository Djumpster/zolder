// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.AssetHandling;
using Talespin.Core.Foundation.Events;
using Talespin.Core.Foundation.Injection;
using Talespin.Core.Foundation.Logging;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Talespin.Core.Foundation.Timeline
{
	/// <summary>
	/// Invokes an IEvent on GlobalEvents when a clip starts.
	/// </summary>
	[System.Serializable]
	public class TimelineGlobalEventsPlayableBehaviour : PlayableBehaviour
	{
		public TimelineClip Clip
		{
			get { return clip; }
			set
			{
				clip = value;
				clip.displayName = string.IsNullOrEmpty(globalEventType) ||
				GuidDatabaseManager.Instance.MapGuidToTypes(globalEventType) == null ||
				GuidDatabaseManager.Instance.MapGuidToTypes(globalEventType).Length == 0 ?
					"Global Event <<NULL>>" : GuidDatabaseManager.Instance.MapGuidToTypes(globalEventType)[0].Name;
			}
		}

		private TimelineClip clip;
		private string globalEventType;

		/// <summary>
		/// Initializes this behaviour with a string representing the event
		/// </summary>
		/// <param name="globalEventType"></param>
		public void Initialize(string globalEventType)
		{
			if (!Application.isPlaying)
			{
				return;
			}

			this.globalEventType = globalEventType;
		}

		public override void OnBehaviourPlay(Playable playable, FrameData info)
		{
			if (!Application.isPlaying)
			{
				return;
			}

			if (string.IsNullOrEmpty(globalEventType) ||
				GuidDatabaseManager.Instance.MapGuidToTypes(globalEventType) == null ||
				GuidDatabaseManager.Instance.MapGuidToTypes(globalEventType).Length == 0 ||
				!Application.isPlaying)
			{
				LogUtil.Warning(LogTags.ANIMATION, this, "Can't invoke global event with type name " + globalEventType);
				return;
			}

			GlobalDependencyLocator.Instance.Get<GlobalEvents>().Invoke((IEvent)System.Activator.CreateInstance(GuidDatabaseManager.Instance.MapGuidToTypes(globalEventType)[0]));
		}
	}
}
