// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using Talespin.Core.Foundation.Services;
using UnityEngine;

namespace Talespin.Core.Foundation.Audio
{
	public class AudioListenerService : IDisposable
	{
		public AudioListener Listener { get; private set; }

		private readonly List<Transform> claimingTransforms;

		private readonly ICallbackService callbackService;

		public AudioListenerService(ICallbackService callbackService)
		{
			this.callbackService = callbackService;
			claimingTransforms = new List<Transform>();

			callbackService.UpdateEvent += Update;

			GameObject go = new GameObject("AudioListener");
			go.transform.position = Vector3.zero;
			Listener = go.AddComponent<AudioListener>();

			UnityEngine.Object.DontDestroyOnLoad(go);
		}

		public void ClaimListener(Transform transform)
		{
			claimingTransforms.Add(transform);
		}

		public void ReleaseListener(Transform transform)
		{
			if (claimingTransforms.Contains(transform))
			{
				claimingTransforms.Remove(transform);
			}
		}

		public void Dispose()
		{
			callbackService.UpdateEvent -= Update;
			UnityEngine.Object.Destroy(Listener.gameObject);
		}

		private void Update()
		{
			Vector3 topTransformPosition;
			Quaternion topTransformRotation;

			if (claimingTransforms.Count > 0 && claimingTransforms[claimingTransforms.Count - 1])
			{
				topTransformPosition = claimingTransforms[claimingTransforms.Count - 1].position;
				topTransformRotation = claimingTransforms[claimingTransforms.Count - 1].rotation;
			}
			else
			{
				topTransformPosition = Vector3.zero;
				topTransformRotation = Quaternion.identity;
			}
			Listener.transform.position = topTransformPosition;
			Listener.transform.rotation = topTransformRotation;
		}
	}
}