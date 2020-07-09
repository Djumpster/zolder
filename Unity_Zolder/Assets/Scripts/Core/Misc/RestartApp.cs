// Copyright 2018 Talespin, LLC. All Rights Reserved.

using Talespin.Core.Foundation.Injection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Talespin.Core.Foundation.Misc
{
	/// <summary>
	/// Use this to restart the app while cleaning up everything including services and spawned objects that are marked
	/// dontDestroyOnLoad. Will load the given scene to destroy other spawned objects.
	/// </summary>
	public static class RestartApp
	{
		public static void Restart(string startScene = "Main")
		{
			GlobalDependencyLocator.Instance.Dispose();

			GameObject[] gameObjects = Object.FindObjectsOfType<GameObject>();
			for (int i = 0; i < gameObjects.Length; i++)
			{
				GameObject go = gameObjects[i];
				if (go != null)
				{
					Object.DestroyImmediate(go);
				}
			}

			SceneManager.LoadScene(startScene);
		}
	}
}