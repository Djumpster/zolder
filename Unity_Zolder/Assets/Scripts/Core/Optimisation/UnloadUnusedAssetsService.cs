// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections;
using Talespin.Core.Foundation.Logging;
using Talespin.Core.Foundation.Services;
using UnityEngine;

namespace Talespin.Core.Foundation.Optimisation
{
	/// <summary>
	/// Calls Resources.UnloadUnusedAssets after a given delay.
	/// </summary>
	public class UnloadUnusedAssetsService
	{
		private ICoroutineService coroutineService;

		public UnloadUnusedAssetsService(ICoroutineService coroutineService)
		{
			this.coroutineService = coroutineService;
		}

		public void Unload(float secondsToWait)
		{
			object context = new object();
			coroutineService.StartCoroutine(Unload(secondsToWait, context), context, GetType().Name + ".Unload");
		}

		private IEnumerator Unload(float secondsToWait, object context)
		{
			LogUtil.Log(LogTags.SYSTEM, this, "Unloading assets in " + secondsToWait + " seconds.");
			if (secondsToWait > 0)
			{
				yield return new WaitForSeconds(secondsToWait);
			}

			Resources.UnloadUnusedAssets();

			yield break;
		}
	}
}