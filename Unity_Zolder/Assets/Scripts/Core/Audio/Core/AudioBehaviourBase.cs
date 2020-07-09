// Copyright 2018 Talespin, LLC. All Rights Reserved.

using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	/// <summary>
	/// Abstract class, auto providing <see cref="ChanneledAudio"/>.
	/// </summary>
	public abstract class AudioBehaviourBase : MonoBehaviour
	{
		public abstract bool ReadyForCleanup { get; }

		public ChanneledAudio ChanneledAudio { get { return channeledAudio; } }

		protected ChanneledAudio channeledAudio;
		protected object context;

		protected virtual void Awake()
		{
			channeledAudio = gameObject.AddComponent<ChanneledAudio>();
		}

		/// <summary>
		/// Called by AudioService right before it removes this instance.
		/// </summary>
		public virtual void OnCleanup()
		{
			context = null;
		}

		protected void InitBase(object context, int numChannels)
		{
			this.context = context;

			channeledAudio.Init(numChannels);
			DontDestroyOnLoad(channeledAudio.transform.root.gameObject);
		}
	}
}
