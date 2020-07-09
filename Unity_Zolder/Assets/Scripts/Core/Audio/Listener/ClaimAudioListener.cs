// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Injection;
using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	public class ClaimAudioListener : MonoBehaviour
	{
		private AudioListenerService audioListenerService;

		protected void Awake()
		{
			audioListenerService = GlobalDependencyLocator.Instance.Get<AudioListenerService>();
		}

		protected void OnEnable()
		{
			audioListenerService.ClaimListener(transform);
		}

		protected void OnDisable()
		{
			audioListenerService.ReleaseListener(transform);
		}
	}
}