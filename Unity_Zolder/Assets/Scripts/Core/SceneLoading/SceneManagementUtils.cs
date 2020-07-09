// Copyright 2018 Talespin, LLC. All Rights Reserved.

using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Talespin.Core.Foundation.SceneLoading
{
	public static class SceneManagementUtils
	{
#if UNITY_EDITOR
		public static List<string> GetAllScenesInBuild()
		{
			List<string> availableScenes = new List<string>();

			for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
			{
				string sceneName = EditorBuildSettings.scenes[i].path;

				int startIndex = sceneName.LastIndexOf("/");

				if (startIndex == -1)
				{
					continue;
				}

				string result = sceneName.Substring(startIndex + 1);

				int endIndex = result.IndexOf(".unity");
				string final = result.Remove(endIndex);

				availableScenes.Add(final);
			}

			return availableScenes;
		}
#endif
	}
}
