// Copyright 2019 Talespin, LLC. All Rights Reserved.

using System;
using System.Collections.Generic;
using Talespin.Core.Foundation.Injection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Talespin.Core.Foundation.Cameras
{
	/// <summary>
	/// This Service tracks all flares in the hierarchy and allows you to toggle them on and off
	/// </summary>
	public class LensFlareService : IBootstrappable, IDisposable
	{
		private List<LensFlare> flares = new List<LensFlare>();
		public LensFlareService()
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
		{
			flares.Clear();
			flares.AddRange(UnityEngine.Object.FindObjectsOfType<LensFlare>());
		}

		public void SetFlaresEnabled(bool enabled)
		{
			foreach (LensFlare lensflare in flares)
			{
				if (lensflare != null)
				{
					lensflare.enabled = enabled;
				}
			}
		}

		public void Dispose()
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
		}
	}
}